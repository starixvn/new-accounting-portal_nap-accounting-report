namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class NegativeBalance
    {
        public NegativeBalance()
        {
            NegativeBalanceUserList = new List<NegativeBalanceUser>();
        }

        public string LicenseeSite { get; set; }

        public int TotalNegativeBalanceList { get; set; }

        public List<NegativeBalanceUser> NegativeBalanceUserList { get; set; }
    }
}
