// --------------------------------------------------------------------------
// <copyright file="SportTypeWinLossInfo.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Main.Common;
    using MainSportConstants = Fanex.AccountingReports.Main.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class SportTypeWinLossInfo
    {
        public SportTypeWinLossInfo()
        {
            BetTypeWinLosses = new List<BetTypeWinLossInfo>();
            Total = new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0,
            };
            TotalForNewCricket = new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0,
            };
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool CanShowDetailLink
        {
            get
            {
                if (Id == MainConstants.WinLossSumamryCasinoSportId ||
                    Id == MainConstants.WinLossSummaryLiveCasinoSportId ||
                    Id == MainConstants.WinLossSummaryBingoSportId ||
                    Id == MainConstants.WinLossSummaryP2PSportId ||
                    Id == MainConstants.WinLossSummaryMixParlaySportId ||
                    Id == MainConstants.WinLossSummarySystemParlaySportId ||
                    Id == MainConstants.WinLossSummaryKenoSportId ||
                    Id == SportContants.GoldDeluxe ||
                    Id == SportContants.MetricGamingSuperLive ||
                    Id == SportContants.MicrogamingLiveCasino ||
                    Id == SportContants.MicrogamingRNGCasino ||
                    Id == SportContants.AGCasino ||
                    Id == SportContants.Macau ||
                    Id == MainSportConstants.SportContants._2dBettrade ||
                    Id == SportContants.ColossusBet ||
                    Id == SportContants.CockFighting)
                {
                    return false;
                }

                return true;
            }
        }

        public List<BetTypeWinLossInfo> BetTypeWinLosses { get; set; }

        /// <summary>
        /// Gets or sets IBC Total = 8RM + BARM.
        /// </summary>
        /// <value>
        /// The IBC total.
        /// </value>
        public WinLossInfo IBCTotal { get; set; }

        public WinLossInfo Total { get; set; }

        public WinLossInfo TotalForNewCricket { get; set; }

        public WinLossInfo HedgeTotal { get; set; }

        /// <summary>
        /// Calculates the total.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void CalculateTotalNewCricket(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.TotalForNewCricket.WinLoss += item.WinLoss;
            this.TotalForNewCricket.Position += item.Position;
            this.TotalForNewCricket.Commission += item.Commission;
            this.TotalForNewCricket.RealLineTotal += item.RealLineTotal;
            this.TotalForNewCricket.VirtualLineTotal += item.VirtualLineTotal;
            this.TotalForNewCricket.RealTurnover += item.RealTurnover;
            this.TotalForNewCricket.VirtualTurnover += item.VirtualTurnover;
        }

        /// <summary>
        /// Calculates the total.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void CalculateTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.Total.WinLoss += item.WinLoss;
            this.Total.Position += item.Position;
            this.Total.Commission += item.Commission;
            this.Total.RealLineTotal += item.RealLineTotal;
            this.Total.VirtualLineTotal += item.VirtualLineTotal;
            this.Total.RealTurnover += item.RealTurnover;
            this.Total.VirtualTurnover += item.VirtualTurnover;
        }

        /// <summary>
        /// Calculates the IBC total.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void CalculateIBCTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.IBCTotal = this.IBCTotal ?? new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0
            };

            this.IBCTotal.WinLoss += item.WinLoss;
            this.IBCTotal.Position += item.Position;
            this.IBCTotal.Commission += item.Commission;

            this.IBCTotal.RealLineTotal += item.RealLineTotal;
            this.IBCTotal.VirtualLineTotal += item.VirtualLineTotal;
            this.IBCTotal.RealTurnover += item.RealTurnover;
            this.IBCTotal.VirtualTurnover += item.VirtualTurnover;
        }

        /// <summary>
        /// Calculate the hedge total.
        /// </summary>
        /// <param name="item">The win loss information.</param>
        public void CalculateHedgeTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.HedgeTotal.WinLoss += item.WinLoss;
            this.HedgeTotal.Position += item.Position;
            this.HedgeTotal.Commission += item.Commission;
            this.HedgeTotal.RealLineTotal += item.RealLineTotal;
            this.HedgeTotal.VirtualLineTotal += item.VirtualLineTotal;
            this.HedgeTotal.RealTurnover += item.RealTurnover;
            this.HedgeTotal.VirtualTurnover += item.VirtualTurnover;
        }
    }
}