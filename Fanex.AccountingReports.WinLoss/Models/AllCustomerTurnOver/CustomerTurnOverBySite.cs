namespace Fanex.AccountingReports.WinLoss.Models.AllCustomerTurnOver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Customer turn over by site - detail.
    /// </summary>
    [Serializable]
    public class CustomerTurnoverBySite
    {
        public CustomerTurnoverBySite()
        {
            Site = new CustomerTurnoverBySiteDetail();
        }

        public CustomerTurnoverBySiteDetail Site { get; set; }

        /// <summary>
        /// Gets or sets the total of users of all sites.
        /// </summary>
        /// <value>The total of users of all sites.</value>
        public int TotalUsers { get; set; }
    }
}
