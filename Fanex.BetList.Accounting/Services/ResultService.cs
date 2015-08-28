// --------------------------------------------------------------------------
// <copyright file="ResultService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David>Tran" date="2014.10.24">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Models;
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Result service.
    /// </summary>
    public class ResultService : IResultService
    {
        /// <summary>
        /// Get tennis result.
        /// </summary>
        /// <param name="criteria">Criteria object.</param>
        /// <param name="matchId">Match tennis Id.</param>
        /// <param name="bettypeId">Bet type Id.</param>
        /// <returns>Match tennis object.</returns>
        public List<TennisLeagueResult> GetTennisResult(CriteriaBase criteria, string matchId, int bettypeId)
        {
            try
            {
                if (criteria.IsValid())
                {
                    var leagues = new List<TennisLeagueResult>();
                    
                    using (var objDb = new ObjectDb(criteria.GetSettingKey()))
                    {
                        using (var reader = objDb.ExecuteReader(criteria.GetSpParams()))
                        {
                            while (reader.Read())
                            {
                                var match = new TennisMatchResult();
                                if (!matchId.Equals("-1") || (matchId.Equals("-1") && reader.ToDecimal("sporttype").Equals(5)))
                                {
                                    this.FectchTennisMatchInfor(reader, match, matchId, bettypeId);
                                    this.InsertMatchToLeague(leagues, match);
                                }
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    this.InsertMatchToLeague(reader, leagues);
                                }
                            }
                        }
                    }

                    foreach (var league in leagues)
                    {
                        foreach (var match in league.Matchs)
                        {
                            this.SetMatchStatus(match);
                            this.InsertInexistenceSetToMatch(match);
                        }
                    }

                    return leagues;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TennisLeagueResult>();
        }

        /// <summary>
        /// Fetch data to match.
        /// </summary>
        /// <param name="reader">DataReader object.</param>
        /// <param name="match">MatchTennis id.</param>
        /// <param name="matchId">MatchTennis  object.</param>
        /// <param name="bettypeId">Bet type Id.</param>
        private void FectchTennisMatchInfor(IDataReader reader, TennisMatchResult match, string matchId, int bettypeId)
        {
            match.MatchId = matchId.Equals("-1") ? reader.ToString("matchid") : matchId;
            match.BettypeId = bettypeId.Equals(-1) ? reader.ToInt("bettype") : bettypeId;
            match.KickOffTime = reader.ToDateTime("kickofftime");
            match.HomeName = CacheRef.GetTeamName(reader.ToString("homeID"));
            match.AwayName = CacheRef.GetTeamName(reader.ToString("awayID"));
            match.LeagueId = reader.ToInt("LeagueID");
            match.LeagueName = CacheRef.GetLeagueName(match.LeagueId);
            match.EventStatus = reader.ToString("eventstatus");
            match.IsPending = reader.ToBool("IsPending");
            match.SettlementStatusId = 4;
        }

        /// <summary>
        /// Fetch data to match.
        /// </summary>
        /// <param name="reader">DataReader object.</param>
        /// <param name="leagues">Leagues object.</param>
        private void InsertMatchToLeague(IDataReader reader, List<TennisLeagueResult> leagues)
        {
            var tennisEntity = new TennisResultEntity
            {
                MatchId = reader.ToString("MatchId"),
                ResourceId = reader.ToString("ResourceId"),
                HomeValue = reader.ToString("HomeValue"),
                AwayValue = reader.ToString("AwayValue"),
                ResultStatusId = reader.ToInt("ResultStatusID"),
                SettlementStatusId = reader.ToInt("SettlementStatusID")
            };

            leagues.Select(league =>
            {
                foreach (var match in league.Matchs)
                {
                    if (match.MatchId.Equals(tennisEntity.MatchId) || match.MatchId.Equals("-1"))
                    {
                        if (tennisEntity.ResourceId.Length == 2)
                        {
                            this.InsertSetToMatch(reader, match, tennisEntity);
                        }
                        else
                        {
                            this.InsertGameToSet(match, tennisEntity);
                        }
                    }
                }

                return league;
            }).ToList();
        }

        /// <summary>
        /// Insert match to league.
        /// </summary>
        /// <param name="leagues">League object.</param>
        /// <param name="match">Match object.</param>
        private void InsertMatchToLeague(List<TennisLeagueResult> leagues, TennisMatchResult match)
        {
            var isExistedLeagueId = leagues.Find(item => item.LeagueId.Equals(match.LeagueId)) != null;

            if (!isExistedLeagueId)
            {
                var league = new TennisLeagueResult
                {
                    LeagueId = match.LeagueId,
                    LeagueName = match.LeagueName
                };
                league.Matchs.Add(match);

                leagues.Add(league);
            }
            else
            {
                leagues = leagues.Select(item =>
                {
                    if (item.LeagueId.Equals(match.LeagueId))
                    {
                        item.Matchs.Add(match);
                        return item;
                    }
                    else
                    {
                        return item;
                    }
                }).ToList();
            }
        }

        /// <summary>
        /// Insert set to match.
        /// </summary>
        /// <param name="reader">DataReader object.</param>
        /// <param name="match">MatchTennis object.</param>
        /// <param name="tennisEntity">Tennis entity.</param>
        private void InsertSetToMatch(IDataReader reader, TennisMatchResult match, TennisResultEntity tennisEntity)
        {
            if (tennisEntity.ResourceId.Equals("00"))
            {
                match.HomeTotalSets = reader.ToString("HomeValue");
                match.AwayTotalSets = reader.ToString("AwayValue");
                match.SettlementStatusId = reader.ToInt("SettlementStatusID");
            }
            else
            {
                this.SetValueIfEmpty(tennisEntity);
                match.SetResults.Add(new TennisSetResult
                {
                    SetDetail = tennisEntity
                });
            }
        }

        /// <summary>
        /// Insert set inexistence to match.
        /// </summary>
        /// <param name="match">MatchTennis  object.</param>
        private void InsertInexistenceSetToMatch(TennisMatchResult match)
        {
            for (int i = 0; i < 5; i++)
                {
                    var isExistedSet = match.SetResults.Find(item => Converter.ToInt(item.SetDetail.ResourceId).Equals(i + 1)) != null;
                    if (!isExistedSet)
                    {
                        match.SetResults.Insert(
                            i, 
                            new TennisSetResult 
                            { 
                                SetDetail = new TennisResultEntity
                                {
                                    ResourceId = "0" + i.ToString(),
                                    HomeValue = "-",
                                    AwayValue = "-",
                                    SettlementStatusId = 1
                                }
                        });
                    }
                    else
                    {
                        var set = match.SetResults[i];
                        this.InsertInexistenceGameToSet(set);
                        if (set.SetDetail.SettlementStatusId.Equals(4))
                        {
                            match.HomeTotalGames = (Converter.ToDecimal(match.HomeTotalGames) + Converter.ToDecimal(set.SetDetail.HomeValue)).ToString();
                            match.AwayTotalGames = (Converter.ToDecimal(match.AwayTotalGames) + Converter.ToDecimal(set.SetDetail.AwayValue)).ToString();
                        }
                    }
            }
        }

        /// <summary>
        /// Insert game inexistence to set.
        /// </summary>
        /// <param name="set">SetTennis  object.</param>
        private void InsertInexistenceGameToSet(TennisSetResult set)
        {
            var games = set.MaxGame;
            for (int j = 0; j < games; j++)
            {
                var resourceId = set.SetDetail.ResourceId + (j + 1).ToString("00");
                var isExistedGame = set.GameResults.Find(item => item.TennisGame.ResourceId.Equals(resourceId)) != null;
                if (!isExistedGame)
                {
                    set.GameResults.Insert(
                        j, 
                        new TennisGameResult 
                        {
                            TennisGame = new TennisResultEntity
                            {
                                ResourceId = resourceId,
                                HomeValue = "-",
                                AwayValue = "-"
                            }
                    });
                }
            }
        }

        /// <summary>
        /// Insert game to set.
        /// </summary>
        /// <param name="match">MatchTennis object.</param>
        /// <param name="tennisEntity">Tennis entity.</param>
        private void InsertGameToSet(TennisMatchResult match, TennisResultEntity tennisEntity)
        {
            var setId = tennisEntity.ResourceId.Substring(0, 2);
            int gameId = Converter.ToInt(tennisEntity.ResourceId.Substring(2, 2));
            SetValueIfEmpty(tennisEntity);

            var isExistedSetId = match.SetResults.Find(item => item.SetDetail.ResourceId == setId) != null;
            var game = new TennisGameResult
                {
                    TennisGame = tennisEntity,
                    GameId = gameId
                };

            if (!isExistedSetId)
            {
                var tennisSet = new TennisSetResult
                {
                    MaxGame = gameId,
                    SetDetail = new TennisResultEntity
                    {
                        ResourceId = setId
                    }
                };

                tennisSet.GameResults.Add(game);

                match.SetResults.Add(tennisSet);
            }
            else
            {
                match.SetResults = match.SetResults.Select(item =>
                {
                    if (item.SetDetail.ResourceId.Equals(setId))
                    {
                        item.GameResults.Add(game);
                        if (item.MaxGame < gameId)
                        {
                            item.MaxGame = gameId;
                        }
                    }

                    return item;
                }).ToList();
            }
        }

        /// <summary>
        /// Set value of set if value is Empty.
        /// </summary>
        /// <param name="tennisEntity">TennisEntity object.</param>
        private void SetValueIfEmpty(TennisResultEntity tennisEntity)
        {
            var refundlist = new List<int> { 1, 2, 3 };
            if (refundlist.Contains(tennisEntity.SettlementStatusId))
            {
                tennisEntity.HomeValue = this.SetSettlementStatus(tennisEntity.SettlementStatusId);
                tennisEntity.AwayValue = this.SetSettlementStatus(tennisEntity.SettlementStatusId);
            }
        }

        /// <summary>
        /// Set match status beyond status.
        /// </summary>
        /// <param name="match">Match tennis object.</param>
        private void SetMatchStatus(TennisMatchResult match)
        {
            switch (match.EventStatus.ToLower())
            {
                case "closed":
                    match.EventStatus = "Pending";
                    break;
                case "running":
                    match.EventStatus = match.IsPending ? "Pending" : "Running";
                    break;
                case "refund":
                    match.EventStatus = "Refund";
                    break;
                case "refundft":
                    match.EventStatus = "RefundFT";
                    break;
                case "refunht":
                    match.EventStatus = "RefundHT";
                    break;
                case "completed":
                    match.EventStatus = this.SetSettlementStatus(match.SettlementStatusId);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set settlement status.
        /// </summary>
        /// <param name="settlementStatusId">Status Id.</param>
        /// <returns>Settlement status.</returns>
        private string SetSettlementStatus(int settlementStatusId)
        {
            string status = string.Empty;
            switch (settlementStatusId)
            {
                case 0:
                    status = "-";
                    break;
                case 1:
                    status = "Refund";
                    break;
                case 2:
                    status = "RefundHT";
                    break;
                case 3:
                    status = "RefundFT";
                    break;
                default:
                    status = "Completed";
                    break;
            }

            return status;
        }
    }
}