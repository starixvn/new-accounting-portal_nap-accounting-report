namespace Fanex.AccountingReports.Main.Services.Implement.WinLoss3rdSummary
{
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Models.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Repositories.Implement;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class WinLossSummary3rdService : ServiceBase
    {
        private readonly WinlossSummary3rdRepository _reportRepository;
        private readonly int _casinoSportType = 251;

        private Dictionary<object, string> _sportTypeNameCache;
        private Dictionary<object, string> _betTypeNameCache;
        private Dictionary<object, string> _leagueNameCache;
        private Dictionary<object, string> _teamNameCache;

        public WinLossSummary3rdService()
            : this(null)
        {
        }

        public WinLossSummary3rdService(WinlossSummary3rdRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new WinlossSummary3rdRepository();
        }

        //// Implementation methods

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLoss3rdInfo GetReport(GetReportCriteria criteria)
        {
            WinLoss3rdInfo report = null;
            SportTypeWinLossInfo sportInfo = null;
            WinLossInfo winloss = null;
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

            CacheRef.SaveCacheRefResponse(sportTypeHash.Values.ToList(), CacheRef.GetSportName, ref this._sportTypeNameCache);
            if (_sportTypeNameCache.ContainsKey(_casinoSportType))
            {
                _sportTypeNameCache[_casinoSportType] = Labels.Saba_Casino;
            }

            CacheRef.SaveCacheRefResponse(betTypeHash.Values.ToList(), CacheRef.GetBettype, ref this._betTypeNameCache);

            //var sportTypeGroups = rawData.GroupBy(data => data.sporttype);
            var sportTypeGroups = rawData.Where(x => x.bettype != 913 && x.bettype != 914).GroupBy(data => data.sporttype);
            var sportsNG2Groups = new List<IGrouping<dynamic, dynamic>>();

            foreach (var sportTypeGroup in sportTypeGroups)
            {
                int sportId = Converter.ToInt(sportTypeGroup.Key);
                sportInfo = new SportTypeWinLossInfo
                {
                    Id = sportId,
                    Name = this.GetSportName(sportId)
                };

                if (sportInfo.Id >= 213 && sportInfo.Id <= 218)
                {
                    sportsNG2Groups.Add(sportTypeGroup);
                    continue;
                }

                GenerateSport(report, sportTypeGroup, isBeforeSingleWalletCasinoLaunchDate, winloss);
            }

            if (sportsNG2Groups.Count > 0)
            {
                CalculateNG2Product(report, isBeforeSingleWalletCasinoLaunchDate, sportsNG2Groups);
            }

            var cashOutData = rawData.Where(x => x.bettype == 913 || x.bettype == 914).ToList();
            CalculateCashOutProduct(report, isBeforeSingleWalletCasinoLaunchDate, cashOutData);

            this.CalculateHedgeTotalBySport(report);
            this.CalculateNormalTotalBySport(report);

            SessionManager.Set<WinLoss3rdInfo>(MainConstants.WinLossSummary3rdReportSessionKey, report);

            return report;
        }

        public WinLossInfo CreateWinlossInfo(dynamic item, bool isShowNotAvalable)
        {
            return new WinLossInfo
            {
                Id = Converter.ToInt(item.bettype),
                SportId = Converter.ToInt(item.sporttype),
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
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLoss3rdInfo CalculateNG2Product(WinLoss3rdInfo report, bool isBeforeDate, List<IGrouping<dynamic, dynamic>> sportTypeGroups)
        {
            var sportId = 23;
            bool isShowNotAvalable = sportId == Constants.SportContants._2dReportCasino && isBeforeDate;
            var sportInfo = new SportTypeWinLossInfo
            {
                Id = sportId,
                Name = "NG2"
            };

            foreach (var sportTypeGroup in sportTypeGroups)
            {
                var winlossTotal = new WinLossInfo();
                winlossTotal.Name = this.GetSportName(sportTypeGroup.Key);

                foreach (var item in sportTypeGroup)
                {
                    var betInfo = new BetTypeWinLossInfo();
                    betInfo.Id = item.bettype;
                    betInfo.Name = this.GetBetType(item.bettype);

                    WinLossInfo winloss = CreateWinlossInfo(item, isShowNotAvalable);

                    betInfo.Normal = winloss;

                    sportInfo.CalculateTotal(winloss);

                    sportInfo.BetTypeWinLosses.Add(betInfo);
                    winlossTotal = CalculateWinlossTotal(winlossTotal, winloss);
                }

                var totalSportType = new BetTypeWinLossInfo();
                totalSportType.Id = sportTypeGroup.Key;
                totalSportType.Name = this.GetSportName(sportTypeGroup.Key);
                totalSportType.Normal = winlossTotal;

                sportInfo.BetTypeWinLosses.Add(totalSportType);
            }

            report.SportTypeWinLosses.Add(sportInfo);
            return report;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLoss3rdInfo CalculateCashOutProduct(WinLoss3rdInfo report, bool isBeforeDate, List<dynamic> cashOutItems)
        {
            var sportId = -3;
            bool isShowNotAvalable = sportId == Constants.SportContants._2dReportCasino && isBeforeDate;
            var sportInfo = new SportTypeWinLossInfo
            {
                Id = sportId,
                Name = "CashOut",
            };

            var sportTypeGroups = cashOutItems.GroupBy(x => new { x.sporttype, x.liveindicator });

            foreach (var sportTypeGroup in sportTypeGroups)
            {
                var winlossTotal = new WinLossInfo();
                winlossTotal.Name = this.GetSportName(sportTypeGroup.Key.sporttype);

                foreach (var item in sportTypeGroup)
                {
                    WinLossInfo winloss = CreateWinlossInfo(item, isShowNotAvalable);
                    sportInfo.CalculateTotal(winloss);
                    winlossTotal = CalculateWinlossTotal(winlossTotal, winloss);
                }

                winlossTotal.LiveIndicator = (bool)sportTypeGroup.Key.liveindicator ? 1 : 0;
                var totalSportType = new BetTypeWinLossInfo();
                totalSportType.Id = sportTypeGroup.Key.sporttype;
                totalSportType.Name = this.GetSportName(sportTypeGroup.Key.sporttype);
                totalSportType.LiveIndicator = (bool)sportTypeGroup.Key.liveindicator ? 1 : 0;
                totalSportType.Normal = winlossTotal;

                sportInfo.BetTypeWinLosses.Add(totalSportType);
            }

            report.SportTypeWinLosses.Add(sportInfo);
            return report;
        }

        public WinLossInfo CalculateWinlossTotal(WinLossInfo total, WinLossInfo item)
        {
            total.WinLoss += item.WinLoss;
            total.Position += item.Position;
            total.Commission += item.Commission;
            total.RealLineTotal = total.RealLineTotal == null ? item.RealLineTotal : total.RealLineTotal += item.RealLineTotal;
            total.VirtualLineTotal = total.VirtualLineTotal == null ? item.VirtualLineTotal : total.VirtualLineTotal += item.VirtualLineTotal;
            total.RealTurnover = total.RealTurnover == null ? item.RealTurnover : total.RealTurnover += item.RealTurnover;
            total.VirtualTurnover = total.VirtualTurnover == null ? item.VirtualTurnover : total.VirtualTurnover += item.VirtualTurnover;
            return total;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLoss3rdInfo GenerateSport(WinLoss3rdInfo report, IGrouping<dynamic, dynamic> sportTypeGroup, bool isBeforeDate, WinLossInfo winloss)
        {
            int sportId = Converter.ToInt(sportTypeGroup.Key);
            BetTypeWinLossInfo betInfo = null;
            var sportInfo = new SportTypeWinLossInfo
            {
                Id = sportId,
                Name = this.GetSportName(sportId)
            };

            bool isShowNotAvalable = sportId == Constants.SportContants._2dReportCasino && isBeforeDate;
            var betTypeGroup = sportTypeGroup.GroupBy(data => new { data.bettype, data.liveindicator, data.IsFreePlay });
            var cnt = betTypeGroup.Count();

            var isNewCricket = false;
            var dataForNewCricket = betTypeGroup.First();

            foreach (var data in betTypeGroup)
            {
                var bettypeName = this.GetBetType(data.Key.bettype);
                var isFreePlay = Converter.ToInt(data.Key.IsFreePlay) == 1;
                betInfo = new BetTypeWinLossInfo();
                betInfo.Id = data.Key.bettype;
                betInfo.LiveIndicator = (bool)data.Key.liveindicator ? 1 : 0;
                betInfo.Name = isFreePlay ? bettypeName + "(Free Play)" : bettypeName;

                if (data.Key.bettype == 500)
                {
                    dataForNewCricket = data;
                    isNewCricket = true;
                    continue;
                }

                if (data.Key.bettype == 9 || data.Key.bettype == 29)
                {
                    var mixPalayBetTypes = GetParlayBetTypes(data, data.Key.bettype, isShowNotAvalable, sportInfo);
                    sportInfo.BetTypeWinLosses.AddRange(mixPalayBetTypes);
                    continue;
                }

                foreach (var item in data)
                {
                    winloss = CreateWinlossInfo(item, isShowNotAvalable);

                    if (winloss.IBC == RowType.EightRM)
                    {
                        winloss.Name = Labels.EightRM;
                        betInfo.EightRM = winloss;

                        // IBC Total by BetType
                        sportInfo.CalculateIBCTotal(winloss);
                    }
                    else if (winloss.IBC == RowType.BARM)
                    {
                        winloss.Name = Labels.BARM;
                        betInfo.BARM = winloss;

                        // IBC Total by BetType
                        sportInfo.CalculateIBCTotal(winloss);
                    }
                    else
                    {
                        if (MainConstants.WinLossSummaryRacingSportIds.Contains(sportInfo.Id))
                        {
                            // Horse Racing, Greyhounds, Harness are grouped by Racing
                            betInfo.Name = sportInfo.Name;
                            sportInfo.Name = Labels.Racing;
                            winloss.Id = sportId;
                        }
                        else if (sportInfo.Id >= MainConstants.WinLossSummaryBeginVirtualSportId &&
                            sportInfo.Id <= MainConstants.WinLossSummaryEndVirtualSportId)
                        {
                            betInfo.Name = sportInfo.Name;
                            sportInfo.Name = Labels.VirtualSport;
                            winloss.Id = sportId;
                        }
                        else if (sportInfo.Id == 161 || sportInfo.Id == 164 || sportInfo.Id == 168)
                        {
                            betInfo.Name = this.GetSportName(sportId);
                            sportInfo.Name = "Number Game";
                            winloss.Id = item.bettype;
                        }
                        else if (sportInfo.Id >= MainConstants.WinLossSummaryBeginBetradarVirtualSportId &&
                            sportInfo.Id <= MainConstants.WinLossSummaryEndBetradarVirtualSportId)
                        {
                            betInfo.Name = sportInfo.Name;
                            sportInfo.Name = "Virtual Sports 2";
                            winloss.Id = sportId;
                        }
                        else
                        {
                            var freePlay = isFreePlay ? "(Free Play)" : string.Empty;
                            betInfo.Name = this.GetBetType(winloss.Id) + freePlay;
                        }

                        if (winloss.IsHedge)
                        {
                            winloss.Name = Labels.Hedge_title;
                            betInfo.Hedge = winloss;
                        }
                        else
                        {
                            winloss.Name = Labels.Normal_title;
                            betInfo.Normal = winloss;
                        }
                    }

                    sportInfo.CalculateTotal(winloss);
                    sportInfo.CalculateTotalNewCricket(winloss);
                }

                sportInfo.BetTypeWinLosses.Add(betInfo);
            }

            if (isNewCricket)
            {
                sportInfo = CalculatorForNewCricket(dataForNewCricket, isShowNotAvalable, sportInfo, sportId);
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
            else if (sportInfo.Id == 161 || sportInfo.Id == 164 || sportInfo.Id == 168)
            {
                SportTypeWinLossInfo ng = report.SportTypeWinLosses.FirstOrDefault(sport => sport.Id == 161);
                if (ng == null)
                {
                    ng = sportInfo;
                    report.SportTypeWinLosses.Add(ng);
                }
                else
                {
                    foreach (var row in sportInfo.BetTypeWinLosses)
                    {
                        ng.BetTypeWinLosses.Add(row);
                        ng.CalculateTotal(row.Normal);
                    }
                }
            }
            else if (sportInfo.Id >= MainConstants.WinLossSummaryBeginBetradarVirtualSportId && sportInfo.Id <= MainConstants.WinLossSummaryEndBetradarVirtualSportId)
            {
                SportTypeWinLossInfo betradarVirtualSport = report.SportTypeWinLosses.FirstOrDefault(sport => sport.Id == MainConstants.WinLossSummaryBetradarVirtualSportId);
                if (betradarVirtualSport == null)
                {
                    sportInfo.Id = MainConstants.WinLossSummaryBetradarVirtualSportId;
                    betradarVirtualSport = sportInfo;
                    report.SportTypeWinLosses.Add(betradarVirtualSport);
                }
                else
                {
                    betradarVirtualSport.BetTypeWinLosses.Add(betInfo);
                    betradarVirtualSport.CalculateTotal(winloss);
                }
            }
            else
            {
                report.SportTypeWinLosses.Add(sportInfo);
            }

            return report;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
          "SP2100:CodeLineMustNotBeLongerThan",
          Justification = "Reviewed. Suppression is OK here.")]
        public List<BetTypeWinLossInfo> GetParlayBetTypes(IGrouping<dynamic, dynamic> data, int betType, bool isShowNotAvalable, SportTypeWinLossInfo sportInfo)
        {
            var parlayBetTypes = new List<BetTypeWinLossInfo>();
            foreach (var item in data)
            {
                var winloss = CreateWinlossInfo(item, isShowNotAvalable);
                var betInfo = new BetTypeWinLossInfo();
                betInfo.Id = betType;
                betInfo.Name = this.GetBetType(winloss.Id);

                if (!string.IsNullOrWhiteSpace(winloss.Status))
                {
                    betInfo.Name += " (" + winloss.Status + ")";
                }

                if (winloss.IsHedge)
                {
                    winloss.Name = Labels.Hedge_title;
                    betInfo.Hedge = winloss;
                }
                else
                {
                    winloss.Name = Labels.Normal_title;
                    betInfo.Normal = winloss;
                }

                sportInfo.CalculateTotal(winloss);

                parlayBetTypes.Add(betInfo);
            }

            return parlayBetTypes;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SportTypeWinLossInfo CalculatorForNewCricket(dynamic data, bool isShowNotAvalable, SportTypeWinLossInfo sportInfo, int sportId)
        {
            BetTypeWinLossInfo betInfo = new BetTypeWinLossInfo();

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
                }
                else if (winloss.IBC == RowType.BARM)
                {
                    winloss.Name = Labels.BARM;
                    betInfo.BARM = winloss;

                    // IBC Total by BetType
                    sportInfo.CalculateIBCTotal(winloss);
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
                        betInfo.Name = sportInfo.Name;
                        sportInfo.Name = Labels.VirtualSport;
                        winloss.Id = sportId;
                    }
                    else if (sportInfo.Id >= MainConstants.WinLossSummaryBeginBetradarVirtualSportId &&
                        sportInfo.Id <= MainConstants.WinLossSummaryEndBetradarVirtualSportId)
                    {
                        betInfo.Name = sportInfo.Name;
                        sportInfo.Name = "Virtual Sports 2";
                        winloss.Id = sportId;
                    }
                    else
                    {
                        betInfo.Name = "Charged Commission";
                    }

                    if (!string.IsNullOrWhiteSpace(winloss.Status))
                    {
                        betInfo.Name += " (" + winloss.Status + ")";
                    }

                    if (winloss.IsHedge)
                    {
                        winloss.Name = Labels.Hedge_title;
                        betInfo.Hedge = winloss;
                    }
                    else
                    {
                        winloss.Name = Labels.Normal_title;
                        betInfo.Normal = winloss;
                    }
                }

                sportInfo.CalculateTotal(winloss);
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
            this._reportRepository.RecalculateWinLoss(criteria);
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The details.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
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

                    if (type == SportTypeWinLoss.SportBook || type == SportTypeWinLoss.NumberGame || type == SportTypeWinLoss.VirtualSports)
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
                if (type == SportTypeWinLoss.SportBook || type == SportTypeWinLoss.NumberGame || type == SportTypeWinLoss.VirtualSports)
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
                    var leagueGroups = betTypeId == BetTypeConstants.BullfightingBetType ?
                        betTypeGroup.GroupBy(data => new { data.leagueid, data.leaguecode }) :
                        betTypeGroup.GroupBy(data => data.leagueid);
                    foreach (var leagueGroup in leagueGroups)
                    {
                        leagueid = betTypeId == BetTypeConstants.BullfightingBetType ?
                            Converter.ToInt(leagueGroup.Key.leagueid) :
                            Converter.ToInt(leagueGroup.Key);
                        var leagueName = betTypeId == BetTypeConstants.BullfightingBetType ?
                            this.GetBullfightingLeagueName(leagueid, leagueGroup.Key.leaguecode) :
                            this.GetLeagueName(leagueid);

                        leagueSummary = new MatchGameLeagueInfo
                        {
                            Key = new EntityBase<int, string>
                            {
                                Id = leagueid,
                                Name = leagueName
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

                case SportContants.Bettrade:
                    sportName = Labels.Bettrade;
                    break;

                case SportContants.Allbet:
                    sportName = Labels.Allbet;
                    break;

                case SportContants.CashOut:
                    sportName = Labels.CashOut;
                    break;

                case SportContants.VirtualSports3:
                    sportName = Labels.VirtualSports3;
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
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
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

                case SportTypeWinLoss.Bullfighting:
                    report = rawData.Select(r => new
                    {
                        matchid = r.matchid,
                        matchcode = r.matchcode,
                        homeid = r.homeid,
                        awayid = r.awayid,
                        bettype = r.bettype,
                        leagueid = r.leagueid,
                        leaguecode = r.LeagueCode,
                        realWinLost = r.realWinlost,
                        realPosition = r.realPosition,
                        realCommission = r.realCommission,
                        realTurnover = r.realTurnover,
                        virtualTurnover = r.virtualTurnover,
                        realLineTotal = r.realLineTotal,
                        virtualLineTotal = r.virtualLineTotal,
                        liveindicator = r.liveindicator, // add live indicator
                        isNeutral = r.isNeutral
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
                case SportTypeWinLoss.NumberGame:
                    matchGameWinLoss = this.BuildSportBookWinLossDetail(param);
                    break;

                case SportTypeWinLoss.VirtualSports:
                    matchGameWinLoss = this.BuildVirtualSportWinLossDetail(param);
                    break;

                case SportTypeWinLoss.Racing:
                case SportTypeWinLoss.VirtualRacing:
                    matchGameWinLoss = this.BuildRacingAndVirtualRacingWinLossDetail(param);
                    break;

                case SportTypeWinLoss.Bullfighting:
                    matchGameWinLoss = this.BuilBullfightingWinLossDetail(param);
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
            matchWinLoss.HomeTeam.Name = this.GetTeamName(matchWinLoss.HomeTeam.Id) + GetPlayOffBettype(true, matchWinLoss.IsNeutral, matchWinLoss.MatchCode);
            matchWinLoss.AwayTeam.Name = this.GetTeamName(matchWinLoss.AwayTeam.Id) + GetPlayOffBettype(true, matchWinLoss.IsNeutral, matchWinLoss.MatchCode);

            return matchWinLoss;
        }

        private Dictionary<string, string> PlayOffBettype => new Dictionary<string, string>
        {
            { "(o)", "(O/U)" },
            { "(h)", "(HDP)" }
        };

        private string GetPlayOffBettype(bool isHome, bool isNeutral, string matchCode)
        {
            matchCode = matchCode.Replace(" ", string.Empty).ToLower();
            var bettype = PlayOffBettype.FirstOrDefault(s => matchCode.Contains(s.Key)).Value ?? string.Empty;
            return isHome && isNeutral ? $" (N) {bettype}" : $" {bettype}";
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
                SportId = Converter.ToInt(param.sporttype),
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
        /// Calculate hedge total by sport.
        /// </summary>
        /// <param name="report">The report.</param>
        private void CalculateNormalTotalBySport(WinLoss3rdInfo report)
        {
            foreach (var sportType in report.SportTypeWinLosses)
            {
                sportType.NormalTotal = new WinLossInfo
                {
                    RealLineTotal = 0,
                    VirtualLineTotal = 0,
                    RealTurnover = 0,
                    VirtualTurnover = 0
                };

                foreach (var betType in sportType.BetTypeWinLosses)
                {
                    sportType.CalculateNormalTotal(betType.Normal);
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

                case SportTypeWinLoss.NumberGame:
                    return CreateWinLossSummary3rdNumberGameSportTypeCriteria(criteria);

                case SportTypeWinLoss.Racing:
                    return CreateWinLossSummary3rdRacingSportTypeCriteria(criteria);

                case SportTypeWinLoss.VirtualSports:
                    return CreateWinLossSummary3rdVirtualSportSportTypeCriteria(criteria);

                case SportTypeWinLoss.VirtualRacing:
                    return CreateWinLossSummary3rdVirtualRacingSportTypeCriteria(criteria);

                case SportTypeWinLoss.Bullfighting:
                    return CreateWinLossSummary3rdBullfightingSportTypeCriteria(criteria);

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

        private CriteriaBase CreateWinLossSummary3rdNumberGameSportTypeCriteria(GetMatchGameDetailsCriteria criteria)
        {
            return new GetWinLossSummary3rdNumberGameSportTypeCriteria
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
                SportType = criteria.SportType,
                BetType = criteria.BetType
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

        private CriteriaBase CreateWinLossSummary3rdBullfightingSportTypeCriteria(GetMatchGameDetailsCriteria criteria)
        {
            return new GetWinLossSummary3rdBullfightingSportTypeCriteria
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
            };
        }

        private MatchGameWinLossInfo BuilBullfightingWinLossDetail(dynamic param)
        {
            MatchGameWinLossInfo matchWinLoss = new MatchGameWinLossInfo
            {
                MatchCode = Converter.ToString2(param.matchcode),
                HomeTeam = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(param.homeid)
                },
                AwayTeam = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(param.awayid)
                },
                LeagueId = Converter.ToInt(param.leagueid),
                LeagueCode = Converter.ToString2(param.leaguecode),
                WinLossDetail = this.BuildWinLossDetail(param)
            };

            // update Team name
            matchWinLoss.HomeTeam.Name = CacheRef.GetTeamName3rd(matchWinLoss.HomeTeam.Id);
            matchWinLoss.AwayTeam.Name = CacheRef.GetTeamName3rd(matchWinLoss.AwayTeam.Id);

            return matchWinLoss;
        }

        private string GetBullfightingLeagueName(int leagueId, string leagueCode)
        {
            return $"{leagueCode} - {CacheRef.GetLeagueName3rd(leagueId)}";
        }
    }
}