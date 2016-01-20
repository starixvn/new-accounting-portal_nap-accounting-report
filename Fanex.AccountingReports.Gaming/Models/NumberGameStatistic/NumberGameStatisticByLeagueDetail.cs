namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;

    [Serializable]
    public class NumberGameStatisticByLeagueDetail
    {
        public NumberGameStatisticByLeagueDetail()
        {
            NumberGameStatisticDetail = new NumberGameStatisticDetail();
        }

        public string LeagueName { get; set; }

        public int Players { get; set; }

        public NumberGameStatisticDetail NumberGameStatisticDetail { get; set; }
    }
}