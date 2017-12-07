namespace Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Services.Impl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal CustomerWinLoss { get; set; }

        public decimal AgencyWinLoss { get; set; }

        public int RoleLevelId { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Commission { get; set; }

        public decimal LineTotal { get; set; }

        public decimal VirtualLineTotal { get; set; }

        public decimal Turnover { get; set; }

        public decimal Variance
        {
            get
            {
                return Math.Round(LineTotal - VirtualLineTotal, 4, MidpointRounding.AwayFromZero);
            }
        }

        public decimal Margin
        {
            get
            {
                return Math.Round(ReportService.CalculateMargin(LineTotal, Turnover), 4, MidpointRounding.AwayFromZero);
            }
        }

        public List<WinLossInfo> DetailInfo { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        /// <summary>
        /// Formats the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>Win Loss Info.</returns>
        public static WinLossInfo Format(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.AgencyWinLoss = Math.Round(winLoss.AgencyWinLoss, 2, MidpointRounding.AwayFromZero);
            winLoss.CustomerWinLoss = Math.Round(winLoss.CustomerWinLoss, 2, MidpointRounding.AwayFromZero);

            winLoss.WinLoss = Math.Round(winLoss.WinLoss, 2, MidpointRounding.AwayFromZero);
            winLoss.Commission = Math.Round(winLoss.Commission, 2, MidpointRounding.AwayFromZero);
            winLoss.LineTotal = Math.Round(winLoss.LineTotal, 2, MidpointRounding.AwayFromZero);
            winLoss.VirtualLineTotal = Math.Round(winLoss.VirtualLineTotal, 2, MidpointRounding.AwayFromZero);
            winLoss.Turnover = Math.Round(winLoss.Turnover, 2, MidpointRounding.AwayFromZero);
            return winLoss;
        }

        /// <summary>
        /// Adds the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void Sum(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            CustomerWinLoss += winLoss.CustomerWinLoss;
            AgencyWinLoss += winLoss.AgencyWinLoss;

            WinLoss += winLoss.WinLoss;
            Commission += winLoss.Commission;
            LineTotal += winLoss.LineTotal;
            VirtualLineTotal += winLoss.VirtualLineTotal;
            Turnover += winLoss.Turnover;
        }
    }
}
