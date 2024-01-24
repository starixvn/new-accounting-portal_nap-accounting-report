namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;

    [Serializable]
    public class NegativeBalanceUser
    {
        public string UserName { get; set; }

        public double Balance { get; set; }

        public string Currency { get; set; }

        public string Site { get; set; }
    }
}
