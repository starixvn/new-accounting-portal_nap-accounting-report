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
            MailList = new List<NegativeBalanceMemberMailFormat>();
        }

        public List<NegativeBalance> NegativeBalanceList { get; set; }

        public List<NegativeBalanceMemberMailFormat> MailList { get; set; }

        public int TotalNegativeBalanceUser { get; set; }
    }
}
