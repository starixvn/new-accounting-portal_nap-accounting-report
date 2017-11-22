namespace Fanex.AccountingReports.Core.Repository.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;

    public interface IReportHelperRepository
    {
        List<dynamic> UserNameSuggestion(UserNameSuggestionCriteria criteria);

        List<dynamic> GetCurrencyByLicensee(GetCurrencyByLicenseeCriteria criteria);

        List<dynamic> GetLicensee_SupportByProduct(GetLicensee_SupportByProductCriteria criteria);

        DWHDateRange GetDWHDateRange(GetDWHDateRangeCriteria criteria);
    }
}
