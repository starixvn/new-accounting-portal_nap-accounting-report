// --------------------------------------------------------------------------
// <copyright file="PunterAnalysisService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Implement.PunterAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Special.Constants;
    using Fanex.AccountingReports.Special.Criteria.PunterAnalysis;
    using Fanex.AccountingReports.Special.Models.PunterAnalysis;
    using Fanex.AccountingReports.Special.Resources;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class PunterAnalysisService : ServiceBase, IPunterAnalysisService
    {
        private const int MinMetricGamingBettypeValue = 801;

        private const int MaxMetricGamingBettypeValue = 899;

        private const string NegativeNumberFormat = "[>=0]#,##0{0}_);[RED][<0](#,##0{1})";

        private static Dictionary<object, string> _betTypeNameCache;

        private static Dictionary<object, string> _sportTypeNameCache;

        private static Dictionary<object, string> _leagueNameCache;

        /// <summary>
        /// Gets the products.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The products.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<ProductInfo> GetProducts(GetProductsCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);
                List<ProductInfo> products = new List<ProductInfo>();

                var productGroups = rawData.GroupBy(rawInfo => rawInfo.Product);

                foreach (var productGroup in productGroups)
                {
                    var product = new ProductInfo
                    {
                        Name = Converter.ToString2(productGroup.Key),
                        ChildsInfo = new List<ProductInfo>()
                    };

                    if (string.Compare(product.Name, Labels.Sportsbook, true) == 0)
                    {
                        products.Add(product);
                        continue;
                    }

                    var sportGroups = productGroup.GroupBy(rawInfo => new
                    {
                        rawInfo.sporttype,
                        rawInfo.SportName
                    });

                    foreach (var sportGroup in sportGroups)
                    {
                        var sport = new ProductInfo
                        {
                            Id = Converter.ToInt(sportGroup.Key.sporttype),
                            Name = Converter.ToString2(sportGroup.Key.SportName),
                            ChildsInfo = new List<ProductInfo>()
                        };

                        foreach (var data in sportGroup)
                        {
                            var info = new ProductInfo
                            {
                                Id = Converter.ToInt(data.bettype),
                                Name = Converter.ToString2(data.bettypename)
                            };

                            sport.ChildsInfo.Add(info);
                        }

                        product.ChildsInfo.Add(sport);
                    }

                    products.Add(product);
                }

                return products;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets the name of the sport.
        /// </summary>
        /// <param name="sportId">The sport identifier.</param>
        /// <returns>The name of the sport.</returns>
        public string GetSportName(int sportId)
        {
            switch (sportId)
            {
                case SpecialConstants.PunterAnalysisSoccerGroupId:
                    return CacheRef.GetSportName(SportContants.Soccer);
                case SpecialConstants.PunterAnalysisOtherSportGroupId:
                    return Labels.Others;
                case SpecialConstants.PunterAnalysisPArlayGroupId:
                    return Labels.Parlay;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the sports.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The sports.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<ProductInfo> GetSports(GetProductsCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);
                List<ProductInfo> sports = new List<ProductInfo>();

                var sportTypeList = rawData.Select(info => info.sporttype as dynamic).Distinct().ToList();
                PunterAnalysisService._sportTypeNameCache = CacheRef.SaveCacheRefResponse(sportTypeList, CacheRef.GetSportName, ref PunterAnalysisService._sportTypeNameCache);

                var indGroups = rawData.GroupBy(rawInfo => rawInfo.ind);
                foreach (var indGroupRawData in indGroups)
                {
                    var indGroup = new ProductInfo
                    {
                        Id = Converter.ToInt(indGroupRawData.Key),
                        ChildsInfo = new List<ProductInfo>()
                    };

                    indGroup.Name = this.GetSportName(indGroup.Id);
                    switch (indGroup.Id)
                    {
                        case SpecialConstants.PunterAnalysisSoccerGroupId:
                            indGroup.Id = SpecialConstants.PunterAnalysisSoocerSportId;
                            break;
                        case SpecialConstants.PunterAnalysisOtherSportGroupId:
                            indGroup.Id = SpecialConstants.PunterAnalysisOtherSportId;
                            break;
                        case SpecialConstants.PunterAnalysisPArlayGroupId:
                            indGroup.Id = SpecialConstants.PunterAnalysisParlaySportId;
                            break;
                    }

                    if (indGroup.Id == SpecialConstants.PunterAnalysisOtherSportId)
                    {
                        var sportGroups = indGroupRawData.GroupBy(rawInfo =>
                            new
                            {
                                rawInfo.sporttype,
                                rawInfo.SportName
                            });

                        foreach (var sportGroup in sportGroups)
                        {
                            var sport = new ProductInfo
                            {
                                Id = Converter.ToInt(sportGroup.Key.sporttype),
                                Name = Converter.ToString2(sportGroup.Key.SportName),
                                ChildsInfo = new List<ProductInfo>()
                            };

                            foreach (var data in sportGroup)
                            {
                                var info = new ProductInfo
                                {
                                    Id = Converter.ToInt(data.bettype),
                                    Name = Converter.ToString2(data.bettypename)
                                };

                                sport.ChildsInfo.Add(info);
                            }

                            indGroup.ChildsInfo.Add(sport);
                        }
                    }
                    else
                    {
                        foreach (var data in indGroupRawData)
                        {
                            var info = new ProductInfo
                            {
                                Id = Converter.ToInt(data.bettype),
                                Name = Converter.ToString2(data.bettypename)
                            };

                            indGroup.ChildsInfo.Add(info);
                        }
                    }

                    sports.Add(indGroup);
                }

                SessionManager.Set<List<ProductInfo>>(SpecialConstants.PunterAnalysisSportsSessionKey, sports);
                return sports;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<List<ProductInfo>>(SpecialConstants.PunterAnalysisSportsSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the leagues by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The leagues by sport.</returns>
        public List<EntityBase<int, string>> GetLeaguesBySport(GetLeaguesBySportCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);
                var leagues = new List<EntityBase<int, string>>();
                foreach (var data in rawData)
                {
                    leagues.Add(new EntityBase<int, string>
                    {
                        Id = Converter.ToInt(data.leagueid),
                        Name = Converter.ToString2(data.leaguename)
                    });
                }

                return leagues;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets the bet types by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The bet types by sport.</returns>
        public List<EntityBase<int, string>> GetBetTypesBySport(GetBetTypesBySportCriteria criteria)
        {
            try
            {
                var sports = SessionManager.Get<List<ProductInfo>>(SpecialConstants.PunterAnalysisSportsSessionKey);

                var betTypes = new List<EntityBase<int, string>>();
                foreach (var indGroup in sports)
                {
                    if (indGroup.Id == SpecialConstants.PunterAnalysisOtherSportId)
                    {
                        foreach (var sportGroup in indGroup.ChildsInfo)
                        {
                            if (sportGroup.Id == criteria.Sporttype)
                            {
                                foreach (var betType in sportGroup.ChildsInfo)
                                {
                                    if (betType.Id < MinMetricGamingBettypeValue || betType.Id > MaxMetricGamingBettypeValue)
                                    {
                                        betTypes.Add(new EntityBase<int, string>
                                        {
                                            Id = betType.Id,
                                            Name = betType.Name
                                        });
                                    }
                                }
                            }
                        }

                        continue;
                    }

                    if (indGroup.Id == criteria.Sporttype)
                    {
                        foreach (var betType in indGroup.ChildsInfo)
                        {
                            if (betType.Id < MinMetricGamingBettypeValue || betType.Id > MaxMetricGamingBettypeValue)
                            {
                                betTypes.Add(new EntityBase<int, string>
                                {
                                    Id = betType.Id,
                                    Name = betType.Name
                                });
                            }
                        }
                    }
                }

                return betTypes;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets the top punter report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The top punter report.</returns>
        public PunterWinLossInfo GetTopPunterReport(GetTopPunterReportCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var report = new PunterWinLossInfo
                    {
                        ChildsInfo = new List<PunterWinLossInfo>(),
                        GrandTotal = new PunterWinLossInfo
                        {
                            Name = Labels.Total
                        }
                    };
                    foreach (var data in rawData)
                    {
                        var info = new PunterWinLossInfo
                        {
                            Id = Converter.ToInt(data.custID),
                            Name = Converter.ToString2(data.username),
                            WinLoss = Converter.ToDecimal(data.winloss),
                            Turnover = Converter.ToDecimal(data.turnover),
                            BetCount = Converter.ToInt(data.betcount)
                        };

                        info.Margin = ReportService.CalculateMargin(
                            info.WinLoss,
                            info.Turnover);
                        report.GrandTotal.Add(info);
                        info = PunterWinLossInfo.Format(info);
                        report.ChildsInfo.Add(info);
                    }

                    report.GrandTotal.Margin = ReportService.CalculateMargin(
                        report.GrandTotal.WinLoss,
                        report.GrandTotal.Turnover);
                    report.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal);

                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterReportSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterReportSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the top punter by user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The top punter by user.</returns>
        public PunterWinLossInfo GetTopPunterByUser(GetTopPunterByUserCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var report = new PunterWinLossInfo
                    {
                        Id = criteria.CustID,
                        Name = criteria.UserName,
                        ChildsInfo = new List<PunterWinLossInfo>(),
                        GrandTotal = new PunterWinLossInfo
                        {
                            Name = Labels.Total
                        }
                    };
                    foreach (var data in rawData)
                    {
                        var info = new PunterWinLossInfo
                        {
                            Id = Converter.ToInt(data.SportType),
                            Name = Converter.ToString2(data.SportName),
                            WinLoss = Converter.ToDecimal(data.winloss),
                            Turnover = Converter.ToDecimal(data.turnover),
                            BetCount = Converter.ToInt(data.Betcount)
                        };

                        info.Margin = ReportService.CalculateMargin(info.WinLoss, info.Turnover);
                        report.GrandTotal.Add(info);
                        info = PunterWinLossInfo.Format(info);
                        report.ChildsInfo.Add(info);
                        report.Id = Converter.ToInt(data.CustID);
                    }

                    report.GrandTotal.Margin = ReportService.CalculateMargin(
                        report.GrandTotal.WinLoss,
                        report.GrandTotal.Turnover);
                    report.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal);
                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByUSerSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByUSerSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the top punter bet count by user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The top punter bet count by user.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public PunterWinLossInfo GetTopPunterBetCount_ByUser(GetTopPunterBetCountByUserCriteria criteria)
        {
            try
            {
                if (criteria.Sporttype > 0)
                {
                    criteria.Sportbettype = GetSporttypeBettypeBySportId(criteria.Sportbettype, criteria.Sporttype.ToString());
                }

                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var report = new PunterWinLossInfo
                    {
                        Id = criteria.CustID,
                        Name = criteria.UserName,
                        ChildsInfo = new List<PunterWinLossInfo>(),
                        GrandTotal = new PunterWinLossInfo
                        {
                            Name = Labels.Total
                        }
                    };

                    foreach (var data in rawData)
                    {
                        var info = new PunterWinLossInfo
                        {
                            Name = Converter.ToString2(data.rptDate),
                            WinLoss = Converter.ToDecimal(data.winloss),
                            BetCount = Converter.ToInt(data.betcount),
                            WinCount = Converter.ToInt(data.wincount),
                            DrawCount = Converter.ToInt(data.drawcount)
                        };

                        report.GrandTotal.Add(info);
                        info = PunterWinLossInfo.Format(info);
                        report.ChildsInfo.Add(info);
                    }

                    report.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal);
                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterBetCountByUserSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterBetCountByUserSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the top punter IP by user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The top punter IP by user.</returns>
        public PunterWinLossInfo GetTopPunterIP_ByUser(GetTopPunterIPByUserCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var report = new PunterWinLossInfo
                    {
                        Id = criteria.CustID,
                        Name = criteria.UserName,
                        ChildsInfo = new List<PunterWinLossInfo>()
                    };

                    foreach (var data in rawData)
                    {
                        var info = new PunterWinLossInfo
                        {
                            Month = Converter.ToInt(data.rptmonth),
                            Year = Converter.ToInt(data.rptyear),
                            IP = Converter.ToString2(data.IP),
                            Country = Converter.ToString2(data.Country)
                        };

                        report.ChildsInfo.Add(info);
                    }

                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterIPByUserSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterIPByUserSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the type of the top punter by user sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The type of the top punter by user sport.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public PunterWinLossInfo GetTopPunter_ByUser_SportType(GetTopPunter_ByUser_SportTypeCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<Punter>(criteria);
                if (rawData != null)
                {
                    var nonLiveInfo = new PunterWinLossInfo
                    {
                        LiveType = LiveTypeConstants.NonLive
                    };

                    var liveInfo = new PunterWinLossInfo
                    {
                        LiveType = LiveTypeConstants.Live
                    };

                    var mixLiveTypeInfo = new PunterWinLossInfo
                    {
                        LiveType = LiveTypeConstants.MixLiveTypes
                    };

                    var report = new PunterWinLossInfo
                    {
                        Id = criteria.Sporttype,
                        ChildsInfo = new List<PunterWinLossInfo>(),
                        GrandTotal = new PunterWinLossInfo
                        {
                            Name = Labels.Total,
                            ChildsInfo = new List<PunterWinLossInfo>
                            {
                                nonLiveInfo,
                                liveInfo,
                                mixLiveTypeInfo,
                            }
                        }
                    };

                    var betTypeList = rawData.Select(info => info.BetType as dynamic).Distinct().ToList();
                    this.SaveBetTypeCacheRef(betTypeList, ref PunterAnalysisService._betTypeNameCache);

                    var betTypeGroups = rawData.GroupBy(info => info.BetType);
                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        nonLiveInfo = new PunterWinLossInfo
                        {
                            LiveType = LiveTypeConstants.NonLive,
                            Name = Labels.Non_Live
                        };

                        liveInfo = new PunterWinLossInfo
                        {
                            LiveType = LiveTypeConstants.Live,
                            Name = Labels.Live
                        };

                        mixLiveTypeInfo = new PunterWinLossInfo
                        {
                            LiveType = LiveTypeConstants.MixLiveTypes,
                            Name = string.Format("{0} & {1}", Labels.Non_Live, Labels.Live)
                        };

                        var betTypeInfo = new PunterWinLossInfo
                        {
                            Id = Converter.ToInt(betTypeGroup.Key),
                            Name = PunterAnalysisService._betTypeNameCache[betTypeGroup.Key],
                            ChildsInfo = new List<PunterWinLossInfo>
                            {
                                nonLiveInfo,
                                liveInfo,
                                mixLiveTypeInfo,
                            }
                        };

                        foreach (var liveType in betTypeGroup)
                        {
                            var info = new PunterWinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(liveType.WinLoss),
                                Turnover = Converter.ToDecimal(liveType.Turnover),
                                BetCount = Converter.ToInt(liveType.BetCount),
                                Margin = Converter.ToDecimal(liveType.Margin),
                                LiveType = Converter.ToInt(liveType.IsLive),
                            };

                            info = PunterWinLossInfo.Format(info);
                            switch (info.LiveType)
                            {
                                case LiveTypeConstants.Live:
                                    info.Name = Labels.Live;
                                    betTypeInfo.ChildsInfo[1].HasData = true;
                                    betTypeInfo.ChildsInfo[2].HasData = true;
                                    betTypeInfo.ChildsInfo[1].Add(info);
                                    break;
                                case LiveTypeConstants.NonLive:
                                    info.Name = Labels.Non_Live;
                                    betTypeInfo.ChildsInfo[0].HasData = true;
                                    betTypeInfo.ChildsInfo[2].HasData = true;
                                    betTypeInfo.ChildsInfo[0].Add(info);
                                    break;
                            }

                            betTypeInfo.ChildsInfo[2].Add(info);
                        }

                        for (var i = 0; i < betTypeInfo.ChildsInfo.Count; i++)
                        {
                            report.GrandTotal.ChildsInfo[i].Add(betTypeInfo.ChildsInfo[i]);

                            betTypeInfo.ChildsInfo[i].Margin = ReportService.CalculateMargin(
                                betTypeInfo.ChildsInfo[i].WinLoss,
                                betTypeInfo.ChildsInfo[i].Turnover);

                            betTypeInfo.ChildsInfo[i] = PunterWinLossInfo.Format(betTypeInfo.ChildsInfo[i]);
                        }

                        report.ChildsInfo.Add(betTypeInfo);
                    }

                    for (var i = 0; i < report.GrandTotal.ChildsInfo.Count; i++)
                    {
                        report.GrandTotal.ChildsInfo[i].Margin = ReportService.CalculateMargin(
                            report.GrandTotal.ChildsInfo[i].WinLoss,
                            report.GrandTotal.ChildsInfo[i].Turnover);
                        report.GrandTotal.ChildsInfo[i] = PunterWinLossInfo.Format(report.GrandTotal.ChildsInfo[i]);
                    }

                    report.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal);

                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByUserSportTypeSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByUserSportTypeSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the type of the top punter bet count by user bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The type of the top punter bet count by user bet.</returns>
        public PunterWinLossInfo GetTopPunterBetCount_ByUser_BetType(GetTopPunterBetCount_ByUser_BetTypeCriteria criteria)
        {
            try
            {
                if (criteria.Status == LiveTypeConstants.MixLiveTypes)
                {
                    criteria.Status = null;
                }

                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var report = new PunterWinLossInfo
                    {
                        Id = criteria.CustID,
                        Name = criteria.UserName,
                        ChildsInfo = new List<PunterWinLossInfo>(),
                        GrandTotal = new PunterWinLossInfo
                        {
                            Name = Labels.Total
                        }
                    };

                    foreach (var data in rawData)
                    {
                        var info = new PunterWinLossInfo
                        {
                            Name = Converter.ToString2(data.rptDate),
                            WinLoss = Converter.ToDecimal(data.Winloss),
                            BetCount = Converter.ToInt(data.betcount),
                            WinCount = Converter.ToInt(data.Wincount),
                            DrawCount = Converter.ToInt(data.Drawcount)
                        };
                        report.GrandTotal.Add(info);
                        info = PunterWinLossInfo.Format(info);
                        report.ChildsInfo.Add(info);
                    }

                    report.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal);
                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterBetCountByUserBetTypeSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterBetCountByUserBetTypeSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the top punter by user bet type non daily.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderedData">The ordered data.</param>
        /// <returns>The top punter by user bet type non daily.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public PunterWinLossInfo GetTopPunter_ByUser_BetType_NonDaily(
            GetTopPunter_ByUser_BetTypeCriteria criteria,
            List<dynamic> orderedData)
        {
            Dictionary<string, dynamic> periodRange = new Dictionary<string, dynamic>();
            var report = new PunterWinLossInfo
            {
                ChildsInfo = new List<PunterWinLossInfo>(),
                GrandTotal = new PunterWinLossInfo
                {
                    Name = Labels.Grand_Total,
                    WinLossDate_Str = criteria.Reporttype == PeriodTypeConstants.Annualy ?
                                        Labels.Year + " " + criteria.FromDate.Year.ToString() :
                                        Labels.Grand_Total,
                    ChildsInfo = new List<PunterWinLossInfo>(),
                    GrandTotal = new PunterWinLossInfo()
                }
            };

            var index = 0;
            for (int i = 0; i < orderedData.Count; i++)
            {
                var data = orderedData[i];
                var year = Converter.ToInt(data.rptYear);
                var month = Converter.ToInt(data.rptMonth);
                var week = Converter.ToInt(data.week);
                var date = this.GetInfoPeriodRangeId(criteria.Reporttype, year, month, week);
                if (!periodRange.ContainsKey(date))
                {
                    var name = GetInfoPeriodRangeName(criteria.Reporttype, year, month, week);
                    periodRange.Add(
                        date,
                        new
                        {
                            Index = index++,
                            Id = date,
                            Name = name,
                        });

                    report.GrandTotal.ChildsInfo.Add(new PunterWinLossInfo
                    {
                        WinLossDate_Str = date,
                        Id = year,
                        Name = name
                    });
                }
            }

            var leagueGroups = orderedData.GroupBy(info => info.leagueid).Distinct().ToList();
            foreach (var leagueGroup in leagueGroups)
            {
                var leagueInfo = new PunterWinLossInfo
                {
                    Id = Converter.ToInt(leagueGroup.Key),
                    Name = PunterAnalysisService._leagueNameCache[leagueGroup.Key],
                    ChildsInfo = Enumerable.Repeat((PunterWinLossInfo)null, periodRange.Count).ToList(),
                    GrandTotal = new PunterWinLossInfo
                    {
                        Name = Labels.Grand_Total
                    }
                };

                foreach (var data in leagueGroup)
                {
                    var year = Converter.ToInt(data.rptYear);
                    var month = Converter.ToInt(data.rptMonth);
                    var week = Converter.ToInt(data.week);
                    var date = this.GetInfoPeriodRangeId(criteria.Reporttype, year, month, week);
                    index = periodRange[date].Index;
                    var info = new PunterWinLossInfo
                    {
                        Name = periodRange[date].Name,
                        WinLoss = Converter.ToDecimal(data.winloss),
                        Turnover = Converter.ToDecimal(data.turnover),
                        BetCount = Converter.ToInt(data.betcount),
                        Margin = Converter.ToInt(data.margin),
                        WinLossDate_Str = periodRange[date].Id
                    };

                    info.Margin = ReportService.CalculateMargin(info.WinLoss, info.Turnover);
                    leagueInfo.GrandTotal.Add(info);
                    report.GrandTotal.ChildsInfo[index].Add(info);
                    info = PunterWinLossInfo.Format(info);
                    if (leagueInfo.ChildsInfo[index] == null)
                    {
                        leagueInfo.ChildsInfo[index] = info;
                    }
                    else
                    {
                        leagueInfo.ChildsInfo[index].Add(info);
                        leagueInfo.ChildsInfo[index].Margin = ReportService.CalculateMargin(
                            leagueInfo.ChildsInfo[index].WinLoss,
                            leagueInfo.ChildsInfo[index].Turnover);
                        leagueInfo.ChildsInfo[index] = PunterWinLossInfo.Format(leagueInfo.ChildsInfo[index]);
                    }
                }

                leagueInfo.GrandTotal.Margin = ReportService.CalculateMargin(
                    leagueInfo.GrandTotal.WinLoss,
                    leagueInfo.GrandTotal.Turnover);
                leagueInfo.GrandTotal = PunterWinLossInfo.Format(leagueInfo.GrandTotal);
                report.ChildsInfo.Add(leagueInfo);
            }

            for (var i = 0; i < report.GrandTotal.ChildsInfo.Count; i++)
            {
                report.GrandTotal.ChildsInfo[i].Margin = ReportService.CalculateMargin(
                   report.GrandTotal.ChildsInfo[i].WinLoss,
                   report.GrandTotal.ChildsInfo[i].Turnover);
                report.GrandTotal.ChildsInfo[i] = PunterWinLossInfo.Format(report.GrandTotal.ChildsInfo[i]);
                report.GrandTotal.GrandTotal.Add(report.GrandTotal.ChildsInfo[i]);
            }

            report.GrandTotal.GrandTotal.Margin = ReportService.CalculateMargin(
                            report.GrandTotal.GrandTotal.WinLoss,
                            report.GrandTotal.GrandTotal.Turnover);
            report.GrandTotal.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal.GrandTotal);
            return report;
        }

        /// <summary>
        /// Gets the top punter by user bet type daily.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="orderedData">The ordered data.</param>
        /// <returns>The top punter by user bet type daily.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public PunterWinLossInfo GetTopPunter_ByUser_BetType_Daily(
            GetTopPunter_ByUser_BetTypeCriteria criteria,
            List<dynamic> orderedData)
        {
            var report = new PunterWinLossInfo
            {
                ChildsInfo = new List<PunterWinLossInfo>(),
                GrandTotal = new PunterWinLossInfo
                {
                    Name = Labels.Grand_Total,
                    ChildsInfo = new List<PunterWinLossInfo>(),
                    GrandTotal = new PunterWinLossInfo()
                }
            };

            //// InitRange

            Dictionary<string, dynamic> yearRange = new Dictionary<string, dynamic>();
            var yearRangeGroups = orderedData.GroupBy(info => info.rptYear).Distinct().ToList();
            var yearIndex = 0;
            int year;
            int month;
            int week;
            foreach (var yearRangeGroup in yearRangeGroups)
            {
                year = Converter.ToInt(yearRangeGroup.Key);
                var yearId = this.GetInfoPeriodRangeId(PeriodTypeConstants.MonthRange, year, 0, 0);
                var yearname = GetInfoPeriodRangeName(PeriodTypeConstants.MonthRange, year, 0, 0);
                var yearRangeInfo = new
                {
                    Index = yearIndex++,
                    Name = yearname,
                    ChildsInfo = new Dictionary<string, dynamic>()
                };

                var grandTotalYearInfo = new PunterWinLossInfo
                {
                    WinLossDate_Str = yearId,
                    Name = yearname,
                    ChildsInfo = new List<PunterWinLossInfo>()
                };

                var dateIndex = 0;
                foreach (var data in yearRangeGroup)
                {
                    month = Converter.ToInt(data.rptMonth);
                    week = 0;
                    var dateId = this.GetInfoPeriodRangeId(criteria.Reporttype, year, month, week);
                    var dateName = this.GetInfoPeriodRangeName(criteria.Reporttype, year, month, week);
                    if (!yearRangeInfo.ChildsInfo.ContainsKey(dateId))
                    {
                        yearRangeInfo.ChildsInfo.Add(
                            dateId,
                            new
                            {
                                Index = dateIndex++,
                                Id = dateId,
                                Name = dateName
                            });

                        grandTotalYearInfo.ChildsInfo.Add(new PunterWinLossInfo
                            {
                                WinLossDate_Str = dateId,
                                Name = dateName
                            });
                    }
                }

                yearRange.Add(yearId, yearRangeInfo);
                report.GrandTotal.ChildsInfo.Add(grandTotalYearInfo);
            }

            //// InitRange

            var leagueGroups = orderedData.GroupBy(info => info.leagueid).Distinct().ToList();
            foreach (var leagueGroup in leagueGroups)
            {
                var leagueInfo = new PunterWinLossInfo
                {
                    Id = Converter.ToInt(leagueGroup.Key),
                    ChildsInfo = Enumerable.Repeat((PunterWinLossInfo)null, report.GrandTotal.ChildsInfo.Count).ToList(),
                    GrandTotal = new PunterWinLossInfo
                    {
                        Name = Labels.Grand_Total
                    }
                };

                leagueInfo.Name = PunterAnalysisService._leagueNameCache[leagueInfo.Id];
                var yearGroups = leagueGroup.GroupBy(info => info.rptYear).Distinct().ToList();
                foreach (var yearGroup in yearGroups)
                {
                    year = Converter.ToInt(yearGroup.Key);
                    var yearId = this.GetInfoPeriodRangeId(PeriodTypeConstants.MonthRange, year, 0, 0);
                    var yearRangeInfo = yearRange[yearId];
                    yearIndex = yearRange[yearId].Index;
                    var yearInfo = new PunterWinLossInfo
                    {
                        WinLossDate_Str = yearId,
                        Name = yearRangeInfo.Name,
                        ChildsInfo = Enumerable.Repeat((PunterWinLossInfo)null, report.GrandTotal.ChildsInfo[yearIndex].ChildsInfo.Count).ToList(),
                        GrandTotal = new PunterWinLossInfo()
                    };

                    foreach (var data in yearGroup)
                    {
                        month = Converter.ToInt(data.rptMonth);
                        week = Converter.ToInt(data.week);
                        var dateId = this.GetInfoPeriodRangeId(criteria.Reporttype, year, month, week);
                        var dateIndex = yearRange[yearId].ChildsInfo[dateId].Index;
                        var info = new PunterWinLossInfo
                        {
                            WinLossDate_Str = yearRange[yearId].ChildsInfo[dateId].Id,
                            Name = yearRange[yearId].ChildsInfo[dateId].Name,
                            WinLoss = Converter.ToDecimal(data.winloss),
                            Turnover = Converter.ToDecimal(data.turnover),
                            BetCount = Converter.ToInt(data.betcount),
                            Margin = Converter.ToInt(data.margin)
                        };

                        yearInfo.GrandTotal.Add(info);
                        report.GrandTotal.ChildsInfo[yearIndex].ChildsInfo[dateIndex].Add(info);
                        if (yearInfo.ChildsInfo[dateIndex] == null)
                        {
                            info.Margin = ReportService.CalculateMargin(info.WinLoss, info.Turnover);
                            info = PunterWinLossInfo.Format(info);
                            yearInfo.ChildsInfo[dateIndex] = info;
                        }
                        else
                        {
                            yearInfo.ChildsInfo[dateIndex].Add(info);
                            yearInfo.ChildsInfo[dateIndex].Margin = ReportService.CalculateMargin(
                                yearInfo.ChildsInfo[dateIndex].WinLoss,
                                 yearInfo.ChildsInfo[dateIndex].Turnover);
                            yearInfo.ChildsInfo[dateIndex] = PunterWinLossInfo.Format(yearInfo.ChildsInfo[dateIndex]);
                        }
                    }

                    yearInfo.GrandTotal.Margin = ReportService.CalculateMargin(
                        yearInfo.GrandTotal.WinLoss,
                        yearInfo.GrandTotal.Turnover);
                    leagueInfo.GrandTotal.Add(yearInfo.GrandTotal);
                    report.GrandTotal.ChildsInfo[yearIndex].Add(yearInfo.GrandTotal);
                    yearInfo.GrandTotal = PunterWinLossInfo.Format(yearInfo.GrandTotal);
                    leagueInfo.ChildsInfo[yearIndex] = yearInfo;
                }

                leagueInfo.GrandTotal.Margin = ReportService.CalculateMargin(
                    leagueInfo.GrandTotal.WinLoss,
                    leagueInfo.GrandTotal.Turnover);
                report.GrandTotal.Add(leagueInfo.GrandTotal);
                leagueInfo.GrandTotal = PunterWinLossInfo.Format(leagueInfo.GrandTotal);
                report.ChildsInfo.Add(leagueInfo);
            }

            for (var i = 0; i < report.GrandTotal.ChildsInfo.Count; i++)
            {
                var info = report.GrandTotal.ChildsInfo[i];
                for (var j = 0; j < info.ChildsInfo.Count; j++)
                {
                    report.GrandTotal.ChildsInfo[i].ChildsInfo[j].Margin = ReportService.CalculateMargin(
                                    report.GrandTotal.ChildsInfo[i].ChildsInfo[j].WinLoss,
                                    report.GrandTotal.ChildsInfo[i].ChildsInfo[j].Turnover);
                    report.GrandTotal.ChildsInfo[i].ChildsInfo[j] = PunterWinLossInfo.Format(report.GrandTotal.ChildsInfo[i].ChildsInfo[j]);
                }

                report.GrandTotal.ChildsInfo[i].Margin = ReportService.CalculateMargin(
                   report.GrandTotal.ChildsInfo[i].WinLoss,
                   report.GrandTotal.ChildsInfo[i].Turnover);
                report.GrandTotal.ChildsInfo[i] = PunterWinLossInfo.Format(report.GrandTotal.ChildsInfo[i]);
                report.GrandTotal.GrandTotal.Add(report.GrandTotal.ChildsInfo[i]);
            }

            report.GrandTotal.GrandTotal.Margin = ReportService.CalculateMargin(
                           report.GrandTotal.GrandTotal.WinLoss,
                           report.GrandTotal.GrandTotal.Turnover);
            report.GrandTotal.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal.GrandTotal);
            return report;
        }

        /// <summary>
        /// Gets the type of the top punter by user bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The type of the top punter by user bet.</returns>
        public PunterWinLossInfo GetTopPunter_ByUser_BetType(GetTopPunter_ByUser_BetTypeCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);

                if (rawData != null)
                {
                    rawData = CorrectWeekCrossMonth(rawData);

                    var orderedData = rawData.OrderBy(data => data.rptYear)
                                             .ThenBy(data => data.rptMonth)
                                             .ThenBy(data => data.week)
                                             .ThenBy(data => data.leagueid)
                                             .ToList();

                    var leagueIdList = rawData.Select(info => info.leagueid as dynamic).Distinct().ToList();
                    CacheRef.SaveCacheRefResponse(leagueIdList, CacheRef.GetLeagueName, ref PunterAnalysisService._leagueNameCache);
                    PunterWinLossInfo report = null;
                    switch (criteria.Reporttype)
                    {
                        case PeriodTypeConstants.Daily:
                            report = this.GetTopPunter_ByUser_BetType_Daily(
                                criteria,
                               orderedData);
                            break;
                        default:
                            report = this.GetTopPunter_ByUser_BetType_NonDaily(
                                criteria,
                                orderedData);
                            break;
                    }

                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByUserBetTypeSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByUserBetTypeSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the top punter by league.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The top punter by league.</returns>
        public PunterWinLossInfo GetTopPunter_ByLeague(GetTopPunter_ByLeagueCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var report = new PunterWinLossInfo
                    {
                        ChildsInfo = new List<PunterWinLossInfo>(),
                        GrandTotal = new PunterWinLossInfo
                        {
                            Name = Labels.Total
                        }
                    };

                    foreach (var data in rawData)
                    {
                        var info = new PunterWinLossInfo
                        {
                            Id = Converter.ToInt(data.CustID),
                            Name = Converter.ToString2(data.username),
                            WinLoss = Converter.ToDecimal(data.Winloss),
                            Turnover = Converter.ToDecimal(data.TurnOver),
                            BetCount = Converter.ToInt(data.BetCount)
                        };

                        info.Margin = ReportService.CalculateMargin(
                            info.WinLoss,
                            info.Turnover);
                        report.GrandTotal.Add(info);
                        info = PunterWinLossInfo.Format(info);
                        report.ChildsInfo.Add(info);
                    }

                    report.GrandTotal.Margin = ReportService.CalculateMargin(
                        report.GrandTotal.WinLoss,
                        report.GrandTotal.Turnover);
                    report.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal);
                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByLeagueSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByLeagueSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the top punter by league user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The top punter by league user.</returns>
        public PunterWinLossInfo GetTopPunter_ByLeague_User(GetTopPunter_ByLeague_UserCriteria criteria)
        {
            try
            {
                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var report = new PunterWinLossInfo
                    {
                        Id = criteria.CustID,
                        Name = criteria.UserName,
                        ChildsInfo = new List<PunterWinLossInfo>(),
                        GrandTotal = new PunterWinLossInfo
                        {
                            Name = Labels.Total
                        }
                    };
                    var sportTypeList = rawData.Select(info => info.sporttype as dynamic).Distinct().ToList();
                    PunterAnalysisService._sportTypeNameCache = CacheRef.SaveCacheRefResponse(sportTypeList, CacheRef.GetSportName, ref PunterAnalysisService._sportTypeNameCache);
                    foreach (var data in rawData)
                    {
                        var info = new PunterWinLossInfo
                        {
                            Id = Converter.ToInt(data.sporttype),
                            WinLoss = Converter.ToDecimal(data.winloss),
                            Turnover = Converter.ToDecimal(data.turnover),
                            BetCount = Converter.ToInt(data.betcount)
                        };
                        info.Name = PunterAnalysisService._sportTypeNameCache[info.Id];
                        info.Margin = ReportService.CalculateMargin(info.WinLoss, info.Turnover);
                        report.GrandTotal.Add(info);
                        info = PunterWinLossInfo.Format(info);
                        report.ChildsInfo.Add(info);
                    }

                    report.GrandTotal.Margin = ReportService.CalculateMargin(
                        report.GrandTotal.WinLoss,
                        report.GrandTotal.Turnover);
                    report.GrandTotal = PunterWinLossInfo.Format(report.GrandTotal);
                    SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByLeagueUserSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByLeagueUserSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the name of the sport bet type.
        /// </summary>
        /// <param name="sportIds">The sport ids.</param>
        /// <returns>The name of the sport bet type.</returns>
        public string GetSportBetTypeName(string sportIds)
        {
            if (string.IsNullOrEmpty(sportIds))
            {
                return string.Empty;
            }

            if (sportIds.Contains(SpecialConstants.PunterAnalysisMixParlaySport) ||
                sportIds.Contains(SpecialConstants.PunterAnalysisSystemParlaySport))
            {
                return Labels.All;
            }

            var sport_BetTypes = sportIds.Split(',');
            var result = new List<string>();
            foreach (var sport_BetType in sport_BetTypes)
            {
                var sportType = Converter.ToInt(sport_BetType.Split('_')[0]);
                var sportTypeName = PunterAnalysisService._sportTypeNameCache.ContainsKey(sportType) ? PunterAnalysisService._sportTypeNameCache[sportType] : string.Empty;
                if (result.Contains(sportTypeName))
                {
                    continue;
                }

                result.Add(sportTypeName);
            }

            return string.Join(", ", result);
        }

        /// <summary>
        /// Saves the bet type cache reference.
        /// </summary>
        /// <param name="betTypeIdList">The identifier list.</param>
        /// <param name="currentStorage">The current storage.</param>
        /// <returns>The bet type cache reference.</returns>
        private Dictionary<object, string> SaveBetTypeCacheRef(
            List<dynamic> betTypeIdList,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var betTypeIdInfo in betTypeIdList)
            {
                if (!currentStorage.ContainsKey(betTypeIdInfo))
                {
                    int id = Converter.ToInt(betTypeIdInfo);
                    switch (id)
                    {
                        case BetTypeConstants.KickOff:
                            currentStorage.Add(betTypeIdInfo, Labels.Kick_Off);
                            break;
                        default:
                            currentStorage.Add(betTypeIdInfo, CacheRef.GetBettype(id));
                            break;
                    }
                }
            }

            return currentStorage;
        }

        /// <summary>
        /// Gets the information period range identifier.
        /// </summary>
        /// <param name="periodType">Type of the period.</param>
        /// <param name="year">The year value.</param>
        /// <param name="month">The month value.</param>
        /// <param name="week">The week value.</param>
        /// <returns>The information period range identifier.</returns>
        private string GetInfoPeriodRangeId(
            int periodType,
           int year,
           int month,
           int week)
        {
            if (periodType == PeriodTypeConstants.MonthRange)
            {
                month = 0;
                week = 0;
            }

            return string.Format("{0}_{1}_{2}", year, month, week);
        }

        /// <summary>
        /// Gets the name of the information period range.
        /// </summary>
        /// <param name="periodType">Type of the period.</param>
        /// <param name="year">The year value.</param>
        /// <param name="month">The month value.</param>
        /// <param name="week">The week value.</param>
        /// <returns>The name of the information period range.</returns>
        private string GetInfoPeriodRangeName(
            int periodType,
            int year,
            int month,
            int week)
        {
            var name = string.Empty;
            switch (periodType)
            {
                case PeriodTypeConstants.Daily:
                    name = DateTimeExtensions.GetFullMonthName(month);
                    break;
                case PeriodTypeConstants.Weekly:
                    name = string.Format(Labels.Week + " {0} ({1})", week, DateTimeExtensions.FirstDateOfWeek(year, week).ToShortFormatedDate());
                    break;
                case PeriodTypeConstants.Monthly:
                    name = string.Format("{0} - {1}", year, DateTimeExtensions.GetFullMonthName(month));
                    break;
                case PeriodTypeConstants.Annualy:
                    name = DateTimeExtensions.GetFullMonthName(month);
                    break;
                case PeriodTypeConstants.MonthRange:
                    name = string.Format(Labels.Year + " {0}", year);
                    break;
            }

            return name;
        }

        /// <summary>
        /// Gets bet type by sport type.
        /// </summary>
        /// <param name="sporttype_bettype">Sports type and bets type parameter.</param>
        /// <param name="sportId">Sport type.</param>
        /// <returns>List bets type by sport.</returns>
        private string GetSporttypeBettypeBySportId(string sporttype_bettype, string sportId)
        {
            List<string> sporttype_bettypes = new List<string>();
            string result = string.Empty;
            sporttype_bettype = string.IsNullOrEmpty(sporttype_bettype) ? string.Empty : sporttype_bettype;

            var arrS_B = sporttype_bettype.Split(',');

            foreach (string s_b in arrS_B)
            {
                var arr = s_b.Split('_');
                if (arr[0] == sportId)
                {
                    sporttype_bettypes.Add(s_b);
                }
            }

            result = string.Join(",", sporttype_bettypes);

            return result;
        }

        /// <summary>
        /// Update data cross month, same week.
        /// </summary>
        /// <param name="rawData">Data source.</param>
        /// <returns>Correct data source.</returns>
        private IEnumerable<dynamic> CorrectWeekCrossMonth(IEnumerable<dynamic> rawData)
        {
            var years = rawData.Select(s => s.rptYear).Distinct().ToList();
            foreach (var year in years)
            {
                var dataByYear = rawData.Where(s => s.rptYear == year).Distinct().ToList();
                var weeks = dataByYear.Select(s => s.week).Distinct().ToList();
                foreach (var week in weeks)
                {
                    if (week == 0)
                    {
                        continue;
                    }

                    var dataByWeek = dataByYear.Where(s => s.week == week).Distinct().ToList();
                    var months = dataByWeek.Select(s => s.rptMonth).Distinct().ToList();
                    if (months.Count > 1)
                    {
                        var maxMonth = months.OrderByDescending(s => s).FirstOrDefault();
                        var minMonth = months.OrderBy(s => s).FirstOrDefault();
                        var leagueIds = rawData
                            .Where(s =>
                                (s.rptYear == year)
                                && (s.week == week)
                                && (s.rptMonth == maxMonth))
                                .Select(s => s.leagueid)
                                .ToList();

                        foreach (var leagueId in leagueIds)
                        {
                            foreach (var row in rawData)
                            {
                                if (row.leagueid == leagueId)
                                {
                                    row.rptMonth = minMonth;
                                }
                            }
                        }
                    }
                }
            }

            return rawData;
        }
    }
}
