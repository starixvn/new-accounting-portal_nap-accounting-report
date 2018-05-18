namespace Fanex.AccountingReports.WinLoss.Models.TreeWinLoss
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TreeWinLossVirtualRate
    {
        public TreeWinLossVirtualRate()
        {
            SMAVirtualRate = new SMAVirtualRate();
        }

        public int NumberUserEnabledVirtualRate { get; set; }

        public Dictionary<string, int> EnabledVirtualRateUser { get; set; }

        public SMAVirtualRate SMAVirtualRate { get; set; }
    }
}
