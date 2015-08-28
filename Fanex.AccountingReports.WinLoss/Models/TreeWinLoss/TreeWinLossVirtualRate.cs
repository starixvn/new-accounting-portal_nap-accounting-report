// -----------------------------------------------------------------------
// <copyright file="TreeWinLossVirtualRate.cs" company="Nexcel Solutions Vietnam">
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
    public class TreeWinLossVirtualRate
    {
        public TreeWinLossVirtualRate()
        {
            TreeWinLoss = new TreeWinLoss();
            SMAVirtualRate = new SMAVirtualRate();
        }

        public int NumberUserEnabledVirtualRate { get; set; }

        public Dictionary<string, int> EnabledVirtualRateUser { get; set; }

        public TreeWinLoss TreeWinLoss { get; set; }

        public SMAVirtualRate SMAVirtualRate { get; set; }
    }
}