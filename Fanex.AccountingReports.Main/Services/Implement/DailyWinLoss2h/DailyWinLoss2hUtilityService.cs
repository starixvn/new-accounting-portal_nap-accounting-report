// -----------------------------------------------------------------------
// <copyright file="DailyWinLoss2hUtilityService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyWinLoss2hUtilityService
    {
        private RMElement _8rmConfig;
        private RMElement _barmConfig;

        public DailyWinLoss2hUtilityService()
        {
            this._8rmConfig = RMSection.Instance.Report[CoreConstants.ReportName.DailyWinLoss2h.ToString()]
                                                .ReportConfigs[RMSection.RMType.EightRM.ToString()];
            this._barmConfig = RMSection.Instance.Report[CoreConstants.ReportName.DailyWinLoss2h.ToString()]
                                                 .ReportConfigs[RMSection.RMType.BARM.ToString()];
        }

        public RMElement EightRMElement
        {
            get
            {
                return _8rmConfig;
            }
        }

        public RMElement BARMElement
        {
            get
            {
                return _barmConfig;
            }
        }

        public bool IsHideRealTurnover(int sportId)
        {
            return sportId == CoreConstants.SportContants.Bingo;
        }

        public bool IsApplyVirtualRate(int sportId)
        {
            return sportId == SportContants._2hReportOtherSport ||
                   sportId == SoccerBetTypeConstants.SoccerMajorBetType ||
                   sportId == SoccerBetTypeConstants.SoccerMinorBetType ||
                   sportId == SportContants._2hReportMixParlay ||
                   sportId == CoreConstants.SportContants.CombinationMixParlay ||
                   sportId == CoreConstants.SportContants.NumberGame;
        }

        public Dictionary<object, string> SaveBetTypeCacheRef(
           List<dynamic> betTypeIdList,
           ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var betTypeIdInfo in betTypeIdList)
            {
                if (!currentStorage.ContainsKey(betTypeIdInfo))
                {
                    currentStorage.Add(betTypeIdInfo, CacheRef.GetBettype(Converter.ToInt(betTypeIdInfo)));
                }
            }

            return currentStorage;
        }

        public Dictionary<object, string> SaveSportTypeCacheRef(
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

        public string GetSportName(int sportId)
        {
            switch (sportId)
            {
                case SportContants._2hReportOtherSport:
                    return Labels.Other_Sports;
                case SportContants._2hReportP2P:
                    return Labels.P2P;
                case SportContants._2hReportMixParlay:
                    return Labels.Mix_Parlay;
                case CoreConstants.SportContants.CombinationMixParlay:
                    return Labels.System_Parlay;
                case CoreConstants.SportContants.HorseRacing:
                    return Labels.Racing;
                case CoreConstants.SportContants.VirtualSoccer:
                    return Labels.VirtualSports;
                case CoreConstants.SportContants.MetricGamingSuperLive:
                    return Labels.Metric_Gaming_Super_Live;
                case SportContants._2hLiveCasinoSport:
                    return Labels.SLI;
                case SportContants._2hCasinoSport:
                    return Labels.MBC_RNG_Casino;
                case SportContants._2hGoldDeluxeSport:
                    return Labels.Gold_Deluxe;
                case SportContants._2hMicroGamingLiveCasinoSport:
                    return Labels.MicroGaming_LiveCasino;
                case SportContants._2hMicroGamingRNGCasinoSport:
                    return Labels.Microgaming_RNGCasino;
                case SportContants._2hAGCasinoSport:
                    return Labels.AG_Casino;
                case SportContants._2hVegasGamesSport:
                    return Labels.Macau;
                case SportContants._2hBettrade:
                    return Labels.Bettrade;
                default:
                    return CacheRef.GetSportName(sportId);
            }
        }

        public bool IsSportBookSport(int sportType)
        {
            return sportType == SoccerBetTypeConstants.SoccerMajorBetType ||
                   sportType == SoccerBetTypeConstants.SoccerMinorBetType ||
                   sportType == CoreConstants.SportContants.Motorsports ||
                   sportType == CoreConstants.SportContants.CombinationMixParlay ||
                   sportType == SportContants._2hReportOtherSport ||
                   sportType == CoreConstants.SportContants.MetricGamingSuperLive;
        }

        public int GetRowType(int ibc, int site)
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

        /// <summary>
        /// Exports the header columns sport report not apply virtual rate.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="isSportSummary">If set to <c>true</c> [is sport summary].</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportHeaderColumns_SportReport_NotApplyVirtualRate(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            bool isSportSummary)
        {
            //// ExcelTypes

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            int maxColumnIndex = 6;
            var currentColumn = startColumn;
            ExcelCell currentCell;

            //// ExcelTypes

            workSheet.Columns[startColumn].Width = 190 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 130 * ExcelExtensions.FontWidth;
            }

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (isSportSummary)
            {
                currentCell.Value = Labels.Sport_Type;
            }
            else
            {
                currentCell.Value = Labels.Bet_Type;
            }

            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Member_Win_Loss;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.SMA_Win_Loss;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Commission;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Company_Income;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Company_Turnover;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Margin;
            currentCell.Style = headerTextStyle;
            currentColumn++;
            currentRow++;
        }

        /// <param name="sportId">The sport identifier.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportContent_SportReport(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            WinLossInfo report,
            int sportId)
        {
            int maxColumnIndex = 6;
            if (report.EnableVirtual)
            {
                maxColumnIndex = 10;
            }

            var currentColumn = startColumn;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                true,
                false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);

            var subTotalRowColor = ExcelExtensions.BackGroundColor;
            var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
            var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                false,
                false,
                false);
            var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                false,
                true,
                false);
            var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                subTotalRowColor,
                false,
                true,
                false);

            var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            if (report != null && report.ChildsInfo.Count > 0)
            {
                var isShowVirtualTurnover = this.IsApplyVirtualRate(sportId) ||
                    sportId == CoreConstants.SportContants.LiveCasino ||
                    sportId == CoreConstants.SportContants.VirtualSportGroup;
                this.SetOddEvenColor_ExportExcel(
                    ref textStyle,
                    ref twoDecimalNumberStyle,
                    ref twoDecimalPositiveNumberStyle,
                    ref twoDecimalPositivePercentNumberStyle,
                    true);

                WinLossInfo info;
                var betTypeGroupLength = report.ChildsInfo.Count;
                var betTypeForNewCricket = report.ChildsInfo[0];
                var isNewCricket = false;
                for (var i = 0; i < betTypeGroupLength; i++)
                {
                    var betTypeGroup = report.ChildsInfo[i];
                    if (betTypeGroup.Id == 500)
                    {
                        betTypeForNewCricket = report.ChildsInfo[i];
                        isNewCricket = true;
                        continue;
                    }
                    info = betTypeGroup.GrandTotal;
                    this.ExportNormalRow(
                        ref workSheet,
                         textStyle,
                         twoDecimalNumberStyle,
                         twoDecimalPositiveNumberStyle,
                         twoDecimalPositivePercentNumberStyle,
                         info,
                         startColumn,
                         ref currentRow,
                         RowType.Normal,
                         report.EnableVirtual,
                         isShowVirtualTurnover);

                    info = betTypeGroup.CreditTotal;
                    this.ExportNormalRow(
                        ref workSheet,
                        subTotalTextStyle,
                        subTotalTwoDecimalNumberStyle,
                        subTotalTwoDecimalPositiveNumberStyle,
                        subTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportCredit,
                        report.EnableVirtual,
                        isShowVirtualTurnover);

                    info = betTypeGroup.DepositTotal;
                    this.ExportNormalRow(
                        ref workSheet,
                        subTotalTextStyle,
                        subTotalTwoDecimalNumberStyle,
                        subTotalTwoDecimalPositiveNumberStyle,
                        subTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportDeposit,
                        report.EnableVirtual,
                        isShowVirtualTurnover);
                }
                // for *cricket
                if (isNewCricket == true)
                {
                    info = report.Total;
                    this.ExportNormalRow(
                        ref workSheet,
                        grandTotalRowTextStyle_FirstRow,
                        grandTotalRowTwoDecimalNumberStyle_FirstRow,
                        grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                        gtrPercentNumberStyle_FirstRow,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportGrandTotal,
                        report.EnableVirtual,
                        isShowVirtualTurnover);

                    info = report.CreditTotalNewCricket;
                    this.ExportNormalRow(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportCredit,
                        report.EnableVirtual,
                        isShowVirtualTurnover);

                    info = report.DepositTotalNewCricket;
                    this.ExportNormalRow(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportDeposit,
                        report.EnableVirtual,
                        isShowVirtualTurnover);
                    // for charge commission
                    info = betTypeForNewCricket.GrandTotal;
                    this.ExportNormalRow(
                        ref workSheet,
                        grandTotalRowTextStyle_FirstRow,
                        grandTotalRowTwoDecimalNumberStyle_FirstRow,
                        grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                        gtrPercentNumberStyle_FirstRow,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportGrandTotal,
                        report.EnableVirtual,
                        isShowVirtualTurnover);

                    info = betTypeForNewCricket.CreditTotal;
                    this.ExportNormalRow(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportCredit,
                        report.EnableVirtual,
                        isShowVirtualTurnover);

                    info = betTypeForNewCricket.DepositTotal;
                    this.ExportNormalRow(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportDeposit,
                        report.EnableVirtual,
                        isShowVirtualTurnover);
                }
                //
                info = report.GrandTotal;
                this.ExportNormalRow(
                    ref workSheet,
                    grandTotalRowTextStyle_FirstRow,
                    grandTotalRowTwoDecimalNumberStyle_FirstRow,
                    grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                    gtrPercentNumberStyle_FirstRow,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportGrandTotal,
                    report.EnableVirtual,
                    isShowVirtualTurnover);

                info = report.CreditTotal;
                this.ExportNormalRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportCredit,
                    report.EnableVirtual,
                    isShowVirtualTurnover);

                info = report.DepositTotal;
                this.ExportNormalRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportDeposit,
                    report.EnableVirtual,
                    isShowVirtualTurnover);

                //// IBCTotal

                info = report.EightRMTotal;
                if (this._8rmConfig.IsActive &&
                    info != null &&
                    info.WinLoss != 0)
                {
                    this.ExportIBCRow_SportReport(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        maxColumnIndex);
                }

                info = report.BARMTotal;
                if (this._8rmConfig.IsActive &&
                    info != null &&
                    info.WinLoss != 0)
                {
                    this.ExportIBCRow_SportReport(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        maxColumnIndex);
                }

                //// IBCTotal
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }
        }

        /// <summary>
        /// Sets the odd even color export excel.
        /// </summary>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="isEvenRow">If set to <c>true</c> [is even row].</param>
        public void SetOddEvenColor_ExportExcel(
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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
       "SP2101:MethodMustNotContainMoreLinesThan",
       Justification = "Reviewed. Suppression is OK here.")]
        public void ExportIBCRow_SportReport(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex)
        {
            ExcelCell currentCell;

            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualLineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Variance;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Turnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualTurnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Margin;
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualMargin;
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentRow++;
        }

        /// <summary>
        /// Exports the normal row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="info">The information.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="rowType">Type of the row.</param>
        /// <param name="isEnableVirtual">If set to <c>true</c> [is enable virtual].</param>
        /// <param name="isShowVirtualTurnover">If set to <c>true</c> [is show virtual turnover].</param>
        /// <param name="isHideMargin">If set to <c>true</c> [is hide margin].</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportNormalRow(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int rowType,
            bool isEnableVirtual,
            bool isShowVirtualTurnover = false,
            bool isHideMargin = false)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            switch (rowType)
            {
                case RowType._2hReportCredit:
                case RowType._2hReportDeposit:
                case RowType._2hReportVirtualAllTotal:
                case RowType._2hReportGrandTotal:
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            switch (rowType)
            {
                case RowType._2hReportVirtualAllTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;

                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                    break;
            }

            currentColumn++;

            if (isEnableVirtual)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentCell.Value = info.VirtualLineTotal;

                switch (rowType)
                {
                    case RowType._2hReportVirtualAllTotal:
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        break;

                    case RowType._2hReportGrandTotal:
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                        break;
                }

                currentColumn++;
            }

            if (isEnableVirtual)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentCell.Value = info.Variance;
                switch (rowType)
                {
                    case RowType._2hReportVirtualAllTotal:
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        break;

                    case RowType._2hReportGrandTotal:
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                        break;
                }

                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            if (info.IsHideRealTurnover)
            {
                currentCell.Value = Labels.NotAvailable;
            }
            else
            {
                currentCell.Value = info.Turnover;
            }

            switch (rowType)
            {
                case RowType._2hReportVirtualAllTotal:
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;
            }

            currentColumn++;
            if (isEnableVirtual)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNumberStyle;
                if (isShowVirtualTurnover)
                {
                    currentCell.Value = info.VirtualTurnover;
                }
                else
                {
                    currentCell.Value = Labels.NotAvailable;
                }

                switch (rowType)
                {
                    case RowType._2hReportVirtualAllTotal:
                    case RowType._2hReportGrandTotal:
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        break;
                }

                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;

            if (!isHideMargin)
            {
                this.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            }

            currentColumn++;

            if (isEnableVirtual)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalPositivePercentNumberStyle;

                if (!isHideMargin)
                {
                    this.ExportColumnTurnover(info.VirtualMargin, info.VirtualTurnover, ref currentCell);
                }

                currentColumn++;
            }

            currentRow++;
        }

        public void ExportColumnTurnover(
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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2101:MethodMustNotContainMoreLinesThan",
           Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMainCasinoHeader(
           ref ExcelWorksheet workSheet,
           ref int currentRow,
           int startRow,
           int startColumn)
        {
            //// ExcelTypes

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            int maxColumnIndex = 8;
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            //// ExcelTypes

            workSheet.Columns[startColumn].Width = 190 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 130 * ExcelExtensions.FontWidth;
            }

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Sport_Type;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member_Win_Loss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.SMA_Winloss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Commission;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Income;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Turnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Margin;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;
            currentRow++;

            // SecondRow
            currentColumn = startColumn + 4;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.After_Extra_PT;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.After_Extra_PT;
            currentColumn++;
            currentRow++;
        }

        /// <param name="sportId">The sport identifier.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMainCasinoContent(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            WinLossInfo report)
        {
            int maxColumnIndex = 8;
            var currentColumn = startColumn;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                true,
                false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);

            var subTotalRowColor = ExcelExtensions.BackGroundColor;
            var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
            var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                false,
                false,
                false);
            var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                false,
                true,
                false);
            var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                subTotalRowColor,
                false,
                true,
                false);

            var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            if (report != null && report.ChildsInfo.Count > 0)
            {
                this.SetOddEvenColor_ExportExcel(
                    ref textStyle,
                    ref twoDecimalNumberStyle,
                    ref twoDecimalPositiveNumberStyle,
                    ref twoDecimalPositivePercentNumberStyle,
                    true);

                WinLossInfo info;
                var betTypeGroupLength = report.ChildsInfo.Count;
                for (var i = 0; i < betTypeGroupLength; i++)
                {
                    var betTypeGroup = report.ChildsInfo[i];
                    info = betTypeGroup.GrandTotal;
                    this.ExportMainCasinoRow(
                        ref workSheet,
                         textStyle,
                         twoDecimalNumberStyle,
                         twoDecimalPositiveNumberStyle,
                         twoDecimalPositivePercentNumberStyle,
                         info,
                         startColumn,
                         ref currentRow,
                         RowType.Normal);

                    info = betTypeGroup.CreditTotal;
                    this.ExportMainCasinoRow(
                        ref workSheet,
                        subTotalTextStyle,
                        subTotalTwoDecimalNumberStyle,
                        subTotalTwoDecimalPositiveNumberStyle,
                        subTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportCredit);

                    info = betTypeGroup.DepositTotal;
                    this.ExportMainCasinoRow(
                        ref workSheet,
                        subTotalTextStyle,
                        subTotalTwoDecimalNumberStyle,
                        subTotalTwoDecimalPositiveNumberStyle,
                        subTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportDeposit);
                }

                info = report.GrandTotal;
                this.ExportMainCasinoRow(
                    ref workSheet,
                    grandTotalRowTextStyle_FirstRow,
                    grandTotalRowTwoDecimalNumberStyle_FirstRow,
                    grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                    gtrPercentNumberStyle_FirstRow,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportGrandTotal);

                info = report.CreditTotal;
                this.ExportMainCasinoRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportCredit);

                info = report.DepositTotal;
                this.ExportMainCasinoRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportDeposit);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2101:MethodMustNotContainMoreLinesThan",
           Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMainCasinoRow(
           ref ExcelWorksheet workSheet,
           CellStyle textStyle,
           CellStyle twoDecimalNumberStyle,
           CellStyle twoDecimalPositiveNumberStyle,
           CellStyle twoDecimalPositivePercentNumberStyle,
           WinLossInfo info,
           int startColumn,
           ref int currentRow,
           int rowType)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            switch (rowType)
            {
                case RowType._2hReportCredit:
                case RowType._2hReportDeposit:
                case RowType._2hReportVirtualAllTotal:
                case RowType._2hReportGrandTotal:
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;

            switch (rowType)
            {
                case RowType._2hReportVirtualAllTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;

                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                    break;
            }

            currentColumn++;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.IsShowAvailableExtraPositionTaking ? info.ExtraPositionTaking : (object)Labels.NotAvailable;
            currentCell.Style = twoDecimalPositiveNumberStyle;

            currentColumn++;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.AfterExtraPositionTaking;
            currentCell.Style = twoDecimalPositiveNumberStyle;

            currentColumn++;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            if (info.IsHideRealTurnover)
            {
                currentCell.Value = Labels.NotAvailable;
            }
            else
            {
                currentCell.Value = info.Turnover;
            }

            switch (rowType)
            {
                case RowType._2hReportVirtualAllTotal:
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;
            }

            currentColumn++;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            this.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            currentColumn++;
            currentRow++;
        }
    }
}