using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class LeagueWinLossModel
    {
        public List<SportWinLoss> SportWinLoss { get; set; }

        public LeagueWinLossItem Total { get; set; }

        public LeagueWinLossItem SoccerRM { get; set; }

        public LeagueWinLossItem OtherSportRM { get; set; }

        public LeagueWinLossItem SoccerBARM { get; set; }

        public LeagueWinLossItem OtherSportBARM { get; set; }

        public LeagueWinLossItem GrandTotal { get; set; }
    }
}
