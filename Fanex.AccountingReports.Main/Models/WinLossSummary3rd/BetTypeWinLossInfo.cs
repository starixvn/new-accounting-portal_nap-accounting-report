// --------------------------------------------------------------------------
// <copyright file="BetTypeWinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Models.WinLossSummary3rd
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class BetTypeWinLossInfo
    {
        public BetTypeWinLossInfo()
        {
            IBCs = new List<WinLossInfo>();
        }

        public List<WinLossInfo> IBCs { get; set; }

        public int Id { get; set; }

        public WinLossInfo EightRM { get; set; }

        public WinLossInfo BARM { get; set; }

        public WinLossInfo Hedge { get; set; }

        public string Name { get; set; }

        public WinLossInfo Total
        {
            get
            {
                WinLossInfo total = new WinLossInfo
                {
                    RealLineTotal = 0,
                    VirtualLineTotal = 0,
                    RealTurnover = 0,
                    VirtualTurnover = 0
                };

                if (IBCs != null)
                {
                    foreach (WinLossInfo item in IBCs)
                    {
                        this.CalculateTotal(ref total, item);
                    }
                }

                if (EightRM != null)
                {
                    this.CalculateTotal(ref total, EightRM);
                }

                if (BARM != null)
                {
                    this.CalculateTotal(ref total, BARM);
                }

                if (Hedge != null)
                {
                    this.CalculateTotal(ref total, Hedge);
                }

                return total;
            }
        }

        /// <summary>
        /// Calculates the total.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="plusWinLoss">The plus win loss.</param>
        private void CalculateTotal(ref WinLossInfo winLoss, WinLossInfo plusWinLoss)
        {
            winLoss.WinLoss += plusWinLoss.WinLoss;
            winLoss.Position += plusWinLoss.Position;
            winLoss.Commission += plusWinLoss.Commission;

            winLoss.RealLineTotal += plusWinLoss.RealLineTotal;
            winLoss.VirtualLineTotal += plusWinLoss.VirtualLineTotal;
            winLoss.RealTurnover += plusWinLoss.RealTurnover;
            winLoss.VirtualTurnover += plusWinLoss.VirtualTurnover;
        }
    }
}
