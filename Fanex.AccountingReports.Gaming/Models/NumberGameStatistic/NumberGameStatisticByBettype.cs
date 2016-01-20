namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class NumberGameStatisticByBettype
    {
        public NumberGameStatisticByBettype()
        {
            ReportRows = new List<NumberGameStatisticByBettypeDetail>();
        }

        public List<NumberGameStatisticByBettypeDetail> ReportRows { get; set; }

        public NumberGameStatisticByBettypeDetail GrandTotal { get; set; }
    }
}