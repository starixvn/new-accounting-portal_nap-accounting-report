// -----------------------------------------------------------------------
// <copyright file="MatchAnalysisView.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.12.26">Created</change>
// </history>

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
            Total = new MatchAnalysis();
        }

        public List<MatchAnalysis> Rows { get; set; }

        public MatchAnalysis Total { get; set; }
    }
}
