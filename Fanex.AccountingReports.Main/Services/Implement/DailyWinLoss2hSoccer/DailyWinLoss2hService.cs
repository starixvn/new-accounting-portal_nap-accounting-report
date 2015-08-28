// --------------------------------------------------------------------------
// <copyright file="DailyWinLoss2hService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hSoccer
{
    using System.Collections.Generic;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.AccountingReports.Main.Repositories.Implement;
    using Fanex.AccountingReports.Main.Repositories.Interface;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hSoccerService : ServiceBase, IDailyWinLoss2hSoccerService
    {
        private IDailyWinLoss2hSoccerRepository _reportRepository;
        private RMElement _8rmConfig;
        private RMElement _barmConfig;
        private Dictionary<object, string> _sportTypeNameCache;

        public DailyWinLoss2hSoccerService()
            : this(null)
        {
        }

        public DailyWinLoss2hSoccerService(IDailyWinLoss2hSoccerRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new DailyWinLoss2hSoccerRepository();
            this._8rmConfig = RMSection.Instance.Report[CoreConstants.ReportName.DailyWinLoss2h.ToString()]
                                                .ReportConfigs[RMSection.RMType.EightRM.ToString()];
            this._barmConfig = RMSection.Instance.Report[CoreConstants.ReportName.DailyWinLoss2h.ToString()]
                                                 .ReportConfigs[RMSection.RMType.BARM.ToString()];
        }

        /// <summary>
        /// Saves the sport type cache reference.
        /// </summary>
        /// <param name="sportIdList">The identifier list.</param>
        /// <param name="currentStorage">The current storage.</param>
        /// <returns>The sport type cache reference.</returns>
        private Dictionary<object, string> SaveSportTypeCacheRef(
            List<dynamic> sportIdList,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var sportIdInfo in sportIdList)
            {
                if (!currentStorage.ContainsKey(sportIdInfo))
                {
                    currentStorage.Add(sportIdInfo, this.GetSportName(Converter.ToInt(sportIdInfo)));
                }
            }

            return currentStorage;
        }

        /// <summary>
        /// Gets the name of the sport.
        /// </summary>
        /// <param name="sportId">The sport identifier.</param>
        /// <returns>The name of the sport.</returns>
        private string GetSportName(int sportId)
        {
            switch (sportId)
            {
                case SoccerBetTypeConstants.SoccerMajorBetType:
                    return Labels.SoccerMajorBettypes;
                case SoccerBetTypeConstants.SoccerMinorBetType:
                    return Labels.SoccerMinorBettypes;
                case SportContants._2hReportOtherSport:
                    return Labels.Other_Sports;
                case SportContants._2hReportP2P:
                    return Labels.P2P;
                default:
                    return CacheRef.GetSportName(sportId);
            }
        }

        private bool IsSportBookSport(int sportType)
        {
            return sportType == CoreConstants.SportContants.Soccer ||
                   sportType == CoreConstants.SportContants.Motorsports ||
                   sportType == CoreConstants.SportContants.CombinationMixParlay ||
                   sportType == SportContants._2hReportOtherSport ||
                   sportType == CoreConstants.SportContants.NumberGame;
        }

        /// <summary>
        /// Gets the type of the row.
        /// </summary>
        /// <param name="ibc">The IBC value.</param>
        /// <param name="site">The site value.</param>
        /// <returns>The type of the row.</returns>
        private int GetRowType(int ibc, int site)
        {
            if (ibc == RowType.Normal)
            {
                switch (site)
                {
                    case CoreConstants.SiteType.Credit:
                        return RowType._2hReportCredit;
                    case CoreConstants.SiteType.Deposit:
                        return RowType._2hReportDeposit;
                }
            }

            return ibc;
        }

        private bool IsApplyVirtualRate(int sportId)
        {
            return sportId == SportContants._2hReportOtherSport ||
                   sportId == CoreConstants.SportContants.Soccer ||
                   sportId == SportContants._2hReportMixParlay ||
                   sportId == CoreConstants.SportContants.CombinationMixParlay ||
                   sportId == CoreConstants.SportContants.NumberGame;
        }

        /// <summary>
        /// Sets the odd even color export excel.
        /// </summary>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="isEvenRow">If set to <c>true</c> [is even row].</param>
        private void SetOddEvenColor_ExportExcel(
            ref CellStyle textStyle,
            ref CellStyle twoDecimalNumberStyle,
            ref CellStyle twoDecimalPositiveNumberStyle,
            ref CellStyle twoDecimalPositivePercentNumberStyle,
            bool isEvenRow)
        {
            if (isEvenRow)
            {
                var evenRowColor = ExcelExtensions.EvenRowColor;
                textStyle.FillPattern.SetSolid(evenRowColor);
                twoDecimalNumberStyle.FillPattern.SetSolid(evenRowColor);
                twoDecimalPositiveNumberStyle.FillPattern.SetSolid(evenRowColor);
                twoDecimalPositivePercentNumberStyle.FillPattern.SetSolid(evenRowColor);
            }
            else
            {
                var oddRowColor = ExcelExtensions.OddRowColor;
                textStyle.FillPattern.SetSolid(oddRowColor);
                twoDecimalNumberStyle.FillPattern.SetSolid(oddRowColor);
                twoDecimalPositiveNumberStyle.FillPattern.SetSolid(oddRowColor);
                twoDecimalPositivePercentNumberStyle.FillPattern.SetSolid(oddRowColor);
            }
        }

        private void ExportColumnTurnover(
            decimal margin,
            decimal turnover,
            ref ExcelCell currentCell)
        {
            if (turnover != 0)
            {
                currentCell.Value = margin;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }
        }
    }
}
