namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MailParam
    {
        public MailParam()
        {
            Mails = new List<NegativeBalanceMemberMailFormat>();
        }

        public List<NegativeBalanceMemberMailFormat> Mails { get; set; }

        public string Subject { get; set; }

        public string MailContent { get; set; }

        public string ReportTitle { get; set; }
    }
}