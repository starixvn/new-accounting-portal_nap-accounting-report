namespace Fanex.AccountingReports.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// <typeparam name="TDetail">The type of the detail.</typeparam>
    public class Report<TDetail> : EntityBase<int, string>
    {
        /// <summary>
        /// Gets or sets the childs.
        /// </summary>
        /// <value>
        /// The childs.
        /// </value>
        public IList<Report<TDetail>> Childs { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        public TDetail Details { get; set; }
    }
}
