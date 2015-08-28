// --------------------------------------------------------------------------
// <copyright file="VirtualSportWinLossNEW.cs" company="Nexcel Solutions Vietnam">
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
    public class VirtualSportWinLossNEW
    {
        public VirtualSportWinLossNEW()
        {
            DistributorRevenue = string.Empty;
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public string BetCount { get; set; }

        public string Turnover { get; set; }

        public string OneworksIncome { get; set; }

        public string GrossWinloss { get; set; }

        public string DistributorRevenue { get; set; }

        public string AmountInspired { get; set; }

        public string AmountOneWorks { get; set; }

        public List<VirtualSportWinLossNEW> Rows { get; set; }
    }
}