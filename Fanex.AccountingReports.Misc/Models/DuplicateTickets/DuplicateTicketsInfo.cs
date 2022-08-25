namespace Fanex.AccountingReports.Misc.Models.DuplicateTickets
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class DuplicateTicketsInfo
    {
        public DuplicateTicketsInfo()
        {
            Rows = new List<DuplicateTicketsByUserName>();
            MD5Hash = string.Empty;
        }

        public List<DuplicateTicketsByUserName> Rows { get; set; }

        public string MD5Hash { get; set; }

        public bool HaveDuplicateTickets { get; set; }
    }
}