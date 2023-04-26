namespace Fanex.AccountingReports.Misc.Models.DuplicateTickets
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class TicketInfo
    {
        public string Status { get; set; }

        public string ReferenceNumber { get; set; }

        public string BetTime { get; set; }

        public double TimeDifference { get; set; }

        public int GroupNo { get; set; }

        public string TransDesc { get; set; }

        public string Site { get; set; }

        public string BetFrom { get; set; }
    }
}