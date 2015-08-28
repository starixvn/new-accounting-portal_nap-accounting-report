// --------------------------------------------------------------------------
// <copyright file="LiveCasinoWinLossDetail.cs" company="Nexcel Solutions Vietnam">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class LiveCasinoWinLossDetail
    {
        public int ResellerType { get; set; }

        public string Licensee { get; set; }

        public string Turnover { get; set; }

        public string TotalTransaction { get; set; }

        public string GrossWin { get; set; }

        public string DistributorRevenue { get; set; }

        public string StarExpressRevenue { get; set; }

        public string AmountDueToOneWorks { get; set; }
    }
}
