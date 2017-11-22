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
    public class TicketInfo
    {
        public string Status { get; set; }

        public long ReferenceNumber { get; set; }

        public string BetTime { get; set; }

        public double TimeDifference { get; set; }

        public int GroupNo { get; set; }
    }
}
