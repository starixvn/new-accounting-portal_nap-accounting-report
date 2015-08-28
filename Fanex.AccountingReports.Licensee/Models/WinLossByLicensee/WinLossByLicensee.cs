// --------------------------------------------------------------------------
// <copyright file="WinLossByLicensee.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.WinLossByLicensee
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossByLicensee
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string BettingDate { get; set; }

        /// <summary>
        /// Gets or sets FromDate of betting time.
        /// Cause JSON parse it to format /Date(total ticks)/ when receive on client
        /// But when submit to server it not recognize, so store with string format.
        /// </summary>
        /// <value>
        /// From date.
        /// </value>
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public decimal Turnover { get; set; }

        public int BetCount { get; set; }

        public decimal CustomerWinLoss { get; set; }

        public decimal CustomerComm { get; set; }

        public decimal CustomerWinLossTotal
        {
            get
            {
                return CustomerWinLoss + CustomerComm;
            }
        }

        public decimal LicenseeWinLoss { get; set; }

        public decimal BAComm { get; set; }

        public decimal RacingWinLoss { get; set; }

        public decimal RacingComm { get; set; }

        public decimal RacingWinLossTotal
        {
            get
            {
                return RacingWinLoss + RacingComm;
            }
        }

        public decimal AmtDuetoBA { get; set; }

        public decimal AmtDuetoRacing { get; set; }

        public decimal AmtDuetoCompany { get; set; }

        public int ReportItemLength { get; set; }

        public List<WinLossByLicensee> TransactionList { get; set; }

        public List<WinLossByLicensee> Total { get; set; }

        public List<WinLossByLicensee> BaseCurrencyTotal { get; set; }
    }
}
