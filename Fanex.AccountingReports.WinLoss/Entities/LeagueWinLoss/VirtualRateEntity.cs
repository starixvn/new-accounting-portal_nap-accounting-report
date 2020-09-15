namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    public class VirtualRateEntity
    {
        public string Sport { get; set; }

        public string BetType { get; set; }

        public string LiveStatus { get; set; }

        public decimal VirtualRate { get; set; }
    }
}