// -----------------------------------------------------------------------
// <copyright file="DuplicateTicketsInfo.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Misc.Models.DuplicateTickets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
    }
}
