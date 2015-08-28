// --------------------------------------------------------------------------
// <copyright file="ResellerLiveCasinoWinLoss.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.12">Created</change>
//   <change who="Ricky.Nguyen" date="2012.12.3">Update Constructor</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.LiveCasinoWinLoss
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ResellerLiveCasinoWinLoss
    {
        public ResellerLiveCasinoWinLoss()
        {
            WinLossTotal = new LiveCasinoWinLossDetail();
            LicenseeWinLossDetail = new List<LiveCasinoWinLossDetail>();
        }

        public LiveCasinoWinLossDetail WinLossTotal { get; set; }

        public List<LiveCasinoWinLossDetail> LicenseeWinLossDetail { get; set; }
    }
}
