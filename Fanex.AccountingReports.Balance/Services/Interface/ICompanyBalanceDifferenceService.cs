namespace Fanex.AccountingReports.Balance.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Balance.Models.CompanyBalanceDifference;

    public interface ICompanyBalanceDifferenceService
    {
        List<DifferenceInfo> GetCompanyBalanceDifference();
    }
}
