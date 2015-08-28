// --------------------------------------------------------------------------
// <copyright file="NumberGameReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        /// <summary>
        /// Gets the summary number game report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The summary number game report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetSummaryNumberGameReport(GetSummaryNumberGameReportCriteria criteria)
        {
            try
            {
                var enableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.NumberGame);
                var report = new LiveAndNoneLiveReport
                {
                    NoneLiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.All_Total,
                            EnableVirtual = enableVirtual
                        },
                        CreditTotal = new WinLossInfo
                        {
                            Name = Labels.Credit,
                            EnableVirtual = enableVirtual
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Name = Labels.Deposit,
                            EnableVirtual = enableVirtual
                        },
                        EnableVirtual = enableVirtual
                    },

                    LiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.All_Total,
                            EnableVirtual = enableVirtual
                        },
                        CreditTotal = new WinLossInfo
                        {
                            Name = Labels.Credit,
                            EnableVirtual = enableVirtual
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Name = Labels.Deposit,
                            EnableVirtual = enableVirtual
                        },
                        EnableVirtual = enableVirtual,
                        IsLive = true
                    }
                };

                var rawData = this._reportRepository.GetSummaryNumberGameReport(criteria);
                if (rawData != null)
                {
                    var betTypeList = rawData.Select(data => Converter.ToInt(data.bettype))
                                             .ToList();
                    CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                    var betTypeGroups = rawData.GroupBy(data => new
                    {
                        Bettype = data.bettype,
                        liveindicator = data.liveindicator,
                        IsHedge = data.IsHedge
                    });

                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        var betTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(betTypeGroup.Key.Bettype),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo
                            {
                                EnableVirtual = enableVirtual
                            },
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit,
                                EnableVirtual = enableVirtual
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit,
                                EnableVirtual = enableVirtual
                            },
                            IsLive = Converter.ToBool(betTypeGroup.Key.liveindicator),
                            Hedge = Converter.ToString2(betTypeGroup.Key.IsHedge)
                        };

                        betTypeWinLoss.Name = this.GetBetTypeNameFromWinLossDetail(betTypeWinLoss);
                        betTypeWinLoss.AliasName = string.IsNullOrEmpty(this._betTypeNameCache[betTypeWinLoss.Id]) ?
                            betTypeWinLoss.Name :
                            this._betTypeNameCache[betTypeWinLoss.Id] + betTypeWinLoss.Hedge;
                        betTypeWinLoss.GrandTotal.Id = betTypeWinLoss.Id;
                        betTypeWinLoss.GrandTotal.Name = betTypeWinLoss.Name;
                        betTypeWinLoss.GrandTotal.AliasName = betTypeWinLoss.AliasName;
                        betTypeWinLoss.GrandTotal.Hedge = betTypeWinLoss.Hedge;
                        foreach (var data in betTypeGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = betTypeWinLoss.Id,
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                                Hedge = betTypeWinLoss.Hedge,
                                IsLive = betTypeWinLoss.IsLive,
                                SiteType = Converter.ToInt(data.Site),
                                EnableVirtual = enableVirtual
                            };

                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);
                            switch (rowType)
                            {
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    betTypeWinLoss.CreditTotal.Add(info);
                                    break;
                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    betTypeWinLoss.DepositTotal.Add(info);
                                    break;
                            }
                        }

                        betTypeWinLoss.GrandTotal.Add(betTypeWinLoss.CreditTotal);
                        betTypeWinLoss.GrandTotal.Add(betTypeWinLoss.DepositTotal);
                        if (betTypeWinLoss.IsLive)
                        {
                            report.LiveReport.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                            report.LiveReport.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                            betTypeWinLoss = WinLossInfo.Format_Total(betTypeWinLoss);
                            report.LiveReport.ChildsInfo.Add(betTypeWinLoss);
                        }
                        else
                        {
                            report.NoneLiveReport.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                            report.NoneLiveReport.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                            betTypeWinLoss = WinLossInfo.Format_Total(betTypeWinLoss);
                            report.NoneLiveReport.ChildsInfo.Add(betTypeWinLoss);
                        }
                    }

                    report.LiveReport.GrandTotal.Add(report.LiveReport.CreditTotal);
                    report.LiveReport.GrandTotal.Add(report.LiveReport.DepositTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.CreditTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.DepositTotal);
                    report.LiveReport = WinLossInfo.Format_Total(report.LiveReport);
                    report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);

                    report.NoneLiveReport.ChildsInfo = report.NoneLiveReport
                                                             .ChildsInfo
                                                             .OrderBy(betType => betType.Hedge)
                                                             .ToList();
                    report.LiveReport.ChildsInfo = report.LiveReport
                                                         .ChildsInfo
                                                         .OrderBy(betType => betType.Hedge)
                                                         .ToList();
                }

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSummaryNumberGameReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSummaryNumberGameReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the number game report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportNumberGameReport(GetSummaryNumberGameReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSummaryNumberGameReportSessionKey);
                if (report == null)
                {
                    report = this.GetSummaryNumberGameReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_NumberGameReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 10;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report.NoneLiveReport,
                    MainResources.DailyWinLoss2h_NumberGameNonLiveReport_Title,
                    true,
                    SportContants.NumberGame);

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report.LiveReport,
                    MainResources.DailyWinLoss2h_NumberGameLiveReport_Title,
                    true,
                    SportContants.NumberGame);

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

        /// <summary>
        /// Exports the number game report by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportNumberGameReportByBetType(GetReportByBetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hReportByBetTypeSessionKey);
                if (report == null)
                {
                    report = this.GetReportByBetType(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_NumberGameReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                var title = string.Format(
                    MainResources.DailyWinLoss2h_ReportByBetType_Title,
                    Labels.NumberGame,
                    criteria.BetTypeName,
                    criteria.Liveindicator ? " \"" + Labels.Live + "\"" : " \"" + Labels.Symbol_Sparse_Non_Live + "\"");
                this.ExportReportByBetType(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report,
                    title,
                    criteria,
                    false);

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
