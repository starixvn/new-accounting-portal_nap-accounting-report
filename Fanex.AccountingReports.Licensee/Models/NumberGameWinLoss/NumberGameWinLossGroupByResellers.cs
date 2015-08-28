// --------------------------------------------------------------------------
// <copyright file="NumberGameWinLossGroupByResellers.cs" company="Nexcel Solutions Vietnam">
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
    public class NumberGameWinLossGroupByResellers
    {
        public NumberGameWinLossGroupByResellers()
        {
            RevenueInfo = new LicenseeNumberGameWinLoss();
            Resellers = new List<ResellerNumberGameWinLoss>();
        }

        public LicenseeNumberGameWinLoss RevenueInfo { get; set; }

        public List<ResellerNumberGameWinLoss> Resellers { get; set; }
    }
}