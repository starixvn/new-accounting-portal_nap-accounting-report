// --------------------------------------------------------------------------
// <copyright file="TennisMatchResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2014.10.09">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Tennis model.
    /// </summary>
    public class TennisMatchResult
    {
        public TennisMatchResult()
        {
            this.SetResults = new List<TennisSetResult>();
        }

        public string MatchId { get; set; }

        public int BettypeId { get; set; }

        public DateTime KickOffTime { get; set; }

        public int SettlementStatusId { get; set; }

        public bool IsPending { get; set; }

        public List<TennisSetResult> SetResults { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public string HomeTotalGames { get; set; }

        public string AwayTotalGames { get; set; }

        public string HomeTotalSets { get; set; }

        public string AwayTotalSets { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public string EventStatus { get; set; }
    }
}