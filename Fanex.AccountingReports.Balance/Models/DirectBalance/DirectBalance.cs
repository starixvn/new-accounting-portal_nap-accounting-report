namespace Fanex.AccountingReports.Balance.Models.DirectBalance
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class DirectBalance
    {
        public List<DirectBalanceDetail> DirectBalanceList { get; set; }

        public string BaseBalanceTotal { get; set; }
    }
}
