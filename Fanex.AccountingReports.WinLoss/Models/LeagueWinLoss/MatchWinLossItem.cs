using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class MatchWinLossItem
    {
        public MatchWinLossByLiveStatus Total { get; set; }

        public List<MatchWinLossByLiveStatus> Matches { get; set; }
    }
}