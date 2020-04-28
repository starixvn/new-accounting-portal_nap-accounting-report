namespace Fanex.AccountingReports.WinLoss.Services.Implement
{
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Constants;
    using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Repositories.Implement;
    using Fanex.AccountingReports.WinLoss.Resources;
    using Fanex.Common;
    using System;
    using System.Collections;
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
            this._8rmConfig = RMSection.Instance.Report[ReportName.LeagueWinLoss.ToString()]
                .ReportConfigs[RMSection.RMType.EightRM.ToString()];
            this._barmConfig = RMSection.Instance.Report[ReportName.LeagueWinLoss.ToString()]
                .ReportConfigs[RMSection.RMType.BARM.ToString()];
        }

        /// <summary>
        /// Gets the league win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The league win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LeagueWinLossInfo GetLeagueWinLoss(GetLeagueWinLossCriteria criteria)
        {
            var outputParam = new Hashtable();
            var rawData = this._reportRepository.GetLeagueWinLoss(criteria, ref outputParam);
            var leagueWinLoss = new LeagueWinLossInfo
            {
                ChildsInfo = new List<LeagueWinLossInfo>(),
                EightRM = new LeagueWinLossInfo
                {
                    Name = Labels.EightRM + " " + Labels.Total
                },
                BARM = new LeagueWinLossInfo
                {
                    Name = Labels.BARM + " " + Labels.Total
                },
                Total = new LeagueWinLossInfo
                {
                    Name = Labels.Total
                },
                GrandTotal = new LeagueWinLossInfo
                {
                    Name = Labels.Grand_Total
                }
            };

            var sportGroups = rawData.OrderBy(rawWinLossInfo => rawWinLossInfo.sporttype)
                                     .GroupBy(rawWinLossInfo => rawWinLossInfo.sporttype);
            foreach (var sportGroup in sportGroups)
            {
                const int NewCricketSportstype = 50;
                var sportWinLoss = Converter.ToInt(sportGroup.Key) == NewCricketSportstype
                    ? CalculateNewCricket(sportGroup)
                    : CalculateNormalSports(sportGroup);
                sportWinLoss.Id = Converter.ToInt(sportGroup.Key);
                leagueWinLoss.AddToTotal(sportWinLoss.GrandTotal);
                leagueWinLoss.ChildsInfo.Add(sportWinLoss);
            }

            leagueWinLoss.EightRM.LineTotal = Converter.ToDecimal(outputParam["winlost_8rm"]);
            leagueWinLoss.BARM.LineTotal = Converter.ToDecimal(outputParam["winlost_barm"]);
            leagueWinLoss.AddToGrandTotal(leagueWinLoss.Total);
            if (this._8rmConfig.IsActive)
            {
                leagueWinLoss.AddToGrandTotal(leagueWinLoss.EightRM);
                leagueWinLoss.EightRM = this.RoundLeagueWinLossInfo(leagueWinLoss.EightRM);
            }

            if (this._barmConfig.IsActive)
            {
                leagueWinLoss.AddToGrandTotal(leagueWinLoss.BARM);
                leagueWinLoss.BARM = this.RoundLeagueWinLossInfo(leagueWinLoss.BARM);
            }

            leagueWinLoss.Total.Margin = this.GetWinLossMargin(
                leagueWinLoss.Total.TurnOver,
                leagueWinLoss.Total.LineTotal,
                4,
                false);
            leagueWinLoss.Total = this.RoundLeagueWinLossInfo(leagueWinLoss.Total);

            leagueWinLoss.GrandTotal.Margin = this.GetWinLossMargin(
                leagueWinLoss.GrandTotal.TurnOver,
                leagueWinLoss.GrandTotal.LineTotal,
                4,
                false);
            leagueWinLoss.GrandTotal = this.RoundLeagueWinLossInfo(leagueWinLoss.GrandTotal);

            SessionManager.Set<LeagueWinLossInfo>(WinLossContants.LeagueWinLossSessionKey, leagueWinLoss);
            return leagueWinLoss;

            SessionManager.Set<LeagueWinLossInfo>(WinLossContants.LeagueWinLossSessionKey, null);
            return null;
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

        private LeagueWinLossInfo CalculateNormalSports(IGrouping<dynamic, dynamic> sportGroup)
        {
            var sportWinLoss = new LeagueWinLossInfo
            {
                Id = Converter.ToInt(sportGroup.Key),
                ChildsInfo = new List<LeagueWinLossInfo>(),
                GrandTotal = new LeagueWinLossInfo()
            };

            sportWinLoss.Name = this.GetSportName(sportWinLoss.Id);
            sportWinLoss.GrandTotal.Name = sportWinLoss.Name + " " + Labels.Total;
            var leagueList = sportGroup.Select(rawWinLossInfo => Converter.ToInt(rawWinLossInfo.leagueid))
                                       .ToList();
            CacheRef.SaveCacheRefResponse(leagueList, CacheRef.GetLeagueName, ref this._leagueNameCache);
            var orderedSportGroup = sportGroup.OrderBy(rawWinLossInfo => this._leagueNameCache[rawWinLossInfo.leagueid]);

            foreach (var info in orderedSportGroup)
            {
                var winLoss = new LeagueWinLossInfo
                {
                    Id = Converter.ToInt(info.leagueid),
                    WinLoss = Converter.ToDecimal(info.WinLost),
                    Position = Converter.ToDecimal(info.Position),
                    Commission = Converter.ToDecimal(info.Commission),
                    LineTotal = Converter.ToDecimal(info.LineTotal),
                    TurnOver = Converter.ToDecimal(info.Turnover)
                };

                winLoss.Name = this._leagueNameCache[winLoss.Id];
                winLoss.Margin = this.GetWinLossMargin(
                    winLoss.TurnOver,
                    winLoss.LineTotal,
                    4,
                    true);

                sportWinLoss.AddToGrandTotal(winLoss);
                winLoss = this.RoundLeagueWinLossInfo(winLoss);
                sportWinLoss.ChildsInfo.Add(winLoss);
            }

            sportWinLoss.ChildsInfo = sportWinLoss.ChildsInfo.OrderBy(winLossInfo => winLossInfo.Name).ToList();
            sportWinLoss.GrandTotal.Margin = this.GetWinLossMargin(
                sportWinLoss.GrandTotal.TurnOver,
                sportWinLoss.GrandTotal.LineTotal,
                4,
                true);

            sportWinLoss.GrandTotal = this.RoundLeagueWinLossInfo(sportWinLoss.GrandTotal);

            return sportWinLoss;
        }

        private LeagueWinLossInfo CalculateNewCricket(IGrouping<dynamic, dynamic> sportGroup)
        {
            var sportWinLoss = CalculateNewCricketLeagueWinLoss(sportGroup);
            var matchWinnerTotal = CalculateNewCricketNonChargeCommissionTotal(sportGroup);
            var chargedCommisionTotal = CalculateNewCricketChargedCommissionTotal(sportGroup);
            var grandTotal = CalculateNewCricketGrandTotal(sportGroup);
            sportWinLoss.GrandTotal = new LeagueWinLossInfo
            {
                ChildsInfo = new List<LeagueWinLossInfo> { matchWinnerTotal, chargedCommisionTotal, grandTotal }
            };
            sportWinLoss.AddToGrandTotal(grandTotal);

            return sportWinLoss;
        }

        private LeagueWinLossInfo CalculateNewCricketLeagueWinLoss(IGrouping<dynamic, dynamic> sportGroup)
        {
            var leagueWinLosses = sportGroup.Where(r => r.bettype != CHARGECOMMISSION).ToList();
            var sportWinLoss = new LeagueWinLossInfo
            {
                Id = Converter.ToInt(sportGroup.Key),
                ChildsInfo = new List<LeagueWinLossInfo>(),
            };

            sportWinLoss.Name = this.GetSportName(sportWinLoss.Id);

            var leagueList = leagueWinLosses
                                       .Select(rawWinLossInfo => Converter.ToInt(rawWinLossInfo.leagueid))
                                       .ToList();
            CacheRef.SaveCacheRefResponse(leagueList, CacheRef.GetLeagueName, ref this._leagueNameCache);
            var orderedSportGroup = leagueWinLosses
                                    .OrderBy(rawWinLossInfo => this._leagueNameCache[rawWinLossInfo.leagueid])
                                    .ToList();
            foreach (var info in orderedSportGroup)
            {
                var winLoss = new LeagueWinLossInfo { Id = Converter.ToInt(info.leagueid) };
                winLoss.WinLoss = Converter.ToDecimal(info.WinLost);
                winLoss.Position = Converter.ToDecimal(info.Position);
                winLoss.Commission = Converter.ToDecimal(info.Commission);
                winLoss.LineTotal = Converter.ToDecimal(info.LineTotal);
                winLoss.TurnOver = Converter.ToDecimal(info.Turnover);

                winLoss.Name = this._leagueNameCache[winLoss.Id];
                winLoss.Margin = this.GetWinLossMargin(
                    winLoss.TurnOver,
                    winLoss.LineTotal,
                    4,
                    true);

                winLoss = RoundLeagueWinLossInfo(winLoss);
                sportWinLoss.ChildsInfo.Add(winLoss);
            }

            sportWinLoss.ChildsInfo = sportWinLoss.ChildsInfo.OrderBy(winLossInfo => winLossInfo.Name).ToList();

            return sportWinLoss;
        }

        private LeagueWinLossInfo CalculateNewCricketNonChargeCommissionTotal(IGrouping<dynamic, dynamic> sportGroup)
        {
            var matchWinner = sportGroup.Where(r => r.bettype != CHARGECOMMISSION).ToList();
            var matchWinnerTotal = new LeagueWinLossInfo { Name = "Total" };
            foreach (var info in matchWinner)
            {
                matchWinnerTotal.WinLoss += Converter.ToDecimal(info.WinLost);
                matchWinnerTotal.Position += Converter.ToDecimal(info.Position);
                matchWinnerTotal.Commission += Converter.ToDecimal(info.Commission);
                matchWinnerTotal.LineTotal += Converter.ToDecimal(info.LineTotal);
                matchWinnerTotal.TurnOver += Converter.ToDecimal(info.Turnover);
            }

            matchWinnerTotal.Margin = this.GetWinLossMargin(
                   matchWinnerTotal.TurnOver,
                   matchWinnerTotal.LineTotal,
                   4,
                   true);
            return RoundLeagueWinLossInfo(matchWinnerTotal);
        }

        private LeagueWinLossInfo CalculateNewCricketChargedCommissionTotal(IGrouping<dynamic, dynamic> sportGroup)
        {
            var chargedCommision = sportGroup.Where(r => r.bettype == CHARGECOMMISSION).ToList();
            var chargedCommisionTotal = new LeagueWinLossInfo
            {
                Name = "Charged Commission"
            };
            foreach (var info in chargedCommision)
            {
                chargedCommisionTotal.WinLoss += Converter.ToDecimal(info.WinLost);
                chargedCommisionTotal.Position += Converter.ToDecimal(info.Position);
                chargedCommisionTotal.Commission += Converter.ToDecimal(info.Commission);
                chargedCommisionTotal.LineTotal += Converter.ToDecimal(info.LineTotal);
                chargedCommisionTotal.TurnOver += Converter.ToDecimal(info.Turnover);
            }

            chargedCommisionTotal.Margin = this.GetWinLossMargin(
                   chargedCommisionTotal.TurnOver,
                   chargedCommisionTotal.LineTotal,
                   4,
                   true);
            return RoundLeagueWinLossInfo(chargedCommisionTotal);
        }

        private LeagueWinLossInfo CalculateNewCricketGrandTotal(IGrouping<dynamic, dynamic> sportGroup)
        {
            var grandTotal = new LeagueWinLossInfo
            {
                Name = this.GetSportName(Convert.ToInt32(sportGroup.Key)) + " " + Labels.Total
            };
            foreach (var info in sportGroup)
            {
                grandTotal.WinLoss += Converter.ToDecimal(info.WinLost);
                grandTotal.Position += Converter.ToDecimal(info.Position);
                grandTotal.Commission += Converter.ToDecimal(info.Commission);
                grandTotal.LineTotal += Converter.ToDecimal(info.LineTotal);
                grandTotal.TurnOver += Converter.ToDecimal(info.Turnover);
            }

            grandTotal.Margin = this.GetWinLossMargin(
                   grandTotal.TurnOver,
                   grandTotal.LineTotal,
                   4,
                   true);
            return this.RoundLeagueWinLossInfo(grandTotal);
        }

        /// <summary>
        /// Gets the league win loss by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The league win loss by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LeagueWinLossInfo GetLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria)
        {
            List<dynamic> eightRMTable;
            List<dynamic> barmTable;
            var rawData = this._reportRepository.GetLeagueWinLossBySport(
                criteria,
                out eightRMTable,
                out barmTable);

            var liveWinLoss = rawData.Where(data => Converter.ToBool(data.liveindicator));
            var nonLiveWinLoss = rawData.Where(data => !Converter.ToBool(data.liveindicator));
            var liveReport = this.GetLeagueWinLossBySport(liveWinLoss);
            liveReport.Name = Labels.Live.ToUpper();
            liveReport.Id = LiveTypeConstants.Live;

            var nonLiveReport = this.GetLeagueWinLossBySport(nonLiveWinLoss);
            nonLiveReport.Name = Labels.None_live.ToUpper();
            nonLiveReport.Id = LiveTypeConstants.NonLive;
            if (eightRMTable != null)
            {
                eightRMTable.ForEach(data =>
                {
                    if (Converter.ToBool(data.liveindicator))
                    {
                        liveReport.EightRM.LineTotal += Converter.ToDecimal(data.Winlost8RM);
                    }
                    else
                    {
                        nonLiveReport.EightRM.LineTotal += Converter.ToDecimal(data.Winlost8RM);
                    }
                });
            }

            if (barmTable != null)
            {
                barmTable.ForEach(data =>
                {
                    if (Converter.ToBool(data.liveindicator))
                    {
                        liveReport.BARM.LineTotal += Converter.ToDecimal(data.WinlostBARM);
                    }
                    else
                    {
                        nonLiveReport.BARM.LineTotal += Converter.ToDecimal(data.WinlostBARM);
                    }
                });
            }

            liveReport.Total.Margin = this.GetWinLossMargin(
                liveReport.Total.TurnOver,
                liveReport.Total.LineTotal,
                4,
                true);
            liveReport.Total = this.RoundLeagueWinLossInfo(liveReport.Total);

            nonLiveReport.Total.Margin = this.GetWinLossMargin(
                nonLiveReport.Total.TurnOver,
                nonLiveReport.Total.LineTotal,
                4,
                true);
            nonLiveReport.Total = this.RoundLeagueWinLossInfo(nonLiveReport.Total);

            if (this._8rmConfig.IsActive)
            {
                liveReport.AddToGrandTotal(liveReport.EightRM);
                nonLiveReport.AddToGrandTotal(nonLiveReport.EightRM);
                liveReport.EightRM = this.RoundLeagueWinLossInfo(liveReport.EightRM);
                nonLiveReport.EightRM = this.RoundLeagueWinLossInfo(nonLiveReport.EightRM);
            }

            if (this._barmConfig.IsActive)
            {
                liveReport.AddToGrandTotal(liveReport.BARM);
                nonLiveReport.AddToGrandTotal(nonLiveReport.BARM);
                liveReport.BARM = this.RoundLeagueWinLossInfo(liveReport.BARM);
                nonLiveReport.BARM = this.RoundLeagueWinLossInfo(nonLiveReport.BARM);
            }

            liveReport.GrandTotal.Margin = this.GetWinLossMargin(
                liveReport.GrandTotal.TurnOver,
                liveReport.GrandTotal.LineTotal,
                4,
                true);
            liveReport.GrandTotal = this.RoundLeagueWinLossInfo(liveReport.GrandTotal);

            nonLiveReport.GrandTotal.Margin = this.GetWinLossMargin(
                nonLiveReport.GrandTotal.TurnOver,
                nonLiveReport.GrandTotal.LineTotal,
                4,
                true);
            nonLiveReport.GrandTotal = this.RoundLeagueWinLossInfo(nonLiveReport.GrandTotal);

            var leagueWinLoss = new LeagueWinLossInfo
            {
                ChildsInfo = new List<LeagueWinLossInfo>()
            };

            if (liveReport.ChildsInfo.Count > 0)
            {
                leagueWinLoss.ChildsInfo.Add(liveReport);
            }

            if (nonLiveReport.ChildsInfo.Count > 0)
            {
                leagueWinLoss.ChildsInfo.Add(nonLiveReport);
            }

            SessionManager.Set<LeagueWinLossInfo>(WinLossContants.LeagueWinLossBySportSessionKey, leagueWinLoss);
            return leagueWinLoss;

            SessionManager.Set<LeagueWinLossInfo>(WinLossContants.LeagueWinLossBySportSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the league win loss by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The league win loss by bet type.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LeagueWinLossInfo GetLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria)
        {
            var outputParam = new Hashtable();
            var rawData = this._reportRepository.GetLeagueWinLossByBetType(criteria, ref outputParam);
            var leagueWinLoss = new LeagueWinLossInfo
            {
                ChildsInfo = new List<LeagueWinLossInfo>(),
                EightRM = new LeagueWinLossInfo
                {
                    Name = Labels.EightRM + " " + Labels.Total,
                    LineTotal = Converter.ToDecimal(outputParam["winlost_8rm"])
                },
                BARM = new LeagueWinLossInfo
                {
                    Name = Labels.BARM + " " + Labels.Total,
                    LineTotal = Converter.ToDecimal(outputParam["winlost_barm"])
                },
                Total = new LeagueWinLossInfo
                {
                    Name = Labels.Total
                },
                GrandTotal = new LeagueWinLossInfo
                {
                    Name = Labels.Grand_Total
                }
            };

            List<dynamic> leagueList = new List<dynamic>();
            List<dynamic> teamList = new List<dynamic>();
            rawData.ForEach(data =>
            {
                leagueList.Add(Converter.ToInt(data.leagueid));
                teamList.Add(Converter.ToInt(data.HomeId));
                teamList.Add(Converter.ToInt(data.AwayId));
            });

            CacheRef.SaveCacheRefResponse(leagueList, CacheRef.GetLeagueName, ref this._leagueNameCache);
            CacheRef.SaveCacheRefResponse(teamList, CacheRef.GetTeamName, ref this._teamNameCache);
            rawData.ForEach(data =>
            {
                var info = new LeagueWinLossInfo
                {
                    MatchCode = Converter.ToString2(data.matchcode),
                    HomeTeam = new EntityBase<int, string>
                    {
                        Id = Converter.ToInt(data.HomeId)
                    },
                    AwayTeam = new EntityBase<int, string>
                    {
                        Id = Converter.ToInt(data.AwayId)
                    },
                    IsLive = Converter.ToBool(data.liveindicator),
                    BetType = new EntityBase<int, string>
                    {
                        Id = Converter.ToInt(data.BetType)
                    },
                    League = new EntityBase<int, string>
                    {
                        Id = Converter.ToInt(data.leagueid)
                    },
                    WinLoss = Converter.ToDecimal(data.WinLost),
                    Position = Converter.ToDecimal(data.Position),
                    Commission = Converter.ToDecimal(data.Commission),
                    LineTotal = Converter.ToDecimal(data.LineTotal),
                    TurnOver = Converter.ToDecimal(data.Turnover),
                    IsNeutral = Converter.ToBool(data.IsNeutral)
                };

                info.HomeTeam.Name = this._teamNameCache[info.HomeTeam.Id];
                info.AwayTeam.Name = this._teamNameCache[info.AwayTeam.Id];
                info.League.Name = this._leagueNameCache[info.League.Id];
                info.Margin = GetWinLossMargin(
                    info.TurnOver,
                    info.LineTotal,
                    4,
                    true);
                leagueWinLoss.ChildsInfo.Add(info);
                leagueWinLoss.AddToTotal(info);
                leagueWinLoss.AddToGrandTotal(info);
                info = this.RoundLeagueWinLossInfo(info);
            });

            if (this._8rmConfig.IsActive)
            {
                leagueWinLoss.AddToGrandTotal(leagueWinLoss.EightRM);
                leagueWinLoss.EightRM = this.RoundLeagueWinLossInfo(leagueWinLoss.EightRM);
            }

            if (this._barmConfig.IsActive)
            {
                leagueWinLoss.AddToGrandTotal(leagueWinLoss.BARM);
                leagueWinLoss.BARM = this.RoundLeagueWinLossInfo(leagueWinLoss.BARM);
            }

            leagueWinLoss.Total.Margin = this.GetWinLossMargin(
                leagueWinLoss.Total.TurnOver,
                leagueWinLoss.Total.LineTotal,
                4,
                true);
            leagueWinLoss.Total = this.RoundLeagueWinLossInfo(leagueWinLoss.Total);

            leagueWinLoss.GrandTotal.Margin = this.GetWinLossMargin(
                leagueWinLoss.GrandTotal.TurnOver,
                leagueWinLoss.GrandTotal.LineTotal,
                4,
                true);
            leagueWinLoss.GrandTotal = this.RoundLeagueWinLossInfo(leagueWinLoss.GrandTotal);

            SessionManager.Set<LeagueWinLossInfo>(WinLossContants.LeagueWinLossByBetTypeSessionKey, leagueWinLoss);
            return leagueWinLoss;

            SessionManager.Set<LeagueWinLossInfo>(WinLossContants.LeagueWinLossByBetTypeSessionKey, null);
            return null;
        }

        private string GetSportName(int sportId)
        {
            if (sportId == SportContants.MixParlay)
            {
                return Labels.Mix_Parlay;
            }

            return CacheRef.GetSportName(sportId);
        }

        /// <summary>
        /// Gets the win loss margin.
        /// </summary>
        /// <param name="turnover">The turn over.</param>
        /// <param name="lineTotal">The line total.</param>
        /// <param name="decsFormat">The DECS format.</param>
        /// <param name="isUseNA">If set to <c>true</c> [is use na].</param>
        /// <returns>The win loss margin.</returns>
        private string GetWinLossMargin(decimal turnover, decimal lineTotal, byte decsFormat, bool isUseNA)
        {
            if (turnover != 0)
            {
                return Math.Round(lineTotal / turnover, decsFormat).ToString();
            }

            if (isUseNA)
            {
                return Labels.NotAvailable;
            }

            return 0.Format(decsFormat);
        }

        /// <summary>
        /// Rounds the league win loss information.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The league win loss information.</returns>
        private LeagueWinLossInfo RoundLeagueWinLossInfo(LeagueWinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.WinLoss = Math.Round(winLoss.WinLoss, 2);
            winLoss.Position = Math.Round(winLoss.Position, 2);
            winLoss.Commission = Math.Round(winLoss.Commission, 2);
            winLoss.LineTotal = Math.Round(winLoss.LineTotal, 2);
            winLoss.TurnOver = Math.Round(winLoss.TurnOver, 2);
            return winLoss;
        }

        /// <summary>
        /// Gets the league win loss by sport.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>The league win loss by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private LeagueWinLossInfo GetLeagueWinLossBySport(IEnumerable<dynamic> rawData)
        {
            var report = new LeagueWinLossInfo
            {
                ChildsInfo = new List<LeagueWinLossInfo>(),
                EightRM = new LeagueWinLossInfo
                {
                    Name = Labels.EightRM + " " + Labels.Total
                },
                BARM = new LeagueWinLossInfo
                {
                    Name = Labels.BARM + " " + Labels.Total
                },
                Total = new LeagueWinLossInfo
                {
                    Name = Labels.Total
                },
                GrandTotal = new LeagueWinLossInfo
                {
                    Name = Labels.Grand_Total
                }
            };

            var betTypeList = rawData.Select(rawWinLossInfo => Converter.ToInt(rawWinLossInfo.Bettype))
                                                     .ToList();
            CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
            var chargedCommission = rawData.FirstOrDefault();
            var ischargedCommission = false;
            foreach (var row in rawData)
            {
                var info = new LeagueWinLossInfo
                {
                    Id = Converter.ToInt(row.Bettype),
                    Hedge = Converter.ToString2(row.isHedge),
                    WinLoss = Converter.ToDecimal(row.WinLost),
                    Position = Converter.ToDecimal(row.Position),
                    Commission = Converter.ToDecimal(row.Commission),
                    LineTotal = Converter.ToDecimal(row.LineTotal),
                    TurnOver = Converter.ToDecimal(row.Turnover),
                    IsLive = Converter.ToBool(row.liveindicator)
                };
                if (info.Id == 500)
                {
                    chargedCommission = info;
                    ischargedCommission = true;
                    continue;
                }

                info.Name = this._betTypeNameCache[info.Id];
                info.Margin = this.GetWinLossMargin(info.TurnOver, info.LineTotal, 4, true);

                report.AddToTotal(info);
                info = this.RoundLeagueWinLossInfo(info);
                report.ChildsInfo.Add(info);
            }

            if (ischargedCommission == true)
            {
                chargedCommission.Name = "Charged Commission";
                chargedCommission.Margin = this.GetWinLossMargin(chargedCommission.TurnOver, chargedCommission.LineTotal, 4, true);
                report.ChildsInfo.Add(chargedCommission);
                report.AddToGrandTotal(chargedCommission);
            }

            report.Total.Margin = this.GetWinLossMargin(report.Total.TurnOver, report.Total.LineTotal, 4, false);
            report.AddToGrandTotal(report.Total);
            report.Total = this.RoundLeagueWinLossInfo(report.Total);

            return report;
        }

        private bool IsShowReport(LeagueWinLossInfo report)
        {
            return report != null &&
                   report.ChildsInfo.Count > 0 &&
                  (report.EightRM.LineTotal != 0 ||
                   report.BARM.LineTotal != 0 ||
                   report.GrandTotal.LineTotal != 0);
        }

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        public static class BetTypeContants
        {
            public const int MixParlay2 = 10;
        }
    }
}