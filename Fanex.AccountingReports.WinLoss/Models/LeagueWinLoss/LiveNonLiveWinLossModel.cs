using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class LiveNonLiveWinLossModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public BettypeWinLossModel BetTypeWinLoss { get; set; }
    }
}
