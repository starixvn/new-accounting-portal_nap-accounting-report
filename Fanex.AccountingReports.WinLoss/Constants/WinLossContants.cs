namespace Fanex.AccountingReports.WinLoss.Constants
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossContants
    {
        public const int DailyMixParlayNormalSoccerSportId = 1;
        public const int DailyMixParlayNormalOtherSportsId = 2;
        public const int DailyMixParlayNormalMultiSportId = 3;

        public static readonly string LeagueWinLossSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLoss";
        public static readonly string LeagueWinLossBySportSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossBySport";
        public static readonly string LeagueWinLossByBetTypeSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossByBetType";

        public static readonly string DailyMixParlayReportCombinationPrefix = "m";
        public static readonly string DailyMixParlayReportMaxCombination = "m6";
        public static readonly string DailyMixParlayCombinationReport = "Fanex.AccountingReports.WinLoss.DailyMixParlay.CombinationReport";
        public static readonly string DailyMixParlayAdvanceMixParlayReport = "Fanex.AccountingReports.WinLoss.DailyMixParlay.AdvanceMixParlayReport";
        public static readonly string DailyMixParlayNormalMixParlayReport = "Fanex.AccountingReports.WinLoss.DailyMixParlay.NormalMixParlayReport";

        public enum ReportRowType
        {
            /// <summary>
            /// The total.
            /// </summary>
            Total = 1,

            /// <summary>
            /// The GrandTotal.
            /// </summary>
            GrandTotal = 2,

            /// <summary>
            /// The HeaderSite.
            /// </summary>
            HeaderSite = 3,

            /// <summary>
            /// The EvenRow.
            /// </summary>
            EvenRow = 4,

            /// <summary>
            /// The OddRow.
            /// </summary>
            OddRow = 5
        }      
    }
}
