// --------------------------------------------------------------------------
// <copyright file="LicenseeNumberGameWinLoss.cs" company="Nexcel Solutions Vietnam">
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
    public class LicenseeNumberGameWinLoss
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