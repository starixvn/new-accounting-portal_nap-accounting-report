namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;
    
    [Serializable]
    public class NumberGameStatisticByBettypeDetail
    {
        public NumberGameStatisticByBettypeDetail()
        {
            NumberGameStatisticDetail = new NumberGameStatisticDetail();
        }

        public string Bettype { get; set; }

        public int Players { get; set; }

        public NumberGameStatisticDetail NumberGameStatisticDetail { get; set; }
    }
}