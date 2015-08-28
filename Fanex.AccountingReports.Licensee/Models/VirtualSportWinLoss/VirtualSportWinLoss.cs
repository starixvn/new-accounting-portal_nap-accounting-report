// --------------------------------------------------------------------------
// <copyright file="VirtualSportWinLoss.cs" company="Nexcel Solutions Vietnam">
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
    public class VirtualSportWinLoss
    {
        public VirtualSportWinLoss()
        {
            LicenseesWinLoss = new List<VirtualSportWinLossGroupByResellers>();
            GrandTotal = new LicenseeVirtualSportWinLoss();
        }

        public List<VirtualSportWinLossGroupByResellers> LicenseesWinLoss { get; set; }

        public LicenseeVirtualSportWinLoss GrandTotal { get; set; }
    }
}