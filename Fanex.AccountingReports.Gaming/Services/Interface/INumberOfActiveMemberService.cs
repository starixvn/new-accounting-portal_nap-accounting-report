namespace Fanex.AccountingReports.Gaming.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember;
    using Fanex.AccountingReports.Gaming.Models.NumberOfActiveMember;
    using Fanex.Excel.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface INumberOfActiveMemberService
    {
        List<Product> GetProduct();

        GridOptions GetReport(GetNumberOfActiveMemberCriteria criteria);

        GridOptions SearchNumberOfActiveMemberByProduct(SearchNumberActiveMemberCriteria criteria);

        GridOptions GetReportFromCache(GetNumberOfActiveMemberCriteria criteria);
    }
}
