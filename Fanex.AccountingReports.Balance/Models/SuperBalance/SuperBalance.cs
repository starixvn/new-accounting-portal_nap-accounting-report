// -----------------------------------------------------------------------
// <copyright file="SuperBalance.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2013.10.24">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Models.SuperBalance
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class SuperBalance
    {
        public string CustId { get; set; }

        public string Username { get; set; }

        public string Currency { get; set; }

        public decimal Balance { get; set; }

        public decimal BaseBalance { get; set; }

        public bool HasData { get; set; }

        public List<SuperBalance> SuperBalanceList { get; set; }

        public SuperBalance BaseBalanceTotal { get; set; }

        public static SuperBalance FormatInfo(SuperBalance balanceInfo)
        {
            if (balanceInfo == null)
            {
                return balanceInfo;
            }

            balanceInfo.Balance = Math.Round(balanceInfo.Balance, 2, MidpointRounding.AwayFromZero);
            balanceInfo.BaseBalance = Math.Round(balanceInfo.BaseBalance, 2, MidpointRounding.AwayFromZero);
            return balanceInfo;
        }

        public void AddToBaseBalance(SuperBalance superBalance)
        {
            if (superBalance == null)
            {
                return;
            }

            this.BaseBalance += superBalance.BaseBalance;
        }
    }
}
