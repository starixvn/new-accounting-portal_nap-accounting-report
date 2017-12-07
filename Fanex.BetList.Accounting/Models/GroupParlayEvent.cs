namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GroupParlayEvent
    {
        public DateTime WinLossDate { get; set; }

        public IList<ParlayEvent> ParlayEvents { get; set; }

        public ParlayEvent SubTotal { get; set; }
    }
}
