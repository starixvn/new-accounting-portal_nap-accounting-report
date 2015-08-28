// --------------------------------------------------------------------------
// <copyright file="ParlayWonTransaction.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.ParlayWon
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ParlayWonTransaction
    {
        public ulong RefNo { get; set; }

        public string UserName { get; set; }

        public string Currency { get; set; }

        public decimal Exchange { get; set; }

        public decimal OriginalStake { get; set; }

        public decimal OriginalWinLoss { get; set; }

        public decimal BaseStake { get; set; }

        public decimal BaseWinLoss { get; set; }

        public decimal CompanyStake { get; set; }

        public decimal CompanyWinLoss { get; set; }

        public int Selection { get; set; }

        public decimal WinOdds { get; set; }

        public int ReportItemLength { get; set; }

        public List<ParlayWonTransaction> TransactionList { get; set; }

        public ParlayWonTransaction Total { get; set; }

        /// <summary>
        /// Sums the data to total.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public void SumDataToTotal(ParlayWonTransaction transaction)
        {
            if (transaction == null)
            {
                return;
            }

            this.Total.OriginalStake += transaction.OriginalStake;
            this.Total.OriginalWinLoss += transaction.OriginalWinLoss;
            this.Total.BaseStake += transaction.BaseStake;
            this.Total.BaseWinLoss += transaction.BaseWinLoss;
            this.Total.CompanyStake += transaction.CompanyStake;
            this.Total.CompanyWinLoss += transaction.CompanyWinLoss;
        }
    }
}
