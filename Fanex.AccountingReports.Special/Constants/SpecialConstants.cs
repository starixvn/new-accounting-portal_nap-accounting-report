using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Constants
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class SpecialConstants
    {
        public static readonly string ParlayWonFullReportSessionKey = "Fanex.AccountingReports.Special.ParlayWon.ParlayWonReport";
        public static readonly string MatchAnalysisSessionKey = "Fanex.AccountingReports.Special.MatchAnalysis";

        public static readonly string IntNumberFormat = "#,##0";

        public static readonly short FontSize = 8;

        public static readonly string FontName = "Tahoma";

        public static readonly List<string> ProductsHaveSport = new List<string>
        {
            "sportsbook",
            "ng2"
        };
    }
}
