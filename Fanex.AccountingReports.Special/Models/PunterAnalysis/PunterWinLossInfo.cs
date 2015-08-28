// --------------------------------------------------------------------------
// <copyright file="PunterWinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.PunterAnalysis
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Special.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class PunterWinLossInfo
    {
        public PunterWinLossInfo()
        {
            this.WinLoss_Str = string.Empty;
            this.Turnover_Str = string.Empty;
            this.BetCount_Str = string.Empty;
            this.WinCount_Str = string.Empty;
            this.DrawCount_Str = string.Empty;
            this.Margin_Str = string.Empty;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Turnover { get; set; }

        public decimal Margin { get; set; }

        public int BetCount { get; set; }

        public int WinCount { get; set; }

        public int DrawCount { get; set; }

        public int LiveType { get; set; }

        public List<PunterWinLossInfo> ChildsInfo { get; set; }

        public PunterWinLossInfo GrandTotal { get; set; }

        //// FieldToDisplay

        public string WinLossDate_Str { get; set; }

        public string WinLoss_Str { get; set; }

        public string Turnover_Str { get; set; }

        public string BetCount_Str { get; set; }

        public string WinCount_Str { get; set; }

        public string DrawCount_Str { get; set; }

        public string Margin_Str { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string IP { get; set; }

        public string Country { get; set; }

        public bool HasData { get; set; }

        //// FieldToDisplay

        /// <summary>
        /// Formats the punter win loss information.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The formatted win loss.</returns>
        public static PunterWinLossInfo Format(PunterWinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.WinLoss_Str = winLoss.WinLoss.Format(2);
            winLoss.Turnover_Str = winLoss.Turnover.Format(2);
            winLoss.BetCount_Str = winLoss.BetCount.Format(0);
            winLoss.WinCount_Str = winLoss.WinCount.Format(0);
            winLoss.DrawCount_Str = winLoss.DrawCount.Format(0);

            if (winLoss.Turnover != 0)
            {
                winLoss.Margin_Str = (winLoss.Margin * 100).Format(2) + Labels.Symbol_Percent;
            }
            else
            {
                winLoss.Margin_Str = Labels.NotAvailable;
            }

            return winLoss;
        }

        /// <summary>
        /// Adds the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void Add(PunterWinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            this.WinLoss += winLoss.WinLoss;
            this.Turnover += winLoss.Turnover;
            this.BetCount += winLoss.BetCount;
            this.WinCount += winLoss.WinCount;
            this.DrawCount += winLoss.DrawCount;
        }
    }
}
