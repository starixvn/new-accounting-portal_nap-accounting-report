namespace Fanex.AccountingReports.Special.Models.SaveBetReport
{
    using System;
    using System.Collections.Generic;

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
