// -----------------------------------------------------------------------
// <copyright file="KenoWinLoss.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.KenoWinLoss
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Play tech casino win loss model.
    /// </summary>
    [Serializable]
    public class KenoWinLoss
    {
        public KenoWinLoss()
        {
            ReportRows = new List<KenoWinLoss>();
        }

        public string Date { get; set; }

        public string Currency { get; set; }

        public decimal TurnOver { get; set; }

        public decimal TotalTransaction { get; set; }

        public decimal CustomerWinLoss { get; set; }

        public bool OddEven { get; set; }

        public List<KenoWinLoss> ReportRows { get; set; }

        public KenoWinLoss Total { get; set; }

        public KenoWinLoss SubTotal { get; set; }
    }
}
