namespace Fanex.AccountingReports.Core.Criteria
{
    public interface IPagingCriteria
    {
        int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        int PageIndex { get; set; }

        // StartIndex = PageIndex * PageSize;
        // EndIndex = (PageIndex + 1) * PageSize - 1;
    }
}