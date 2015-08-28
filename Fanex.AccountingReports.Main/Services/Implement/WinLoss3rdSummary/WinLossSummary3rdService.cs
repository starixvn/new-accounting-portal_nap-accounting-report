// --------------------------------------------------------------------------
// <copyright file="WinLossSummary3rdService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.25">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.WinLoss3rdSummary
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Models.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Repositories.Implement;
    using Fanex.AccountingReports.Main.Repositories.Interface;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.Common;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class WinLossSummary3rdService : ServiceBase, IWinLossSummary3rdService
    {
        private readonly IWinlossSummary3rdRepository _reportRepository;
        private Dictionary<object, string> _sportTypeNameCache;
        private Dictionary<object, string> _betTypeNameCache;
        private Dictionary<object, string> _leagueNameCache;
        private Dictionary<object, string> _teamNameCache;

        public WinLossSummary3rdService()
            : this(null)
        {
        }

        public WinLossSummary3rdService(IWinlossSummary3rdRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new WinlossSummary3rdRepository();
        }

        //// Implementation methods

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLoss3rdInfo GetReport(CriteriaBase criteria)
        {
            WinLoss3rdInfo report = null;
            SportTypeWinLossInfo sportInfo = null;
            BetTypeWinLossInfo betInfo = null;
            WinLossInfo winloss = null;
            try
            {
                var rawData = GetReportData<dynamic>(criteria).ToList();
                report = new WinLoss3rdInfo();

                Dictionary<int, object> sportTypeHash = new Dictionary<int, object>();
                Dictionary<int, object> betTypeHash = new Dictionary<int, object>();
                rawData.ForEach(data =>
                {
                    int sportId, betId;
                    sportId = Converter.ToInt(data.sporttype);
                    if (!sportTypeHash.ContainsKey(sportId))
                    {
                        sportTypeHash.Add(sportId, sportId);
                    }

                    betId = Converter.ToInt(data.bettype);
                    if (!betTypeHash.ContainsKey(betId))
                    {
                        betTypeHash.Add(betId, betId);
                    }
                });

                // get name of sport types
                CacheRef.SaveCacheRefResponse(sportTypeHash.Values.ToList(), CacheRef.GetSportName, ref this._sportTypeNameCache);

                // get name of bet types
                CacheRef.SaveCacheRefResponse(betTypeHash.Values.ToList(), CacheRef.GetBettype, ref this._betTypeNameCache);

                // group by SportType
                var sportTypeGroups = rawData.GroupBy(data => data.sporttype);
                foreach (var sportTypeGroup in sportTypeGroups)
                {
                    int sportId = Converter.ToInt(sportTypeGroup.Key);
                    sportInfo = new SportTypeWinLossInfo
                    {
                        Id = sportId,
                        Name = this.GetSportName(sportId) // get name by SportType
                    };

                    // group by BetType & LiveIndicator
                    var betTypeGroup = sportTypeGroup.GroupBy(data => new { data.bettype, data.liveindicator });
                    foreach (var data in betTypeGroup)
                    {
                        betInfo = new BetTypeWinLossInfo();
                        foreach (var item in data)
                        {
                            winloss = new WinLossInfo
                            {
                                Id = Converter.ToInt(item.bettype),
                                LiveIndicator = Converter.ToBool(item.liveindicator) ? 1 : 0,
                                IBC = Converter.ToInt(item.ibc),
                                IsHedge = Converter.ToBool(item.IsHedge),
                                Status = Converter.ToString2(item.status),
                                WinLoss = Converter.ToDecimal(item.realWinlost),
                                Position = Converter.ToDecimal(item.realPosition),
                                Commission = Converter.ToDecimal(item.realCommission),
                                RealLineTotal = ToDecimal(item.realLineTotal),
                                VirtualLineTotal = ToDecimal(item.virtualLineTotal),
                                RealTurnover = ToDecimal(item.realTurnover),
                                VirtualTurnover = ToDecimal(item.virtualTurnover)
                            };

                            if (winloss.IBC == RowType.EightRM)
                            {
                                winloss.Name = Labels.EightRM;
                                betInfo.EightRM = winloss;

                                // IBC Total by BetType
                                sportInfo.CalculateIBCTotal(winloss);

                                // Footer: IBC Total
                                report.CalculateTempIBCTotal(winloss);
                            }
                            else if (winloss.IBC == RowType.BARM)
                            {
                                winloss.Name = Labels.BARM;
                                betInfo.BARM = winloss;

                                // IBC Total by BetType
                                sportInfo.CalculateIBCTotal(winloss);

                                // Footer: IBC Total
                                report.CalculateTempIBCTotal(winloss);
                            }
                            else
                            {
                                if (MainConstants.WinLossSummaryRacingSportIds.Contains(sportInfo.Id))
                                {
                                    // Horse Racing, Greyhounds, Harness are grouped by Racing
                                    winloss.Name = sportInfo.Name;
                                    sportInfo.Name = Labels.Racing;
                                    winloss.Id = sportId;
                                }
                                else if (sportInfo.Id >= MainConstants.WinLossSummaryBeginVirtualSportId &&
                                    sportInfo.Id <= MainConstants.WinLossSummaryEndVirtualSportId)
                                {
                                    winloss.Name = sportInfo.Name;
                                    sportInfo.Name = Labels.VirtualSport;
                                    winloss.Id = sportId;
                                }
                                else
                                {
                                    winloss.Name = this.GetBetType(winloss.Id);
                                }

                                if (!string.IsNullOrWhiteSpace(winloss.Status))
                                {
                                    winloss.Name += " (" + winloss.Status + ")";
                                }

                                if (winloss.IsHedge)
                                {
                                    winloss.Name = Labels.Hedge_title;
                                    betInfo.Hedge = winloss;
                                }
                                else
                                {
                                    betInfo.IBCs.Add(winloss);
                                }
                            }

                            sportInfo.CalculateTotal(winloss);
                            report.CalculateAllTotal(sportId, winloss);
                        }

                        sportInfo.BetTypeWinLosses.Add(betInfo);
                    }

                    if (MainConstants.WinLossSummaryRacingSportIds.Contains(sportInfo.Id))
                    {
                        SportTypeWinLossInfo racingSport = report.SportTypeWinLosses.FirstOrDefault(sport => sport.Id == MainConstants.WinLossSummaryRacingSportId);
                        if (racingSport == null)
                        {
                            sportInfo.Id = MainConstants.WinLossSummaryRacingSportId;
                            racingSport = sportInfo;
                            report.SportTypeWinLosses.Add(racingSport);
                        }
                        else
                        {
                            racingSport.BetTypeWinLosses.Add(betInfo);
                            racingSport.CalculateTotal(winloss);
                        }
                    }
                    else if (sportInfo.Id >= MainConstants.WinLossSummaryBeginVirtualSportId && sportInfo.Id <= MainConstants.WinLossSummaryEndVirtualSportId)
                    {
                        SportTypeWinLossInfo virtualSport = report.SportTypeWinLosses.FirstOrDefault(sport => sport.Id == MainConstants.WinLossSummaryVirtualSportId);
                        if (virtualSport == null)
                        {
                            sportInfo.Id = MainConstants.WinLossSummaryVirtualSportId;
                            virtualSport = sportInfo;
                            report.SportTypeWinLosses.Add(virtualSport);
                        }
                        else
                        {
                            virtualSport.BetTypeWinLosses.Add(betInfo);
                            virtualSport.CalculateTotal(winloss);
                        }
                    }
                    else
                    {
                        report.SportTypeWinLosses.Add(sportInfo);
                    }
                }

                this.CalculateHedgeTotalBySport(report);

                SessionManager.Set<WinLoss3rdInfo>(MainConstants.WinLossSummary3rdReportSessionKey, report);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return report;
        }

        /// <summary>
        /// Recalculates the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void RecalculateReport(CriteriaBase criteria)
        {
            try
            {
                this._reportRepository.RecalculateWinLoss(criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The details.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public MatchGame3rdInfo GetDetails(GetMatchGameDetailsCriteria criteria)
        {
            SportTypeWinLoss type = criteria.Type;
            if (type == SportTypeWinLoss.Unknown)
            {
                return null;
            }

            MatchGame3rdInfo report = null;
            try
            {
                var rawData = this.GetMatchGameDetails(criteria).ToList();
                if (rawData.Count > 0)
                {
                    report = new MatchGame3rdInfo();

                    Dictionary<int, object> betTypeHash = new Dictionary<int, object>();
                    Dictionary<int, object> leagueHash = new Dictionary<int, object>();
                    Dictionary<int, object> teamHash = new Dictionary<int, object>();
                    rawData.ForEach(data =>
                    {
                        int betId, leagueId, homeId, awayId;
                        betId = Converter.ToInt(data.bettype);
                        if (!betTypeHash.ContainsKey(betId))
                        {
                            betTypeHash.Add(betId, betId);
                        }

                        leagueId = Converter.ToInt(data.leagueid);
                        if (!leagueHash.ContainsKey(leagueId))
                        {
                            leagueHash.Add(leagueId, leagueId);
                        }

                        if (type == SportTypeWinLoss.SportBook || type == SportTypeWinLoss.VirtualSports)
                        {
                            homeId = Converter.ToInt(data.HomeId);
                            if (!teamHash.ContainsKey(homeId))
                            {
                                teamHash.Add(homeId, homeId);
                            }

                            awayId = Converter.ToInt(data.AwayId);
                            if (!teamHash.ContainsKey(awayId))
                            {
                                teamHash.Add(awayId, awayId);
                            }
                        }
                    });

                    // get name of BetType
                    CacheRef.SaveCacheRefResponse(betTypeHash.Values.ToList(), CacheRef.GetBettype, ref this._betTypeNameCache);

                    // get name of League
                    CacheRef.SaveCacheRefResponse(leagueHash.Values.ToList(), CacheRef.GetLeagueName, ref this._leagueNameCache);

                    // get team name
                    if (type == SportTypeWinLoss.SportBook || type == SportTypeWinLoss.VirtualSports)
                    {
                        CacheRef.SaveCacheRefResponse(teamHash.Values.ToList(), CacheRef.GetTeamName, ref this._teamNameCache);
                    }

                    // process data
                    bool liveIndicator;
                    int betTypeId, leagueid;
                    MatchGameBetTypeInfo betSummary = null;
                    MatchGameLeagueInfo leagueSummary = null;
                    MatchGameWinLossInfo matchGameWinLoss = null;

                    var betTypeGroups = rawData.GroupBy(data => new { data.bettype, data.liveindicator });
                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        betTypeId = Converter.ToInt(betTypeGroup.Key.bettype);
                        liveIndicator = Converter.ToBool(betTypeGroup.Key.liveindicator);

                        betSummary = new MatchGameBetTypeInfo
                        {
                            Key = new EntityBase<int, string>
                            {
                                Id = betTypeId,
                                Name = this.GetBetType(betTypeId)
                            },
                            LiveIndicator = liveIndicator ? 1 : 0
                        };

                        // 2. Group by league
                        var leagueGroups = betTypeGroup.GroupBy(data => data.leagueid);
                        foreach (var leagueGroup in leagueGroups)
                        {
                            leagueid = Converter.ToInt(leagueGroup.Key);

                            leagueSummary = new MatchGameLeagueInfo
                            {
                                Key = new EntityBase<int, string>
                                {
                                    Id = leagueid,
                                    Name = this.GetLeagueName(leagueid)
                                }
                            };

                            foreach (dynamic item in leagueGroup)
                            {
                                matchGameWinLoss = this.BuildWinLossDetail(item, type);

                                // Total for each League
                                leagueSummary.CalculateTotal(matchGameWinLoss.WinLossDetail);

                                leagueSummary.MatchGameWinLosses.Add(matchGameWinLoss);

                                // Total for each BetType
                                betSummary.CalculateTotal(matchGameWinLoss.WinLossDetail);

                                // Grand Total
                                report.CalculateTotal(matchGameWinLoss.WinLossDetail);
                            }

                            betSummary.MatchGameLeagues.Add(leagueSummary);
                        }

                        report.MatchGameBetTypes.Add(betSummary);
                    }

                    // Set type of details
                    report.Type = type;
                    if (criteria.SportType == SportContants.NumberGame)
                    {
                        report.Type = SportTypeWinLoss.NumberGame;
                    }

                    SessionManager.Set<MatchGame3rdInfo>(MainConstants.WinLossSummary3rdReportDetailSessionKEy, report);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return report;
        }
        
        //// Private methods

        /// <summary>
        /// Gets the name of the sport.
        /// </summary>
        /// <param name="sportId">The sport identifier.</param>
        /// <returns>The name of the sport.</returns>
        private string GetSportName(int sportId)
        {
            string sportName;
            switch (sportId)
            {
                case MainConstants.WinLossSummaryMixParlaySportId:
                    sportName = Labels.Mix_Parlay;
                    break;
                case MainConstants.WinLossSummarySystemParlaySportId:
                    sportName = Labels.System_Parlay;
                    break;
                case SportContants.MetricGamingSuperLive:
                    sportName = Labels.Metric_Gaming_Super_Live;
                    break;
                case SportContants.Macau:
                    sportName = Labels.Macau;
                    break;
                default:
                    sportName = this._sportTypeNameCache[sportId];
                    break;
            }

            return sportName;
        }

        /// <summary>
        /// Gets the type of the bet.
        /// </summary>
        /// <param name="betId">The bet identifier.</param>
        /// <returns>The type of the bet.</returns>
        private string GetBetType(int betId)
        {
            string betName;
            if (betId == MainConstants.WinLossSummaryTotalBetTypeId)
            {
                betName = Labels.Others;
            }
            else
            {
                betName = this._betTypeNameCache[betId];
            }

            return betName;
        }

        private decimal? ToDecimal(object value)
        {
            if (value != System.DBNull.Value)
            {
                return Converter.ToDecimal(value);
            }

            return null;
        }

        // Helper methods of Report Details
        private string GetLeagueName(int leagueId)
        {
            return this._leagueNameCache[leagueId];
        }

        private string GetTeamName(int teamId)
        {
            return this._teamNameCache[teamId];
        }

        /// <summary>
        /// Gets the match game details.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The match game details.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private IEnumerable<dynamic> GetMatchGameDetails(GetMatchGameDetailsCriteria criteria)
        {
            IEnumerable<dynamic> report = null;
            var rawData = GetReportData<dynamic>(criteria);

            SportTypeWinLoss type = criteria.Type;
            switch (type)
            {
                case SportTypeWinLoss.SportBook:
                case SportTypeWinLoss.NumberGame:
                    report = rawData;
                    break;

                case SportTypeWinLoss.Racing:
                    report = rawData.Select(r => new
                    {
                        matchcode = r.race,
                        bettype = r.bettype,
                        leagueid = r.meetingid,
                        realWinLost = r.realWinLost,
                        realPosition = r.realPosition,
                        realCommission = r.realCommission,
                        realTurnover = r.realTurnover,
                        virtualTurnover = r.virtualTurnover,
                        realLineTotal = r.realLineTotal,
                        virtualLineTotal = r.virtualLineTotal,
                        liveindicator = false // add live indicator
                    });
                    break;

                case SportTypeWinLoss.VirtualSports:
                    report = rawData.Select(r => new
                    {
                        matchcode = r.matchcode,
                        HomeId = r.HomeId,
                        AwayId = r.AwayId,
                        bettype = r.bettype,
                        leagueid = r.leagueid,
                        realWinLost = r.realWinLost,
                        realPosition = r.realPosition,
                        realCommission = r.realCommission,
                        realTurnover = r.realTurnover,
                        virtualTurnover = r.virtualTurnover,
                        realLineTotal = r.realLineTotal,
                        virtualLineTotal = r.virtualLineTotal,
                        liveindicator = false // add live indicator
                    });
                    break;

                case SportTypeWinLoss.VirtualRacing:
                    report = rawData.Select(r => new
                    {
                        matchcode = r.race,
                        bettype = r.bettype,
                        leagueid = r.leagueid,
                        realWinLost = r.realWinlost,
                        realPosition = r.realPosition,
                        realCommission = r.realCommission,
                        realTurnover = r.realTurnover,
                        virtualTurnover = r.virtualTurnover,
                        realLineTotal = r.realLineTotal,
                        virtualLineTotal = r.virtualLineTotal,
                        liveindicator = false // add live indicator
                    });
                    break;

                case SportTypeWinLoss.Unknown:
                default:
                    break;
            }

            return report;
        }

        /// <summary>
        /// Builds the win loss detail.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="type">The type value.</param>
        /// <returns>The win loss detail.</returns>
        private MatchGameWinLossInfo BuildWinLossDetail(dynamic param, SportTypeWinLoss type)
        {
            MatchGameWinLossInfo matchGameWinLoss = null;
            switch (type)
            {
                case SportTypeWinLoss.SportBook:
                    matchGameWinLoss = this.BuildSportBookWinLossDetail(param);
                    break;

                case SportTypeWinLoss.VirtualSports:
                    matchGameWinLoss = this.BuildVirtualSportWinLossDetail(param);
                    break;

                case SportTypeWinLoss.Racing:
                case SportTypeWinLoss.VirtualRacing:
                    matchGameWinLoss = this.BuildRacingAndVirtualRacingWinLossDetail(param);
                    break;

                case SportTypeWinLoss.Unknown:
                default:
                    break;
            }

            return matchGameWinLoss;
        }

        /// <summary>
        /// Builds the sport book win loss detail.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>The sport book win loss detail.</returns>
        private MatchGameWinLossInfo BuildSportBookWinLossDetail(dynamic param)
        {
            MatchGameWinLossInfo matchWinLoss = new MatchGameWinLossInfo
            {
                MatchId = Converter.ToInt(param.matchid),
                MatchCode = Converter.ToString2(param.matchcode),
                HomeTeam = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(param.HomeId)
                },
                AwayTeam = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(param.AwayId)
                },
                LeagueId = Converter.ToInt(param.leagueid),
                IsNeutral = Converter.ToBool(param.isNeutral),
                WinLossDetail = this.BuildWinLossDetail(param)
            };

            matchWinLoss.WinLossDetail.LiveIndicator = Converter.ToInt(param.liveindicator);
            matchWinLoss.HomeTeam.Name = this.GetTeamName(matchWinLoss.HomeTeam.Id)
                + (matchWinLoss.IsNeutral ? Labels.IsNeutral : string.Empty);
            matchWinLoss.AwayTeam.Name = this.GetTeamName(matchWinLoss.AwayTeam.Id);

            return matchWinLoss;
        }

        /// <summary>
        /// Builds the virtual sport win loss detail.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>The virtual sport win loss detail.</returns>
        private MatchGameWinLossInfo BuildVirtualSportWinLossDetail(dynamic param)
        {
            MatchGameWinLossInfo matchWinLoss = new MatchGameWinLossInfo
            {
                MatchCode = Converter.ToString2(param.matchcode),
                HomeTeam = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(param.HomeId)
                },
                AwayTeam = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(param.AwayId)
                },
                LeagueId = Converter.ToInt(param.leagueid),
                WinLossDetail = this.BuildWinLossDetail(param)
            };

            // update Team name
            matchWinLoss.HomeTeam.Name = this.GetTeamName(matchWinLoss.HomeTeam.Id);
            matchWinLoss.AwayTeam.Name = this.GetTeamName(matchWinLoss.AwayTeam.Id);

            return matchWinLoss;
        }

        private MatchGameWinLossInfo BuildRacingAndVirtualRacingWinLossDetail(dynamic param)
        {
            return new MatchGameWinLossInfo
            {
                MatchCode = Converter.ToString2(param.matchcode),
                LeagueId = Converter.ToInt(param.leagueid),
                WinLossDetail = this.BuildWinLossDetail(param)
            };
        }

        /// <summary>
        /// Builds the win loss detail.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>The win loss detail.</returns>
        private WinLossInfo BuildWinLossDetail(dynamic param)
        {
            return new WinLossInfo
            {
                WinLoss = Converter.ToDecimal(param.realWinLost),
                Position = Converter.ToDecimal(param.realPosition),
                Commission = Converter.ToDecimal(param.realCommission),
                RealTurnover = this.ToDecimal(param.realTurnover),
                VirtualTurnover = this.ToDecimal(param.virtualTurnover),
                RealLineTotal = this.ToDecimal(param.realLineTotal),
                VirtualLineTotal = this.ToDecimal(param.virtualLineTotal)
            };
        }

        /// <summary>
        /// Calculate hedge total by sport.
        /// </summary>
        /// <param name="report">The report.</param>
        private void CalculateHedgeTotalBySport(WinLoss3rdInfo report)
        {
            bool hedgeIsExist = false;

            foreach (var sportType in report.SportTypeWinLosses)
            {
                hedgeIsExist = CheckHedgeExistInSportType(sportType);
                if (hedgeIsExist)
                {
                    sportType.HedgeTotal = new WinLossInfo
                    {
                        RealLineTotal = 0,
                        VirtualLineTotal = 0,
                        RealTurnover = 0,
                        VirtualTurnover = 0
                    };

                    foreach (var betType in sportType.BetTypeWinLosses)
                    {
                        sportType.CalculateHedgeTotal(betType.Hedge);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the type of the hedge exist in sport.
        /// </summary>
        /// <param name="sportTypeWinLossInfo">The sport type win loss information.</param>
        /// <returns>Hedge match is exist in sport type.</returns>
        private bool CheckHedgeExistInSportType(SportTypeWinLossInfo sportTypeWinLossInfo)
        {
            bool hedgeIsExistInSportType = false;
            foreach (var betType in sportTypeWinLossInfo.BetTypeWinLosses)
            {
                if (betType.Hedge != null)
                {
                    hedgeIsExistInSportType = true;
                    break;
                }
            }

            return hedgeIsExistInSportType;
        }
    }
}