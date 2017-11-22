namespace Fanex.AccountingReports.Core.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.Data.Repository;

    public interface IMostVisitedPagesService
    {
        IEnumerable<MostVisitedPage> GetMostVisitedPages();

        void UpdateCount(MostVisitedPage mostVisitedPage);

        void UpdatePosition(ICriteria mostVisitedPage);

        void Delete(ICriteria mostVisitedPage);
    }
}
