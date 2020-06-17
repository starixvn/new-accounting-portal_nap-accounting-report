namespace Fanex.AccountingReports.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeagueSuggestion
    {
        public string QueryValue { get; set; }

        // JQuery Plugin user suggestions field name to get data back
        public List<LeagueModel> Suggestions { get; set; }

        public List<LeagueModel> DataInfo { get; set; }
    }
}
