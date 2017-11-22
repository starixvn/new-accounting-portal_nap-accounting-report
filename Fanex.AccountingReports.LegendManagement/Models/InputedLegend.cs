namespace Fanex.AccountingReports.LegendManagement.Models
{
    /// <summary>
    /// Inputted Legend.
    /// </summary>
    public class InputedLegend
    {
        public string System { get; set; }

        public int ReportId { get; set; }

        public string ReportName { get; set; }

        public string Content { get; set; }
    }
}
