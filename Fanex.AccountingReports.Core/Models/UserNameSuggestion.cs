namespace Fanex.AccountingReports.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserNameSuggestion
    {
        public string QueryValue { get; set; }

        // JQuery Plugin user suggestions field name to get data back
        public List<string> Suggestions { get; set; }

        public string[] DataInfo { get; set; }
    }
}
