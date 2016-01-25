namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class NumberGameStatisticByUserName
    {
        public NumberGameStatisticByUserName()
        {
            ReportRows = new List<NumberGameStatisticByUserNameDetail>();
        }

        public List<NumberGameStatisticByUserNameDetail> ReportRows { get; set; }

        public NumberGameStatisticByUserNameDetail GrandTotal { get; set; }

        public int TotalItems { get; set; }
    }
}