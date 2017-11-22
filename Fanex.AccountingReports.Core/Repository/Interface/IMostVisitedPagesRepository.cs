namespace Fanex.AccountingReports.Core.Repository.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.Data.Repository;

    public interface IMostVisitedPagesRepository
    {
        IEnumerable<MostVisitedPage> FetchMostVisitedPages(SelectMostVisitedPageCriteria criteria);

        void UpdateCount(ICriteria mostVisitedPage);

        void UpdatePosition(ICriteria mostVisitedPage);

        void Delete(ICriteria mostVisitedPage);
    }
}
