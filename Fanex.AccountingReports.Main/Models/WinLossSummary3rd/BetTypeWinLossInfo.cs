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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class BetTypeWinLossInfo
    {
        public BetTypeWinLossInfo()
        {
            //IBCs = new List<WinLossInfo>();
        }

        //public List<WinLossInfo> IBCs { get; set; }

        public int Id { get; set; }

        public WinLossInfo EightRM { get; set; }

        public WinLossInfo BARM { get; set; }

        public WinLossInfo Hedge { get; set; }

        public WinLossInfo Normal { get; set; }

        public string Name { get; set; }

        public WinLossInfo Total
        {
            get
            {
                WinLossInfo total = new WinLossInfo
                {
                    WinLoss = 0,
                    Position = 0,
                    Commission = 0,
                    RealLineTotal = 0,
                    VirtualLineTotal = 0,
                    RealTurnover = 0,
                    VirtualTurnover = 0,
                    Name = Name,
                    LiveIndicator = Normal == null ? 0 : Normal.LiveIndicator,
                    Status = Normal == null ? "" : Normal.Status,
                    IBC = Normal == null ? 0 : Normal.IBC,
                    Id = Normal == null ? 0 : Normal.Id,
                    IsHedge = Normal == null ? false : Normal.IsHedge
                };

                if (Normal != null)
                {
                    this.CalculateTotal(ref total, Normal, false);
                }
                if (EightRM != null)
                {
                    this.CalculateTotal(ref total, EightRM, true);
                }

                if (BARM != null)
                {
                    this.CalculateTotal(ref total, BARM, true);
                }

                if (Hedge != null)
                {
                    this.CalculateTotal(ref total, Hedge, false);
                }

                total.Variance = total.RealLineTotal - total.VirtualLineTotal;
                total.RealMargin = total.RealTurnover == 0 ? 0 : total.RealLineTotal / total.RealTurnover;
                total.VirtualMargin = total.VirtualTurnover == 0 ? 0 : total.VirtualLineTotal / total.VirtualTurnover;

                return total;
            }
        }

        /// <summary>
        /// Calculates the total.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="plusWinLoss">The plus win loss.</param>
        private void CalculateTotal(ref WinLossInfo winLoss, WinLossInfo plusWinLoss, bool isRM)
        {
            winLoss.WinLoss += plusWinLoss.WinLoss;
            winLoss.Position += plusWinLoss.Position;
            winLoss.Commission += plusWinLoss.Commission;

            winLoss.RealLineTotal += (isRM ? -1 : 1) * plusWinLoss.RealLineTotal;
            winLoss.VirtualLineTotal += (isRM ? -1 : 1) * plusWinLoss.VirtualLineTotal;
            winLoss.RealTurnover += isRM ? 0 : plusWinLoss.RealTurnover;
            winLoss.VirtualTurnover += isRM ? 0 : plusWinLoss.VirtualTurnover;
        }
    }
}
