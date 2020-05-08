namespace Fanex.AccountingReports.WinLoss.Services.Implement
{
    using Entities.LeagueWinLoss;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Repositories.Implement;
    using Fanex.AccountingReports.WinLoss.Resources;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeagueWinLossService : ServiceBase
    {
        private const int CHARGECOMMISSION = 500;

        private LeagueWinLossRepository _reportRepository;
        private RMElement _8rmConfig;
        private RMElement _barmConfig;
        private Dictionary<object, string> _leagueNameCache;
        private Dictionary<object, string> _teamNameCache;
        private Dictionary<object, string> _betTypeNameCache;

        public LeagueWinLossService()
            : this(null)
        {
        }

        public LeagueWinLossService(LeagueWinLossRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new LeagueWinLossRepository();
        }

        /// <summary>
        /// Gets the league win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The league win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LeagueWinLossModel GetLeagueWinLoss(GetLeagueWinLossCriteria criteria)
        {
            var rawData = this._reportRepository.GetLeagueWinLoss(criteria);
            var leagueWinLoss = new LeagueWinLossModel
            {
                SportWinLoss = new List<SportWinLoss>(),
                SoccerBARM = new LeagueWinLossItem(),
                OtherSportBARM = new LeagueWinLossItem(),
                SoccerRM = new LeagueWinLossItem(),
                OtherSportRM = new LeagueWinLossItem(),
                Total = new LeagueWinLossItem(),
                GrandTotal = new LeagueWinLossItem()
            };

            var sportGroups = rawData.Where(x => x.LeagueName != "Soccer 8RM"
                                                && x.LeagueName != "Other Sports 8RM"
                                                && x.LeagueName != "Soccer BARM"
                                                && x.LeagueName != "Other Sports BARM")
                                                 .GroupBy(x => new { x.SportType, x.Sport });

            foreach (var sportGroup in sportGroups)
            {
                var rawSports = rawData.Where(x => x.SportType == sportGroup.Key.SportType);
                var sportName = sportGroup.Key.Sport;
                var leagues = BindingLeagues(rawSports);
                var sportTotal = CalculateSportTotal(sportName + " " + "Total", rawSports);

                var sportWinLoss = new SportWinLoss
                {
                    SportName = sportName,
                    Leagues = leagues.ToList(),
                    Total = sportTotal
                };

                leagueWinLoss.SportWinLoss.Add(sportWinLoss);
            }

            var soccerRM = rawData.FirstOrDefault(x => x.LeagueName == "Soccer 8RM");
            leagueWinLoss.SoccerRM = BindingLeague(soccerRM);

            var otherRM = rawData.FirstOrDefault(x => x.LeagueName == "Other Sports 8RM");
            leagueWinLoss.OtherSportRM = BindingLeague(otherRM);

            var soccerBARM = rawData.FirstOrDefault(x => x.LeagueName == "Soccer BARM");
            leagueWinLoss.SoccerBARM = BindingLeague(soccerBARM);


            var otherBARM = rawData.FirstOrDefault(x => x.LeagueName == "Other Sports BARM");
            leagueWinLoss.OtherSportBARM = BindingLeague(otherBARM);

            var total = rawData.Where(x => x.LeagueName != "Soccer 8RM"
                                        && x.LeagueName != "Other Sports 8RM"
                                        && x.LeagueName != "Soccer BARM"
                                        && x.LeagueName != "Other Sports BARM");
            var allTotal = CalculateSportTotal("Total", total);
            leagueWinLoss.Total = allTotal;

            var grandTotal = CalculateSportTotal("Grand Total", rawData);
            leagueWinLoss.GrandTotal = grandTotal;

            // SessionManager.Set(WinLossContants.LeagueWinLossSessionKey, leagueWinLoss);
            return leagueWinLoss;
        }

        public List<SportGroup> GetSports()
        {
            var sportGroups = new List<SportGroup>();
            var criteria = new GetSportCriteria();
            var rawData = this._reportRepository.GetSport(criteria);
            var groups = rawData.GroupBy(x => new { x.SportGroupType, x.SportGroupName });
            foreach (var group in groups)
            {
                var sportGroup = new SportGroup
                {
                    GroupId = group.Key.SportGroupType,
                    GroupName = group.Key.SportGroupName,
                    ChildItems = new List<Models.LeagueWinLoss.Sport>()
                };

                if (group.Count() == 1)
                {
                    sportGroups.Add(sportGroup);
                    continue;
                }

                foreach (var item in group)
                {
                    sportGroup.ChildItems.Add(new Models.LeagueWinLoss.Sport
                    {
                        SportType = item.SportType,
                        SportName = item.SportName
                    });
                }

                sportGroups.Add(sportGroup);
            }

            return sportGroups;
        }


        /// <summary>
        /// Gets the league win loss by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The league win loss by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<LiveNonLiveWinLossModel> GetLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria)
        {
            var result = new List<LiveNonLiveWinLossModel>();
            var rawData = this._reportRepository.GetLeagueWinLossBySport(criteria);

            if (!rawData.Any())
            {
                return result;
            }

            var liveReport = GetLeagueWinLossBySportLive(rawData);
            var liveBettypeWinLossModel = new LiveNonLiveWinLossModel
            {
                Id = LiveTypeConstants.Live,
                Name = Labels.Live.ToUpper(),
                BetTypeWinLoss = liveReport
            };

            result.Add(liveBettypeWinLossModel);

            var nonLiveReport = GetLeagueWinLossBySportNonLive(rawData);
            var nonLiveBettypeWinLossModel = new LiveNonLiveWinLossModel
            {
                Id = LiveTypeConstants.NonLive,
                Name = Labels.None_live.ToUpper(),
                BetTypeWinLoss = nonLiveReport
            };

            result.Add(nonLiveBettypeWinLossModel);

            // SessionManager.Set(WinLossContants.LeagueWinLossBySportSessionKey, result);
            return result;
        }

        /// <summary>
        /// Gets the league win loss by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The league win loss by bet type.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public MatchWinLossModel GetLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria)
        {
            var rawData = this._reportRepository.GetLeagueWinLossByBetType(criteria);
            var leagueWinLoss = new MatchWinLossModel
            {
                Match = new List<MatchWinLossItem>(),
                RM = new MatchWinLossItem(),
                BARM = new MatchWinLossItem(),
                Total = new MatchWinLossItem(),
                GrandTotal = new MatchWinLossItem()
            };

            var rawMatches = rawData.Where(x =>
                !x.MatchEvent.Contains(criteria.SportName + " " + "BARM")
                && !x.MatchEvent.Contains(criteria.SportName + " " + "8RM"));
            var matches = BindingMatches(rawMatches);
            leagueWinLoss.Match = matches.ToList();

            leagueWinLoss.Total = CalculateMatchTotal("Total", rawMatches);

            var eightRM = rawData.FirstOrDefault(x => x.MatchEvent.Contains(criteria.SportName + " " + "8RM"));
            leagueWinLoss.RM = BindingMatch(eightRM);

            var barRM = rawData.FirstOrDefault(x => x.MatchEvent.Contains(criteria.SportName + " " + "BARM"));
            leagueWinLoss.BARM = BindingMatch(barRM);

            leagueWinLoss.GrandTotal = CalculateMatchTotal("Grand Total", rawData);

            //SessionManager.Set(WinLossContants.LeagueWinLossByBetTypeSessionKey, leagueWinLoss);
            return leagueWinLoss;
        }

        /// <summary>
        /// Gets the league win loss by sport.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>The league win loss by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private BettypeWinLossModel GetLeagueWinLossBySportLive(IEnumerable<BettypeWinLossEntity> rawData)
        {
            var liveWinLoss = rawData.Where(data => data.LiveIndicator);
            var report = CalculateBettypeByLiveNonLive(liveWinLoss);

            return report;
        }

        private BettypeWinLossModel GetLeagueWinLossBySportNonLive(IEnumerable<BettypeWinLossEntity> rawData)
        {
            var nonLiveWinLoss = rawData.Where(data => !data.LiveIndicator);
            var report = CalculateBettypeByLiveNonLive(nonLiveWinLoss);
            return report;
        }

        private BettypeWinLossModel CalculateBettypeByLiveNonLive(IEnumerable<BettypeWinLossEntity> rawData)
        {
            var report = new BettypeWinLossModel
            {
                BettypeWinLoss = new List<BettypeWinLossItem>(),
                RM = new BettypeWinLossItem(),
                BARM = new BettypeWinLossItem(),
                Total = new BettypeWinLossItem(),
                GrandTotal = new BettypeWinLossItem()
            };

            var bettypeWinLoss = BindingBettypes(rawData);
            report.BettypeWinLoss = bettypeWinLoss.ToList();

            var total = rawData.Where(x => !x.BettypeName.Contains("R8M")
                                       && !x.BettypeName.Contains("BARM"));
            var allTotal = CalculateBettypeTotal(total);
            report.Total = allTotal;

            var eightRM = rawData.FirstOrDefault(x => x.BettypeName.Contains("8RM"));
            report.RM = BindingBettype(eightRM);

            var barRM = rawData.FirstOrDefault(x => x.BettypeName.Contains("BARM"));
            report.BARM = BindingBettype(barRM);

            var grandTotal = CalculateBettypeTotal(rawData);
            grandTotal.BettypeName = Labels.Grand_Total;
            report.GrandTotal = grandTotal;

            return report;
        }

        private LeagueWinLossItem CalculateSportTotal(string sport, IEnumerable<LeagueWinLossEntity> sportGroup)
        {
            return new LeagueWinLossItem
            {
                LeagueName = sport,
                MemberWinLoss = sportGroup.Sum(x => x.MemberWinLoss),
                SMAWinLoss = sportGroup.Sum(x => x.SMAWinLoss),
                ExtraPT = sportGroup.Sum(x => x.ExtraPT),
                Commission = sportGroup.Sum(x => x.Commission),
                VirtualWinLoss = sportGroup.Sum(x => x.VirtualWinLoss),
                RealIncome = sportGroup.Sum(x => x.RealIncome),
                VirtualTurnover = sportGroup.Sum(x => x.VirtualTurnover),
                RealTurnover = sportGroup.Sum(x => x.RealTurnover)
            };
        }

        private IEnumerable<LeagueWinLossItem> BindingLeagues(IEnumerable<LeagueWinLossEntity> sportGroup)
        {
            return sportGroup.Select(x => BindingLeague(x));
        }

        private LeagueWinLossItem BindingLeague(LeagueWinLossEntity rawData)
        {
            if (rawData == null)
            {
                return new LeagueWinLossItem();
            }

            return new LeagueWinLossItem
            {
                Sport = rawData.Sport,
                SportType = rawData.SportType,
                LeagueId = rawData.LeagueId,
                LeagueName = rawData.LeagueName,
                MemberWinLoss = rawData.MemberWinLoss,
                SMAWinLoss = rawData.SMAWinLoss,
                ExtraPT = rawData.ExtraPT,
                Commission = rawData.Commission,
                VirtualWinLoss = rawData.VirtualWinLoss,
                RealIncome = rawData.RealIncome,
                VirtualTurnover = rawData.VirtualTurnover,
                RealTurnover = rawData.RealTurnover
            };
        }

        private BettypeWinLossItem CalculateBettypeTotal(IEnumerable<BettypeWinLossEntity> bettypes)
        {
            return new BettypeWinLossItem
            {
                BettypeName = Labels.Total,
                MemberWinLoss = bettypes.Sum(x => x.MemberWinLoss),
                SMAWinLoss = bettypes.Sum(x => x.SMAWinLoss),
                ExtraPT = bettypes.Sum(x => x.ExtraPT),
                Commission = bettypes.Sum(x => x.Commission),
                VirtualWinLoss = bettypes.Sum(x => x.VirtualWinLoss),
                RealIncome = bettypes.Sum(x => x.RealIncome),
                VirtualTurnover = bettypes.Sum(x => x.VirtualTurnover),
                RealTurnover = bettypes.Sum(x => x.RealTurnover)
            };
        }

        private IEnumerable<BettypeWinLossItem> BindingBettypes(IEnumerable<BettypeWinLossEntity> bettypeGroup)
        {
            return bettypeGroup.Select(x => BindingBettype(x));
        }

        private BettypeWinLossItem BindingBettype(BettypeWinLossEntity rawData)
        {
            if (rawData == null)
            {
                return new BettypeWinLossItem();
            }

            return new BettypeWinLossItem
            {
                LiveIndicator = rawData.LiveIndicator,
                Bettype = rawData.Bettype,
                BettypeName = rawData.BettypeName,
                IsHedge = rawData.IsHedge,
                MemberWinLoss = rawData.MemberWinLoss,
                SMAWinLoss = rawData.SMAWinLoss,
                ExtraPT = rawData.ExtraPT,
                Commission = rawData.Commission,
                VirtualWinLoss = rawData.VirtualWinLoss,
                RealIncome = rawData.RealIncome,
                VirtualTurnover = rawData.VirtualTurnover,
                RealTurnover = rawData.RealTurnover
            };
        }

        private MatchWinLossItem CalculateMatchTotal(string title, IEnumerable<MatchEntity> matches)
        {
            return new MatchWinLossItem
            {
                MatchEvent = title,
                MemberWinLoss = matches.Sum(x => x.MemberWinLoss),
                SMAWinLoss = matches.Sum(x => x.SMAWinLoss),
                ExtraPT = matches.Sum(x => x.ExtraPT),
                Commission = matches.Sum(x => x.Commission),
                VirtualWinLoss = matches.Sum(x => x.VirtualWinLoss),
                RealIncome = matches.Sum(x => x.RealIncome),
                VirtualTurnover = matches.Sum(x => x.VirtualTurnover),
                RealTurnover = matches.Sum(x => x.RealTurnover)
            };
        }

        private IEnumerable<MatchWinLossItem> BindingMatches(IEnumerable<MatchEntity> matches)
        {
            return matches.Select(x => BindingMatch(x));
        }

        private MatchWinLossItem BindingMatch(MatchEntity rawData)
        {
            if (rawData == null)
            {
                return new MatchWinLossItem();
            }

            return new MatchWinLossItem
            {
                MatchCode = rawData.MatchCode,
                MatchEvent = rawData.MatchEvent,
                MemberWinLoss = rawData.MemberWinLoss,
                SMAWinLoss = rawData.SMAWinLoss,
                ExtraPT = rawData.ExtraPT,
                Commission = rawData.Commission,
                VirtualWinLoss = rawData.VirtualWinLoss,
                RealIncome = rawData.RealIncome,
                VirtualTurnover = rawData.VirtualTurnover,
                RealTurnover = rawData.RealTurnover
            };
        }
    }
}