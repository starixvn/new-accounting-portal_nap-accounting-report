namespace Fanex.AccountingReports.Special.Models
{
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RejectTickets
    {
        public List<RejectTicketDetail> RejectTicketList { get; set; }

        public int TotalChangedStatus { get; set; }
    }
}
