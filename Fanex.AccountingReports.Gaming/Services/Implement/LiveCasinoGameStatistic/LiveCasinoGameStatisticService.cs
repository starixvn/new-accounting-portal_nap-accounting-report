// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameStatisticService.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Services.Impl.Interface;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class LiveCasinoGameStatisticService : ServiceBase, ILiveCasinoGameStatisticService
    {
        private readonly ILiveCasinoGameStatisticRepository _reportRepository;
        private readonly ILiveCasinoProductService _liveCasinoProductService;
        private readonly IUserGroupService _userGroupService;

        private readonly string _reportSessionKey = "Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.Report" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private string _productSessionKey = "Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.Products" +
       (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        public LiveCasinoGameStatisticService()
            : this(null, null, null)
        {
        }

        public LiveCasinoGameStatisticService(ILiveCasinoGameStatisticRepository reportRepository, ILiveCasinoProductService liveCasinoProductService, IUserGroupService userGroupService)
        {
            _reportRepository = reportRepository ?? new LiveCasinoGameStatisticRepository();
            _liveCasinoProductService = liveCasinoProductService ?? new LiveCasinoProductService();
            _userGroupService = userGroupService ?? new UserGroupService();
        }

        public List<BaseModel> GetProductList()
        {
            try
            {
                bool isU3Account = this.CheckUserIsU3Account();
                List<BaseModel> result;
                if (isU3Account)
                {
                    result = _liveCasinoProductService.GetU3ProductList();
                }
                else
                {
                    result = _liveCasinoProductService.GetProductList();
                }

                SessionManager.Set<List<BaseModel>>(_productSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the currency list.
        /// </summary>
        /// <returns>The currency list.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<EntityBase<int, string>> GetCurrencyList(GetExchangeCurrenciesCriteria criteria)
        {
            var result = new List<EntityBase<int, string>>();
            try
            {
                Dictionary<int, string> currencies = ReportService.GetExchangeCurrencies(criteria);
                var currencyList = new List<EntityBase<int, string>>();

                foreach (var row in currencies)
                {
                    if (ReportService.IsExcludeCurrency(row.Value))
                    {
                        continue;
                    }

                    currencyList.Add(new EntityBase<int, string>()
                    {
                        Id = row.Key,
                        Name = row.Value
                    });
                }

                currencyList = currencyList.OrderBy(currency => currency.Name).ToList();

                result.Add(new EntityBase<int, string>()
                {
                    Id = (int)CoreConstants.CurrencyContants.All,
                    Name = Labels.All
                });

                result.Add(new EntityBase<int, string>()
                {
                    Id = (int)CoreConstants.CurrencyContants.BaseRM,
                    Name = Labels.Base_RM
                });

                foreach (var currency in currencyList)
                {
                    result.Add(new EntityBase<int, string>()
                    {
                        Id = currency.Id,
                        Name = currency.Name
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        public List<ProductGameType> GetProductGameTypes(GetGameTypeListCriteria criteria)
        {
            try
            {
                List<ProductGameType> result = new List<ProductGameType>();
                var products = GetProductFromCache();
                var gameTypes = GetReportData<dynamic>(criteria);
                if (gameTypes == null)
                {
                    return result;
                }

                var productGameTypeGroups = gameTypes.GroupBy(x => x.ProductId).ToList();
                foreach (var group in productGameTypeGroups)
                {
                    var productGameType = new ProductGameType()
                    {
                        GameTypes = new List<EntityBase<int, string>>()
                    };

                    productGameType.ProductName = products.FirstOrDefault(x => x.Id == Converter.ToInt(group.Key)).Name;
                    foreach (var gameType in group)
                    {
                        productGameType.GameTypes.Add(new EntityBase<int, string>()
                        {
                            Id = gameType.Id,
                            Name = gameType.Name
                        });
                    }

                    result.Add(productGameType);
                }

                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the game table list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// The game table list.
        /// </returns>
        public List<EntityBase<int, string>> GetGameTableList(GetGameTableListCriteria criteria)
        {
            try
            {
                List<EntityBase<int, string>> result = new List<EntityBase<int, string>>();
                var gameTableList = GetReportData<EntityBase<int, string>>(criteria);
                if (gameTableList == null)
                {
                    return result;
                }

                gameTableList = gameTableList.OrderBy(info => info.Name).ToList();
                result.Add(new EntityBase<int, string>()
                {
                    Id = GameTypeConstants.All,
                    Name = Labels.All
                });

                foreach (var row in gameTableList)
                {
                    result.Add(new EntityBase<int, string>()
                    {
                        Id = row.Id,
                        Name = row.Name
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
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
                var rawData = _reportRepository.GetReport(criteria);
                var report = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo()
                };

                if (rawData == null || rawData.Count < 2)
                {
                    SessionManager.Set<WinLossInfo>(_reportSessionKey, report);
                    return report;
                }

                var winLossInfoTable = rawData[0];
                var grandTotalInfoTable = rawData[1];
                if (winLossInfoTable == null ||
                    winLossInfoTable.Count == 0)
                {
                    SessionManager.Set<WinLossInfo>(_reportSessionKey, report);
                    return report;
                }

                LiveCasinoGameStatisticService reportManager = null;
                switch (criteria.ReportType)
                {
                    case PeriodTypeConstants.LiveCasinoGameStatisticDaily:
                        reportManager = new LiveCasinoGameStatisticServiceByDailyRange();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticMonthly:
                        reportManager = new LiveCasinoGameStatisticServiceByMonthlyRange();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticUsername:
                        reportManager = new LiveCasinoGameStatisticServiceByUsername();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticGameType:
                        reportManager = new LiveCasinoGameStatisticServiceByGameType();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticGameTable:
                        reportManager = new LiveCasinoGameStatisticServiceByGameTable();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                        reportManager = new LiveCasinoGameStatisticServiceByCompany();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticCurrency:
                        reportManager = new LiveCasinoGameStatisticServiceByCurrency();
                        break;

                    default:
                        reportManager = new LiveCasinoGameStatisticService();
                        break;
                }

                report = reportManager.GetReport(winLossInfoTable, criteria);
                SessionManager.Set<WinLossInfo>(_reportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(_reportSessionKey, null);
                throw;
            }
        }

        public virtual WinLossInfo GetReport(
            List<WinLossInfoEntity> winLossInfoTable,
            GetReportCriteria criteria)
        {
            throw new Exception(Labels.Error_MethodNotImplemented);
        }

        /// <summary>
        /// Sorts the report.
        /// </summary>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>Report data after sort.</returns>
        public WinLossInfo SortReport(GetReportCriteria getReportCriteria)
        {
            LiveCasinoGameStatisticService reportManager = null;
            switch (getReportCriteria.ReportType)
            {
                case PeriodTypeConstants.LiveCasinoGameStatisticDaily:
                    reportManager = new LiveCasinoGameStatisticServiceByDailyRange();
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticMonthly:
                    reportManager = new LiveCasinoGameStatisticServiceByMonthlyRange();
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticUsername:
                    reportManager = new LiveCasinoGameStatisticServiceByUsername();
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticGameType:
                    reportManager = new LiveCasinoGameStatisticServiceByGameType();
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticGameTable:
                    reportManager = new LiveCasinoGameStatisticServiceByGameTable();
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                    reportManager = new LiveCasinoGameStatisticServiceByCompany();
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticCurrency:
                    reportManager = new LiveCasinoGameStatisticServiceByCurrency();
                    break;

                default:
                    reportManager = new LiveCasinoGameStatisticService();
                    break;
            }

            var report = SessionManager.Get<WinLossInfo>(_reportSessionKey);

            if (report == null)
            {
                report = this.GetReport(getReportCriteria);
            }

            reportManager.SortReportData(report, getReportCriteria);

            return report;
        }

        public virtual WinLossInfo SortReportData(WinLossInfo report, GetReportCriteria getReportCriteria)
        {
            throw new NotImplementedException(Labels.Error_MethodNotImplemented);
        }

        public virtual void ExportReport(
            WinLossInfo report,
            GetReportCriteria criteria)
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
                    report = SessionManager.Get<WinLossInfo>(_reportSessionKey);
                }

                LiveCasinoGameStatisticService reportManager = null;
                switch (criteria.ReportType)
                {
                    case PeriodTypeConstants.LiveCasinoGameStatisticDaily:
                        reportManager = new LiveCasinoGameStatisticServiceByDailyRange();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticMonthly:
                        reportManager = new LiveCasinoGameStatisticServiceByMonthlyRange();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticUsername:
                        reportManager = new LiveCasinoGameStatisticServiceByUsername();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticGameType:
                        reportManager = new LiveCasinoGameStatisticServiceByGameType();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticGameTable:
                        reportManager = new LiveCasinoGameStatisticServiceByGameTable();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                        reportManager = new LiveCasinoGameStatisticServiceByCompany();
                        break;

                    case PeriodTypeConstants.LiveCasinoGameStatisticCurrency:
                        reportManager = new LiveCasinoGameStatisticServiceByCurrency();
                        break;

                    default:
                        reportManager = new LiveCasinoGameStatisticService();
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
        /// Calculates the grand total.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        /// <param name="grandTotalRM">The grand total RM.</param>
        public void CalculateGrandTotal(
            GetReportCriteria criteria,
            ref WinLossInfo report,
            WinLossInfo grandTotalRM)
        {
            var maxTotalIndex = report.GrandTotal.ChildsInfo.Count - 1;
            for (var i = 0; i <= maxTotalIndex; i++)
            {
                var data = report.GrandTotal.ChildsInfo[i];
                report.GrandTotal.ChildsInfo[i] = WinLossInfo.Format(report.GrandTotal.ChildsInfo[i]);
            }

            report.GrandTotal.ChildsInfo = report.GrandTotal.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName)
                                                                       .ToList();
            if (!string.Equals(criteria.Currency, ((int)CoreConstants.CurrencyContants.All).ToString()) &&
                !string.Equals(criteria.Currency, ((int)CoreConstants.CurrencyContants.BaseRM).ToString()))
            {
                var firstCurrencyTotal = report.GrandTotal.ChildsInfo.Count > 0 ? report.GrandTotal.ChildsInfo[0] : new WinLossInfo();
                var info = new WinLossInfo
                {
                    Name = Labels.Grand_Total + " " + firstCurrencyTotal.CurrencyName,

                    CurrencyName = firstCurrencyTotal.CurrencyName,
                    NumberOfPlayers = firstCurrencyTotal.NumberOfPlayers,
                    NumberOfGames = firstCurrencyTotal.NumberOfGames,

                    TotalBets = firstCurrencyTotal.TotalBets,
                    TotalWins = firstCurrencyTotal.TotalWins,
                    TotalRefund = firstCurrencyTotal.TotalRefund,

                    GameIncome = firstCurrencyTotal.GameIncome,

                    CompanyTurnover = firstCurrencyTotal.CompanyTurnover,
                    CompanyNetIncome = firstCurrencyTotal.CompanyNetIncome,
                    CompanyCommission = firstCurrencyTotal.CompanyCommission
                };

                info = WinLossInfo.Format(info);
                report.GrandTotal.ChildsInfo.Add(info);
            }

            grandTotalRM = WinLossInfo.Format(grandTotalRM);
            report.GrandTotal.ChildsInfo.Add(grandTotalRM);
        }

        /// <summary>
        /// Determines whether [is correct filtered information] [the specified criteria].
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="info">The information.</param>
        /// <returns>Is correct filtered information.</returns>
        public bool IsCorrectFilteredInfo(GetReportCriteria criteria, WinLossInfo info)
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
            GetReportCriteria criteria)
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
                GamingResources.LiveCasinoGameStatistic_ReportTitle,
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
                case PeriodTypeConstants.LiveCasinoGameStatisticDaily:
                    currentCell.Value = Labels.Date;
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticMonthly:
                    currentCell.Value = Labels.Month;
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticUsername:
                    currentCell.Value = Labels.UserName;
                    
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = headerTextStyle;                    
                    currentCell.Value = Labels.CustId;
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticGameType:
                    currentCell.Value = Labels.Game;
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticGameTable:
                    currentCell.Value = Labels.Table;
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                    currentCell.Value = Labels.Company;
                    break;

                case PeriodTypeConstants.LiveCasinoGameStatisticCurrency:
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
            currentCell.Value = Labels.No_Games;
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
            GetReportCriteria criteria,
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
            GetReportCriteria criteria)
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

        /// <summary>
        /// Exports the report_ child total.
        /// </summary>
        /// <param name="grandTotal">The grand total.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport_ChildTotal(
            WinLossInfo grandTotal,
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            GetReportCriteria criteria)
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

            WinLossInfo info;
            var reportLength = grandTotal.ChildsInfo.Count;
            var isShowNoPlayers = criteria.ReportType != PeriodTypeConstants.LiveCasinoGameStatisticUsername;
            var isShowColumnCurrency = criteria.ReportType != PeriodTypeConstants.LiveCasinoGameStatisticCurrency;

            for (var i = 0; i < reportLength - 1; i++)
            {
                info = grandTotal.ChildsInfo[i];
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

        private List<BaseModel> GetProductFromCache()
        {
            List<BaseModel> result = SessionManager.Get<List<BaseModel>>(_productSessionKey);

            if (result == null)
            {
                result = GetProductList();
            }

            return result;
        }

        private bool CheckUserIsU3Account()
        {
            bool isU3Account = false;
            var userGroup = _userGroupService.GetAdminGroup(new GetAdminGroupCriteria() { UserName = AppSession.CurrentUser.Username });
            isU3Account = userGroup.GroupId == AppSettings.U3GroupId && string.Equals(userGroup.GroupName, AppSettings.U3GroupName);
            return isU3Account;
        }
    }
}