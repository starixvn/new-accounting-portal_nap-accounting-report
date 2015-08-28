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
    using System.Web.Script.Serialization;
    using Fanex.AccountingReports.Core.Extensions;

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
            TempIBCTotal = this.TempIBCTotal ?? new WinLossInfo
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
            get
            {
                return new WinLossInfo
                {
                    WinLoss = -this.TempIBCTotal.WinLoss,
                    Position = -this.TempIBCTotal.Position,
                    Commission = -this.TempIBCTotal.Commission,
                    RealLineTotal = -this.TempIBCTotal.RealLineTotal,
                    VirtualLineTotal = -this.TempIBCTotal.VirtualLineTotal,
                    Variance = -this.TempIBCTotal.Variance, // set Variance
                    RealTurnover = -this.TempIBCTotal.RealTurnover,
                    VirtualTurnover = -this.TempIBCTotal.VirtualTurnover,
                    RealMargin = -this.TempIBCTotal.RealMargin, // set RealMargin
                    VirtualMargin = -this.TempIBCTotal.VirtualMargin // set VirtualMargin
                };
            }
        }

        /// <summary>
        /// Gets or sets temporary IBC Total = 8RM + BARM.
        /// </summary>
        /// <value>
        /// The temporary IBC total.
        /// </value>
        [ScriptIgnore]
        public WinLossInfo TempIBCTotal { get; set; }

        public WinLossInfo AllTotal { get; set; }

        /// <summary>
        /// Gets grand total = All Total + IBC Total.
        /// </summary>
        /// <value>
        /// The grand total.
        /// </value>
        public WinLossInfo GrandTotal
        {
            get
            {
                WinLossInfo grandTotal = new WinLossInfo
                {
                    WinLoss = this.AllTotal.WinLoss + this.IBCTotal.WinLoss,
                    Position = this.AllTotal.Position + this.IBCTotal.Position,
                    Commission = this.AllTotal.Commission + this.IBCTotal.Commission,
                    RealLineTotal = this.AllTotal.RealLineTotal + this.IBCTotal.RealLineTotal,
                    VirtualLineTotal = this.AllTotal.VirtualLineTotal + this.IBCTotal.VirtualLineTotal,
                    Variance = this.AllTotal.Variance + this.IBCTotal.Variance, // set Variance
                    RealTurnover = this.AllTotal.RealTurnover + this.IBCTotal.RealTurnover,
                    VirtualTurnover = this.AllTotal.VirtualTurnover + this.IBCTotal.VirtualTurnover,
                };

                if (grandTotal.RealTurnover.HasValue && grandTotal.RealTurnover.Value != 0)
                {
                    grandTotal.RealMargin = grandTotal.RealLineTotal / grandTotal.RealTurnover;
                }

                if (grandTotal.VirtualTurnover.HasValue && grandTotal.VirtualTurnover.Value != 0)
                {
                    grandTotal.VirtualMargin = grandTotal.VirtualLineTotal / grandTotal.VirtualTurnover;
                }

                return grandTotal;
            }
        }

        //// Grand Total

        public string RealGrandTotalStr
        {
            get
            {
                return this.GrandTotal.RealLineTotalStr;
            }
        }

        public string VirtualGrandTotalStr
        {
            get
            {
                return this.GrandTotal.VirtualLineTotalStr;
            }
        }

        public string VarianceGrandTotalStr
        {
            get
            {
                return (this.GrandTotal.RealLineTotal - this.GrandTotal.VirtualLineTotal).Format(2);
            }
        }

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

            this.AllTotal.WinLoss += item.WinLoss;
            this.AllTotal.Position += item.Position;
            this.AllTotal.Commission += item.Commission;

            this.AllTotal.RealLineTotal += item.RealLineTotal.HasValue ? item.RealLineTotal.Value : 0;
            this.AllTotal.VirtualLineTotal += item.VirtualLineTotal.HasValue ? item.VirtualLineTotal.Value : 0;
            this.AllTotal.RealTurnover += item.RealTurnover.HasValue ? item.RealTurnover.Value : 0;
            this.AllTotal.VirtualTurnover += item.VirtualTurnover.HasValue ? item.VirtualTurnover.Value : 0;
        }

        /// <summary>
        /// Calculates the temporary IBC total.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void CalculateTempIBCTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.TempIBCTotal.WinLoss += item.WinLoss;
            this.TempIBCTotal.Position += item.Position;
            this.TempIBCTotal.Commission += item.Commission;

            this.TempIBCTotal.RealLineTotal += item.RealLineTotal.HasValue ? item.RealLineTotal.Value : 0;
            this.TempIBCTotal.VirtualLineTotal += item.VirtualLineTotal.HasValue ? item.VirtualLineTotal.Value : 0;
            this.TempIBCTotal.RealTurnover += item.RealTurnover.HasValue ? item.RealTurnover.Value : 0;
            this.TempIBCTotal.VirtualTurnover += item.VirtualTurnover.HasValue ? item.VirtualTurnover.Value : 0;
        }
    }
}
