// --------------------------------------------------------------------------
// <copyright file="SoccerReportService.cs" company="Nexcel Solutions Vietnam">
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
        private WinLossInfo _firstBettypeGroup;
        private WinLossInfo _secondBettypeGroup;
        private WinLossInfo _thirdBettypeGroup;
        private WinLossInfo _fourthBettypeGroup;
        private WinLossInfo _handicapAndOverUnderBettypesTotal;
        private WinLossInfo _otherBettypesTotal;
        private WinLossInfo _hedgeTotal;

        /// <summary>
        /// Gets the soccer report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The soccer report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetSoccerReport(GetSoccerReportCriteria criteria)
        {
            try
            {
                if (criteria.SoccerType == SoccerBetTypeConstants.SoccerMajorBetType)
                {
                    _firstBettypeGroup = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Total.ToUpper()
                        }
                    };
                    _secondBettypeGroup = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Total.ToUpper()
                        }
                    };
                    _thirdBettypeGroup = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Total.ToUpper()
                        }
                    };
                    _fourthBettypeGroup = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Total.ToUpper()
                        }
                    };
                    _handicapAndOverUnderBettypesTotal = new WinLossInfo
                    {
                        Name = Labels.Main_Bettypes_Total
                    };
                    _otherBettypesTotal = new WinLossInfo
                    {
                        Name = Labels.Other_Bettypes_Total
                    };
                    _hedgeTotal = new WinLossInfo
                    {
                        Name = "Hedge Total"
                    };
                }

                var enableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(CoreConstants.SportContants.Soccer);
                var report = new LiveAndNoneLiveReport
                {
                    NoneLiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.All_Total,
                            ChildsInfo = new List<WinLossInfo>(),
                            EnableVirtual = enableVirtual
                        },
                        EightRMTotal = new WinLossInfo
                        {
                            Name = Labels.EightRM + " " + Labels.Total
                        },
                        BARMTotal = new WinLossInfo
                        {
                            Name = Labels.BARM + " " + Labels.Total
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
                        IsLive = false
                    },

                    LiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.All_Total,
                            EnableVirtual = enableVirtual
                        },
                        EightRMTotal = new WinLossInfo
                        {
                            Name = Labels.EightRM + " " + Labels.Total
                        },
                        BARMTotal = new WinLossInfo
                        {
                            Name = Labels.BARM + " " + Labels.Total
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

                var rawData = this._reportRepository.GetSoccerReport(criteria);
                if (rawData != null)
                {
                    var rawReport = rawData[0] as List<dynamic>;
                    var betTypeList = rawReport.Select(data => Converter.ToInt(data.Bettype))
                                               .ToList();
                    CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                    var betTypeGroups = rawReport.GroupBy(data => new
                    {
                        Bettype = data.Bettype,
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
                            IsLive = Converter.ToBool(betTypeGroup.Key.liveindicator) ||
                                     Converter.ToInt(betTypeGroup.Key.liveindicator) == 1,
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

                        ////change to group bettype for soccer major.
                        if (betTypeWinLoss.IsLive)
                        {
                            report.LiveReport.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                            report.LiveReport.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                            betTypeWinLoss = WinLossInfo.Format_Total(betTypeWinLoss);
                            report.LiveReport.ChildsInfo.Add(betTypeWinLoss);
                        }
                        else
                        {
                            if (criteria.SoccerType == SoccerBetTypeConstants.SoccerMajorBetType)
                            {
                                MapSoccerMajorBettypeToGroup(betTypeWinLoss, report);
                                CalculateTotalGroupByBettype(betTypeWinLoss);
                            }
                            else
                            {
                                report.NoneLiveReport.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                                report.NoneLiveReport.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                                betTypeWinLoss = WinLossInfo.Format_Total(betTypeWinLoss);
                                report.NoneLiveReport.ChildsInfo.Add(betTypeWinLoss);
                            }
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
                                IsLive = Converter.ToBool(data.liveindicator),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover)
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
                    }

                    if (this._barmConfig.IsActive)
                    {
                        var barmReport = rawData[2];
                        foreach (var data in barmReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLostBARM),
                                IsLive = Converter.ToBool(data.liveindicator),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover)
                            };

                            if (info.IsLive)
                            {
                                report.LiveReport.BARMTotal.Add(info);
                            }
                            else
                            {
                                report.NoneLiveReport.BARMTotal.Add(info);
                            }
                        }
                    }

                    if (criteria.SoccerType == SoccerBetTypeConstants.SoccerMajorBetType)
                    {
                        report.NoneLiveReport.ChildsInfo.Add(_firstBettypeGroup);
                        report.NoneLiveReport.ChildsInfo.Add(_secondBettypeGroup);
                        report.NoneLiveReport.ChildsInfo.Add(_thirdBettypeGroup);
                        report.NoneLiveReport.ChildsInfo.Add(_fourthBettypeGroup);
                        report.NoneLiveReport.GrandTotal.ChildsInfo.Add(_handicapAndOverUnderBettypesTotal);
                        report.NoneLiveReport.GrandTotal.ChildsInfo.Add(_otherBettypesTotal);
                        report.NoneLiveReport.GrandTotal.ChildsInfo.Add(_hedgeTotal);
                    }

                    report.LiveReport.GrandTotal.Add(report.LiveReport.CreditTotal);
                    report.LiveReport.GrandTotal.Add(report.LiveReport.DepositTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.CreditTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.DepositTotal);
                    report.LiveReport = WinLossInfo.Format_Total(report.LiveReport);
                    report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);

                    report.LiveReport.ChildsInfo = report.LiveReport
                                                             .ChildsInfo
                                                             .OrderBy(betType => betType.Hedge)
                                                             .ToList();
                    if (criteria.SoccerType == SoccerBetTypeConstants.SoccerMinorBetType)
                    {
                        report.NoneLiveReport.ChildsInfo = report.NoneLiveReport
                                                            .ChildsInfo
                                                            .OrderBy(betType => betType.Hedge)
                                                            .ToList();
                    }
                    else
                    {
                        for (int i = 0; i < report.NoneLiveReport.ChildsInfo.Count; i++)
                        {
                            report.NoneLiveReport.ChildsInfo[i].ChildsInfo = report.NoneLiveReport
                                                                                .ChildsInfo[i]
                                                                                .ChildsInfo
                                                                                .OrderBy(bettype => bettype.Hedge)
                                                                                .ToList();
                        }
                    }
                }

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSoccerReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSoccerReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the soccer report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportSoccerReport(GetSoccerReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSoccerReportSessionKey);
                if (report == null)
                {
                    report = this.GetSoccerReport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SoccerReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);
                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 10;
                var currentRow = startRow;

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report.NoneLiveReport,
                    MainResources.DailyWinLoss2h_SoccerNonLiveReportBySport_Title,
                    false,
                    criteria.SoccerType);

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report.LiveReport,
                    MainResources.DailyWinLoss2h_SoccerLiveReportBySport_Title,
                    false,
                    criteria.SoccerType);

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

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportSoccerReportByBetType(GetReportByBetTypeCriteria criteria)
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
                string sheetName = MainResources.DailyWinLoss2h_SoccerReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                string sportName = string.Empty;
                if (criteria.Bettype >= MainConstants.MinMetricGamingSuperLiveBetType && criteria.Bettype <= MainConstants.MaxMetricGamingSuperLiveBetType)
                {
                    sportName = Labels.MetricGaming_Superlive;
                }
                else
                {
                    sportName = Labels.Soccer;
                }

                var title = string.Format(
                    MainResources.DailyWinLoss2h_ReportByBetType_Title,
                    sportName,
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
                    true);

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
        /// Maps the soccer major bet type to group.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="report">The report.</param>
        private void MapSoccerMajorBettypeToGroup(WinLossInfo info, LiveAndNoneLiveReport report)
        {
            report.NoneLiveReport.CreditTotal.Add(info.CreditTotal);
            report.NoneLiveReport.DepositTotal.Add(info.DepositTotal);

            if (info.Id == CoreConstants.BetTypeConstants.Handicap ||
                info.Id == CoreConstants.BetTypeConstants._1x2BetType ||
                info.Id == CoreConstants.BetTypeConstants.DoubleChanceBetType ||
                info.Id == CoreConstants.BetTypeConstants.DrawNoBetBetType ||
                info.Id == CoreConstants.BetTypeConstants._3wayHandicap ||
                info.Id == CoreConstants.BetTypeConstants.HomeNoBetBetType ||
                info.Id == CoreConstants.BetTypeConstants.AwayNoBetBetType ||
                info.Id == CoreConstants.BetTypeConstants.DrawNoDrawBetType ||
                info.Id == CoreConstants.BetTypeConstants.MyanmarHandicap)
            {
                _firstBettypeGroup.ChildsInfo.Add(info);
                _firstBettypeGroup.GrandTotal.Add(info.CreditTotal);
                _firstBettypeGroup.GrandTotal.Add(info.DepositTotal);
            }
            else if (info.Id == CoreConstants.BetTypeConstants.OverUnder2 ||
                info.Id == CoreConstants.BetTypeConstants.TotalGoal ||
                info.Id == CoreConstants.BetTypeConstants.OddEvenBetType ||
                info.Id == CoreConstants.BetTypeConstants.MyanmarOverUnder)
            {
                _secondBettypeGroup.ChildsInfo.Add(info);
                _secondBettypeGroup.GrandTotal.Add(info.CreditTotal);
                _secondBettypeGroup.GrandTotal.Add(info.DepositTotal);
            }
            else if (info.Id == CoreConstants.BetTypeConstants._1stHandicap ||
                info.Id == CoreConstants.BetTypeConstants._1st1x2BetType ||
                info.Id == CoreConstants.BetTypeConstants._1stDoubleChange ||
                info.Id == CoreConstants.BetTypeConstants._1stMyanmarHandicap)
            {
                _thirdBettypeGroup.ChildsInfo.Add(info);
                _thirdBettypeGroup.GrandTotal.Add(info.CreditTotal);
                _thirdBettypeGroup.GrandTotal.Add(info.DepositTotal);
            }
            else
            {
                _fourthBettypeGroup.ChildsInfo.Add(info);
                _fourthBettypeGroup.GrandTotal.Add(info.CreditTotal);
                _fourthBettypeGroup.GrandTotal.Add(info.DepositTotal);
            }
        }

        /// <summary>
        /// Calculates the total group by bet type.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="report">The report.</param>
        private void CalculateTotalGroupByBettype(WinLossInfo info)
        {
            if ((info.Id == CoreConstants.BetTypeConstants.Handicap ||
                info.Id == CoreConstants.BetTypeConstants._1stHandicap ||
                info.Id == CoreConstants.BetTypeConstants.OverUnder2 ||
                info.Id == CoreConstants.BetTypeConstants._1stOverUnder) && !info.Hedge.Contains("hedge"))
            {
                _handicapAndOverUnderBettypesTotal.Add(info.CreditTotal);
                _handicapAndOverUnderBettypesTotal.Add(info.DepositTotal);
            }
            else if (info.Hedge.Contains("hedge"))
            {
                _hedgeTotal.Add(info.CreditTotal);
                _hedgeTotal.Add(info.DepositTotal);
            }
            else
            {
                _otherBettypesTotal.Add(info.CreditTotal);
                _otherBettypesTotal.Add(info.DepositTotal);
            }
        }
    }
}