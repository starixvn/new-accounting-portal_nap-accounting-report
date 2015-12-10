// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameStatisticServiceByGameTable.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoGameStatisticServiceByGameTable : CasinoGameStatisticService
    {
        public override WinLossInfo GetReport(List<WinLossInfoEntity> winLossInfoTable, WinLossInfoEntity grandTotal, GetReportByDailyCriteria criteria)
        {
            var report = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>()
                }
            };

            WinLossInfo info;
            var gameTableGroups = winLossInfoTable.GroupBy(data => data.Id);
            WinLossInfo grandTotalRM = new WinLossInfo
            {
                Name = Labels.Grand_Total + " " + Labels.RM,
                CurrencyName = Labels.RM
            };

            foreach (var gameTableGroup in gameTableGroups)
            {
                var gameTableWinLoss = new WinLossInfo
                {
                    Id = Converter.ToString2(gameTableGroup.Key),
                    ChildsInfo = new List<WinLossInfo>()
                };

                gameTableWinLoss.Name = CacheRef.GetLeagueName(gameTableWinLoss.Id);
                foreach (var data in gameTableGroup)
                {
                    info = new WinLossInfo
                    {
                        Id = gameTableWinLoss.Id,
                        Name = gameTableWinLoss.Name,
                        CurrencyName = data.CurrencyName,
                        CurrencyId = data.CurrencyId,
                        NumberOfPlayers = data.NumberOfPlayers,
                        NumberOfGames = data.NumberOfGames,

                        TotalBets = data.TotalBets,
                        TotalWins = data.TotalWins,
                        TotalRefund = data.TotalRefund,

                        GameIncome = data.GameIncome,
                        GamePayout = data.GamePayout,

                        CompanyTurnover = data.CompanyTurnover,
                        CompanyNetIncome = data.CompanyNetIncome,
                        CompanyCommission = data.CompanyCommission
                    };

                    info = WinLossInfo.Format(info);
                    gameTableWinLoss.ChildsInfo.Add(info);
                }

                if (gameTableWinLoss.ChildsInfo.Count > 0)
                {
                    gameTableWinLoss.ChildsInfo = gameTableWinLoss.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName).ToList();
                    report.ChildsInfo.Add(gameTableWinLoss);
                }
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name).ToList();

            Utilities.AssignGrandTotal(grandTotal, grandTotalRM);

            report.GrandTotal.ChildsInfo.Add(grandTotalRM);

            return report;
        }

        /// <summary>
        /// Sorts the report data.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>Report data after sort.</returns>
        public override WinLossInfo SortReportData(WinLossInfo report, GetReportByDailyCriteria getReportCriteria)
        {
            if (getReportCriteria.SortColumnName == "Name")
            {
                ListExtensions.SortDataByProperty<WinLossInfo>(
                    report.ChildsInfo,
                    getReportCriteria.SortDirection,
                    getReportCriteria.SortColumnName,
                    getReportCriteria.SortDataType);
            }
            else
            {
                foreach (var childInfo in report.ChildsInfo)
                {
                    ListExtensions.SortDataByProperty<WinLossInfo>(
                        childInfo.ChildsInfo,
                        getReportCriteria.SortDirection,
                        getReportCriteria.SortColumnName,
                        getReportCriteria.SortDataType);
                }

                PropertyInfo propInfo = typeof(WinLossInfo).GetProperty(getReportCriteria.SortColumnName);
                bool isAsc = string.Equals(getReportCriteria.SortDirection, CoreConstants.SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);
                if (isAsc)
                {
                    report.ChildsInfo = report.ChildsInfo
                                              .OrderBy(winLossInfo => propInfo.GetValue(winLossInfo.ChildsInfo[0], null))
                                              .ToList();
                }
                else
                {
                    report.ChildsInfo = report.ChildsInfo
                                              .OrderByDescending(winLossInfo => propInfo.GetValue(winLossInfo.ChildsInfo[0], null))
                                              .ToList();
                }
            }

            return report;
        }

        /// <summary>
        /// Exports the report by game type.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public override void ExportReport(
            WinLossInfo report,
            GetReportByDailyCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = string.Format(Labels.CasinoGameStatistic_SheetName, criteria.Product);
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = criteria.ShowRefundColumn ? 11 : 10;
            var currentRow = startRow;

            var currentColumn = startColumn;
            CellRange currentCellRange;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var intNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.IntNumberFormat, backgroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalNegativeNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                false,
                true);
            var twoDecimalNegativePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                false,
                true);

            ExportReport_Header(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                PeriodTypeConstants.LiveCasinoGameStatisticGameTable,
                maxColumnIndex,
                criteria);

            var gameTableLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (gameTableLength > 0)
            {
                WinLossInfo info;
                var reportLength = 0;
                for (var i = 0; i < gameTableLength; i++)
                {
                    var gameTableWinLoss = report.ChildsInfo[i];
                    reportLength = gameTableWinLoss.ChildsInfo.Count;
                    for (var j = 0; j < reportLength; j++)
                    {
                        info = gameTableWinLoss.ChildsInfo[j];
                        currentColumn = startColumn;
                        if (j == 0)
                        {
                            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + reportLength - 1, currentColumn);
                            currentCellRange.Merged = true;
                            currentCellRange.Value = gameTableWinLoss.Name;
                            currentCellRange.Style = textStyle;
                            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        }

                        currentColumn++;

                        ExportReport_WinLossRow(
                            ref workSheet,
                            currentColumn,
                            ref currentRow,
                            intNumberStyle,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalNegativeNumberStyle,
                            twoDecimalNegativePercentNumberStyle,
                            criteria,
                            info);
                        currentRow++;
                    }
                }

                ExportReport_ChildTotal(
                    report.GrandTotal,
                    ref workSheet,
                    startColumn,
                    ref currentRow,
                    criteria);

                ExportReport_GrandTotal(
                    report.GrandTotal,
                    ref workSheet,
                    startColumn,
                    ref currentRow,
                    criteria);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            var reportName = string.Format(
               GamingResources.CasinoGameStatistic_ReportName,
               criteria.Product.Replace(" ", "_"),
               Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
               Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }
    }
}