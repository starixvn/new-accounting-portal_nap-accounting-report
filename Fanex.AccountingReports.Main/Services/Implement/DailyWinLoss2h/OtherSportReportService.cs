// --------------------------------------------------------------------------
// <copyright file="OtherSportReportService.cs" company="Nexcel Solutions Vietnam">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        /// <summary>
        /// Gets the summary other sport report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The summary other sport report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetSummaryOtherSportReport(GetSummaryOtherSportReportCriteria criteria)
        {
            try
            {
                var enableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants._2hReportOtherSport);
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

                var rawData = this._reportRepository.GetSummaryOtherSportReport(criteria);
                if (rawData != null)
                {
                    var rawReport = rawData[0] as List<dynamic>;
                    var sportTypeList = rawReport.Select(data => Converter.ToInt(data.SportType))
                                                 .ToList();
                    CacheRef.SaveCacheRefResponse(sportTypeList, CacheRef.GetSportName, ref this._sportTypeNameCache);
                    var sportTypeGroups = rawReport.GroupBy(data => new
                    {
                        SportType = data.SportType,
                        liveindicator = data.liveindicator,
                        IsHedge = data.IsHedge
                    });

                    foreach (var sportTypeGroup in sportTypeGroups)
                    {
                        var sportTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(sportTypeGroup.Key.SportType),
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
                            IsLive = Converter.ToBool(sportTypeGroup.Key.liveindicator),
                            Hedge = Converter.ToString2(sportTypeGroup.Key.IsHedge)
                        };

                        sportTypeWinLoss.Name = this._sportTypeNameCache[sportTypeWinLoss.Id];
                        sportTypeWinLoss.GrandTotal.Id = sportTypeWinLoss.Id;
                        sportTypeWinLoss.GrandTotal.Name = sportTypeWinLoss.Name;
                        sportTypeWinLoss.GrandTotal.Hedge = sportTypeWinLoss.Hedge;
                        foreach (var data in sportTypeGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = sportTypeWinLoss.Id,
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                                Hedge = sportTypeWinLoss.Hedge,
                                IsLive = sportTypeWinLoss.IsLive,
                                SiteType = Converter.ToInt(data.Site),
                                EnableVirtual = enableVirtual
                            };

                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);
                            switch (rowType)
                            {
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    sportTypeWinLoss.CreditTotal.Add(info);
                                    break;

                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    sportTypeWinLoss.DepositTotal.Add(info);
                                    break;
                            }
                        }

                        sportTypeWinLoss.GrandTotal.Add(sportTypeWinLoss.CreditTotal);
                        sportTypeWinLoss.GrandTotal.Add(sportTypeWinLoss.DepositTotal);
                        if (sportTypeWinLoss.IsLive)
                        {
                            report.LiveReport.CreditTotal.Add(sportTypeWinLoss.CreditTotal);
                            report.LiveReport.DepositTotal.Add(sportTypeWinLoss.DepositTotal);
                            sportTypeWinLoss = WinLossInfo.Format_Total(sportTypeWinLoss);
                            report.LiveReport.ChildsInfo.Add(sportTypeWinLoss);
                        }
                        else
                        {
                            report.NoneLiveReport.CreditTotal.Add(sportTypeWinLoss.CreditTotal);
                            report.NoneLiveReport.DepositTotal.Add(sportTypeWinLoss.DepositTotal);
                            sportTypeWinLoss = WinLossInfo.Format_Total(sportTypeWinLoss);
                            report.NoneLiveReport.ChildsInfo.Add(sportTypeWinLoss);
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
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Position),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
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
                    }

                    if (this._barmConfig.IsActive)
                    {
                        var barmReport = rawData[2];
                        foreach (var data in barmReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLostBARM),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Position),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                                IsLive = Converter.ToBool(data.liveindicator)
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

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSummaryOtherSportReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSummaryOtherSportReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the other sport report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The other sport report by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetOtherSportReportBySport(GetOtherSportReportBySportCriteria criteria)
        {
            try
            {
                var enableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants._2hReportOtherSport);
                var report = new LiveAndNoneLiveReport
                {
                    NoneLiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        Total = new WinLossInfo
                        {
                            Name = Labels.Total_New_Cricket,
                            EnableVirtual = enableVirtual
                        },

                        CreditTotalNewCricket = new WinLossInfo
                        {
                            Name = Labels.Credit,
                            EnableVirtual = enableVirtual
                        },
                        DepositTotalNewCricket = new WinLossInfo
                        {
                            Name = Labels.Deposit,
                            EnableVirtual = enableVirtual
                        },
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
                        IsLive = false
                    },

                    LiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        Total = new WinLossInfo
                        {
                            Name = Labels.Total_New_Cricket,
                            EnableVirtual = enableVirtual
                        },

                        CreditTotalNewCricket = new WinLossInfo
                        {
                            Name = Labels.Credit,
                            EnableVirtual = enableVirtual
                        },
                        DepositTotalNewCricket = new WinLossInfo
                        {
                            Name = Labels.Deposit,
                            EnableVirtual = enableVirtual
                        },
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

                var rawData = this._reportRepository.GetOtherSportReportBySport(criteria);
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
                    var isbetTypeNewCricket = false;
                    var betTypeNewCricket = betTypeGroups.FirstOrDefault();
                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        if (betTypeGroup.Key.Bettype == 500)
                        {
                            isbetTypeNewCricket = true;
                            betTypeNewCricket = betTypeGroup;
                            continue;
                        }

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
                                IsLive = Converter.ToBool(data.liveindicator),
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
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Position),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
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
                    }

                    if (this._barmConfig.IsActive)
                    {
                        var barmReport = rawData[2];
                        foreach (var data in barmReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLostBARM),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Position),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                                IsLive = Converter.ToBool(data.liveindicator)
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
                    
                    if (isbetTypeNewCricket == true)
                    {
                        report.LiveReport.Total.Add(report.LiveReport.CreditTotal);
                        report.LiveReport.Total.Add(report.LiveReport.DepositTotal);
                        report.NoneLiveReport.Total.Add(report.NoneLiveReport.CreditTotal);
                        report.NoneLiveReport.Total.Add(report.NoneLiveReport.DepositTotal);
                        report.LiveReport = WinLossInfo.Format_Total(report.LiveReport);
                        report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);
                        OtherSportForNewCricket(report, rawReport, enableVirtual);
                    }

                    report.LiveReport.GrandTotal.Add(report.LiveReport.CreditTotal);
                    report.LiveReport.GrandTotal.Add(report.LiveReport.DepositTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.CreditTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.DepositTotal);
                    report.LiveReport = WinLossInfo.Format_Total(report.LiveReport);
                    report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);
                }

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hOtherSportReportBySportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hOtherSportReportBySportSessionKey, null);
                throw;
            }
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport OtherSportForNewCricket(LiveAndNoneLiveReport report, List<dynamic> rawReport, bool enableVirtual)
        {
            var betTypeGroups = rawReport.GroupBy(data => new
            {
                Bettype = data.Bettype,
                liveindicator = data.liveindicator,
                IsHedge = data.IsHedge
            });
            foreach (var betTypeGroup in betTypeGroups)
            {
                if (betTypeGroup.Key.Bettype == 500)
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
                    betTypeWinLoss.GrandTotal.Name = "Charged Commission";
                    betTypeWinLoss.GrandTotal.AliasName = "Charged Commission";
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
                            IsLive = Converter.ToBool(data.liveindicator),
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
            }

            return report;
        }

        /// <summary>
        /// Exports the summary other sport report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportSummaryOtherSportReport(GetSummaryOtherSportReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hSummaryOtherSportReportSessionKey);
                if (report == null)
                {
                    report = this.GetSummaryOtherSportReport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_OtherSportReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);
                int startRow = 0;
                int startColumn = 0;
                var maxColumnIndex = 6;
                var currentRow = startRow;

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    true,
                    report.NoneLiveReport,
                    MainResources.DailyWinLoss2h_OtherSportNonLiveReport_Title,
                    false,
                    SportContants._2hReportOtherSport);

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    true,
                    report.LiveReport,
                    MainResources.DailyWinLoss2h_OtherSportLiveReport_Title,
                    false,
                    SportContants._2hReportOtherSport);

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
        /// Exports the other sport report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportOtherSportReportBySport(GetOtherSportReportBySportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hOtherSportReportBySportSessionKey);
                if (report == null)
                {
                    report = this.GetOtherSportReportBySport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_OtherSportReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);
                int startRow = 0;
                int startColumn = 0;
                var maxColumnIndex = 6;
                var currentRow = startRow;

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report.NoneLiveReport,
                    string.Format(MainResources.DailyWinLoss2h_OtherSportNonLiveReportBySport_Title, criteria.SportTypeName).ToUpper(),
                    false,
                    SportContants._2hReportOtherSport);

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report.LiveReport,
                    string.Format(MainResources.DailyWinLoss2h_OtherSportLiveReportBySport_Title, criteria.SportTypeName).ToUpper(),
                    false,
                    SportContants._2hReportOtherSport);

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
        /// Exports the other sport report by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportOtherSportReportByBetType(GetReportByBetTypeCriteria criteria)
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
                string sheetName = MainResources.DailyWinLoss2h_OtherSportReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                var title = string.Format(
                    MainResources.DailyWinLoss2h_ReportByBetType_Title,
                    Labels.Other_Sports + "|" + criteria.SportTypeName.ToUpper(),
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
    }
}