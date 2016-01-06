namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System.Collections.Generic;

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