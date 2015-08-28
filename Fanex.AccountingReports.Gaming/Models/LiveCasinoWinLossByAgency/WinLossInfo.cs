// --------------------------------------------------------------------------
// <copyright file="WinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Models.LiveCasinoWinLossByAgency
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CurrencyName { get; set; }

        public decimal RollingTurnover { get; set; }

        public decimal GrossCommission { get; set; }

        public decimal TotalBets { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Commission { get; set; }

        public decimal Turnover { get; set; }

        public decimal RealWinLoss { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        /// <summary>
        /// Formats the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The specified win loss.</returns>
        public static WinLossInfo Format(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.TotalBets = Math.Round(winLoss.TotalBets, 0, MidpointRounding.AwayFromZero);
            winLoss.RollingTurnover = Math.Round(winLoss.RollingTurnover, 2, MidpointRounding.AwayFromZero);
            winLoss.GrossCommission = Math.Round(winLoss.GrossCommission, 2, MidpointRounding.AwayFromZero);

            winLoss.WinLoss = Math.Round(winLoss.WinLoss, 2, MidpointRounding.AwayFromZero);
            winLoss.Commission = Math.Round(winLoss.Commission, 2, MidpointRounding.AwayFromZero);
            winLoss.Turnover = Math.Round(winLoss.Turnover, 2, MidpointRounding.AwayFromZero);
            winLoss.RealWinLoss = Math.Round(winLoss.RealWinLoss, 2, MidpointRounding.AwayFromZero);

            if (winLoss.ChildsInfo == null)
            {
                return winLoss;
            }

            for (var i = 0; i < winLoss.ChildsInfo.Count; i++)
            {
                winLoss.ChildsInfo[i] = WinLossInfo.Format(winLoss.ChildsInfo[i]);
            }

            return winLoss;
        }

        public static decimal CalculateRealWinLoss(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return 0;
            }

            return winLoss.WinLoss + winLoss.Commission;
        }

        /// <summary>
        /// Sums the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void Sum(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            this.TotalBets += winLoss.TotalBets;
            this.RollingTurnover += winLoss.RollingTurnover;
            this.GrossCommission += winLoss.GrossCommission;

            this.WinLoss += winLoss.WinLoss;
            this.Commission += winLoss.Commission;
            this.Turnover += winLoss.Turnover;
            this.RealWinLoss += winLoss.RealWinLoss;

            if (winLoss.ChildsInfo == null)
            {
                return;
            }

            for (var i = 0; i < winLoss.ChildsInfo.Count; i++)
            {
                this.ChildsInfo[i].Sum(winLoss.ChildsInfo[i]);
            }
        }
    }
}
