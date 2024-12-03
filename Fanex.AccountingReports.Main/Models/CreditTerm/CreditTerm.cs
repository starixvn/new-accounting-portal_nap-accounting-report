namespace Fanex.AccountingReports.Main.Models.CreditTerm
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CreditTerm
    {
        public CreditTerm()
        {
            RoleName = string.Empty;
            Username = string.Empty;
            CurrencyName = string.Empty;
            AccountRemark = string.Empty;
            PhoneNumber = string.Empty;
            CheckerName = string.Empty;
            AccountRemarkDate = string.Empty;
            Rows = new List<CreditTerm>();
        }

        public int GroupID { get; set; }

        public string RoleName { get; set; }

        public string Username { get; set; }

        public int CustomerID { get; set; }

        public string CurrencyName { get; set; }

        public double PreAgreeAmount { get; set; }

        public double Balance { get; set; }

        public double BalanceBase { get; set; }

        public double Reward { get; set; }

        public double RewardRM { get; set; }

        public double BalanceGroup { get; set; }

        public string AccountRemark { get; set; }

        public string PhoneNumber { get; set; }

        public string CheckerName { get; set; }

        public string AccountRemarkDate { get; set; }

        public bool OverLimit { get; set; }

        public List<CreditTerm> Rows { get; set; }
    }
}
