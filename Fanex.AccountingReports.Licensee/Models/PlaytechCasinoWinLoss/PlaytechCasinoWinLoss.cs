// -----------------------------------------------------------------------
// <copyright file="PlaytechCasinoWinLoss.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.PlaytechCasinoWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Play tech casino win loss model.
    /// </summary>
    [Serializable]
    public class PlaytechCasinoWinLoss
    {
        public PlaytechCasinoWinLoss()
        {
            ReportRows = new List<PlaytechCasinoWinLoss>();
        }

        public string Date { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public decimal TurnOver { get; set; }

        public decimal TotalTransaction { get; set; }

        public decimal CustomerWinLoss { get; set; }

        public decimal LicenseeWinLoss { get; set; }

        public decimal AmountDueTo { get; set; }

        public int TotalItems { get; set; }

        public List<PlaytechCasinoWinLoss> ReportRows { get; set; }

        public List<PlaytechCasinoWinLoss> Total { get; set; }

        public PlaytechCasinoWinLoss BaseCurrencyTotal { get; set; }
    }
}
