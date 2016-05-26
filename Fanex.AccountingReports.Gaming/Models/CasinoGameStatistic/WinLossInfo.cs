// --------------------------------------------------------------------------
// <copyright file="WinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int NumberOfPlayers { get; set; }

        public int NumberOfGames { get; set; }

        public string CurrencyName { get; set; }

        public int CurrencyId { get; set; }

        public decimal TotalBets { get; set; }

        public decimal TotalWins { get; set; }

        public decimal TotalRefund { get; set; }

        public decimal GameIncome { get; set; }

        public decimal GamePayout
        {
            get;
            set;
        }

        public decimal CompanyTurnover { get; set; }

        public decimal CompanyNetIncome { get; set; }

        public decimal CompanyCommission { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        /// <summary>
        /// Sums the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void Sum(WinLossInfo winLoss)
        {
            this.NumberOfPlayers += winLoss.NumberOfPlayers;
            this.NumberOfGames += winLoss.NumberOfGames;

            this.TotalBets += winLoss.TotalBets;
            this.TotalWins += winLoss.TotalWins;
            this.TotalRefund += winLoss.TotalRefund;

            this.GameIncome += winLoss.GameIncome;
            this.GamePayout = winLoss.TotalBets == 0 ? 0 : winLoss.TotalWins / winLoss.TotalBets;
            this.CompanyTurnover += winLoss.CompanyTurnover;
            this.CompanyNetIncome += winLoss.CompanyNetIncome;
            this.CompanyCommission += winLoss.CompanyCommission;
        }
    }
}
