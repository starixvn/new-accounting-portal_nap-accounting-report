namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    public class NumberGameStatisticByUserNameDetail
    {
        public NumberGameStatisticByUserNameDetail()
        {
            NumberGameStatisticDetail = new NumberGameStatisticDetail();
        }

        public string UserName { get; set; }

        public int CustId { get; set; }

        public NumberGameStatisticDetail NumberGameStatisticDetail { get; set; }
    }
}