using System;

namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    [Serializable]
    public class LeagueWinLossEntity : BaseEntity
    {
        public int SportType { get; set; }

        public string Sport { get; set; }

        public string LeagueId { get; set; }

        public string LeagueName { get; set; }

        public int IsSabaSport { get; set; }
    }
}
