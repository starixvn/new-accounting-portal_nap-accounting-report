namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class NumberGameStatisticByDaily
    {
        public NumberGameStatisticByDaily()
        {
            ReportRows = new List<NumberGameStatisticByDailyDetail>();
        }

        public List<NumberGameStatisticByDailyDetail> ReportRows { get; set; }

        public NumberGameStatisticByDailyDetail GrandTotal { get; set; }
    }
}