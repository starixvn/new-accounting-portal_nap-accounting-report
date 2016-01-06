namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    public class NumberGameStatisticByDailyDetail
    {
        public NumberGameStatisticByDailyDetail()
        {
            NumberGameStatisticDetail = new NumberGameStatisticDetail();
        }

        public string Date { get; set; }

        public int Players { get; set; }

        public NumberGameStatisticDetail NumberGameStatisticDetail { get; set; }
    }
}