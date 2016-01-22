namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;

    [Serializable]
    public class NumberGameStatisticByDailyDetail : NumberGameStatisticDetail
    {
        public string Date { get; set; }

        public int Players { get; set; }
    }
}