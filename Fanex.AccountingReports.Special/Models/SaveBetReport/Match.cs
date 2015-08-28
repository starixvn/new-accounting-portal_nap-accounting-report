// -----------------------------------------------------------------------
// <copyright file="Match.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.28">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.SaveBetReport
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Match model.
    /// </summary>
    [Serializable]
    public class Match
    {
        public Match()
        {
            Matchs = new List<Match>();
        }

        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }
        
        public string EventDate { get; set; }

        public int SportType { get; set; }

        public List<Match> Matchs { get; set; }
    }
}