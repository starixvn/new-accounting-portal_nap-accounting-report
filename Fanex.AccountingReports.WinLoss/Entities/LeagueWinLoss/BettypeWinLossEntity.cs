namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    public class BettypeWinLossEntity : BaseEntity
    {
        public int Bettype { get; set; }

        public string BettypeName { get; set; }

        public string SiteType { get; set; }

        public bool IsHedge { get; set; }
    }
}