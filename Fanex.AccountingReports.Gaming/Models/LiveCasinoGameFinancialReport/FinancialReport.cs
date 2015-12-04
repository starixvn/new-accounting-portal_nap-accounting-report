// --------------------------------------------------------------------------
// <copyright file="FinancialReport.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Models.LiveCasinoGameFinancialReport
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class FinancialReport
    {
        public string Name { get; set; }

        public int PlayerCount { get; set; }

        public int GameCount { get; set; }

        public bool IsGameTable { get; set; }

        public int BetCount { get; set; }

        public decimal TotalBets { get; set; }

        public decimal TotalWins { get; set; }

        public decimal GrossIncome { get; set; }

        public decimal Commission { get; set; }

        public decimal NetIncome { get; set; }

        public decimal GamePayout { get; set; }

        public decimal GameLevy { get; set; }

        public decimal GameLevyToPay { get; set; }

        public List<FinancialReport> ChildInfo { get; set; }

        public FinancialReport GrandTotal { get; set; }

        public void AddToGrandTotal(FinancialReport info)
        {
            this.GrandTotal.BetCount += info.BetCount;
            this.GrandTotal.TotalBets += info.TotalBets;
            this.GrandTotal.TotalWins += info.TotalWins;
            this.GrandTotal.GrossIncome += info.GrossIncome;
            this.GrandTotal.Commission += info.Commission;
            this.GrandTotal.NetIncome += info.NetIncome;
            this.GrandTotal.GameLevy += info.GameLevy;
            this.GrandTotal.GameLevyToPay += info.GameLevyToPay;
        }
    }
}
