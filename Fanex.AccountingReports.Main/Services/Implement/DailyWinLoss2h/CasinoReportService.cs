// --------------------------------------------------------------------------
// <copyright file="CasinoReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        /// <summary>
        /// Gets the casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The casino report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")] 
        public LiveAndNoneLiveReport GetCasinoReport(GetCasinoReportCriteria criteria)
        {
            try
            {
                var isHideRealTurnover = _dailyWinLoss2hUtilityService.IsHideRealTurnover(SportContants._2hReportCasino);
                var report = new LiveAndNoneLiveReport
                {
                    NoneLiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Id = SportContants._2hReportCasino,
                            Name = Labels.All_Total,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        CreditTotal = new WinLossInfo
                        {
                            Id = SportContants._2hReportCasino,
                            Name = Labels.Credit,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Id = SportContants._2hReportCasino,
                            Name = Labels.Deposit,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants._2hReportCasino),
                        IsHideRealTurnover = isHideRealTurnover,
                        IsLive = false
                    }
                };

                Hashtable outParams = new Hashtable();
                var rawData = this._reportRepository.GetCasinoReport(criteria, ref outParams);
                if (rawData != null)
                {
                    var productGroups = rawData.GroupBy(data => data.ProductId);

                    foreach (var productGroup in productGroups)
                    {
                        int productId = Converter.ToInt(productGroup.Key);
                        bool hideRealTurnover = productId == SportContants._2hCasinoSport ? true : isHideRealTurnover;
                        var productWinLoss = new WinLossInfo
                        {
                            Id = productId,
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo
                            {
                                IsHideRealTurnover = hideRealTurnover
                            },
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit,
                                IsHideRealTurnover = hideRealTurnover
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit,
                                IsHideRealTurnover = hideRealTurnover
                            }
                        };

                        productWinLoss.Name = _dailyWinLoss2hUtilityService.GetSportName(productGroup.Key);
                        productWinLoss.GrandTotal.Id = productWinLoss.Id;
                        productWinLoss.GrandTotal.Name = productWinLoss.Name;
                        foreach (var data in productGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = Converter.ToInt(data.ProductId),
                                BettypeId = Converter.ToInt(data.BetType),
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover),
                                SiteType = Converter.ToInt(data.Site)
                            };

                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);
                            switch (rowType)
                            {
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    productWinLoss.CreditTotal.Add(info);
                                    productWinLoss.CreditTotal.BettypeId = info.BettypeId;
                                    break;
                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    productWinLoss.DepositTotal.Add(info);
                                    productWinLoss.DepositTotal.BettypeId = info.BettypeId;
                                    break;
                            }
                        }

                        productWinLoss.GrandTotal.Add(productWinLoss.CreditTotal);
                        productWinLoss.GrandTotal.Add(productWinLoss.DepositTotal);
                        report.NoneLiveReport.CreditTotal.Add(productWinLoss.CreditTotal);
                        report.NoneLiveReport.DepositTotal.Add(productWinLoss.DepositTotal);
                        productWinLoss = WinLossInfo.Format_Total(productWinLoss);
                        report.NoneLiveReport.ChildsInfo.Add(productWinLoss);
                    }

                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.CreditTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.DepositTotal);
                    report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);
                }

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hCasinoReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hCasinoReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")] 
        public void ExportCasinoReport(GetCasinoReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hCasinoReportSessionKey);
                if (report == null)
                {
                    report = this.GetCasinoReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_CasinoReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                //// ExcelTypes
                //// MakeExcelFile

                var title = MainResources.DailyWinLoss2h_CasinoReport_Title;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                this.ExportHeaderColumns_SportReport_NotApplyVirtualRate(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false);

                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report.NoneLiveReport,
                    SportContants._2hReportCasino);

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
