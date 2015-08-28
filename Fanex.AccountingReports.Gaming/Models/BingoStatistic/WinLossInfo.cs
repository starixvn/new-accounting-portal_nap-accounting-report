// --------------------------------------------------------------------------
// <copyright file="WinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Models.BingoStatistic
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

        public int NoPlayers { get; set; }

        public string CurrencyName { get; set; }

        public decimal TotalBets { get; set; }

        public decimal TotalWins { get; set; }

        public decimal GameIncome { get; set; }

        public decimal GamePayout { get; set; }

        public decimal CompanyNetIncome { get; set; }

        public decimal CompanyComm { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        /// <summary>
        /// Formats the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The specified win loss.</returns>
        public static WinLossInfo Format(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.TotalBets = Math.Round(winLoss.TotalBets, 2);
            winLoss.TotalWins = Math.Round(winLoss.TotalWins, 2);
            winLoss.GameIncome = Math.Round(winLoss.GameIncome, 2);
            winLoss.GamePayout = Math.Round(winLoss.GamePayout, 4);
            winLoss.CompanyNetIncome = Math.Round(winLoss.CompanyNetIncome, 2);
            winLoss.CompanyComm = Math.Round(winLoss.CompanyComm, 2);
            return winLoss;
        }

        public void Add(WinLossInfo winLoss)
        {
            this.NoPlayers += winLoss.NoPlayers;
            this.TotalBets += winLoss.TotalBets;
            this.TotalWins += winLoss.TotalWins;
            this.GameIncome += winLoss.GameIncome;
            this.GamePayout += winLoss.GamePayout;
            this.CompanyNetIncome += winLoss.CompanyNetIncome;
            this.CompanyComm += winLoss.CompanyComm;
        }
    }
}
