namespace Fanex.BetList.Accounting.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CombinationParlay
    {
        public IList<GroupParlayEvent> GroupParlayEvents { get; set; }

        public ParlayEvent Total { get; set; }
    }
}