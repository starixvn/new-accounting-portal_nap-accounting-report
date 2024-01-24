namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;
    using System.Collections.Generic;

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
