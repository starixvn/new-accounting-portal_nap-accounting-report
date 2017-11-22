namespace Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossDetailInfo : WinLossInfo
    {
        public string MatchCode { get; set; }

        public int MatchId { get; set; }

        public bool LiveIndicator { get; set; }

        public int LeagueId { get; set; }

        public int BetType { get; set; }
    }
}
