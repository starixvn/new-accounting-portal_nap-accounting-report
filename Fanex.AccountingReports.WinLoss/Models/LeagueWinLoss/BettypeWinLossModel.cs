using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BettypeWinLossModel
    {
        public List<BettypeWinLossItem> BettypeWinLoss { get; set; }

        public BettypeWinLossItem Total { get; set; }

        public BettypeWinLossItem RM { get; set; }

        public BettypeWinLossItem BARM { get; set; }

        public BettypeWinLossItem GrandTotal { get; set; }
    }
}
