// --------------------------------------------------------------------------
// <copyright file="WinLoss3rdInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.25">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Models.WinLossSummary3rd
{
    using Common;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLoss3rdInfo
    {
        public WinLoss3rdInfo()
        {
            SportTypeWinLosses = new List<SportTypeWinLossInfo>();
            AllTotal = new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0
            };
            IBCTotal = this.IBCTotal ?? new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0
            };
        }

        public IList<SportTypeWinLossInfo> SportTypeWinLosses { get; set; }

        /// <summary>
        /// Gets IBC Total = 8RM + BARM.
        /// </summary>
        /// <value>
        /// The IBC total.
        /// </value>
        public WinLossInfo IBCTotal
        {
            get;
        }

        public WinLossInfo AllTotal { get; set; }

        /// <summary>
        /// Calculates all total.
        /// </summary>
        /// <param name="sportId">The sport identifier.</param>
        /// <param name="item">The item value.</param>
        public void CalculateAllTotal(int sportId, WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            var isRM = item.IBC == RowType.BARM || item.IBC == RowType.EightRM;

            this.AllTotal.WinLoss += item.WinLoss;
            this.AllTotal.Position += item.Position;
            this.AllTotal.Commission += item.Commission;

            this.AllTotal.RealLineTotal += item.RealLineTotal.HasValue ? item.RealLineTotal.Value : 0;
            this.AllTotal.VirtualLineTotal += item.VirtualLineTotal.HasValue ? item.VirtualLineTotal.Value : 0;
            this.AllTotal.RealTurnover += !isRM && item.RealTurnover.HasValue ? item.RealTurnover.Value : 0;
            this.AllTotal.VirtualTurnover += !isRM && item.VirtualTurnover.HasValue ? item.VirtualTurnover.Value : 0;
        }

        /// <summary>
        /// Calculates the temporary IBC total.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void CalculateIBCTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.IBCTotal.WinLoss += -item.WinLoss;
            this.IBCTotal.Position += item.Position;
            this.IBCTotal.Commission += item.Commission;

            this.IBCTotal.RealLineTotal += item.RealLineTotal.HasValue ? -item.RealLineTotal.Value : 0;
            this.IBCTotal.VirtualLineTotal += item.VirtualLineTotal.HasValue ? -item.VirtualLineTotal.Value : 0;
            this.IBCTotal.RealTurnover += item.RealTurnover.HasValue ? item.RealTurnover.Value : 0;
            this.IBCTotal.VirtualTurnover += item.VirtualTurnover.HasValue ? item.VirtualTurnover.Value : 0;
        }
    }
}
