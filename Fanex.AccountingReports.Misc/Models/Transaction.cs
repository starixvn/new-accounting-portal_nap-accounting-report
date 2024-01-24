namespace Fanex.AccountingReports.Misc.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Transaction
    {
        public string SiteName { get; set; }

        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public decimal MemberWinLoss { get; set; }

        public decimal MarginPercent { get; set; }

        public decimal AgentWinLoss { get; set; }

        public decimal MasterWinLoss { get; set; }

        public decimal SuperWinLoss { get; set; }

        public decimal CompanyWinLoss { get; set; }

        public decimal TotalWinLoss { get; set; }

        public decimal Commission { get; set; }

        public decimal ExtraPT { get; set; }

        public List<Transaction> TransactionList { get; set; }

        public Transaction Total { get; set; }

        public void SumDataToTotal(Transaction transaction)
        {
            if (transaction == null)
            {
                return;
            }

            this.Total.BetCount += transaction.BetCount;
            this.Total.Turnover += transaction.Turnover;
            this.Total.MemberWinLoss += transaction.MemberWinLoss;
            this.Total.AgentWinLoss += transaction.AgentWinLoss;
            this.Total.MasterWinLoss += transaction.MasterWinLoss;
            this.Total.SuperWinLoss += transaction.SuperWinLoss;
            this.Total.CompanyWinLoss += transaction.CompanyWinLoss;
            this.Total.TotalWinLoss += transaction.TotalWinLoss;
            this.Total.Commission += transaction.Commission;
            this.Total.ExtraPT += transaction.ExtraPT;
        }
    }
}
