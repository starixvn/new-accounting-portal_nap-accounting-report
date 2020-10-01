using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class MatchWinLossByLiveStatus : BaseWinLoss
    {
        public int MatchId { get; set; }

        public bool LiveIndicator { get; set; }

        public string MatchEvent { get; set; }

        public bool IsHedge { get; set; }
    }
}