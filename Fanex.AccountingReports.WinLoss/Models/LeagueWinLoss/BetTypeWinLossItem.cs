using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BetTypeWinLossItem
    {
        public BettypeWinLossBySite Total { get; set; }

        public List<BettypeWinLossBySite> BetTypes { get; set; }
    }
}