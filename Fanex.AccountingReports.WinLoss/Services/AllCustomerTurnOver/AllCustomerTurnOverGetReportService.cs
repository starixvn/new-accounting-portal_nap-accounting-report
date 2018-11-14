namespace Fanex.AccountingReports.WinLoss.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Criteria.GetAllCustomerTurnOver;
    using Fanex.AccountingReports.WinLoss.Models.AllCustomerTurnOver;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AllCustomerTurnOverGetReportService : ServiceBase
    {
        private const string PagingSessionKeyConstant = "Fanex.AccountingReports.WinLoss.AllCustomerTurnOver.PagingReport.";

        private const string AllCustomerSessionKeyConstant = "Fanex.AccountingReports.WinLoss.AllCustomerTurnOver.";

        private const string SiteDefault = "All";

        private const string FromDefault = "i,p";

        private string _allCustomerTurnOverSessionKey;

        public AllCustomerTurnOverGetReportService()
        {
            this._allCustomerTurnOverSessionKey = AllCustomerSessionKeyConstant + AppSession.CurrentUser.Username;
        }

        /// <summary>
        /// Get Report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public CustomerTurnoverBySite GetReport(GetAllCustomerTurnOverCriteria criteria)
        {
            try
            {
                CustomerTurnoverBySite fullReport = null;
                CustomerTurnoverBySite pagingReport = null;

                var startIndex = criteria.StartIndex;
                var usersPerPage = criteria.UsersPerPage;

                if (!criteria.IsPagingCallBack)
                {
                    fullReport = this.ConvertReportData(criteria);
                    this.ClearAllPagingSessionKey();
                    SessionManager.Set<CustomerTurnoverBySite>(this._allCustomerTurnOverSessionKey, fullReport);
                }
                else
                {
                    fullReport = SessionManager.Get<CustomerTurnoverBySite>(this._allCustomerTurnOverSessionKey);
                }

                if (fullReport != null && fullReport.Site != null &&
                        fullReport.Site.Rows != null && fullReport.Site.Rows.Count > 0)
                {
                    string pagingSessionKey = PagingSessionKeyConstant + AppSession.CurrentUser.Username
                            + "." + startIndex + "." + usersPerPage
                            + "." + criteria.FromDate + "." + criteria.ToDate
                            + "." + criteria.Site + "." + criteria.From;
                    pagingReport = this.GetPagingReport(fullReport, startIndex, usersPerPage, criteria.IsPagingCallBack, pagingSessionKey);
                }

                return pagingReport;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Clear all paging session key when user generate new report.
        /// </summary>
        private void ClearAllPagingSessionKey()
        {
            for (int i = 0; i < HttpContext.Current.Session.Count; i++)
            {
                if (HttpContext.Current.Session.Keys[i].StartsWith(PagingSessionKeyConstant))
                {
                    HttpContext.Current.Session.Remove(HttpContext.Current.Session.Keys[i]);
                    i--;
                }
            }
        }

        /// <summary>
        /// Get Paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="usersPerPage">The users per page.</param>
        /// <param name="isPagingCallBack">Is paging call back.</param>
        /// <param name="pagingSessionKey">The paging session key.</param>
        /// <returns>The paging report.</returns>
        private CustomerTurnoverBySite GetPagingReport(
           CustomerTurnoverBySite fullReport,
           int startIndex,
           int usersPerPage,
           bool isPagingCallBack,
           string pagingSessionKey)
        {
            CustomerTurnoverBySite pagingReport = null;
            pagingReport = SessionManager.Get<CustomerTurnoverBySite>(pagingSessionKey);

            if (pagingReport == null || !isPagingCallBack)
            {
                pagingReport = this.GetPagingReportDetail(fullReport, startIndex, usersPerPage, pagingSessionKey);
            }

            return pagingReport;
        }

        /// <summary>
        /// Get paging report detail.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="usersPerPage">The users per page.</param>
        /// <param name="pagingSessionKey">The paging session key.</param>
        /// <returns>The paging report.</returns>
        private CustomerTurnoverBySite GetPagingReportDetail(
            CustomerTurnoverBySite fullReport,
            int startIndex,
            int usersPerPage,
            string pagingSessionKey)
        {
            var pagingReport = new CustomerTurnoverBySite
            {
                Site = new CustomerTurnoverBySiteDetail
                {
                    Rows = new List<CustomerTurnoverBySiteDetail>(),
                    Turnover = fullReport.Site.Turnover
                },
                TotalUsers = fullReport.TotalUsers
            };

            this.LoopAndGetPagingReport(fullReport, startIndex, usersPerPage, ref pagingReport);

            SessionManager.Set<CustomerTurnoverBySite>(pagingSessionKey, pagingReport);
            return pagingReport;
        }

        /// <summary>
        /// Loop and get paging report.
        /// if number of users in site are greater than the start index, we get the subtraction numberOfUsersRemain between them.
        ///    if the numberOfUsersRemain are less and equal than the items per page, we get array from start index with length equal the subtraction.
        ///        if the numberOfUsersRemain are equals, we break loop.
        ///        if the numberOfUsersRemain are less than, we assign the start index = 0  and subtract the items per page with the items.
        ///    if not, get array from start index with length equals the items per page, then break loop.
        /// if not, we subtract the start index with the number items of site.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="usersPerPage">The users per page.</param>
        /// <param name="pagingReport">The paging report.</param>
        private void LoopAndGetPagingReport(
            CustomerTurnoverBySite fullReport,
            int startIndex,
            int usersPerPage,
            ref CustomerTurnoverBySite pagingReport)
        {
            var numberOfUsersInSite = 0;
            var numberOfSites = fullReport.Site.Rows.Count;
            var index = 0;

            while (index < numberOfSites)
            {
                var site = fullReport.Site.Rows[index];
                numberOfUsersInSite = site.Rows.Count;

                if (numberOfUsersInSite > startIndex)
                {
                    var numberOfUsersRemain = numberOfUsersInSite - startIndex;
                    if (numberOfUsersRemain <= usersPerPage)
                    {
                        this.AddNewSiteIntoPagingReport(startIndex, ref pagingReport, site, numberOfUsersRemain);
                        if (numberOfUsersRemain == usersPerPage)
                        {
                            break;
                        }
                        else
                        {
                            startIndex = 0;
                            usersPerPage -= numberOfUsersRemain;
                        }
                    }
                    else
                    {
                        this.AddNewSiteIntoPagingReport(startIndex, ref pagingReport, site, usersPerPage);
                        break;
                    }
                }
                else
                {
                    startIndex -= numberOfUsersInSite;
                }

                index++;
            }
        }

        /// <summary>
        /// Add New Site Into Paging Report.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="site">The information of site.</param>
        /// <param name="items">The items.</param>
        private void AddNewSiteIntoPagingReport(int startIndex, ref CustomerTurnoverBySite pagingReport, CustomerTurnoverBySiteDetail site, int items)
        {
            var newSite = new CustomerTurnoverBySiteDetail
            {
                Rows = site.Rows.Skip(startIndex).Take(items).ToList(),
                Name = site.Name,
                Turnover = site.Turnover
            };
            pagingReport.Site.Rows.Add(newSite);
        }

        /// <summary>
        /// Convert and put data to report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The result report.</returns>
        private CustomerTurnoverBySite ConvertReportData(GetAllCustomerTurnOverCriteria criteria)
        {
            var report = new CustomerTurnoverBySite();

            var rowsData = GetReportData<dynamic>(criteria);
            var listSiteName = rowsData.GroupBy(row => row.site).Distinct().ToList();

            foreach (var siteName in listSiteName)
            {
                CustomerTurnoverBySiteDetail currentSite = new CustomerTurnoverBySiteDetail
                {
                    Name = siteName.Key
                };
                var rowsBySite = rowsData.Where(row => row.site == currentSite.Name).ToList();

                foreach (var row in rowsBySite)
                {
                    CustomerTurnoverBySiteDetail userBySite = new CustomerTurnoverBySiteDetail
                    {
                        Name = Converter.ToString2(row.username),
                        Turnover = Converter.ToDecimal(row.turnover)
                    };
                    currentSite.Rows.Add(userBySite);
                    currentSite.SumTotal(userBySite);
                    report.TotalUsers += 1;
                }

                report.Site.Rows.Add(currentSite);
                report.Site.SumTotal(currentSite);
            }

            return report;
        }
    }
}