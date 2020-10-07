using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class SuperWinLossModel
    {
        public List<SuperWinLossItem> Supers { get; set; }

        public SuperWinLossItem Total { get; set; }

        public SuperWinLossItem RM { get; set; }

        public SuperWinLossItem BARM { get; set; }

        public SuperWinLossItem GrandTotal { get; set; }
    }
}