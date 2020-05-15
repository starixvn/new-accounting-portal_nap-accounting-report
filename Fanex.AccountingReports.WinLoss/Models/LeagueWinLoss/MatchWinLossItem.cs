using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class MatchWinLossItem : BaseWinLoss
    {
        public string MatchCode { get; set; }

        public string MatchEvent { get; set; }
    }
}
