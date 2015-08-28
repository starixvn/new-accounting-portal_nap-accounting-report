// --------------------------------------------------------------------------
// <copyright file="NumberGameWinLoss.cs" company="Nexcel Solutions Vietnam">
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
    public class NumberGameWinLoss
    {
        public NumberGameWinLoss()
        {
            LicenseesWinLoss = new List<NumberGameWinLossGroupByResellers>();
            GrandTotal = new LicenseeNumberGameWinLoss();
        }

        public List<NumberGameWinLossGroupByResellers> LicenseesWinLoss { get; set; }

        public LicenseeNumberGameWinLoss GrandTotal { get; set; }
    }
}