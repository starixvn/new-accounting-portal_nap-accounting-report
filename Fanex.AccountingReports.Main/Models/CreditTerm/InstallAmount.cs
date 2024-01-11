namespace Fanex.AccountingReports.Main.Models.CreditTerm
{
    using System;

    [Serializable]
    public class InstallAmount
    {
        public string Username { get; set; }

        public string CurrencyName { get; set; }

        public double Balance { get; set; }

        public double BalanceBase { get; set; }

        public double InstallmentAmount { get; set; }

        public int NumberDayInstallment { get; set; }
    }
}
