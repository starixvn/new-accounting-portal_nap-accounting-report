using System.Collections.Generic;

namespace Fanex.AccountingReports.Misc.Constants
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class MiscConstants
    {
        public const int MiscSoccerGroupId = 1;
        public const int MiscParLayGroupId = -9;
        public const int MiscOtherSportId = 999;
        public const int MiscParLaySportId = -9;
        public const int MiscVirtualSportsId = 5;
        public const int MiscBetradarVirtualSportsId = 27;
        public const int MiscSportbookId = 1;
        public const int MiscNG2ProductID = 23;
        public const int BetBuilderProductID = -38;
        public const int MiscBasketballGroupId = 2;
        public const int MiscEsportsGroupId = 43;
        public const int SabaClubProductId = 42;
        public const int SBAdjustmentId = -999;
        public const int VGaming = 66;

        public static readonly string BetCountSessionKey = "Fanex.AccountingReports.Misc.BetCount.BetCountReport";
        public static readonly string BetCountHistoryReportSessionKey = "Fanex.AccountingReports.Misc.BetCount.HistoryBetCountReport";
        public static readonly string CreditSiteBetCountsSessionKey = "Fanex.AccountingReports.Misc.CreditSitesBetCounts.CreditSitesBetCountsReport";

        // Total SMA & Member Active
        public static readonly string TotalSMAMemberActiveSessionKey = "Fanex.AccountingReports.Misc.TotalSMAMemberActive.TotalSMAMemberActiveReport";

        public static readonly string TotalSMAMemberActiveDetailSessionKey = "Fanex.AccountingReports.Misc.TotalSMAMemberActive.TotalSMAMemberActiveDetailReport";
        public static readonly string DuplicateTicketsPrefixSessionKey = "Fanex.AccountingReports.Misc.DuplicateTickets.";
        public static readonly string MiscSportsSessionKey = "Fanex.AccountingReports.Misc.BetCount.Sports";

        public static List<string> ProductsHaveSports => new List<string>
        {
            "sportsbook",
            "virtual sports",
            "ng2",
            "virtual sports 2",
            "number game",
            "bet builder",
            "saba.club",
            "vgaming"
        };

        /// <summary>
        /// The one sheet can has only 65000 rows.
        /// </summary>
        public static readonly int MaxRowsData = 65000;

        /// <summary>
        /// Type of Rows in Report.
        /// </summary>
        public enum ReportRowType
        {
            /// <summary>
            /// The total.
            /// </summary>
            Total = 1,

            /// <summary>
            /// The grand total.
            /// </summary>
            GrandTotal = 2,

            /// <summary>
            /// The normal even row.
            /// </summary>
            NormalEvenRow = 3,

            /// <summary>
            /// The normal odd row.
            /// </summary>
            NormalOddRow = 4
        }
    }
}