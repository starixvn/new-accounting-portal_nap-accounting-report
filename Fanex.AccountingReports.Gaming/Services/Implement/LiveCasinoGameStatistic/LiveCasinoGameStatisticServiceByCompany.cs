namespace Fanex.AccountingReports.Gaming.Services.Implement.LiveCasinoGameStatistic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoGameStatisticServiceByCompany : LiveCasinoGameStatisticService
    {
        private readonly string _reportSessionKey = "Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.Report" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        /// <summary>
        /// Gets the report by company.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report by company.</returns>
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
            var companyGroups = winLossInfoTable.GroupBy(data => data.Id);
            Dictionary<string, int> currencyIndexDictionary = new Dictionary<string, int>();
            WinLossInfo grandTotalRM = new WinLossInfo
            {
                Name = Labels.Grand_Total + " " + Labels.RM,
                CurrencyName = Labels.RM
            };

            foreach (var companyGroup in companyGroups)
            {
                var companyWinLoss = new WinLossInfo
                {
                    Name = Converter.ToString2(companyGroup.Key),
                    ChildsInfo = new List<WinLossInfo>()
                };

                foreach (var data in companyGroup)
                {
                    info = new WinLossInfo
                    {
                        Name = companyWinLoss.Name,
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
                    companyWinLoss.ChildsInfo.Add(info);
                }

                if (companyWinLoss.ChildsInfo.Count > 0)
                {
                    companyWinLoss.ChildsInfo = companyWinLoss.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName).ToList();
                    report.ChildsInfo.Add(companyWinLoss);
                }
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name).ToList();
            CalculateGrandTotal(criteria, ref report, grandTotalRM);
            return report;
        }

        /// <summary>
        /// Sorts the report data.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>Report data after sort.</returns>
        public override WinLossInfo SortReportData(WinLossInfo report, GetReportCriteria getReportCriteria)
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
        /// Exports the report by company.
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
                PeriodTypeConstants.LiveCasinoGameStatisticCompany,
                maxColumnIndex,
                criteria);

            var companyLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (companyLength > 0)
            {
                WinLossInfo info;
                var reportLength = 0;
                for (var i = 0; i < companyLength; i++)
                {
                    var companyWinLoss = report.ChildsInfo[i];
                    reportLength = companyWinLoss.ChildsInfo.Count;
                    for (var j = 0; j < reportLength; j++)
                    {
                        info = companyWinLoss.ChildsInfo[j];
                        currentColumn = startColumn;
                        if (j == 0)
                        {
                            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + reportLength - 1, currentColumn);
                            currentCellRange.Merged = true;
                            currentCellRange.Value = companyWinLoss.Name;
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
                GamingResources.LiveCasinoGameStatistic_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }
    }
}
