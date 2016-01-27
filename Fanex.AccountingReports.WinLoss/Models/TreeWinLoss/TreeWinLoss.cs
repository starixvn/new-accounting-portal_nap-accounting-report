// -----------------------------------------------------------------------
// <copyright file="TreeWinLoss.cs" company="Nexcel Solutions Vietnam">
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
    public class TreeWinLoss
    {
        public List<TreeWinLossInfo> ChildsInfo { get; set; }

        public List<TreeWinLossInfo> TotalInfo { get; set; }

        public bool IsShowNotAvailableTurnover { get; set; }
    }
}
