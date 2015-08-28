// --------------------------------------------------------------------------
// <copyright file="WinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2013.02.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Models.WinLossSummary
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [ScriptIgnore]
        public decimal WinLoss { get; set; }

        [ScriptIgnore]
        public decimal Position { get; set; }

        [ScriptIgnore]
        public decimal Commission { get; set; }

        [ScriptIgnore]
        public decimal LineTotal { get; set; }

        public int LiveType { get; set; }

        public string Status { get; set; }

        public int IBC { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        //// FieldToDisplay

        public string Commission_Str { get; set; }

        public string LineTotal_Str { get; set; }

        public string WinLoss_Str { get; set; }

        public string Position_Str { get; set; }

        //// FieldToDisplay

        /// <summary>
        /// Adds the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void Add(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            this.WinLoss += winLoss.WinLoss;
            this.Position += winLoss.Position;
            this.Commission += winLoss.Commission;
            this.LineTotal += winLoss.LineTotal;
        }
    }
}
