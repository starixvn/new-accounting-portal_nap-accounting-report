// --------------------------------------------------------------------------
// <copyright file="BingoStatisticService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.BingoStatistic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.BingoStatistic;
    using Fanex.AccountingReports.Gaming.Models.BingoStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BingoStatisticService : ServiceBase, IBingoStatisticService
    {
        private readonly IBingoStatisticRepository _reportRepository;

        public BingoStatisticService()
            : this(null)
        {
        }

        public BingoStatisticService(IBingoStatisticRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new BingoStatisticRepository();
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetReport(GetReportCriteria criteria)
        {
            try
            {
                var report = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo()
                };

                var rawData = this._reportRepository.GetReport(criteria);
                if (rawData != null && rawData.Count >= 2)
                {
                    var winLossInfoTable = rawData[0];
                    var grandTotalInfoTable = rawData[1];
                    switch (criteria.ReportType)
                    {
                        case PeriodTypeConstants.GamingBingoStatisticDaily:
                            report = this.GetReport_ByDailyRange(
                                winLossInfoTable,
                                criteria);
                            break;
                        case PeriodTypeConstants.GamingBingoStatisticMonthly:
                            report = this.GetReport_ByMonthlyRange(
                                winLossInfoTable,
                                criteria);
                            break;
                        case PeriodTypeConstants.GamingBingoStatisticUsername:
                            report = this.GetReport_ByUserName(
                                winLossInfoTable,
                                criteria);
                            break;
                        case PeriodTypeConstants.GamingBingoStatisticCompany:
                            report = this.GetReport_ByCompany(
                                winLossInfoTable,
                                criteria);
                            break;
                        case PeriodTypeConstants.GamingBingoStatisticCurrency:
                            report = this.GetReport_ByCurrency(
                                winLossInfoTable,
                                criteria);
                            break;
                        case PeriodTypeConstants.GamingBingoStatisticAgent:
                            report = this.GetReport_ByAgent(
                                winLossInfoTable,
                                criteria);
                            break;
                    }

                    report.GrandTotal = this.GetReport_GrandTotal(grandTotalInfoTable, criteria);
                }

                SessionManager.Set<WinLossInfo>(GamingConstants.BingoStatisticReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(GamingConstants.BingoStatisticReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the name of the report by user.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The name of the report by user.</returns>
        public WinLossInfo GetReport_ByUserName(
            List<dynamic> winLossInfoTable,
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
            foreach (var data in winLossInfoTable)
            {
                info = new WinLossInfo
                {
                    Name = Converter.ToString2(data.Id),
                    NoPlayers = Converter.ToInt(data.NoOfPlayers),
                    CurrencyName = Converter.ToString2(data.Currency),
                    TotalBets = Converter.ToDecimal(data.TotalBets),
                    TotalWins = Converter.ToDecimal(data.TotalWins),
                    GameIncome = Converter.ToDecimal(data.GameIncome),
                    GamePayout = Converter.ToDecimal(data.GamePayout) / 100,
                    CompanyNetIncome = Converter.ToDecimal(data.CompanyNetIncome),
                    CompanyComm = Converter.ToDecimal(data.CompanyComm)
                };

                info = WinLossInfo.Format(info);
                report.ChildsInfo.Add(info);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName)
                                                 .ThenBy(winLoss => winLoss.Name)
                                                 .ToList();
            return report;
        }

        /// <summary>
        /// Gets the report by currency.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report by currency.</returns>
        public WinLossInfo GetReport_ByCurrency(
            List<dynamic> winLossInfoTable,
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
            foreach (var data in winLossInfoTable)
            {
                info = new WinLossInfo
                {
                    Name = Converter.ToString2(data.Id),
                    NoPlayers = Converter.ToInt(data.NoOfPlayers),
                    CurrencyName = Converter.ToString2(data.Currency),
                    TotalBets = Converter.ToDecimal(data.TotalBets),
                    TotalWins = Converter.ToDecimal(data.TotalWins),
                    GameIncome = Converter.ToDecimal(data.GameIncome),
                    GamePayout = Converter.ToDecimal(data.GamePayout) / 100,
                    CompanyNetIncome = Converter.ToDecimal(data.CompanyNetIncome),
                    CompanyComm = Converter.ToDecimal(data.CompanyComm)
                };

                info = WinLossInfo.Format(info);
                report.ChildsInfo.Add(info);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name)
                                                 .ToList();
            return report;
        }

        /// <summary>
        /// Gets the report by agent.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report by agent.</returns>
        public WinLossInfo GetReport_ByAgent(
            List<dynamic> winLossInfoTable,
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
            foreach (var data in winLossInfoTable)
            {
                info = new WinLossInfo
                {
                    Name = Converter.ToString2(data.Id),
                    NoPlayers = Converter.ToInt(data.NoOfPlayers),
                    CurrencyName = Converter.ToString2(data.Currency),
                    TotalBets = Converter.ToDecimal(data.TotalBets),
                    TotalWins = Converter.ToDecimal(data.TotalWins),
                    GameIncome = Converter.ToDecimal(data.GameIncome),
                    GamePayout = Converter.ToDecimal(data.GamePayout) / 100,
                    CompanyNetIncome = Converter.ToDecimal(data.CompanyNetIncome),
                    CompanyComm = Converter.ToDecimal(data.CompanyComm)
                };

                info = WinLossInfo.Format(info);
                report.ChildsInfo.Add(info);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName)
                                                 .ThenBy(winLoss => winLoss.Name)
                                                 .ToList();
            return report;
        }

        /// <summary>
        /// Gets the report by company.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report by company.</returns>
        public WinLossInfo GetReport_ByCompany(
            List<dynamic> winLossInfoTable,
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
                        NoPlayers = Converter.ToInt(data.NoOfPlayers),
                        CurrencyName = Converter.ToString2(data.Currency),
                        TotalBets = Converter.ToDecimal(data.TotalBets),
                        TotalWins = Converter.ToDecimal(data.TotalWins),
                        GameIncome = Converter.ToDecimal(data.GameIncome),
                        GamePayout = Converter.ToDecimal(data.GamePayout) / 100,
                        CompanyNetIncome = Converter.ToDecimal(data.CompanyNetIncome),
                        CompanyComm = Converter.ToDecimal(data.CompanyComm)
                    };

                    info = WinLossInfo.Format(info);
                    companyWinLoss.ChildsInfo.Add(info);
                }

                companyWinLoss.ChildsInfo = companyWinLoss.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName).ToList();
                report.ChildsInfo.Add(companyWinLoss);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name).ToList();
            return report;
        }

        /// <summary>
        /// Gets the report by daily range.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report by daily range.</returns>
        public WinLossInfo GetReport_ByDailyRange(
            List<dynamic> winLossInfoTable,
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
            var timeRangeGroups = winLossInfoTable.GroupBy(
                data => Converter.ToDateTime(data.Id));
            foreach (var timeRangeGroup in timeRangeGroups)
            {
                DateTime dateTime = timeRangeGroup.Key;
                var timeRangeWinLoss = new WinLossInfo
                {
                    Id = dateTime.ToShortFormatedDate(),
                    Name = dateTime.ToShortFormatedDate(),
                    ChildsInfo = new List<WinLossInfo>()
                };

                foreach (var data in timeRangeGroup)
                {
                    info = new WinLossInfo
                    {
                        Id = timeRangeWinLoss.Id,
                        Name = timeRangeWinLoss.Name,
                        NoPlayers = Converter.ToInt(data.NoOfPlayers),
                        CurrencyName = Converter.ToString2(data.Currency),
                        TotalBets = Converter.ToDecimal(data.TotalBets),
                        TotalWins = Converter.ToDecimal(data.TotalWins),
                        GameIncome = Converter.ToDecimal(data.GameIncome),
                        GamePayout = Converter.ToDecimal(data.GamePayout) / 100,
                        CompanyNetIncome = Converter.ToDecimal(data.CompanyNetIncome),
                        CompanyComm = Converter.ToDecimal(data.CompanyComm)
                    };

                    info = WinLossInfo.Format(info);
                    timeRangeWinLoss.ChildsInfo.Add(info);
                }

                timeRangeWinLoss.ChildsInfo = timeRangeWinLoss.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName).ToList();
                report.ChildsInfo.Add(timeRangeWinLoss);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name).ToList();
            return report;
        }

        /// <summary>
        /// Gets the report by monthly range.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report by monthly range.</returns>
        public WinLossInfo GetReport_ByMonthlyRange(
            List<dynamic> winLossInfoTable,
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
            var timeRangeGroups = winLossInfoTable.GroupBy(
                data => Converter.ToDateTime(data.Id));
            foreach (var timeRangeGroup in timeRangeGroups)
            {
                DateTime dateTime = timeRangeGroup.Key;
                var timeRangeWinLoss = new WinLossInfo
                {
                    Id = dateTime.ToShortFormatedDate(),
                    Name = dateTime.ToMonthYearWithSlash(),
                    ChildsInfo = new List<WinLossInfo>()
                };

                foreach (var data in timeRangeGroup)
                {
                    info = new WinLossInfo
                    {
                        Id = timeRangeWinLoss.Id,
                        Name = timeRangeWinLoss.Name,
                        NoPlayers = Converter.ToInt(data.NoOfPlayers),
                        CurrencyName = Converter.ToString2(data.Currency),
                        TotalBets = Converter.ToDecimal(data.TotalBets),
                        TotalWins = Converter.ToDecimal(data.TotalWins),
                        GameIncome = Converter.ToDecimal(data.GameIncome),
                        GamePayout = Converter.ToDecimal(data.GamePayout) / 100,
                        CompanyNetIncome = Converter.ToDecimal(data.CompanyNetIncome),
                        CompanyComm = Converter.ToDecimal(data.CompanyComm)
                    };

                    info = WinLossInfo.Format(info);
                    timeRangeWinLoss.ChildsInfo.Add(info);
                }

                timeRangeWinLoss.ChildsInfo = timeRangeWinLoss.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName).ToList();
                report.ChildsInfo.Add(timeRangeWinLoss);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name).ToList();
            return report;
        }

        /// <summary>
        /// Gets the report grand total.
        /// </summary>
        /// <param name="grandTotalInfoTable">The grand total information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report grand total.</returns>
        public WinLossInfo GetReport_GrandTotal(
            List<dynamic> grandTotalInfoTable,
           GetReportCriteria criteria)
        {
            var grandTotal = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>()
            };

            WinLossInfo info;
            var maxTotalIndex = grandTotalInfoTable.Count - 1;
            for (var i = 0; i <= maxTotalIndex; i++)
            {
                var data = grandTotalInfoTable[i];
                info = new WinLossInfo
                {
                    NoPlayers = Converter.ToInt(data.NoOfPlayers),
                    CurrencyName = Converter.ToString2(data.Currency),
                    TotalBets = Converter.ToDecimal(data.TotalBets),
                    TotalWins = Converter.ToDecimal(data.TotalWins),
                    GameIncome = Converter.ToDecimal(data.GameIncome),
                    GamePayout = Converter.ToDecimal(data.GamePayout) / 100,
                    CompanyNetIncome = Converter.ToDecimal(data.CompanyNetIncome),
                    CompanyComm = Converter.ToDecimal(data.CompanyComm)
                };

                if (i == maxTotalIndex)
                {
                    grandTotal.CurrencyName = Labels.RM;
                    grandTotal.Name = string.Format("{0} {1}", Labels.Grand_Total, grandTotal.CurrencyName);
                    grandTotal.Add(info);
                    grandTotal = WinLossInfo.Format(grandTotal);
                }
                else
                {
                    info.Name = string.Format("{0} {1}", Labels.Total, Converter.ToString2(data.Id));
                    info = WinLossInfo.Format(info);
                    grandTotal.ChildsInfo.Add(info);
                }
            }

            return grandTotal;
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetReportCriteria criteria)
        {
            try
            {
                WinLossInfo report;
                if (criteria.IsChangedValue)
                {
                    report = this.GetReport(criteria);
                }
                else
                {
                    report = SessionManager.Get<WinLossInfo>(GamingConstants.BingoStatisticReportSessionKey);
                }

                switch (criteria.ReportType)
                {
                    case PeriodTypeConstants.GamingBingoStatisticDaily:
                        this.ExportReport_ByDailyRange(report, criteria);
                        break;
                    case PeriodTypeConstants.GamingBingoStatisticMonthly:
                        this.ExportReport_ByMonthlyRange(report, criteria);
                        break;
                    case PeriodTypeConstants.GamingBingoStatisticUsername:
                        this.ExportReport_ByUserName(report, criteria);
                        break;
                    case PeriodTypeConstants.GamingBingoStatisticCompany:
                        this.ExportReport_ByCompany(report, criteria);
                        break;
                    case PeriodTypeConstants.GamingBingoStatisticCurrency:
                        this.ExportReport_ByCurrency(report, criteria);
                        break;
                    case PeriodTypeConstants.GamingBingoStatisticAgent:
                        this.ExportReport_ByAgent(report, criteria);
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the report by daily range.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_ByDailyRange(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.BingoStatistic_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 8;
            var currentRow = startRow;

            var currentColumn = startColumn;
            CellRange currentCellRange;
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
            
            this.ExportReport_Header(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                PeriodTypeConstants.GamingBingoStatisticDaily,
                maxColumnIndex,
                criteria);

            var timeRangeLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (timeRangeLength > 0)
            {
                WinLossInfo info;
                var reportLength = 0;
                for (var i = 0; i < timeRangeLength; i++)
                {
                    var timeRangeWinLoss = report.ChildsInfo[i];
                    reportLength = timeRangeWinLoss.ChildsInfo.Count;
                    for (var j = 0; j < reportLength; j++)
                    {
                        info = timeRangeWinLoss.ChildsInfo[j];
                        currentColumn = startColumn;
                        if (j == 0)
                        {
                            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + reportLength - 1, currentColumn);
                            currentCellRange.Merged = true;
                            currentCellRange.Value = timeRangeWinLoss.Name;
                            currentCellRange.Style = textStyle;
                            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        }

                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = intNumberStyle;
                        currentCell.Value = info.NoPlayers;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentCell.Value = info.CurrencyName;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNumberStyle;
                        currentCell.Value = info.TotalBets;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNumberStyle;
                        currentCell.Value = info.TotalWins;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.GameIncome;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativePercentNumberStyle;
                        currentCell.Value = info.GamePayout;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.CompanyComm;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.CompanyNetIncome;
                        currentColumn++;
                        currentRow++;
                    }
                }

                this.ExportReport_GrandTotal(
                    report.GrandTotal,
                    ref workSheet,
                    startColumn,
                    ref currentRow,
                    PeriodTypeConstants.GamingBingoStatisticDaily);
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, startColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.There_is_no_data;
                currentCellRange.Style = textStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
            }

            var reportName = string.Format(
                GamingResources.BingoStatistic_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the report by monthly range.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_ByMonthlyRange(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.BingoStatistic_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 8;
            var currentRow = startRow;

            var currentColumn = startColumn;
            CellRange currentCellRange;
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
            
            this.ExportReport_Header(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                PeriodTypeConstants.GamingBingoStatisticMonthly,
                maxColumnIndex,
                criteria);

            var timeRangeLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (timeRangeLength > 0)
            {
                WinLossInfo info;
                var reportLength = 0;
                for (var i = 0; i < timeRangeLength; i++)
                {
                    var timeRangeWinLoss = report.ChildsInfo[i];
                    reportLength = timeRangeWinLoss.ChildsInfo.Count;
                    for (var j = 0; j < reportLength; j++)
                    {
                        info = timeRangeWinLoss.ChildsInfo[j];
                        currentColumn = startColumn;
                        if (j == 0)
                        {
                            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + reportLength - 1, currentColumn);
                            currentCellRange.Merged = true;
                            currentCellRange.Value = timeRangeWinLoss.Name;
                            currentCellRange.Style = textStyle;
                            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        }

                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = intNumberStyle;
                        currentCell.Value = info.NoPlayers;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentCell.Value = info.CurrencyName;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNumberStyle;
                        currentCell.Value = info.TotalBets;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNumberStyle;
                        currentCell.Value = info.TotalWins;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.GameIncome;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativePercentNumberStyle;
                        currentCell.Value = info.GamePayout;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.CompanyComm;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.CompanyNetIncome;
                        currentColumn++;
                        currentRow++;
                    }
                }

                this.ExportReport_GrandTotal(
                    report.GrandTotal,
                    ref workSheet,
                    startColumn,
                    ref currentRow,
                    PeriodTypeConstants.GamingBingoStatisticMonthly);
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, startColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.There_is_no_data;
                currentCellRange.Style = textStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
            }

            var reportName = string.Format(
                GamingResources.BingoStatistic_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the report by company.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_ByCompany(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.BingoStatistic_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 8;
            var currentRow = startRow;

            var currentColumn = startColumn;
            CellRange currentCellRange;
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
            
            this.ExportReport_Header(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                PeriodTypeConstants.GamingBingoStatisticCompany,
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

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = intNumberStyle;
                        currentCell.Value = info.NoPlayers;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentCell.Value = info.CurrencyName;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNumberStyle;
                        currentCell.Value = info.TotalBets;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNumberStyle;
                        currentCell.Value = info.TotalWins;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.GameIncome;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativePercentNumberStyle;
                        currentCell.Value = info.GamePayout;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.CompanyComm;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalNegativeNumberStyle;
                        currentCell.Value = info.CompanyNetIncome;
                        currentColumn++;
                        currentRow++;
                    }
                }

                this.ExportReport_GrandTotal(
                    report.GrandTotal,
                    ref workSheet,
                    startColumn,
                    ref currentRow,
                    PeriodTypeConstants.GamingBingoStatisticCompany);
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, startColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.There_is_no_data;
                currentCellRange.Style = textStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
            }

            var reportName = string.Format(
                GamingResources.BingoStatistic_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the name of the report by user.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_ByUserName(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.BingoStatistic_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 7;
            var currentRow = startRow;

            var currentColumn = startColumn;
            ExcelCell currentCell;
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
            
            this.ExportReport_Header(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                PeriodTypeConstants.GamingBingoStatisticUsername,
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
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentCell.Value = info.CurrencyName;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNumberStyle;
                    currentCell.Value = info.TotalBets;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNumberStyle;
                    currentCell.Value = info.TotalWins;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.GameIncome;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativePercentNumberStyle;
                    currentCell.Value = info.GamePayout;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.CompanyComm;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.CompanyNetIncome;
                    currentColumn++;
                    currentRow++;
                }

                this.ExportReport_GrandTotal(
                    report.GrandTotal,
                    ref workSheet,
                    startColumn,
                    ref currentRow,
                    PeriodTypeConstants.GamingBingoStatisticUsername);
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, startColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.There_is_no_data;
                currentCellRange.Style = textStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
            }

            var reportName = string.Format(
                GamingResources.BingoStatistic_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the report by currency.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_ByCurrency(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.BingoStatistic_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 7;
            var currentRow = startRow;

            var currentColumn = startColumn;
            ExcelCell currentCell;
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

            this.ExportReport_Header(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                PeriodTypeConstants.GamingBingoStatisticCurrency,
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
                    currentCell.Style = intNumberStyle;
                    currentCell.Value = info.NoPlayers;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNumberStyle;
                    currentCell.Value = info.TotalBets;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNumberStyle;
                    currentCell.Value = info.TotalWins;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.GameIncome;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativePercentNumberStyle;
                    currentCell.Value = info.GamePayout;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.CompanyComm;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.CompanyNetIncome;
                    currentColumn++;
                    currentRow++;
                }

                this.ExportReport_GrandTotal(
                    report.GrandTotal,
                    ref workSheet,
                    startColumn,
                    ref currentRow,
                    PeriodTypeConstants.GamingBingoStatisticCurrency);
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, startColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.There_is_no_data;
                currentCellRange.Style = textStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
            }

            var reportName = string.Format(
                GamingResources.BingoStatistic_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the report by agent.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_ByAgent(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.BingoStatistic_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 8;
            var currentRow = startRow;

            var currentColumn = startColumn;
            ExcelCell currentCell;
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
            
            this.ExportReport_Header(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                PeriodTypeConstants.GamingBingoStatisticAgent,
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
                    currentCell.Style = intNumberStyle;
                    currentCell.Value = info.NoPlayers;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentCell.Value = info.CurrencyName;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNumberStyle;
                    currentCell.Value = info.TotalBets;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNumberStyle;
                    currentCell.Value = info.TotalWins;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.GameIncome;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativePercentNumberStyle;
                    currentCell.Value = info.GamePayout;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.CompanyComm;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = twoDecimalNegativeNumberStyle;
                    currentCell.Value = info.CompanyNetIncome;
                    currentColumn++;
                    currentRow++;
                }

                this.ExportReport_GrandTotal(
                    report.GrandTotal,
                    ref workSheet,
                    startColumn,
                    ref currentRow,
                    PeriodTypeConstants.GamingBingoStatisticAgent);
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, startColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.There_is_no_data;
                currentCellRange.Style = textStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
            }

            var reportName = string.Format(
                GamingResources.BingoStatistic_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="periodType">Type of the period.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_Header(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int periodType,
            int maxColumnIndex,
            GetReportCriteria criteria)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var isShowNoPlayers = periodType != PeriodTypeConstants.GamingBingoStatisticUsername;
            var isShowColumnCurrency = periodType != PeriodTypeConstants.GamingBingoStatisticCurrency;
            
            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + maxColumnIndex].Width = 150 * ExcelExtensions.FontWidth;

            var title = string.Format(
                GamingResources.BingoStatistic_ReportTitle,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.GamingBingoStatisticDaily:
                    currentCell.Value = Labels.Date;
                    break;
                case PeriodTypeConstants.GamingBingoStatisticMonthly:
                    currentCell.Value = Labels.Month;
                    break;
                case PeriodTypeConstants.GamingBingoStatisticUsername:
                    currentCell.Value = Labels.User_Name;
                    break;
                case PeriodTypeConstants.GamingBingoStatisticCompany:
                    currentCell.Value = Labels.Company;
                    break;
                case PeriodTypeConstants.GamingBingoStatisticCurrency:
                    currentCell.Value = Labels.Currency;
                    break;
                case PeriodTypeConstants.GamingBingoStatisticAgent:
                    currentCell.Value = Labels.Agent;
                    break;
            }

            currentColumn++;

            if (isShowNoPlayers)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.No_Players;
                currentColumn++;
            }

            if (isShowColumnCurrency)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Currency;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Total_Bets;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Total_Wins;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Game_income;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Game_payout;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company_comm;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company_net_income;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the report grand total.
        /// </summary>
        /// <param name="grandTotal">The grand total.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="periodType">Type of the period.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_GrandTotal(
            WinLossInfo grandTotal,
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            int periodType)
        {
            var grandTotalColor = ExcelExtensions.GrandTotalRowColor;
            var textStyle_GrandTotal = ExcelExtensions.GetCellTextStyle(grandTotalColor, true);
            var intNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.IntNumberFormat, grandTotalColor, true, false, false);
            var twoDecimalNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                false);
            var twoDecimalNegativeNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                true);
            var twoDecimalNegativePercentNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalColor,
                true,
                false,
                true);

            var grandTotalSeparatorColor = ExcelExtensions.GrandTotalSeparatorColor;
            var textStyle_GrandTotal_1stRow = ExcelExtensions.GetCellTextStyle(grandTotalColor, true);
            var intNumberStyle_GrandTotal_1stRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.IntNumberFormat,
                grandTotalColor,
                true,
                false,
                false);
            var twoDecimalNumberStyle_GrandTotal_1stRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                false);
            var twoDecimalNegativeNumberStyle_GrandTotal_1stRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                true);
            var twoDecimalNegativePercentNumberStyle_GrandTotal_1stRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalColor,
                true,
                false,
                true);
            textStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            intNumberStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNumberStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNegativeNumberStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNegativePercentNumberStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);

            var currentColumn = startColumn;
            ExcelCell currentCell;
            
            WinLossInfo info;
            var reportLength = grandTotal.ChildsInfo.Count;
            var isGrandTotalFirstRow = true;
            var isShowNoPlayers = periodType != PeriodTypeConstants.GamingBingoStatisticUsername;
            var isShowColumnCurrency = periodType != PeriodTypeConstants.GamingBingoStatisticCurrency;
            for (var i = 0; i < reportLength; i++)
            {                
                info = grandTotal.ChildsInfo[i];
                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Name;
                currentCell.Style = isGrandTotalFirstRow ?
                    textStyle_GrandTotal_1stRow :
                    textStyle_GrandTotal;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentColumn++;

                if (isShowNoPlayers)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = isGrandTotalFirstRow ?
                        intNumberStyle_GrandTotal_1stRow :
                        intNumberStyle_GrandTotal;
                    currentCell.Value = info.NoPlayers;
                    currentColumn++;
                }

                if (isShowColumnCurrency)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = isGrandTotalFirstRow ?
                        textStyle_GrandTotal_1stRow :
                        textStyle_GrandTotal;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentCell.Value = info.CurrencyName;
                    currentColumn++;
                }

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = isGrandTotalFirstRow ?
                    twoDecimalNumberStyle_GrandTotal_1stRow :
                    twoDecimalNumberStyle_GrandTotal;
                currentCell.Value = info.TotalBets;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = isGrandTotalFirstRow ?
                    twoDecimalNumberStyle_GrandTotal_1stRow :
                    twoDecimalNumberStyle_GrandTotal;
                currentCell.Value = info.TotalWins;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = isGrandTotalFirstRow ?
                    twoDecimalNegativeNumberStyle_GrandTotal_1stRow :
                    twoDecimalNegativeNumberStyle_GrandTotal;
                currentCell.Value = info.GameIncome;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = isGrandTotalFirstRow ?
                    twoDecimalNegativePercentNumberStyle_GrandTotal_1stRow :
                    twoDecimalNegativePercentNumberStyle_GrandTotal;
                currentCell.Value = info.GamePayout;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = isGrandTotalFirstRow ?
                    twoDecimalNegativeNumberStyle_GrandTotal_1stRow :
                    twoDecimalNegativeNumberStyle_GrandTotal;
                currentCell.Value = info.CompanyComm;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = isGrandTotalFirstRow ?
                    twoDecimalNegativeNumberStyle_GrandTotal_1stRow :
                    twoDecimalNegativeNumberStyle_GrandTotal;
                currentCell.Value = info.CompanyNetIncome;
                currentColumn++;
                currentRow++;

                if (isGrandTotalFirstRow)
                {
                    isGrandTotalFirstRow = false;
                }
            }

                        info = grandTotal;
            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle_GrandTotal;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            if (isShowNoPlayers)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = intNumberStyle_GrandTotal;
                currentCell.Value = info.NoPlayers;
                currentColumn++;
            }

            if (isShowColumnCurrency)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = textStyle_GrandTotal;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCell.Value = info.CurrencyName;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle_GrandTotal;
            currentCell.Value = info.TotalBets;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle_GrandTotal;
            currentCell.Value = info.TotalWins;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle_GrandTotal;
            currentCell.Value = info.GameIncome;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativePercentNumberStyle_GrandTotal;
            currentCell.Value = info.GamePayout;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle_GrandTotal;
            currentCell.Value = info.CompanyComm;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle_GrandTotal;
            currentCell.Value = info.CompanyNetIncome;
            currentColumn++;
            currentRow++;
        }
    }
}