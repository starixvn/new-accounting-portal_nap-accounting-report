namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;

    [Serializable]
    public class NumberGameStatisticByUserNameDetail : NumberGameStatisticDetail
    {
        public string UserName { get; set; }

        public int CustId { get; set; }
    }
}