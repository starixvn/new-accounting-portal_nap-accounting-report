// --------------------------------------------------------------------------
// <copyright file="VirtualSportWinLossGroupByResellers.cs" company="Nexcel Solutions Vietnam">
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
    public class VirtualSportWinLossGroupByResellers
    {
        public VirtualSportWinLossGroupByResellers()
        {
            RevenueInfo = new LicenseeVirtualSportWinLoss();
            Resellers = new List<ResellerVirtualSportWinLoss>();
        }

        public LicenseeVirtualSportWinLoss RevenueInfo { get; set; }

        public List<ResellerVirtualSportWinLoss> Resellers { get; set; }
    }
}