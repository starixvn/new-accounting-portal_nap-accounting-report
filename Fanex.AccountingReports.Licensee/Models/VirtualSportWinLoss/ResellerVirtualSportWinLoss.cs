// --------------------------------------------------------------------------
// <copyright file="ResellerVirtualSportWinLoss.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.VirtualSportWinLoss
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ResellerVirtualSportWinLoss
    {
        public ResellerVirtualSportWinLoss()
        {
            WinLossTotal = new LicenseeVirtualSportWinLoss();
            LicenseeWinLossDetail = new List<LicenseeVirtualSportWinLoss>();
        }

        public LicenseeVirtualSportWinLoss WinLossTotal { get; set; }

        public List<LicenseeVirtualSportWinLoss> LicenseeWinLossDetail { get; set; }
    }
}