using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BettypeWinLossItem : BaseWinLoss
    {
        public bool LiveIndicator { get; set; }

        public int Bettype { get; set; }

        public string BettypeName { get; set; }

        public bool IsHedge { get; set; }
    }
}
