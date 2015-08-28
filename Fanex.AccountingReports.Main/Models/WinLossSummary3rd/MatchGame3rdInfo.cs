// --------------------------------------------------------------------------
// <copyright file="MatchGame3rdInfo.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Main.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class MatchGame3rdInfo
    {
        public MatchGame3rdInfo()
        {
            MatchGameBetTypes = new List<MatchGameBetTypeInfo>();
            this.Total = new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0
            };
        }

        public SportTypeWinLoss Type { get; set; }

        public IList<MatchGameBetTypeInfo> MatchGameBetTypes { get; set; }

        // Grand Total
        public WinLossInfo Total { get; set; }

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
            this.Total.RealLineTotal += item.RealLineTotal.HasValue ? item.RealLineTotal.Value : 0;
            this.Total.VirtualLineTotal += item.VirtualLineTotal.HasValue ? item.VirtualLineTotal.Value : 0;
            this.Total.RealTurnover += item.RealTurnover.HasValue ? item.RealTurnover.Value : 0;
            this.Total.VirtualTurnover += item.VirtualTurnover.HasValue ? item.VirtualTurnover.Value : 0;
        }
    }
}
