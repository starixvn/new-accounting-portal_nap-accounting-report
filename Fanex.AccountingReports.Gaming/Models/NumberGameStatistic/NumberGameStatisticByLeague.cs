namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class NumberGameStatisticByLeague
    {
        public NumberGameStatisticByLeague()
        {
            ReportRows = new List<NumberGameStatisticByLeagueDetail>();
        }

        public List<NumberGameStatisticByLeagueDetail> ReportRows { get; set; }

        public NumberGameStatisticByLeagueDetail GrandTotal { get; set; }
    }
}