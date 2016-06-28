// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameStatisticServiceByUsername.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.LiveCasinoGameStatistic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoGameStatisticServiceByUsername : LiveCasinoGameStatisticService
    {
        /// <summary>
        /// Gets the name of the report by user.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The name of the report by user.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
             "SP2101:MethodMustNotContainMoreLinesThan",
             Justification = "Reviewed. Suppression is OK here.")]
        public override WinLossInfo GetReport(
            List<WinLossInfoEntity> winLossInfoTable,
            GetReportCriteria criteria)
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
            Dictionary<string, int> currencyIndexDictionary = new Dictionary<string, int>();
            WinLossInfo grandTotalRM = new WinLossInfo
            {
                Name = Labels.Grand_Total + " " + Labels.RM,
                CurrencyName = Labels.RM
            };

            foreach (var data in winLossInfoTable)
            {
                info = new WinLossInfo
                {
                    Name = data.Id,
                    Id = data.CustId,
                    CurrencyName = data.CurrencyName,
                    NumberOfPlayers = data.NumberOfPlayers,
                    NumberOfGames = data.NumberOfGames,

                    TotalBets = data.TotalBets,
                    TotalWins = data.TotalWins,
                    TotalRefund = data.TotalRefund,

                    GameIncome = data.GameIncome,

                    CompanyTurnover = data.CompanyTurnover,
                    CompanyNetIncome = data.CompanyNetIncome,
                    CompanyCommission = data.CompanyCommission
                };

                if (!IsCorrectFilteredInfo(criteria, info))
                {
                    continue;
                }

                grandTotalRM.Sum(new WinLossInfo
                {
                    NumberOfPlayers = data.NumberOfPlayers,
                    NumberOfGames = data.NumberOfGames,

                    TotalBets = data.TotalBetsRM,
                    TotalWins = data.TotalWinsRM,
                    TotalRefund = data.TotalRefundRM,

                    GameIncome = data.GameIncomeRM,

                    CompanyTurnover = data.CompanyTurnoverRM,
                    CompanyNetIncome = data.CompanyNetIncomeRM,
                    CompanyCommission = data.CompanyCommissionRM
                });

                if (!currencyIndexDictionary.ContainsKey(info.CurrencyName))
                {
                    report.GrandTotal.ChildsInfo.Add(new WinLossInfo
                    {
                        Name = Labels.Total + " " + info.CurrencyName,
                        CurrencyName = info.CurrencyName
                    });

                    currencyIndexDictionary.Add(info.CurrencyName, report.GrandTotal.ChildsInfo.Count - 1);
                }

                var currencyIndex = currencyIndexDictionary[info.CurrencyName];
                report.GrandTotal.ChildsInfo[currencyIndex].Sum(info);
                info = WinLossInfo.Format(info);
                report.ChildsInfo.Add(info);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name)
                                                 .ThenBy(winLoss => winLoss.CurrencyName)
                                                 .ToList();
            CalculateGrandTotal(criteria, ref report, grandTotalRM);
            return report;
        }

        public override WinLossInfo SortReportData(WinLossInfo report, GetReportCriteria getReportCriteria)
        {
            ListExtensions.SortDataByProperty<WinLossInfo>(
                report.ChildsInfo,
                getReportCriteria.SortDirection,
                getReportCriteria.SortColumnName,
                getReportCriteria.SortDataType);
            return report;
        }

        /// <summary>
        /// Exports the name of the report by user.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public override void ExportReport(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.LiveCasinoGameStatistic_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = criteria.ShowRefundColumn ? 11 : 10;
            var currentRow = startRow;

            var currentColumn = startColumn;
            ExcelCell currentCell;

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
                PeriodTypeConstants.LiveCasinoGameStatisticUsername,
                maxColumnIndex,
                criteria);

            var reportLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (reportLength > 0)
            {
                WinLossInfo info;
                for (var i = 0; i < reportLength; i++)
                {
                    info = report.ChildsInfo[i];
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.To<int>(info.Id);
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
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
                GamingResources.LiveCasinoGameStatistic_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }
    }
}
