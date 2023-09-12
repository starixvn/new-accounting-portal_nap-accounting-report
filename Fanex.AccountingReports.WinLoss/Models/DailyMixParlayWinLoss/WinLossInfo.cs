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
        public decimal MemberWinLoss { get; set; }

        [ScriptIgnore]
        public decimal SMAWinLoss { get; set; }

        [ScriptIgnore]
        public decimal Commission { get; set; }

        [ScriptIgnore]
        public decimal RealIncome { get; set; }

        [ScriptIgnore]
        public decimal RealTurnover { get; set; }

        [ScriptIgnore]
        public decimal Margin { get; set; }

        [ScriptIgnore]
        public decimal ExtraPT { get; set; }

        [ScriptIgnore]
        public decimal MemberBonus { get; set; }

        [ScriptIgnore]
        public decimal HouseBonus { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        public WinLossInfo GrandTotal { get; set; }
        #region FieldToDisplay

        public string Status { get; set; }

        public string Commission_Str { get; set; }

        public string RealIncome_Str { get; set; }

        public string MemberWinLoss_Str { get; set; }

        public string SMAWinLoss_Str { get; set; }

        public string MemberTurnover_Str { get; set; }

        public string RealTurnover_Str { get; set; }

        public string BetCount_Str { get; set; }

        public string Margin_Str { get; set; }

        public string ExtraPT_Str { get; set; }

        public string MemberBonus_Str { get; set; }

        public string HouseBonus_Str { get; set; }

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
            this.MemberWinLoss += winLoss.MemberWinLoss;
            this.SMAWinLoss += winLoss.SMAWinLoss;
            this.Commission += winLoss.Commission;
            this.RealIncome += winLoss.RealIncome;
            this.RealTurnover += winLoss.RealTurnover;
            this.ExtraPT += winLoss.ExtraPT;
            this.MemberBonus += winLoss.MemberBonus;
            this.HouseBonus += winLoss.HouseBonus;
        }
    }
}
