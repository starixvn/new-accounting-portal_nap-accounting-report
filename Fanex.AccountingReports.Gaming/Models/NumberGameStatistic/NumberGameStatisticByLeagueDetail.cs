namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;

    [Serializable]
    public class NumberGameStatisticByLeagueDetail : NumberGameStatisticDetail
    {
        public string LeagueName { get; set; }

        public int Players { get; set; }
    }
}