using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BetTypeWinLossLiveGroupModel
    {
        public List<BettypeWinLossBySite> BettypeWinLoss { get; set; }

        public BettypeWinLossBySite Total { get; set; }
    }
}
