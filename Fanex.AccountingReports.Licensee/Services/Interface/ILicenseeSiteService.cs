namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILicenseeSiteService
    {
        List<Licensee> GetLicenseeSiteList();

        List<Currency> GetCurrenciesByLicensee(GetCurrenciesByLicenseeCriteria criteria);
    }
}
