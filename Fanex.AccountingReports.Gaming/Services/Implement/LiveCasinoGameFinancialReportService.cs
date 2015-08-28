// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameFinancialReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.LiveCasinoGameFinancialReport
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Logging;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoGameFinancialReportService : ServiceBase, ILiveCasinoGameFinancialReportService
    {
        private ILiveCasinoGameFinancialReportRepository _reportRepository;

        public LiveCasinoGameFinancialReportService() :
            this(null)
        {
        }

        public LiveCasinoGameFinancialReportService(ILiveCasinoGameFinancialReportRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new LiveCasinoGameFinancialReportRepository();
        }

        /// <summary>
        /// Gets the licensees.
        /// </summary>
        /// <returns>The licensees.</returns>
        public List<BaseModel> GetLicensees()
        {
            try
            {
                var rawData = GetReportData<dynamic>(new GetResellerCriteria
                {
                    ExcludeTest = true,
                    ExcludeBA = false,
                    ProductCode = Labels.LiveCasinoProductCode
                });

                var liveCasinoFinancialReport_Licensee = AppSettings.LiveCasinoFinancialReport_Licensee;
                var report = new List<BaseModel>();

                foreach (var data in rawData)
                {
                    var licenseeID = Convert.ToInt32(data.LicenseeID);
                    if (liveCasinoFinancialReport_Licensee.Contains(licenseeID))
                    {
                        var info = new BaseModel
                        {
                            Id = licenseeID,
                            Name = data.LicenseeName.ToString(),
                            AliasName = data.ResellerName.ToString()
                        };

                        report.Add(info);
                    }
                }

                report = report.OrderBy(info => info.Name).ToList();
                return report;
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Gets the currencies.
        /// </summary>
        /// <returns>The currencies.</returns>
        public List<BaseModel> GetCurrencies()
        {
            try
            {
                var rawData = this._reportRepository.GetCurrencies();
                var liveCasinoFinancialReport_Currency = AppSettings.LiveCasinoFinancialReport_Currency;
                var report = new List<BaseModel>();

                foreach (var data in rawData)
                {
                    var exchangeID = Convert.ToInt32(data.Id);

                    if (liveCasinoFinancialReport_Currency.Contains(exchangeID))
                    {
                        var info = new BaseModel
                        {
                            Id = exchangeID,
                            Name = data.Name.ToString()
                        };

                        report.Add(info);
                    }
                }

                report = report.OrderBy(info => info.Name).ToList();
                return report;
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Gets the game types.
        /// </summary>
        /// <returns>The game types.</returns>
        public List<BaseModel> GetGameTypes()
        {
            try
            {
                var rawData = this._reportRepository.GetGameTypes();
                var liveCasinoFinancialReport_GameType = AppSettings.LiveCasinoFinancialReport_GameType;
                var report = new List<BaseModel>();

                foreach (var data in rawData)
                {
                    var typeID = Convert.ToInt32(data.typeID);

                    if (liveCasinoFinancialReport_GameType.Contains(typeID))
                    {
                        var info = new BaseModel
                        {
                            Id = typeID,
                            Name = data.typenamee.ToString()
                        };
                        report.Add(info);
                    }
                }

                report = report.OrderBy(info => info.Name).ToList();
                return report;
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Gets the game tables.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The game tables.</returns>
        public List<BaseModel> GetGameTables(GetGameTableCriteria criteria)
        {
            try
            {
                if (!criteria.IsValid())
                {
                    return new List<BaseModel>();
                }

                var rawData = this._reportRepository.GetGameTables(criteria);
                var report = new List<BaseModel>();

                foreach (var data in rawData)
                {
                    var info = new BaseModel
                    {
                        Id = Convert.ToInt32(data.GameTableID),
                        Name = data.GameTable.ToString()
                    };

                    report.Add(info);
                }

                report = report.OrderBy(info => info.Name).ToList();
                return report;
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Gets the financial report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The financial report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public FinancialReport GetFinancialReport(GetFinancialReportCriteria criteria)
        {
            try
            {
                Hashtable outputParam = new Hashtable();
                var rawData = this._reportRepository.GetFinancialReport(criteria, ref outputParam);
                FinancialReport report = new FinancialReport
                {
                    ChildInfo = new List<FinancialReport>(),
                    GrandTotal = new FinancialReport
                    {
                        Name = string.Format("{0} ({1})", Labels.Grand_Total, Labels.USD)
                    }
                };

                bool isWholeMonthSelected = this.IsWholeMonth(criteria.DateStart, criteria.DateEnd);
                decimal grandTotalNetIncome = 0;

                report.GameLevy = Convert.ToDecimal(outputParam["gaminglevy"]);
                report.GameLevyToPay = Convert.ToDecimal(outputParam["prorated"]);
                var totalDay = (criteria.DateEnd - criteria.DateStart).Days + 1;
                var gameLevyToPay = (report.GameLevyToPay * totalDay) / DateTime.DaysInMonth(criteria.DateStart.Year, criteria.DateStart.Month);
                foreach (var data in rawData)
                {
                    var info = new FinancialReport
                    {
                        Name = data.TableName,
                        PlayerCount = Convert.ToInt32(data.players),
                        GameCount = Convert.ToInt32(data.games),
                        TotalBets = Convert.ToDecimal(data.totalbets),
                        TotalWins = Convert.ToDecimal(data.totalwins),
                        GrossIncome = Convert.ToDecimal(data.GrossIncome),
                        Commission = Convert.ToDecimal(data.Comm),
                        NetIncome = Convert.ToDecimal(data.NetIncome)
                    };

                    info.GamePayout = this.CaculateGamePayout(info);
                    info.GameLevy = report.GameLevy * info.NetIncome;
                    if (isWholeMonthSelected && info.GameLevy < 0)
                    {
                        info.GameLevy = 0;
                    }
                    else
                    {
                        grandTotalNetIncome += data.NetIncome;
                    }

                    info.GameLevyToPay = Math.Max(gameLevyToPay, info.GameLevy);

                    report.AddToGrandTotal(info);
                    info = this.FormatFinancialReport(info);
                    report.ChildInfo.Add(info);
                }

                report.GrandTotal.PlayerCount = Convert.ToInt32(outputParam["totalplayers"]);
                report.GrandTotal.GameCount = Convert.ToInt32(outputParam["totalgames"]);
                report.GrandTotal.GamePayout = this.CaculateGamePayout(report.GrandTotal);
                report.GrandTotal.GameLevy = report.GameLevy * grandTotalNetIncome;
                SessionExtension.AddSession(HttpContext.Current.Session, GamingConstants.LiveCasinoGameFinancialReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }

            SessionExtension.AddSession(HttpContext.Current.Session, GamingConstants.LiveCasinoGameFinancialReportSessionKey, null);
            return null;
        }

        /// <summary>
        /// Exports the financial report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportFinancialReport(GetFinancialReportCriteria criteria)
        {
            try
            {
                var report = (FinancialReport)SessionExtension.GetSession(HttpContext.Current.Session, GamingConstants.LiveCasinoGameFinancialReportSessionKey);
                if (report == null)
                {
                    report = this.GetFinancialReport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = GamingResources.LiveCasinoGameFinancialReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);
                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                var headerColor = ExcelExtensions.HeaderTitleColor;
                var grandTotalColor = ExcelExtensions.GrandTotalRowColor;
                var backGroundColor = ExcelExtensions.BackGroundColor;

                var headerText = ExcelExtensions.GetCellTitleStyle(headerColor);
                var textType = ExcelExtensions.GetCellTextStyle(backGroundColor, false);
                var intType = ExcelExtensions.GetCellNumberStyle(string.Empty, backGroundColor, false, false, false);
                var negativeTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backGroundColor, false, false, true);

                var textType_GrandTotal = ExcelExtensions.GetCellTextStyle(grandTotalColor, true);
                var intType_GrandTotal = ExcelExtensions.GetCellNumberStyle(string.Empty, grandTotalColor, true, false, false);
                var negativeTwoDecimalNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalColor,
                    true,
                    false,
                    true);

                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 10);
                currentCellRange.Merged = true;
                currentCellRange.Value = string.Format(
                    GamingResources.Live_Casino_Game_Financial_Report_Title,
                    criteria.DateStart.ToShortFormatedDate(),
                    criteria.DateEnd.ToShortFormatedDate());
                currentCellRange.Style = headerText;
                currentRow += 2;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 10);
                currentCellRange.Merged = true;
                currentCellRange.Value = string.Empty;
                currentCellRange.Style = textType;
                currentRow++;

                ////FirstRow
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Table;
                currentCellRange.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 120 * 40;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.LiveCasinoGameFinancialReport_Players;
                currentCellRange.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 80 * 40;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.LiveCasinoGameFinancialReport_Games;
                currentCellRange.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 80 * 40;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Company;
                currentCellRange.Style = headerText;
                currentColumn += 5;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.LiveCasinoGameFinancialReport_GamePayout;
                currentCellRange.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 100 * 40;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = string.Format(Labels.LiveCasinoGameFinancialReport_GameLevy, Math.Round(report.GameLevy * 100, 0));
                currentCellRange.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 100 * 40;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Game_Levy_to_pay;
                currentCellRange.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 100 * 40;
                currentColumn++;
                currentRow++;

                ////SecondRow
                currentColumn = startColumn + 3;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Total_Bets;
                currentCell.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 100 * 40;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Total_Wins;
                currentCell.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 100 * 40;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Gross_Income;
                currentCell.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 100 * 40;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Comm;
                currentCell.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 100 * 40;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Net_Income;
                currentCell.Style = headerText;
                workSheet.Columns[startColumn + currentColumn].Width = 100 * 40;
                currentColumn++;
                currentRow++;
                
                if (report != null && report.ChildInfo.Count > 0)
                {
                    var length = report.ChildInfo.Count;
                    FinancialReport info;
                    for (var i = 0; i < length; i++)
                    {
                        info = report.ChildInfo[i];
                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Name;
                        currentCell.Style = textType;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.PlayerCount;
                        currentCell.Style = intType;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.GameCount;
                        currentCell.Style = intType;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.TotalBets;
                        currentCell.Style = negativeTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.TotalWins;
                        currentCell.Style = negativeTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.GrossIncome;
                        currentCell.Style = negativeTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = negativeTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.NetIncome;
                        currentCell.Style = negativeTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.GamePayout * 100;
                        currentCell.Style = negativeTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.GameLevy;
                        currentCell.Style = negativeTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.GameLevyToPay;
                        currentCell.Style = negativeTwoDecimalNumberStyle;
                        currentColumn++;
                        currentRow++;
                    }

                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = textType_GrandTotal;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.PlayerCount;
                    currentCell.Style = intType_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.GameCount;
                    currentCell.Style = intType_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.TotalBets;
                    currentCell.Style = negativeTwoDecimalNumberStyle_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.TotalWins;
                    currentCell.Style = negativeTwoDecimalNumberStyle_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.GrossIncome;
                    currentCell.Style = negativeTwoDecimalNumberStyle_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Commission;
                    currentCell.Style = negativeTwoDecimalNumberStyle_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.NetIncome;
                    currentCell.Style = negativeTwoDecimalNumberStyle_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.GamePayout * 100;
                    currentCell.Style = negativeTwoDecimalNumberStyle_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.GameLevy;
                    currentCell.Style = negativeTwoDecimalNumberStyle_GrandTotal;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.GameLevyToPay;
                    currentCell.Style = negativeTwoDecimalNumberStyle_GrandTotal;
                    currentColumn++;
                    currentRow++;
                }
                else
                {
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 10);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Messages.Thereisnodata;
                    currentCellRange.Style = textType;
                    currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentRow++;
                }

                var reportName = string.Format(
                    GamingResources.LiveCasinoGameFinancialReport_ReportName,
                    criteria.DateStart.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.DateEnd.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }
        }

        /// <summary>
        /// Formats the financial report.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>The financial report.</returns>
        private FinancialReport FormatFinancialReport(FinancialReport info)
        {
            info.TotalBets = Math.Round(info.TotalBets, 2);
            info.TotalWins = Math.Round(info.TotalWins, 2);
            info.GrossIncome = Math.Round(info.GrossIncome, 2);
            info.Commission = Math.Round(info.Commission, 2);
            info.NetIncome = Math.Round(info.NetIncome, 2);
            info.GamePayout = Math.Round(info.GamePayout, 4);
            info.GameLevy = Math.Round(info.GameLevy, 2);
            info.GameLevyToPay = Math.Round(info.GameLevyToPay, 2);

            return info;
        }

        private decimal CaculateGamePayout(FinancialReport info)
        {
            if (info.TotalWins != 0)
            {
                return info.TotalBets / info.TotalWins;
            }

            return 0;
        }

        private bool IsWholeMonth(DateTime fromDate, DateTime toDate)
        {
            // exclude time
            fromDate = fromDate.Date;
            toDate = toDate.Date;

            DateTime firstDayOfMonth = fromDate.FirstDayOfMonth();
            DateTime lastDayOfMonth = fromDate.LastDayOfMonth();

            return fromDate == firstDayOfMonth && toDate == lastDayOfMonth;
        }
    }
}
