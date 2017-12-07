namespace Fanex.AccountingReports.Core.Models
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class MostVisitedPage
    {
        public MostVisitedPage()
        {
            Count = 1;
        }

        public string Id { get; set; }

        public int UserId { get; set; }

        public int MenuId { get; set; }

        public string UserName { get; set; }

        public DateTime LastVisitDay { get; set; }

        public string MenuName { get; set; }

        public string MenuURL { get; set; }

        public string Title { get; set; }

        public ulong Count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is pinned].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is pinned]; otherwise, <c>false</c>.
        /// </value>
        public bool IsPinned { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position { get; set; }
    }
}
