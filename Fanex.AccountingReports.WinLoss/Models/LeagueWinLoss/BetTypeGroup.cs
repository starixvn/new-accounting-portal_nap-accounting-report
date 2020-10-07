using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BetTypeGroup
    {
        public string BetTypeGroupName { get; set; }

        public List<BetType> BetTypes { get; set; }
    }
}