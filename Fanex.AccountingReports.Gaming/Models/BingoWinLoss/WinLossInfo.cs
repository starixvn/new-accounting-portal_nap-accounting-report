// --------------------------------------------------------------------------
// <copyright file="WinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Models.BingoWinLoss
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

        public int Currency { get; set; }

        public string CurrencyName { get; set; }

        [ScriptIgnore]
        public decimal WinLoss { get; set; }

        [ScriptIgnore]
        public decimal Turnover { get; set; }

        [ScriptIgnore]
        public decimal Position { get; set; }

        [ScriptIgnore]
        public decimal Commission { get; set; }

        [ScriptIgnore]
        public decimal LineTotal { get; set; }

        public WinLossInfo Total { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        #region FieldToDisplay

        public string WinLoss_Str { get; set; }

        public string Turnover_Str { get; set; }

        public string Position_Str { get; set; }

        public string Commission_Str { get; set; }

        public string LineTotal_Str { get; set; }

        #endregion FieldToDisplay

        public void Add(WinLossInfo winLoss)
        {
            this.WinLoss += winLoss.WinLoss;
            this.Position += winLoss.Position;
            this.Commission += winLoss.Commission;
            this.LineTotal += winLoss.LineTotal;
            this.Turnover += winLoss.Turnover;
        }
    }
}
