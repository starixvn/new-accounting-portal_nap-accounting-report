// --------------------------------------------------------------------------
// <copyright file="BingoReportService.cs" company="Nexcel Solutions Vietnam">
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
        /// Gets the bingo report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The bingo report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetBingoReport(GetBingoReportCriteria criteria)
        {
            try
            {
                var isHideRealTurnover = _dailyWinLoss2hUtilityService.IsHideRealTurnover(SportContants.Bingo);
                var report = new LiveAndNoneLiveReport
                {
                    NoneLiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.All_Total,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        EightRMTotal = new WinLossInfo(),
                        BARMTotal = new WinLossInfo(),
                        CreditTotal = new WinLossInfo
                        {
                            Name = Labels.Credit,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Name = Labels.Deposit,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.Bingo),
                        IsHideRealTurnover = isHideRealTurnover
                    },

                    LiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.All_Total,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        EightRMTotal = new WinLossInfo(),
                        BARMTotal = new WinLossInfo(),
                        CreditTotal = new WinLossInfo
                        {
                            Name = Labels.Credit,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Name = Labels.Deposit,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.Bingo),
                        IsHideRealTurnover = isHideRealTurnover
                    }
                };

                var rawData = this._reportRepository.GetBingoReport(criteria);
                if (rawData != null)
                {
                    var rawReport = rawData[0] as List<dynamic>;
                    var betTypeList = rawReport.Select(data => Converter.ToInt(data.Bettype))
                                               .ToList();
                    CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                    var betTypeGroups = rawReport.GroupBy(data => new
                    {
                        Bettype = data.Bettype,
                        liveindicator = data.liveindicator
                    });

                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        var betTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(betTypeGroup.Key.Bettype),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo
                            {
                                IsHideRealTurnover = isHideRealTurnover
                            },
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit,
                                IsHideRealTurnover = isHideRealTurnover
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit,
                                IsHideRealTurnover = isHideRealTurnover
                            },
                            IsLive = Converter.ToBool(betTypeGroup.Key.liveindicator) ||
                                     Converter.ToInt(betTypeGroup.Key.liveindicator) == 1
                        };

                        betTypeWinLoss.Name = this.GetBetTypeNameFromWinLossDetail(betTypeWinLoss);
                        betTypeWinLoss.GrandTotal.Id = betTypeWinLoss.Id;
                        betTypeWinLoss.GrandTotal.Name = betTypeWinLoss.Name;
                        foreach (var data in betTypeGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = betTypeWinLoss.Id,
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover),
                                Hedge = Converter.ToString2(data.IsHedge),
                                IsLive = betTypeWinLoss.IsLive,
                                SiteType = Converter.ToInt(data.Site)
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

                    if (this._8rmConfig.IsActive)
                    {
                        var _8RMReport = rawData[1];
                        foreach (var data in _8RMReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLost8RM),
                                IsLive = Converter.ToBool(data.liveindicator)
                            };

                            if (info.IsLive)
                            {
                                report.LiveReport.EightRMTotal.Add(info);
                            }
                            else
                            {
                                report.NoneLiveReport.EightRMTotal.Add(info);
                            }
                        }

                        report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.EightRMTotal);
                        report.LiveReport.GrandTotal.Add(report.LiveReport.EightRMTotal);
                    }

                    if (this._barmConfig.IsActive)
                    {
                        var barmReport = rawData[2];
                        foreach (var data in barmReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLostBARM),
                                IsLive = Converter.ToBool(data.liveindicator)
                            };

                            if (info.IsLive)
                            {
                                report.NoneLiveReport.BARMTotal.Add(info);
                            }
                            else
                            {
                                report.LiveReport.BARMTotal.Add(info);
                            }
                        }

                        report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.BARMTotal);
                        report.LiveReport.GrandTotal.Add(report.LiveReport.BARMTotal);
                    }

                    report.LiveReport.GrandTotal.Add(report.LiveReport.CreditTotal);
                    report.LiveReport.GrandTotal.Add(report.LiveReport.DepositTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.CreditTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.DepositTotal);
                    report.LiveReport = WinLossInfo.Format_Total(report.LiveReport);
                    report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);
                }

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hBingoReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hBingoReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the bingo report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportBingoReport(GetBingoReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hBingoReportSessionKey);
                if (report == null)
                {
                    report = this.GetBingoReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_BingoReport_SheetName;
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
                //// Header

                var title = MainResources.DailyWinLoss2h_BingoReport_Title;
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

                //// Header

                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report.NoneLiveReport,
                    SportContants.Bingo);

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
