namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Tennis model.
    /// </summary>
    public class TennisLeagueResult
    {
        public TennisLeagueResult()
        {
            this.Matchs = new List<TennisMatchResult>();
        }

        public List<TennisMatchResult> Matchs { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }
    }
}
