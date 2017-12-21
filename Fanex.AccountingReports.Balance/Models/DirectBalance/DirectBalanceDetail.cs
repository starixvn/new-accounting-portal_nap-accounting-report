namespace Fanex.AccountingReports.Balance.Models.DirectBalance
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class DirectBalanceDetail
    {
        public string CustId { get; set; }

        public string Username { get; set; }

        public string Currency { get; set; }

        public string Balance { get; set; }

        public string BaseBalance { get; set; }
    }
}