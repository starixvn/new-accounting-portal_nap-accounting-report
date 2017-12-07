namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class MatchAnalysisView
    {
        public MatchAnalysisView()
        {
            Rows = new List<MatchAnalysis>();
            Total = new List<MatchAnalysis>();
        }

        public List<MatchAnalysis> Rows { get; set; }

        public List<MatchAnalysis> Total { get; set; }
    }
}
