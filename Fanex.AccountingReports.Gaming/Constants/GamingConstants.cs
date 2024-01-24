namespace Fanex.AccountingReports.Gaming.Constants
{
    using Fanex.AccountingReports.Core.Web;

    public static class GamingConstants
    {
        public static readonly string NumberGameWinLossByDateSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByDate";

        public static readonly string NumberGameWinLossByCurrencyMainSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencyMain";

        public static readonly string NumberGameWinLossByCurrencySuperSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencySuper";

        public static readonly string NumberGameWinLossByCurrencyMasterSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencyMaster";

        public static readonly string NumberGameWinLossByCurrencyAgentSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencyAgent";

        public static readonly string NumberGameWinLossByCUrrencyMemberSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencyMember";

        public static readonly string BingoWinLossReportSessionKey = "Fanex.AccountingReports.Gaming.BingoWinLoss.Report";

        public static readonly string LiveCasinoGameFinancialReportSessionKey = "Fanex.AccountingReports.Gaming.LiveCasinoGames.FinancialReport";

        public static readonly string BingoStatisticReportSessionKey = "Fanex.AccountingReports.Gaming.BingoStatistic.Report";

        public static readonly string BingoOutstandingReportSessionKey = "Fanex.AccountingReports.Gaming.BingoOutStanding.Report";

        public static readonly int MaxRowsData = 65000;

        public static readonly string NumberGameStatisticReportByLeagueCachedDataKey = "Fanex.AccountingReports.Gaming.NumberGameStatisticReportByLeague." +
         (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public static readonly string NumberGameStatisticReportByDailyCachedDataKey = "Fanex.AccountingReports.Gaming.NumberGameStatisticReportByDaily." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public static readonly string NumberGameStatisticReportByUserNameCachedDataKey = "Fanex.AccountingReports.Gaming.NumberGameStatisticReportByUserName." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public static readonly string NumberGameStatisticReportByBettypeCachedDataKey = "Fanex.AccountingReports.Gaming.NumberGameStatisticReportByBettype." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public enum ReportRowType
        {
            Total = 1,

            GrandTotal = 2,

            HeaderSite = 3,

            EvenRow = 4,

            OddRow = 5
        }
    }
}
