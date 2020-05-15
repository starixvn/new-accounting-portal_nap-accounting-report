using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class LeagueWinLossItem : BaseWinLoss
    {
        public int SportType { get; set; }

        public string Sport { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }
    }
}
