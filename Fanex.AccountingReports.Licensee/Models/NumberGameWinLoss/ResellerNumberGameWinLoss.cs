// --------------------------------------------------------------------------
// <copyright file="ResellerNumberGameWinLoss.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.NumberGameWinLoss
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ResellerNumberGameWinLoss
    {
        public ResellerNumberGameWinLoss()
        {
            WinLossTotal = new LicenseeNumberGameWinLoss();
            LicenseeWinLossDetail = new List<LicenseeNumberGameWinLoss>();
        }

        public LicenseeNumberGameWinLoss WinLossTotal { get; set; }

        public List<LicenseeNumberGameWinLoss> LicenseeWinLossDetail { get; set; }
    }
}