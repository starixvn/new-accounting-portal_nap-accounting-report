using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class SportWinLoss
    {
        public string SportName { get; set; }

        public List<LeagueWinLossItem> Leagues { get; set; }

        public LeagueWinLossItem Total { get; set; }
    }
}
