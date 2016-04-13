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
    using System.Linq;
    using Fanex.AccountingReports.Core.Constants;
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
    using Fanex.Data.Repository;
    using Fanex.AccountingReports.Core.Configurations;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class WinLossSummary3rdService : ServiceBase, IWinLossSummary3rdService
    {
        private readonly IWinlossSummary3rdRepository _reportRepository;
        private readonly int _casinoSportType = 251;

        private Dictionary<object, string> _sportTypeNameCache;
        private Dictionary<object, string> _betTypeNameCache;
        private Dictionary<object, string> _leagueNameCache;
        private Dictionary<object, string> _teamNameCache;

        private string _winlossSummary3rdCacheCockFightingArenaReportKey = "Fanex.AccountingReports.Main.WinLossSummary3rd.CockFightingArenaReport" +
          (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

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
        public WinLoss3rdInfo GetReport(GetReportCriteria criteria)
        {
            WinLoss3rdInfo report = null;
            SportTypeWinLossInfo sportInfo = null;
            BetTypeWinLossInfo betInfo = null;
            WinLossInfo winloss = null;
            try
            {
                var rawData = GetReportData<dynamic>(criteria).ToList();
                bool isBeforeSingleWalletCasinoLaunchDate = DateTime.Compare(criteria.Fromdate, AppSettings.CasinoSingleWalletLaunchDate) < 0;
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
                if (_sportTypeNameCache.ContainsKey(_casinoSportType))
                {
                    _sportTypeNameCache[_casinoSportType] = Labels.MBC_RNG_Casino;
                }

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

                    bool isShowNotAvalable = sportId == Constants.SportContants._2dReportCasino && isBeforeSingleWalletCasinoLaunchDate;
                    // group by BetType & LiveIndicator
                    var betTypeGroup = sportTypeGroup.GroupBy(data => new { data.bettype, data.liveindicator });

                    var isNewCricket = false;
                    var dataForNewCricket = betTypeGroup.First();

                    foreach (var data in betTypeGroup)
                    {
                        betInfo = new BetTypeWinLossInfo();
                        betInfo.Id = data.Key.bettype;
                        if (data.Key.bettype == 500)
                        {
                            dataForNewCricket = data;
                            isNewCricket = true;
                            continue;
                        }
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
                                RealTurnover = isShowNotAvalable ? 0 : ToDecimal(item.realTurnover),
                                VirtualTurnover = isShowNotAvalable ? 0 : ToDecimal(item.virtualTurnover)
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
                            sportInfo.CalculateTotalNewCricket(winloss);
                            report.CalculateAllTotal(sportId, winloss);
                        }
                        sportInfo.BetTypeWinLosses.Add(betInfo);
                    }
                    if (isNewCricket == true)
                    {
                        sportInfo = CalculatorForNewCricket(dataForNewCricket, isShowNotAvalable, sportInfo, sportId, report);
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


        public SportTypeWinLossInfo CalculatorForNewCricket(dynamic data, bool isShowNotAvalable,
                                            SportTypeWinLossInfo sportInfo, int sportId, WinLoss3rdInfo report)
        {
            BetTypeWinLossInfo betInfo = new BetTypeWinLossInfo();
            //betInfo.Id = data.Key.bettype;
            foreach (var item in data)
            {
                betInfo.Id = item.bettype;
                WinLossInfo winloss = new WinLossInfo
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
                    RealTurnover = isShowNotAvalable ? 0 : ToDecimal(item.realTurnover),
                    VirtualTurnover = isShowNotAvalable ? 0 : ToDecimal(item.virtualTurnover)
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
                        winloss.Name = "Charged Commission";
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
            return sportInfo;
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

        public CockFightingArena GetCockFightingReport(GetWinLossSummary3rdCockFightingCriteria criteria)
        {
            var cockFightingArena = new CockFightingArena();
            var rawData = GetReportData<CockFightingMatchDetail>(criteria).ToList();
            cockFightingArena.ReportRows = GetCockFightingArena(rawData);
            cockFightingArena.GrandTotal = GetCockFightingArenaGrandTotal(rawData);
            CacheExtension.Set(_winlossSummary3rdCacheCockFightingArenaReportKey, cockFightingArena);
            return cockFightingArena;
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
                    sportName = Labels.MetricGaming_Superlive;
                    break;

                case SportContants.Macau:
                    sportName = Labels.Macau;
                    break;

                case SportContants.Bettrade:
                    sportName = Labels.Bettrade;
                    break;
                case SportContants.Allbet:
                    sportName = Labels.Allbet;
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
            else if (betId == MainConstants.Bettrade)
            {
                betName = Labels.Bettrade;
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
            var winLossBySportCriteria = this.CreateWinLossBySportTypeCriteria(criteria);
            var rawData = GetReportData<dynamic>(winLossBySportCriteria);

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

        private CriteriaBase CreateWinLossBySportTypeCriteria(GetMatchGameDetailsCriteria criteria)
        {
            SportTypeWinLoss type = criteria.Type;

            switch (type)
            {
                case SportTypeWinLoss.SportBook:
                    return CreateWinLossSummary3rdSportbookSportTypeCriteria(criteria);
                case SportTypeWinLoss.Racing:
                    return CreateWinLossSummary3rdRacingSportTypeCriteria(criteria);
                case SportTypeWinLoss.VirtualSports:
                    return CreateWinLossSummary3rdVirtualSportSportTypeCriteria(criteria);
                case SportTypeWinLoss.VirtualRacing:
                    return CreateWinLossSummary3rdVirtualRacingSportTypeCriteria(criteria);
                case SportTypeWinLoss.Unknown:
                default:
                    return criteria;
            }
        }

        private CriteriaBase CreateWinLossSummary3rdSportbookSportTypeCriteria(GetMatchGameDetailsCriteria criteria)
        {
            return new GetWinLossSummary3rdSportbookSportTypeCriteria
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
                SportType = criteria.SportType,
                BetType = criteria.BetType,
                LiveIndicator = criteria.LiveIndicator,
                DrillDownByType = criteria.DrillDownByType,
                SubProduct = criteria.SubProduct
            };
        }

        private CriteriaBase CreateWinLossSummary3rdVirtualSportSportTypeCriteria(GetMatchGameDetailsCriteria criteria)
        {
            return new GetWinLossSummary3rdVirtualSportSportTypeCriteria
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
                SportType = criteria.SportType
            };
        }

        private CriteriaBase CreateWinLossSummary3rdRacingSportTypeCriteria(GetMatchGameDetailsCriteria criteria)
        {
            return new GetWinLossSummary3rdRacingSportTypeCriteria
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
                SportType = criteria.SportType
            };
        }

        private CriteriaBase CreateWinLossSummary3rdVirtualRacingSportTypeCriteria(GetMatchGameDetailsCriteria criteria)
        {
            return new GetWinLossSummary3rdVirtualRacingSportTypeCriteria
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
                SportType = criteria.SportType
            };
        }

        private List<CockFightingArenaDetail> GetCockFightingArena(List<CockFightingMatchDetail> reportData)
        {
            List<CockFightingArenaDetail> arena = new List<CockFightingArenaDetail>();
            var arenaGroup = reportData.GroupBy(x => x.ArenaId);
            foreach (var group in arenaGroup)
            {
                var arenaMatchs = group.ToList();
                CockFightingArenaDetail arenaDetail = new CockFightingArenaDetail()
                {
                    ArenaName = CacheRef.GetLeagueName(Converter.ToInt(group.Key)),
                    CockFightingMatchs = new List<CockFightingMatchDetail>(),
                    ArenaTotal = GetCockFightingArenaGrandTotal(arenaMatchs)
                };

                foreach (var data in arenaMatchs)
                {
                    arenaDetail.CockFightingMatchs.Add(data);
                }

                arena.Add(arenaDetail);
            }

            return arena;
        }

        private CockFightingMatchDetail GetCockFightingArenaGrandTotal(List<CockFightingMatchDetail> cockFightingMatchs)
        {
            CockFightingMatchDetail grandTotal = new CockFightingMatchDetail();

            foreach (var item in cockFightingMatchs)
            {
                grandTotal.RealWinLoss += item.RealWinLoss;
                grandTotal.RealPosition += item.RealPosition;
                grandTotal.RealCommission += item.RealCommission;
                grandTotal.RealTurnover += item.RealTurnover;
                grandTotal.RealLineTotal += item.RealLineTotal;
                grandTotal.VirtualTurnover += item.VirtualTurnover;
                grandTotal.VirtualLineTotal += item.VirtualLineTotal;
            }

            return grandTotal;
        }
    }
}