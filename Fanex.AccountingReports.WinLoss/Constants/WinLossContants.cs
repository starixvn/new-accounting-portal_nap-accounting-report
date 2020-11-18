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
        public static readonly string LeagueWinLossViewMatchesSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewMatches";
        public static readonly string LeagueWinLossViewMatchesFromBetTypeStatusSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewMatchesFromBetTypeStatus";
        public static readonly string LeagueWinLosViewBetTypesSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypes";
        public static readonly string LeagueWinLosViewBetTypesByStatusSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypesByStatus";
        public static readonly string LeagueWinLosViewSitesSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewSites";
        public static readonly string LeagueWinLosViewSupersSessionKey = "Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewSupers";

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