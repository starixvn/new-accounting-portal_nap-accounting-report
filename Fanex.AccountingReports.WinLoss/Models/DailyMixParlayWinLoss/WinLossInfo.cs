namespace Fanex.AccountingReports.WinLoss.Models.DailyMixParlayWinLoss
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
        public string Name { get; set; }

        public int Id { get; set; }

        [ScriptIgnore]
        public int BetCount { get; set; }

        [ScriptIgnore]
        public decimal MemberTurnover { get; set; }

        [ScriptIgnore]
        public decimal WinLoss { get; set; }

        [ScriptIgnore]
        public decimal Position { get; set; }

        [ScriptIgnore]
        public decimal Commission { get; set; }

        [ScriptIgnore]
        public decimal LineTotal { get; set; }

        [ScriptIgnore]
        public decimal Turnover { get; set; }

        [ScriptIgnore]
        public decimal Margin { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        public WinLossInfo GrandTotal { get; set; }
        #region FieldToDisplay

        public string Status { get; set; }

        public string Commission_Str { get; set; }

        public string LineTotal_Str { get; set; }

        public string WinLoss_Str { get; set; }

        public string Position_Str { get; set; }

        public string MemberTurnover_Str { get; set; }

        public string Turnover_Str { get; set; }

        public string BetCount_Str { get; set; }

        public string Margin_Str { get; set; }

        public int TotalRowDetail { get; set; }

        #endregion FieldToDisplay

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

            this.BetCount += winLoss.BetCount;
            this.MemberTurnover += winLoss.MemberTurnover;
            this.WinLoss += winLoss.WinLoss;
            this.Position += winLoss.Position;
            this.Commission += winLoss.Commission;
            this.LineTotal += winLoss.LineTotal;
            this.Turnover += winLoss.Turnover;
        }
    }
}
