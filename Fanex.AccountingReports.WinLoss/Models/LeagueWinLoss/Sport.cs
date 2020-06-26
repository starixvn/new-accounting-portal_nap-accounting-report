using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class Sport
    {
        public int SportType { get; set; }

        public string SportName { get; set; }

        public List<BetType> Bettypes { get; set; }
    }
}
