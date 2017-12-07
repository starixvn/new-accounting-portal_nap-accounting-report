namespace Fanex.AccountingReports.Gaming.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria;
    using Fanex.AccountingReports.Gaming.Models;

    public interface IGamingService
    {
        List<EntityBase<int, string>> GetLicensee_SupportBingo();

        CompanyInfo GetNumberGameCompanyList(GetNumberGameCompanyListCriteria criteria);

        CompanyInfo GetLicenseesByProduct(GetLicenseesByProductCriteria criteria);

        List<EntityBase<int, string>> GetNumberGameBetTypeList(GetNumberGameBetTypeListCriteria criteria);

        CompanyInfo GetLiveCasinoLicenseeList(GetLiveCasinoGameCompanyListCriteria criteria);
    }
}
