// --------------------------------------------------------------------------
// <copyright file="LicenseeSiteService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.16">Created</change>
//   <change who="Louis.Bao" date="2012.11.26">Modified</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.PlaytechCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Services.Interface;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LicenseeSiteService : ServiceBase, ILicenseeSiteService
    {
        /// <summary>
        /// Gets the licensee site list.
        /// </summary>
        /// <returns>The licensee site list.</returns>
        public List<Licensee> GetLicenseeSiteList()
        {
            var licenseeSiteCriteria = new GetAllLicenseeCriteria();
            var licenseeSiteList = new List<Licensee>();
            try
            {
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
            }
            catch (Exception ex)
            {
                HandleException(ex);
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
            try
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
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets the licensee site list.
        /// </summary>
        /// <returns>The licensee site list.</returns>
        public List<Licensee> GetPlaytechCasino()
        {
            var licenseeSiteCriteria = new GetPlaytechCasinoLicenseeCriteria();
            var licenseeSiteList = new List<Licensee>();
            try
            {
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
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return licenseeSiteList;
        }
    }
}
