namespace Fanex.AccountingReports.Core.Models
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MailItem
    {
        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Title { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public IList<MailAttachment> Attachments { get; set; }

        public AlternateView AltView { get; set; }
    }
}
