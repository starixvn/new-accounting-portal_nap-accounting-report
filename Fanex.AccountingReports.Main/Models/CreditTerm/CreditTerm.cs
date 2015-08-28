// -----------------------------------------------------------------------
// <copyright file="CreditTerm.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Models.CreditTerm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The model of Credit Term : Pre Agreed report.
    /// </summary>
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

        public decimal PreAgreeAmount { get; set; }

        public decimal Balance { get; set; }

        public decimal BalanceBase { get; set; }

        public decimal BalanceGroup { get; set; }

        public string AccountRemark { get; set; }

        public string PhoneNumber { get; set; }

        public string CheckerName { get; set; }

        public string AccountRemarkDate { get; set; }

        public bool OverLimit { get; set; }

        public List<CreditTerm> Rows { get; set; }
    }
}
