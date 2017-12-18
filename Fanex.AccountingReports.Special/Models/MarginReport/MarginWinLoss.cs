namespace Fanex.AccountingReports.Special.Models.MarginReport
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class MarginWinLoss
    {
        public MarginWinLoss()
        {
            this.ChildsInfo = new List<MarginWinLoss>();
        }

        public string UserName { get; set; }

        public int BettypeId { get; set; }

        public string BettypeName { get; set; }

        public decimal WinningAmount { get; set; }

        public decimal Turnover { get; set; }

        public decimal Ratio { get; set; }

        public int TotalBet { get; set; }

        public List<MarginWinLoss> ChildsInfo { get; set; }
    }
}