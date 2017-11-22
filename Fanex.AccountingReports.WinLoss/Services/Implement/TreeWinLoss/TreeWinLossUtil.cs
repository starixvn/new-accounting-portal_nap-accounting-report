namespace Fanex.AccountingReports.WinLoss.Services.Implement.TreeWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.WinLoss.Models.TreeWinLoss;

    /// <summary>
    /// Class contains methods which were belonged to service.
    /// </summary>
    public static class TreeWinLossUtil
    {
        public static readonly string TreeWinLossByProductSession = "Fanex.AccountingReports.WinLoss.TreeWinLoss.WinLossByProduct";
        public static readonly string TreeWinLossByCommSession = "Fanex.AccouningReports.WinLoss.TreeWinLoss.WinLossByComm";
        public static readonly string TreeWinLossBySuperSession = "Fanex.AccouningReports.WinLoss.TreeWinLoss.WinLossBySuper";
        public static readonly string TreeWinLossByMasterSession = "Fanex.AccouningReports.WinLoss.TreeWinLoss.WinLossByMaster";
        public static readonly string TreeWinLossByAgentSession = "Fanex.AccouningReports.WinLoss.TreeWinLoss.WinLossByAgent";

        /// <summary>
        /// Add win loss value to Total.
        /// </summary>
        /// <param name="winLoss">The win loss value.</param>
        /// <param name="total">The win loss total.</param>
        public static void AddToTotal(TreeWinLossInfo winLoss, ref TreeWinLossInfo total)
        {
            if (winLoss != null)
            {
                total.BaseRMTurnover += winLoss.BaseRMTurnover;
                total.BaseRMVirtualTurnover += winLoss.BaseRMVirtualTurnover;
                total.BaseRMTOVarianceMinus += winLoss.BaseRMTOVarianceMinus;
                total.BaseRMTOVariancePlus += winLoss.BaseRMTOVariancePlus;
                total.BaseRMWLVarianceMinus += winLoss.BaseRMWLVarianceMinus;
                total.BaseRMWLVariancePlus += winLoss.BaseRMWLVariancePlus;
                total.BaseRMTotal += winLoss.BaseRMTotal;
                total.BaseRMVirtualTotal += winLoss.BaseRMVirtualTotal;
            }
        }

        /// <summary>
        /// Round object value. 
        /// </summary>
        /// <param name="winLoss">The Tree Win Loss Info.</param>
        /// <returns>The Tree Win Loss Info after rounding.</returns>
        public static TreeWinLossInfo RoundTreeWinLossInfo(TreeWinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.Turnover = Math.Round(winLoss.Turnover, 2);
            winLoss.Comm = Math.Round(winLoss.Comm, 2);
            winLoss.WinLoss = Math.Round(winLoss.WinLoss, 2);
            winLoss.WinLossTotal = Math.Round(winLoss.WinLossTotal, 2);
            winLoss.BaseRMTurnover = Math.Round(winLoss.BaseRMTurnover, 2);
            winLoss.BaseRMVirtualTurnover = Math.Round(winLoss.BaseRMVirtualTurnover, 2);
            winLoss.BaseRMTOVarianceMinus = Math.Round(winLoss.BaseRMTOVarianceMinus, 2);
            winLoss.BaseRMTOVariancePlus = Math.Round(winLoss.BaseRMTOVariancePlus, 2);
            winLoss.BaseRMWLVarianceMinus = Math.Round(winLoss.BaseRMWLVarianceMinus, 2);
            winLoss.BaseRMWLVariancePlus = Math.Round(winLoss.BaseRMWLVariancePlus, 2);
            winLoss.BaseRMVirtualTotal = Math.Round(winLoss.BaseRMVirtualTotal, 2);
            winLoss.BaseRMTotal = Math.Round(winLoss.BaseRMTotal, 2);

            return winLoss;
        }
    }
}
