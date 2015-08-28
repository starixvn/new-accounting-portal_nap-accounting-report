// -----------------------------------------------------------------------
// <copyright file="SMAVirtualRate.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Models.TreeWinLoss
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class SMAVirtualRate
    {
        public SMAVirtualRate()
        {
            ReportRows = new List<SMAVirtualRate>();
        }

        public string UserName { get; set; }

        public string Sport { get; set; }

        public List<VirtualRateByBettypeInfo> VirtualRateByBettypeInfo { get; set; }

        public List<SMAVirtualRate> ReportRows { get; set; }
    }
}