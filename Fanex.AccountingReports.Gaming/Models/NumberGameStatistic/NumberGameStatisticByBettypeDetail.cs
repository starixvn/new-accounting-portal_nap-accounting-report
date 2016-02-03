namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;
    
    [Serializable]
    public class NumberGameStatisticByBettypeDetail : NumberGameStatisticDetail
    {
        public string Bettype { get; set; }

        public int Players { get; set; }
    }
}