namespace Fanex.AccountingReports.Special.Models
{
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NormalTickets
    {
        public List<NormalTicketDetail> NormalTicketList { get; set; }

        public int TotalChangedStake { get; set; }

        public int TotalChangedOdds { get; set; }

        public int TotalChangedBetteam { get; set; }

        public int TotalChangedPT { get; set; }
    }
}
