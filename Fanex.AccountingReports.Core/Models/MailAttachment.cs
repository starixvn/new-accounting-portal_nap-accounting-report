namespace Fanex.AccountingReports.Core.Models
{
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MailAttachment
    {
        public MemoryStream Content { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }
    }
}
