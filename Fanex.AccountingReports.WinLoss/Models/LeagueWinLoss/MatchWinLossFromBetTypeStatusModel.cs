using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class MatchWinLossFromBetTypeStatusModel
    {
        public List<MatchWinLossByLiveStatus> Match { get; set; }

        public MatchWinLossByLiveStatus Total { get; set; }

        public MatchWinLossByLiveStatus RM { get; set; }

        public MatchWinLossByLiveStatus BARM { get; set; }

        public MatchWinLossByLiveStatus GrandTotal { get; set; }
    }
}