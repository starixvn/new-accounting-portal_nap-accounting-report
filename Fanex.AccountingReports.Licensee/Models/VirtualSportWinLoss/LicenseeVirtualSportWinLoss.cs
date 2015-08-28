// --------------------------------------------------------------------------
// <copyright file="LicenseeVirtualSportWinLoss.cs" company="Nexcel Solutions Vietnam">
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
    public class LicenseeVirtualSportWinLoss
    {
        public int ResellerType { get; set; }

        public string Licensee { get; set; }

        public string Turnover { get; set; }

        public string TotalTransaction { get; set; }

        public string GrossWin { get; set; }

        public string DistributorRevenue { get; set; }

        public string LupineRevenue { get; set; }

        public string AmountDueToOneWorks { get; set; }
    }
}