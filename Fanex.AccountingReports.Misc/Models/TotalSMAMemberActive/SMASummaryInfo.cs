namespace Fanex.AccountingReports.Misc.Models.TotalSMAMemberActive
{
    using System;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class SMASummaryInfo
    {
        /// <summary>
        /// Gets or sets status of SMA: New Created/Active/Suspended/Closed.
        /// </summary>
        /// <value>
        /// The Status of SMA.
        /// </value>
        public EntityBase<int, string> Key { get; set; }

        public int Super { get; set; }

        public int Master { get; set; }

        public int Agent { get; set; }
    }
}
