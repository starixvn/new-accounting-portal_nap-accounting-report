// --------------------------------------------------------------------------
// <copyright file="Transaction.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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

        public List<Transaction> TransactionList { get; set; }

        public Transaction Total { get; set; }

        /// <summary>
        /// Sums the data to total.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
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
        }
    }
}
