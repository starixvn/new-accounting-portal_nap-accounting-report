namespace Fanex.AccountingReports.WinLoss.Models.TreeWinLoss
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class TreeWinLoss
    {
        public List<TreeWinLossInfo> ChildsInfo { get; set; }

        public List<TreeWinLossInfo> TotalInfo { get; set; }

        public bool IsShowNotAvailableTurnover { get; set; }
    }
}