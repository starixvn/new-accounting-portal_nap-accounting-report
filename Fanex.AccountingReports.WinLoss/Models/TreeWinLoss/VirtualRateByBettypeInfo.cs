// -----------------------------------------------------------------------
// <copyright file="VirtualRateByBettypeInfo.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Models.TreeWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class VirtualRateByBettypeInfo
    {
        public string BettypeName { get; set; }

        public decimal? VirtualRate { get; set; }
    }
}
