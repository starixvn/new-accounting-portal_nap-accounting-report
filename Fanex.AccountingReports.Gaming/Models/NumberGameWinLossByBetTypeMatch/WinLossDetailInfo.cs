namespace Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch
{
    using System;

    [Serializable]
    public class WinLossDetailInfo : WinLossInfo
    {
        public string MatchCode { get; set; }

        public int MatchId { get; set; }

        public int SportType { get; set; }

        public int LeagueId { get; set; }

        public int BetType { get; set; }
    }
}
