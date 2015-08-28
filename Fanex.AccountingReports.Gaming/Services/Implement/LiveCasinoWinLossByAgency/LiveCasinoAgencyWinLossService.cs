// --------------------------------------------------------------------------
// <copyright file="LiveCasinoAgencyWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.LiveCasinoWinLossByAgency
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoWinLossByAgency;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoWinLossByAgency;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoAgencyWinLossService : ServiceBase, ILiveCasinoAgencyWinLossService
    {
        public const int MaxItemPerSheet = 65000;

        private readonly ILiveCasinoAgencyWinLossRepository _reportRepository;
        private readonly ILiveCasinoProductService _liveCasinoProductService;

        private readonly string _reportSessionKey = "Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.Report" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private string _productSessionKey = "Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.Products" +
        (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        public LiveCasinoAgencyWinLossService()
            : this(null, null)
        {
        }

        public LiveCasinoAgencyWinLossService(ILiveCasinoAgencyWinLossRepository reportRepository, ILiveCasinoProductService liveCasinoProductService)
        {
            _reportRepository = reportRepository ?? new LiveCasinoAgencyWinLossRepository();
            _liveCasinoProductService = liveCasinoProductService ?? new LiveCasinoProductService();
        }

        public List<BaseModel> GetProductList()
        {
            try
            {
                List<BaseModel> result = _liveCasinoProductService.GetProductList();
                SessionManager.Set<List<BaseModel>>(_productSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
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
        /// Gets the currency list.
        /// </summary>
        /// <returns>The currency list.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<EntityBase<int, string>> GetCurrencyList(GetExchangeCurrenciesCriteria criteria)
        {
            try
            {
                var result = new List<EntityBase<int, string>>();
                var currencyList = new List<EntityBase<int, string>>();
                Dictionary<int, string> currencies = ReportService.GetExchangeCurrencies(criteria);

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
                    SessionManager.Set<WinLossInfo>(_reportSessionKey + criteria.UserName, report);
                    return report;
                }

                var winLossInfoTable = rawData[0] as List<WinLossInfoEntity>;
                var grandTotalInfoTable = rawData[1] as List<WinLossInfoEntity>;
                LiveCasinoAgencyWinLossService reportManager = GetReportManager(criteria);
                report = reportManager.GetReport(winLossInfoTable, criteria);
                report.Id = rawData[2].ToString();
                report.GrandTotal = GetReport_GrandTotal(grandTotalInfoTable, criteria);
                SessionManager.Set<WinLossInfo>(_reportSessionKey + criteria.UserName, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(_reportSessionKey + criteria.UserName, null);
                throw;
            }
        }

        /// <summary>
        /// Sorts the report.
        /// </summary>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>Report data after sort.</returns>
        public WinLossInfo SortReport(GetReportCriteria getReportCriteria)
        {
            LiveCasinoAgencyWinLossService reportManager = GetReportManager(getReportCriteria);

            var report = SessionManager.Get<WinLossInfo>(_reportSessionKey + getReportCriteria.UserName);

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

        /// <summary>
        /// Sorts the second sub level report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>Second sub level of report data after sort.</returns>
        public virtual WinLossInfo SortSecondSubLevelReport(WinLossInfo report, GetReportCriteria getReportCriteria)
        {
            bool isAsc = string.Equals(getReportCriteria.SortDirection, CoreConstants.SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);

            foreach (var childInfo in report.ChildsInfo)
            {
                switch (getReportCriteria.SortColumnName)
                {
                    case "RollingTurnover":

                        childInfo.ChildsInfo = isAsc ? childInfo.ChildsInfo.OrderBy(winLossInfo => winLossInfo.RollingTurnover).ToList() :
                                            childInfo.ChildsInfo.OrderByDescending(winLossInfo => winLossInfo.RollingTurnover).ToList();
                        break;

                    case "MemberTotal":
                        childInfo.ChildsInfo = isAsc ? childInfo.ChildsInfo.OrderBy(winLossInfo => winLossInfo.ChildsInfo[0].RealWinLoss).ToList() :
                                            childInfo.ChildsInfo.OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[0].RealWinLoss).ToList();
                        break;

                    case "AgentTotal":
                        childInfo.ChildsInfo = isAsc ? childInfo.ChildsInfo.OrderBy(winLossInfo => winLossInfo.ChildsInfo[1].RealWinLoss).ToList() :
                                            childInfo.ChildsInfo.OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[1].RealWinLoss).ToList();
                        break;

                    case "MasterTotal":
                        childInfo.ChildsInfo = isAsc ? childInfo.ChildsInfo.OrderBy(winLossInfo => winLossInfo.ChildsInfo[2].RealWinLoss).ToList() :
                                            childInfo.ChildsInfo.OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[2].RealWinLoss).ToList();
                        break;

                    case "SuperTotal":
                        childInfo.ChildsInfo = isAsc ? childInfo.ChildsInfo.OrderBy(winLossInfo => winLossInfo.ChildsInfo[3].RealWinLoss).ToList() :
                                            childInfo.ChildsInfo.OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[3].RealWinLoss).ToList();
                        break;

                    case "CompanyWinLoss":
                        childInfo.ChildsInfo = isAsc ? childInfo.ChildsInfo.OrderBy(winLossInfo => winLossInfo.ChildsInfo[4].RealWinLoss).ToList() :
                                            childInfo.ChildsInfo.OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[4].RealWinLoss).ToList();
                        break;

                    default:
                        break;
                }
            }

            return report;
        }

        /// <summary>
        /// Sorts the first sub level report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>First sub level of report data after sort.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public virtual WinLossInfo SortFirstSubLevelReport(WinLossInfo report, GetReportCriteria getReportCriteria)
        {
            bool isAsc = string.Equals(getReportCriteria.SortDirection, CoreConstants.SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);
            switch (getReportCriteria.SortColumnName)
            {
                case "RollingTurnover":

                    report.ChildsInfo = isAsc ?
                                        report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[0].RollingTurnover)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[0].RollingTurnover)
                                              .ToList();
                    break;

                case "MemberTotal":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[0].RealWinLoss)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[0].RealWinLoss)
                                              .ToList();
                    break;

                case "AgentTotal":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[1].RealWinLoss)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[1].RealWinLoss)
                                              .ToList();
                    break;

                case "MasterTotal":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[2].RealWinLoss)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[2].RealWinLoss)
                                              .ToList();
                    break;

                case "SuperTotal":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[3].RealWinLoss)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[3].RealWinLoss)
                                              .ToList();
                    break;

                case "CompanyWinLoss":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[4].RealWinLoss)
                                              .ToList() :
                    report.ChildsInfo = report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[0].ChildsInfo[4].RealWinLoss)
                                              .ToList();
                    break;

                default:
                    break;
            }

            return report;
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
                    report = SessionManager.Get<WinLossInfo>(_reportSessionKey + criteria.UserName);
                }

                LiveCasinoAgencyWinLossService reportManager = GetReportManager(criteria);
                reportManager.ExportReport(report, criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public virtual WinLossInfo GetReport(
            List<WinLossInfoEntity> winLossInfoTable,
            GetReportCriteria criteria)
        {
            throw new Exception(Labels.Error_MethodNotImplemented);
        }

        public virtual void ExportReport(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            throw new Exception(Labels.Error_MethodNotImplemented);
        }

        /// <summary>
        /// Gets the win loss information.
        /// </summary>
        /// <param name="data">The data information.</param>
        /// <returns>The win loss information.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetWinLossInfo(WinLossInfoEntity data)
        {
            var info = new WinLossInfo
            {
                CurrencyName = data.CurrencyName,
                TotalBets = data.TotalBets,
                GrossCommission = data.GrossCommission,
                RollingTurnover = data.RollingTurnover,
                ChildsInfo = new List<WinLossInfo>()
            };

            var memberWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Member.ToString(),
                WinLoss = data.MemberWinLoss,
                Commission = data.MemberCommission
            };

            memberWinLoss.RealWinLoss = WinLossInfo.CalculateRealWinLoss(memberWinLoss);

            var agentWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Agent.ToString(),
                WinLoss = data.AgentWinLoss,
                Commission = data.AgentCommission
            };

            agentWinLoss.RealWinLoss = WinLossInfo.CalculateRealWinLoss(agentWinLoss);

            var masterWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Master.ToString(),
                WinLoss = data.MasterWinLoss,
                Commission = data.MasterCommission
            };

            masterWinLoss.RealWinLoss = WinLossInfo.CalculateRealWinLoss(masterWinLoss);

            var superWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Master.ToString(),
                WinLoss = data.SuperWinLoss,
                Commission = data.SuperCommission
            };

            superWinLoss.RealWinLoss = WinLossInfo.CalculateRealWinLoss(superWinLoss);

            var companyWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Master.ToString(),
                WinLoss = data.CompanyWinLoss,
                Turnover = data.CompanyTurnover
            };

            companyWinLoss.RealWinLoss = WinLossInfo.CalculateRealWinLoss(companyWinLoss);

            info.ChildsInfo.Add(memberWinLoss);
            info.ChildsInfo.Add(agentWinLoss);
            info.ChildsInfo.Add(masterWinLoss);
            info.ChildsInfo.Add(superWinLoss);
            info.ChildsInfo.Add(companyWinLoss);
            return info;
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
            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 1].Width = 90 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 5].Width = 100 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 7].Width = 100 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 10].Width = 100 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 13].Width = 100 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 16].Width = 100 * ExcelExtensions.FontWidth;

            var title = criteria.ReportType == PeriodTypeConstants.LiveCasinoAgencyWinLossAgent ?
                criteria.ReportTitle :
                string.Format(
                    GamingResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                    Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.LiveCasinoAgencyWinLossDaily:
                    currentCellRange.Value = Labels.Date;
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossMonthly:
                    currentCellRange.Value = Labels.Month;
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossUsername:
                    currentCellRange.Value = Labels.UserName;
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossGameType:
                    currentCellRange.Value = Labels.Game;
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossGameTable:
                    currentCellRange.Value = Labels.Table;
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossAgent:
                    currentCellRange.Value = Labels.UserName;
                    break;
            }

            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Currency;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Total_Bets;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Turnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Gross_Commission;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Symbol_TotalBets;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Agent;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Master;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Super;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;
            currentRow++;

            currentColumn = startColumn + 6;
            for (var i = 0; i < 4; i++)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Win_Loss;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Commission;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Total;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the report win loss row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="intNumberStyle">The integer number style.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalNegativeNumberStyle">The two decimal negative number style.</param>
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
            GetReportCriteria criteria,
            WinLossInfo info)
        {
            ExcelCell currentCell;
            var companyWinLoss = info.ChildsInfo[4];

            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCell.Value = info.CurrencyName;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.RollingTurnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = companyWinLoss.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.GrossCommission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = intNumberStyle;
            currentCell.Value = info.TotalBets;
            currentColumn++;

            var downlinesMaxIndex = info.ChildsInfo.Count - 1;
            for (var i = 0; i < downlinesMaxIndex; i++)
            {
                var winLoss = info.ChildsInfo[i];
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNumberStyle;
                currentCell.Value = winLoss.WinLoss;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNumberStyle;
                currentCell.Value = winLoss.Commission;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNumberStyle;
                currentCell.Value = winLoss.RealWinLoss;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = companyWinLoss.WinLoss;
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
            var subTotalColor = ExcelExtensions.TotalRowColor;
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

            var textStyle_SubTotal = ExcelExtensions.GetCellTextStyle(subTotalColor, true);
            var intNumberStyle_SubTotal = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.IntNumberFormat, subTotalColor, true, false, false);
            var twoDecimalNumberStyle_SubTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalColor,
                true,
                false,
                false);
            var twoDecimalNegativeNumberStyle_SubTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalColor,
                true,
                false,
                true);

            var grandTotalSeperatorColor = ExcelExtensions.GrandTotalSeparatorColor;

            textStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
            intNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
            twoDecimalNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
            twoDecimalNegativeNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);

            var currentColumn = startColumn;
            WinLossInfo info;
            var reportLength = grandTotal.ChildsInfo.Count;

            for (var i = 0; i < reportLength; i++)
            {
                info = grandTotal.ChildsInfo[i];
                ExportReport_WinLossRow_GrandTotal(
                    ref workSheet,
                    currentColumn,
                    ref currentRow,
                    intNumberStyle_SubTotal,
                    textStyle_SubTotal,
                    twoDecimalNumberStyle_SubTotal,
                    twoDecimalNegativeNumberStyle_SubTotal,
                    criteria,
                    info);
                currentRow++;
            }

            info = grandTotal.GrandTotal;
            ExportReport_WinLossRow_GrandTotal(
                ref workSheet,
                currentColumn,
                ref currentRow,
                intNumberStyle_GrandTotal,
                textStyle_GrandTotal,
                twoDecimalNumberStyle_GrandTotal,
                twoDecimalNegativeNumberStyle_GrandTotal,
                criteria,
                info);
            currentRow++;
        }

        /// <summary>
        /// Exports the report win loss row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="intNumberStyle">The integer number style.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalNegativeNumberStyle">The two decimal negative number style.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="info">The information.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport_WinLossRow_GrandTotal(
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            CellStyle intNumberStyle,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalNegativeNumberStyle,
            GetReportCriteria criteria,
            WinLossInfo info)
        {
            ExcelCell currentCell;
            CellRange currentCellRange;
            var companyWinLoss = info.ChildsInfo[4];

            var currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCellRange.Value = info.Name;
            currentColumn += 2;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.RollingTurnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = companyWinLoss.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.GrossCommission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = intNumberStyle;
            currentCell.Value = info.TotalBets;
            currentColumn++;

            var downlinesMaxIndex = info.ChildsInfo.Count - 1;
            for (var i = 0; i < downlinesMaxIndex; i++)
            {
                var winLoss = info.ChildsInfo[i];
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNumberStyle;
                currentCell.Value = winLoss.WinLoss;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNumberStyle;
                currentCell.Value = winLoss.Commission;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNumberStyle;
                currentCell.Value = winLoss.RealWinLoss;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = companyWinLoss.WinLoss;
            currentColumn++;
        }

        /// <summary>
        /// Gets the report manager.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report manager.</returns>
        private LiveCasinoAgencyWinLossService GetReportManager(GetReportCriteria criteria)
        {
            LiveCasinoAgencyWinLossService reportManager = null;
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.LiveCasinoAgencyWinLossDaily:
                    reportManager = new LiveCasinoAgencyWinLossByDailyRangeService();
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossMonthly:
                    reportManager = new LiveCasinoAgencyWinLossByMonthlyRangeService();
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossUsername:
                    reportManager = new LiveCasinoAgencyWinLossByUsernameService();
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossGameType:
                    reportManager = new LiveCasinoAgencyWinLossByGameTypeService();
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossGameTable:
                    reportManager = new LiveCasinoAgencyWinLossByGameTableService();
                    break;

                case PeriodTypeConstants.LiveCasinoAgencyWinLossAgent:
                    reportManager = new LiveCasinoAgencyWinLossByAgentService();
                    break;

                default:
                    reportManager = new LiveCasinoAgencyWinLossService();
                    break;
            }

            return reportManager;
        }

        /// <summary>
        /// Gets the report grand total.
        /// </summary>
        /// <param name="grandTotalInfoTable">The grand total information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report grand total.</returns>
        private WinLossInfo GetReport_GrandTotal(
            List<WinLossInfoEntity> grandTotalInfoTable,
            GetReportCriteria criteria)
        {
            var grandTotal = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>()
            };

            WinLossInfo info;
            var maxTotalIndex = grandTotalInfoTable.Count - 1;
            for (var i = 0; i < grandTotalInfoTable.Count; i++)
            {
                var data = grandTotalInfoTable[i];
                info = GetWinLossInfo(data);
                if (i == maxTotalIndex)
                {
                    info.CurrencyName = Labels.RM;
                    info.Name = string.Format("{0} {1}", Labels.Grand_Total, info.CurrencyName);
                    info = WinLossInfo.Format(info);
                    grandTotal.GrandTotal = info;
                }
                else
                {
                    info.Name = string.Format("{0} {1}", Labels.Total, data.CurrencyName);
                    info = WinLossInfo.Format(info);
                    grandTotal.ChildsInfo.Add(info);
                }
            }

            return grandTotal;
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
    }
}