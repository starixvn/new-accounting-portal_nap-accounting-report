namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LicenseeSiteService : ServiceBase
    {
        /// <summary>
        /// Gets the licensee site list.
        /// </summary>
        /// <returns>The licensee site list.</returns>
        public List<Licensee> GetLicenseeSiteList()
        {
            var licenseeSiteCriteria = new GetAllLicenseeCriteria();
            var licenseeSiteList = new List<Licensee>();
            var rawData = GetReportData<dynamic>(licenseeSiteCriteria);
            if (rawData != null)
            {
                foreach (var row in rawData)
                {
                    var licenseeSite = new Licensee()
                    {
                        LicenseeName = row.LicenseeName,
                        BrandName = row.Brandname,
                        LicenseeId = row.LicenseeID,
                        IsEnabledBA = row.EnableBA,
                        IsMultiAgent = AppSettings.MultiAgentLicensses.Contains(row.LicenseeName)
                    };

                    licenseeSiteList.Add(licenseeSite);
                }
            }

            return licenseeSiteList;
        }

        /// <summary>
        /// Gets the currencies by licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The currencies by licensee.</returns>
        public List<Currency> GetCurrenciesByLicensee(GetCurrenciesByLicenseeCriteria criteria)
        {
            var currencies = new List<Currency>();
            var rawData = GetReportData<dynamic>(criteria);
            rawData.ToList().ForEach(currency =>
            {
                currencies.Add(new Currency
                {
                    CurrencyId = currency.currency,
                    CurrencyName = currency.CurrencyName
                });
            });

            return currencies;
        }
    }
}