namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    public class MatchEntity : BaseEntity
    {
        public int MatchID { get; set; }

        public string MatchCode { get; set; }

        public bool LiveIndicator { get; set; }

        public string MatchEvent { get; set; }

        public bool IsHedge { get; set; }
    }
}