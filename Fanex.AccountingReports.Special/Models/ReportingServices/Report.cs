namespace Fanex.AccountingReports.Special.Models.ReportingServices
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Reporting services model.
    /// </summary>
    public class Report
    {
        public Report()
        {
            Files = new List<Report>();
            Folders = new List<Report>();
        }

        public string Path { get; set; }

        public string FileName { get; set; }

        public string FolderName { get; set; }

        public string ParentFolder { get; set; }

        public string ParentFolderName { get; set; }

        public List<Report> Files { get; set; }

        public List<Report> Folders { get; set; }
    }
}
