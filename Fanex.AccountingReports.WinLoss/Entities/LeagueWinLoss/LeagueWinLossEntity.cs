using System;

namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    [Serializable]
    public class LeagueWinLossEntity : BaseEntity
    {
        public int SportType { get; set; }

        public string Sport { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public bool IsSabaSport { get; set; }
    }
}
