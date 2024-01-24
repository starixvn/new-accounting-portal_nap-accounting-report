namespace Fanex.AccountingReports.Special.Models.SaveBetReport
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class League
    {
        public League()
        {
            Leagues = new List<League>();
        }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public List<League> Leagues { get; set; }
    }
}
