using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class MatchWinLossModel
    {
        public List<MatchWinLossItem> Match { get; set; }

        public MatchWinLossItem Total { get; set; }

        public MatchWinLossItem RM { get; set; }

        public MatchWinLossItem BARM { get; set; }

        public MatchWinLossItem GrandTotal { get; set; }
    }
}
