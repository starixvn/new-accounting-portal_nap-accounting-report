// -----------------------------------------------------------------------
// <copyright file="ICompanyWinLossService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Licensee.Criteria.CompanyWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.Data.Repository;
    using Fanex.Excel.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICompanyWinLossService
    {
        List<Currency> GetCurrenciesByLicensee(ICriteria criteria);

        List<Product> GetProductListByLicense(ICriteria criteria);

        GridOptions GetCompanyWinLoss(CompanyWinLossCriteria criteria);

        GridOptions GetFullReportFromCache(CompanyWinLossCriteria criteria);

        string GetReportName(CompanyWinLossCriteria criteria);
    }
}