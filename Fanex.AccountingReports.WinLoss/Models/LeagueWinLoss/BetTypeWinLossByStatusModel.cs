using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BetTypeWinLossByStatusModel
    {
        public BetTypeWinLossLiveModel BetTypeWinLossLive { get; set; }

        public BetTypeWinLossNonLiveModel BetTypeWinLossNonLive { get; set; }
    }
}