namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NegativeBalanceModel
    {
        public NegativeBalanceModel()
        {
            NegativeBalanceList = new List<NegativeBalance>();
        }

        public List<NegativeBalance> NegativeBalanceList { get; set; }

        public int TotalNegativeBalanceUser { get; set; }
    }
}
