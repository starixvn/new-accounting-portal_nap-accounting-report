// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameStatisticService.cs" company="Nexcel Solutions Vietnam">
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
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Services.Impl.Interface;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class CasinoGameStatisticService : ServiceBase, ICasinoGameStatisticService
    {
        private int _timeCaching = 2;
        private readonly ICasinoGameStatisticRepository _reportRepository;        

        private readonly string _reportSessionKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.Report" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private readonly string _reportDrillDownSessionKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.ReportDrillDown" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private string _grandTotalSessionKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.GrandTotal" +
       (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private string _hourlySessionKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.Hourly" +
       (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        public CasinoGameStatisticService()
            : this(null)
        {
        }

        public CasinoGameStatisticService(ICasinoGameStatisticRepository reportRepository)
        {
            _reportRepository = reportRepository ?? new CasinoGameStatisticRepository();            
        }       

        public WinLossInfo GetReport(GetReportByDailyCriteria criteria)
        {
            try
            {
                var rawData = _reportRepository.GetReport(criteria);
                var report = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo()
                };

                if (rawData == null || rawData.Count < 2)
                {
                    CacheManager.Set<WinLossInfo>(_reportSessionKey, report, DateTime.Now.AddHours(_timeCaching));
                    return report;
                }

                var winLossInfoTable = rawData[0];
                var grandTotalInfoTable = rawData[1].FirstOrDefault();
                if (winLossInfoTable == null ||
                    winLossInfoTable.Count == 0)
                {
                    CacheManager.Set<WinLossInfo>(_reportSessionKey, report, DateTime.Now.AddHours(_timeCaching));
                    return report;
                }

                CasinoGameStatisticService reportManager = null;
                switch (criteria.ReportType)
                {
                    case PeriodTypeConstants.LiveCasinoGameStatisticDaily:
                        reportManager = new CasinoGameStatisticServiceByDailyRange();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticMonthly:
                        reportManager = new CasinoGameStatisticServiceByMonthlyRange();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticUsername:
                        reportManager = new CasinoGameStatisticServiceByUsername();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticGameType:
                        reportManager = new CasinoGameStatisticServiceByGameType();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticGameTable:
                        reportManager = new CasinoGameStatisticServiceByGameTable();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                        reportManager = new CasinoGameStatisticServiceByCompany();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticCurrency:
                        reportManager = new CasinoGameStatisticServiceByCurrency();
                        break;

                    default:
                        reportManager = new CasinoGameStatisticService();
                        break;
                }

                report = reportManager.GetReport(winLossInfoTable, grandTotalInfoTable, criteria);
                string key = criteria.IsDrillDown ? _reportSessionKey : _reportDrillDownSessionKey;
                CacheManager.Set<WinLossInfo>(key, report, DateTime.Now.AddHours(_timeCaching));
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                CacheManager.Set<WinLossInfo>(_reportSessionKey, null);
                CacheManager.Set<WinLossInfo>(_reportDrillDownSessionKey, null);
                throw;
            }
        }

        public virtual WinLossInfo GetReport(List<WinLossInfoEntity> winLossInfoTable, WinLossInfoEntity grandTotal, GetReportByDailyCriteria criteria)
        {
            throw new Exception(Labels.Error_MethodNotImplemented);
        }

        /// <summary>
        /// Sorts the report.
        /// </summary>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>Report data after sort.</returns>
        public WinLossInfo SortReport(GetReportByDailyCriteria getReportCriteria)
        {
            CasinoGameStatisticService reportManager = null;
            switch (getReportCriteria.ReportType)
            {
                case PeriodTypeConstants.CasinoGameStatisticDaily:
                    reportManager = new CasinoGameStatisticServiceByDailyRange();
                    break;

                case PeriodTypeConstants.CasinoGameStatisticMonthly:
                    reportManager = new CasinoGameStatisticServiceByMonthlyRange();
                    break;

                case PeriodTypeConstants.CasinoGameStatisticUsername:
                    reportManager = new CasinoGameStatisticServiceByUsername();
                    break;

                case PeriodTypeConstants.CasinoGameStatisticGameType:
                    reportManager = new CasinoGameStatisticServiceByGameType();
                    break;

                case PeriodTypeConstants.CasinoGameStatisticGameTable:
                    reportManager = new CasinoGameStatisticServiceByGameTable();
                    break;

                case PeriodTypeConstants.CasinoGameStatisticCompany:
                    reportManager = new CasinoGameStatisticServiceByCompany();
                    break;

                case PeriodTypeConstants.CasinoGameStatisticCurrency:
                    reportManager = new CasinoGameStatisticServiceByCurrency();
                    break;

                default:
                    reportManager = new CasinoGameStatisticService();
                    break;
            }

            string key = getReportCriteria.IsDrillDown ? _reportSessionKey : _reportDrillDownSessionKey;
            var report = CacheManager.Get<WinLossInfo>(key);

            if (report == null)
            {
                report = GetReport(getReportCriteria);
            }

            reportManager.SortReportData(report, getReportCriteria);

            return report;
        }

        public virtual WinLossInfo SortReportData(WinLossInfo report, GetReportByDailyCriteria getReportCriteria)
        {
            throw new NotImplementedException(Labels.Error_MethodNotImplemented);
        }

        public virtual void ExportReport(
            WinLossInfo report,
            GetReportByDailyCriteria criteria)
        {
            throw new Exception(Labels.Error_MethodNotImplemented);
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport(GetReportByDailyCriteria criteria)
        {
            try
            {
                WinLossInfo report;
                string key = criteria.IsDrillDown ? _reportSessionKey : _reportDrillDownSessionKey;
                report = CacheManager.Get<WinLossInfo>(key);

                if (report == null)
                {
                    report = GetReport(criteria);
                }

                CasinoGameStatisticService reportManager = null;
                switch (criteria.ReportType)
                {
                    case PeriodTypeConstants.LiveCasinoGameStatisticDaily:
                        reportManager = new CasinoGameStatisticServiceByDailyRange();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticMonthly:
                        reportManager = new CasinoGameStatisticServiceByMonthlyRange();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticUsername:
                        reportManager = new CasinoGameStatisticServiceByUsername();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticGameType:
                        reportManager = new CasinoGameStatisticServiceByGameType();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticGameTable:
                        reportManager = new CasinoGameStatisticServiceByGameTable();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                        reportManager = new CasinoGameStatisticServiceByCompany();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticCurrency:
                        reportManager = new CasinoGameStatisticServiceByCurrency();
                        break;

                    default:
                        reportManager = new CasinoGameStatisticService();
                        break;
                }

                reportManager.ExportReport(report, criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Determines whether [is correct filtered information] [the specified criteria].
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="info">The information.</param>
        /// <returns>Is correct filtered information.</returns>
        public bool IsCorrectFilteredInfo(GetReportByDailyCriteria criteria, WinLossInfo info)
        {
            return (string.IsNullOrEmpty(criteria.TotalBetLessThan) ||
                    info.TotalBets <= Converter.ToDecimal(criteria.TotalBetLessThan)) &&
                   (string.IsNullOrEmpty(criteria.TotalBetMoreThan) ||
                    Converter.ToDecimal(criteria.TotalBetMoreThan) <= info.TotalBets) &&
                   (string.IsNullOrEmpty(criteria.TotalWinLessThan) ||
                    info.TotalWins <= Converter.ToDecimal(criteria.TotalWinLessThan)) &&
                   (string.IsNullOrEmpty(criteria.TotalWinMoreThan) ||
                    Converter.ToDecimal(criteria.TotalWinMoreThan) <= info.TotalWins);
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
        public void ExportReport_Header(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int periodType,
            int maxColumnIndex,
            GetReportByDailyCriteria criteria)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var isShowNoPlayers = periodType != PeriodTypeConstants.LiveCasinoGameStatisticUsername;
            var isShowColumnCurrency = periodType != PeriodTypeConstants.LiveCasinoGameStatisticCurrency;

            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 1].Width = 90 * ExcelExtensions.FontWidth;
            if (isShowNoPlayers)
            {
                workSheet.Columns[startColumn + 2].Width = 90 * ExcelExtensions.FontWidth;
            }

            if (isShowColumnCurrency)
            {
                workSheet.Columns[startColumn + 3 - (isShowNoPlayers ? 0 : 1)].Width = 90 * ExcelExtensions.FontWidth;
            }

            var columnCompanyTurnoverIndex = 9 - (criteria.ShowRefundColumn ? 0 : 1) -
                (isShowNoPlayers ? 0 : 1) - (isShowColumnCurrency ? 0 : 1);
            workSheet.Columns[startColumn + columnCompanyTurnoverIndex].Width = 140 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + maxColumnIndex].Width = 150 * ExcelExtensions.FontWidth;

            var title = string.Format(
                GamingResources.CasinoGameStatistic_ReportTitle,
                criteria.Product,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());

            if (criteria.IsDrillDown)
            {
                title = string.Format(
                GamingResources.CasinoGameStatisticOnDateByCurrency_ReportTitle,
                criteria.Product,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate());
            }

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
                case PeriodTypeConstants.CasinoGameStatisticDaily:
                    currentCell.Value = Labels.Date;
                    break;

                case PeriodTypeConstants.CasinoGameStatisticMonthly:
                    currentCell.Value = Labels.Month;
                    break;

                case PeriodTypeConstants.CasinoGameStatisticUsername:
                    currentCell.Value = Labels.UserName;
                    
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = headerTextStyle;                    
                    currentCell.Value = Labels.CustId;
                    break;

                case PeriodTypeConstants.CasinoGameStatisticGameType:
                    currentCell.Value = Labels.Game;
                    break;

                case PeriodTypeConstants.CasinoGameStatisticGameTable:
                    currentCell.Value = Labels.Table;
                    break;

                case PeriodTypeConstants.CasinoGameStatisticCompany:
                    currentCell.Value = Labels.Company;
                    break;

                case PeriodTypeConstants.CasinoGameStatisticCurrency:
                    currentCell.Value = Labels.Currency;
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

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Bet_Count;
            currentColumn++;

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

            if (criteria.ShowRefundColumn)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Total_refund;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company_Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company_comm;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company_net_income;

            workSheet.Rows[currentRow].Height = 20 * ExcelExtensions.FontWidth;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the report_ win loss row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="intNumberStyle">The integer number style.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalNegativeNumberStyle">The two decimal negative number style.</param>
        /// <param name="twoDecimalNegativePercentNumberStyle">The two decimal negative percent number style.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="info">The information.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport_WinLossRow(
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            CellStyle intNumberStyle,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalNegativeNumberStyle,
            CellStyle twoDecimalNegativePercentNumberStyle,
            GetReportByDailyCriteria criteria,
            WinLossInfo info)
        {
            ExcelCell currentCell;
            var isShowNoPlayers = criteria.ReportType != PeriodTypeConstants.LiveCasinoGameStatisticUsername;
            var isShowColumnCurrency = criteria.ReportType != PeriodTypeConstants.LiveCasinoGameStatisticCurrency;

            var currentColumn = startColumn;
            if (isShowNoPlayers)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = intNumberStyle;
                currentCell.Value = info.NumberOfPlayers;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = intNumberStyle;
            currentCell.Value = info.NumberOfGames;
            currentColumn++;

            if (isShowColumnCurrency)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCell.Value = info.CurrencyName;
                currentColumn++;
            }

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

            if (criteria.ShowRefundColumn)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNegativeNumberStyle;
                currentCell.Value = info.TotalRefund;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = info.CompanyTurnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = info.CompanyCommission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = info.CompanyNetIncome;
            currentColumn++;
        }

        /// <summary>
        /// Exports the report grand total.
        /// </summary>
        /// <param name="grandTotal">The grand total.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport_GrandTotal(
            WinLossInfo grandTotal,
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            GetReportByDailyCriteria criteria)
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

            textStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            intNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNegativeNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNegativePercentNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);

            var currentColumn = startColumn;
            ExcelCell currentCell;

            WinLossInfo info;
            var reportLength = grandTotal.ChildsInfo.Count;
            var isShowNoPlayers = criteria.ReportType != PeriodTypeConstants.LiveCasinoGameStatisticUsername;
            var isShowColumnCurrency = criteria.ReportType != PeriodTypeConstants.LiveCasinoGameStatisticCurrency;

            info = grandTotal.ChildsInfo[reportLength - 1];
            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle_GrandTotal;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            if (criteria.ReportType == PeriodTypeConstants.LiveCasinoGameStatisticUsername)
            {
                currentColumn++;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = string.Empty;
                currentCell.Style = textStyle_GrandTotal;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            }

            currentColumn++;

            ExportReport_WinLossRow(
                ref workSheet,
                currentColumn,
                ref currentRow,
                intNumberStyle_GrandTotal,
                textStyle_GrandTotal,
                twoDecimalNumberStyle_GrandTotal,
                twoDecimalNegativeNumberStyle_GrandTotal,
                twoDecimalNegativePercentNumberStyle_GrandTotal,
                criteria,
                info);
            currentRow++;
        }

        public void ExportReport_ChildTotal(
            WinLossInfo grandTotal,
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            GetReportByDailyCriteria criteria)
        {
            var totalRowColor = ExcelExtensions.TotalRowColor;
            var textStyle_SubTotal = ExcelExtensions.GetCellTextStyle(totalRowColor, true);
            var intNumberStyle_SubTotal = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.IntNumberFormat, totalRowColor, true, false, false);
            var twoDecimalNumberStyle_SubTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                totalRowColor,
                true,
                false,
                false);
            var twoDecimalNegativeNumberStyle_SubTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                totalRowColor,
                true,
                false,
                true);
            var twoDecimalNegativePercentNumberStyle_SubTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                totalRowColor,
                true,
                false,
                true);

            var currentColumn = startColumn;
            ExcelCell currentCell;

            GetReportByGrandRMCriteria grandRMCriteria = new GetReportByGrandRMCriteria()
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
                UserName = criteria.UserName,
                ExcludeTestData = criteria.ExcludeTestData,
                LicenseeId = criteria.LicenseeId,
                GameTypeId = criteria.GameTypeId,
                GameTableId = criteria.GameTableId,
                Currency = criteria.Currency,
                ShowRefundColumn = criteria.ShowRefundColumn,
                ProductId = criteria.ProductId,
                Product = criteria.Product
            };

            List<WinLossInfo> totalList = new List<WinLossInfo>();
            if (!criteria.IsDrillDown)
            {
                totalList = _reportRepository.GetReportByGrandTotal(grandRMCriteria);
                totalList.RemoveAt(totalList.Count - 1);
            }

            foreach (WinLossInfo info in totalList)
            {
                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Name;
                currentCell.Style = textStyle_SubTotal;

                if (criteria.ReportType == PeriodTypeConstants.LiveCasinoGameStatisticUsername)
                {
                    currentColumn++;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = string.Empty;
                    currentCell.Style = textStyle_SubTotal;
                }

                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentColumn++;

                ExportReport_WinLossRow(
                    ref workSheet,
                    currentColumn,
                    ref currentRow,
                    intNumberStyle_SubTotal,
                    textStyle_SubTotal,
                    twoDecimalNumberStyle_SubTotal,
                    twoDecimalNegativeNumberStyle_SubTotal,
                    twoDecimalNegativePercentNumberStyle_SubTotal,
                    criteria,
                    info);

                currentRow++;
            }
        }

        public WinLossInfo GetReportByGrandTotal(GetReportByGrandRMCriteria criteria)
        {
            WinLossInfo grandTotal = new WinLossInfo();

            if (criteria.IsExportExcel)
            {
                grandTotal = CacheManager.Get<WinLossInfo>(_grandTotalSessionKey);
                if (grandTotal == null)
                {
                    grandTotal.ChildsInfo = _reportRepository.GetReportByGrandTotal(criteria);        
                }
            }
            else
            {
                grandTotal.ChildsInfo = _reportRepository.GetReportByGrandTotal(criteria);
                CacheManager.Set<WinLossInfo>(_grandTotalSessionKey, grandTotal, DateTime.Now.AddHours(_timeCaching));
            }
            
            return grandTotal;
        }

        public Hourly GetReportByHourly(GetReportByHourlyCriteria criteria)
        {
            Hourly hourly = new Hourly();
            if (criteria.IsExportExcel)
            {
                hourly = CacheManager.Get<Hourly>(_hourlySessionKey) ?? _reportRepository.GetReportByHourly(criteria);
            }
            else
            {
                hourly = _reportRepository.GetReportByHourly(criteria);
                CacheManager.Set<Hourly>(_hourlySessionKey, hourly, DateTime.Now.AddHours(_timeCaching));
            }
            
            return hourly;
        }
    }
}