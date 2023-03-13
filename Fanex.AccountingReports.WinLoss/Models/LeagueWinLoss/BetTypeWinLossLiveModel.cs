using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BetTypeWinLossLiveModel
    {
        public List<BetTypeWinLossLiveGroupModel> BetTypeWinLossLiveGroups { get; set; }

        public BettypeWinLossBySite Total { get; set; }

        public BettypeWinLossBySite RM { get; set; }

        public BettypeWinLossBySite BARM { get; set; }

        public BettypeWinLossBySite GrandTotal { get; set; }
    }
}
