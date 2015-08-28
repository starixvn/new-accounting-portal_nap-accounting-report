// -----------------------------------------------------------------------
// <copyright file="RacingGetListLicenseeService.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement.RacingWinLossByLicensee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Racing.Core.Configurations;
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee;
    using Fanex.Common;

    /// <summary>
    /// Get list licensee service.
    /// </summary>
    public class RacingGetListLicenseeService : ServiceBase, IRacingGetListLicenseeService
    {
        /// <summary>
        /// Get list licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list licensee's name.</returns>
        public List<RacingLicensee> GetListLicensee(GetLicenseeCriteria criteria)
        {
            List<RacingLicensee> licensee = new List<RacingLicensee>();
            try
            {
                var rowsData = GetReportData<dynamic>(criteria);
                var listSites = RacingSection.Instance.PtSites.Split(',');

                foreach (var row in rowsData)
                {
                    string licenseeName = Converter.ToString2(row.LicenseeName);
                    var racingLicensee = new RacingLicensee();
                    racingLicensee.ID = Converter.ToInt(row.LicenseeID);
                    racingLicensee.Name = licenseeName;
                    racingLicensee.HasPositoinTaking = CheckContainSite(listSites, licenseeName);

                    licensee.Add(racingLicensee);
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            return licensee;
        }

        /// <summary>
        /// Check if licensee name contain in list site section.
        /// </summary>
        /// <param name="listSites">The list site in Racing Section.</param>
        /// <param name="licenseeName">The licensee.</param>
        /// <returns>The boolean.</returns>
        private bool CheckContainSite(string[] listSites, string licenseeName)
        {
            var numberSites = listSites.Count();

            for (var i = 0; i < numberSites; i++)
            {
                string site = listSites[i].ToLower();
                
                if (site.Equals(licenseeName.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
