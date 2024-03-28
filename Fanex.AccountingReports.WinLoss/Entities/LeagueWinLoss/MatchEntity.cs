namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    public class MatchEntity : BaseEntity
    {
        public int MatchID { get; set; }

        public string MatchCode { get; set; }

        public bool LiveIndicator { get; set; }

        public string MatchEvent { get; set; }

        public bool IsHedge { get; set; }

        public bool IsOutright { get; set; }

        public string BetTypeName { get; set; }

        public string Choice { get; set; }
    }
}