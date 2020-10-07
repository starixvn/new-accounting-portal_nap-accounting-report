namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    public class VirtualRateModel
    {
        public string Sport { get; set; }

        public string BetType { get; set; }

        public string LiveStatus { get; set; }

        public decimal VirtualRate { get; set; }
    }
}