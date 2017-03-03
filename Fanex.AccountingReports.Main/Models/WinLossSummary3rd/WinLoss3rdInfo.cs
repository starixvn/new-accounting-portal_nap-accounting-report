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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLoss3rdInfo
    {
        public WinLoss3rdInfo()
        {
            SportTypeWinLosses = new List<SportTypeWinLossInfo>();
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
            get
            {
                var ibcTotalList = SportTypeWinLosses.Where(s => s.IBCTotal != null).Select(s => s.IBCTotal).ToArray();
                var total = new WinLossInfo
                {
                    Name = string.Empty,
                    Status = string.Empty,
                    Id = 0,
                    IsHedge = false,
                    IBC = 0,
                    LiveIndicator = 0,
                    WinLoss = ibcTotalList.Sum(s => s.WinLoss),
                    Commission = ibcTotalList.Sum(s => s.Commission),
                    Position = ibcTotalList.Sum(s => s.Position),
                    RealLineTotal = ibcTotalList.Sum(s => s.RealLineTotal),
                    VirtualLineTotal = ibcTotalList.Sum(s => s.VirtualLineTotal),
                    RealTurnover = ibcTotalList.Sum(s => s.RealTurnover),
                    VirtualTurnover = ibcTotalList.Sum(s => s.VirtualTurnover),
                };

                total.Variance = total.RealLineTotal - total.VirtualLineTotal;
                total.RealMargin = total.RealTurnover == 0 ? 0 : total.RealLineTotal / total.RealTurnover;
                total.VirtualMargin = total.VirtualTurnover == 0 ? 0 : total.VirtualLineTotal / total.VirtualTurnover;

                return total;
            }
        }

        public WinLossInfo AllTotal
        {
            get
            {
                var total = new WinLossInfo
                {
                    Name = string.Empty,
                    Status = string.Empty,
                    Id = 0,
                    IsHedge = false,
                    IBC = 0,
                    LiveIndicator = 0,
                    WinLoss = SportTypeWinLosses.Sum(s => s.Total.WinLoss),
                    Commission = SportTypeWinLosses.Sum(s => s.Total.Commission),
                    Position = SportTypeWinLosses.Sum(s => s.Total.Position),
                    RealLineTotal = SportTypeWinLosses.Sum(s => s.Total.RealLineTotal),
                    VirtualLineTotal = SportTypeWinLosses.Sum(s => s.Total.VirtualLineTotal),
                    RealTurnover = SportTypeWinLosses.Sum(s => s.Total.RealTurnover),
                    VirtualTurnover = SportTypeWinLosses.Sum(s => s.Total.VirtualTurnover),
                };

                total.Variance = total.RealLineTotal - total.VirtualLineTotal;
                total.RealMargin = total.RealTurnover == 0 ? 0 : total.RealLineTotal / total.RealTurnover;
                total.VirtualMargin = total.VirtualTurnover == 0 ? 0 : total.VirtualLineTotal / total.VirtualTurnover;

                return total;
            }
        }
    }
}
