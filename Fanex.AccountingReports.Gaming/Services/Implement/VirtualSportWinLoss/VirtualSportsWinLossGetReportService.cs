// -----------------------------------------------------------------------
// <copyright file="VirtualSportsWinLossGetReportService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Implement.VirtualSportWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Core.Configurations.VirtualSportsWinLossSection;
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Services.Interface.VirtualSportWinLoss;

    /// <summary>
    /// The get report service of Virtual Sports Win Loss report.
    /// </summary>
    public class VirtualSportsWinLossGetReportService : ServiceBase, IVirtualSportsWinLossGetReportService
    {
        private readonly IVirtualSportsWinLossRepository _reportRepository;

        private string _virtualSportsWinLossPrefixSessionKey = 
            "Fanex.AccountingReports.Gaming.VirtualSportsWinLoss.";

        private string _virtualSportsWinLossSessionKey = string.Empty;

        public VirtualSportsWinLossGetReportService()
        {
            if (AppSession.CurrentUser != null)
            {
                this._virtualSportsWinLossSessionKey = this._virtualSportsWinLossPrefixSessionKey
                                                    + AppSession.CurrentUser.Username;
            }
            else
            {
                this._virtualSportsWinLossSessionKey = this._virtualSportsWinLossPrefixSessionKey;
            }

            this._reportRepository = new VirtualSportsWinLossRepository();
        }

        /// <summary>
        /// Get report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public VirtualSportsWinLoss GetReport(GetVirtualSportsWinLossCriteria criteria)
        {
            try
            {
                var fullReport = new VirtualSportsWinLoss();
                var pagingReport = new VirtualSportsWinLoss();
                var startIndex = criteria.StartIndex;
                var usersPerPage = criteria.UsersPerPage;

                if (!criteria.IsValid())
                {
                    return fullReport;
                }

                if (!criteria.IsPagingCallBack)
                {
                    fullReport = this.GetFullReportAndSaveSession(criteria);
                }
                else
                {
                    fullReport = SessionManager.Get<VirtualSportsWinLoss>(
                                            this._virtualSportsWinLossSessionKey);

                    if (fullReport == null)
                    {
                        fullReport = this.GetFullReportAndSaveSession(criteria);
                    }
                }

                if (fullReport != null && fullReport.Rows != null
                            && fullReport.Rows.Count > 0)
                {
                    string pagingSessionKey = this._virtualSportsWinLossSessionKey + "." + startIndex
                        + "." + criteria.FromDate + "." + criteria.ToDate + "." + criteria.DrawFilter
                        + "." + criteria.SportType + "." + criteria.ExcludeTest + "." + usersPerPage;

                    pagingReport = this.GetPagingReport(
                                                fullReport, 
                                                startIndex, 
                                                usersPerPage, 
                                                pagingSessionKey);
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
        /// Get sports collection.
        /// </summary>
        /// <returns>The sports collection.</returns>
        public List<SportsType> GetSportsCollection()
        {
            try
            {
                var sportsCollection = VirtualSportsWinLossSection.Instance.VirtualSportsGroup;
                var sportsType = new List<SportsType>();

                if (sportsCollection != null && sportsCollection.Count > 0)
                {
                    foreach (VirtualSportsElement sportElement in sportsCollection)
                    {
                        if (sportElement.Active)
                        {
                            var sport = new SportsType()
                            {
                                Id = sportElement.Id,
                                Name = sportElement.Name,
                                Active = sportElement.Active
                            };

                            sportsType.Add(sport);
                        }
                    }
                }

                return sportsType;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// GEt full report and save this report in session using session key.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The full report.</returns>
        private VirtualSportsWinLoss GetFullReportAndSaveSession(GetVirtualSportsWinLossCriteria criteria)
        {
            var fullReport = this._reportRepository.GetVirtualSportsWinLossReport(criteria);
            SessionManager.Set<VirtualSportsWinLoss>(
                                    this._virtualSportsWinLossSessionKey,
                                    fullReport);

            return fullReport;
        }

        /// <summary>
        /// Get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="usersPerPage">The users per page.</param>
        /// <param name="pagingSessionKey">The paging session key.</param>
        /// <returns>The paging report.</returns>
        private VirtualSportsWinLoss GetPagingReport(
            VirtualSportsWinLoss fullReport,
            int startIndex,
            int usersPerPage,
            string pagingSessionKey)
        {
            VirtualSportsWinLoss pagingReport = null;
            pagingReport = SessionManager.Get<VirtualSportsWinLoss>(pagingSessionKey);

            if (pagingReport == null)
            {
                pagingReport = this.GetPagingReportDetail(
                                            fullReport,
                                            startIndex,
                                            usersPerPage);
                SessionManager.Set<VirtualSportsWinLoss>(pagingSessionKey, pagingReport);
            }

            return pagingReport;
        }

        /// <summary>
        /// The detail of get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="usersPerPage">The users per page.</param>
        /// <returns>The paging report.</returns>
        private VirtualSportsWinLoss GetPagingReportDetail(
            VirtualSportsWinLoss fullReport,
            int startIndex,
            int usersPerPage)
        {
            var pagingReport = new VirtualSportsWinLoss
            {
                Currencies = fullReport.Currencies,
                Date = fullReport.Date,
                Rows = new List<VirtualSportsWinLoss>()
            };

            this.LoopAndGetPagingReport(fullReport, startIndex, usersPerPage, ref pagingReport);

            return pagingReport;
        }

        /// <summary>
        /// Loop and get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="usersPerPage">The users per page.</param>
        /// <param name="pagingReport">The paging report.</param>
        private void LoopAndGetPagingReport(
            VirtualSportsWinLoss fullReport,
            int startIndex,
            int usersPerPage,
            ref VirtualSportsWinLoss pagingReport)
        {
            var numberOfCurrenciessInDate = 0;
            var numberOfDates = fullReport.Rows.Count;
            var index = 0;

            while (index < numberOfDates)
            {
                var dateRow = fullReport.Rows[index];
                numberOfCurrenciessInDate = dateRow.Currencies.Count;

                if (numberOfCurrenciessInDate > startIndex)
                {
                    var numberOfCurrenciesRemain = numberOfCurrenciessInDate - startIndex;
                    if (numberOfCurrenciesRemain <= usersPerPage)
                    {
                        this.AddNewDateIntoPagingReport(
                                            startIndex,
                                            ref pagingReport,
                                            dateRow,
                                            numberOfCurrenciesRemain);
                        if (numberOfCurrenciesRemain == usersPerPage)
                        {
                            break;
                        }
                        else
                        {
                            startIndex = 0;
                            usersPerPage -= numberOfCurrenciesRemain;
                        }
                    }
                    else
                    {
                        this.AddNewDateIntoPagingReport(startIndex, ref pagingReport, dateRow, usersPerPage);
                        break;
                    }
                }
                else
                {
                    startIndex -= numberOfCurrenciessInDate;
                }

                index++;
            }
        }

        /// <summary>
        /// Add new date into paging report.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="dateRow">The date row.</param>
        /// <param name="numberOfCurrenciesRemain">The numbers of currencies remain.</param>
        private void AddNewDateIntoPagingReport(
            int startIndex,
            ref VirtualSportsWinLoss pagingReport,
            VirtualSportsWinLoss dateRow,
            int numberOfCurrenciesRemain)
        {
            var newDate = new VirtualSportsWinLoss
            {
                Date = dateRow.Date,
                Currencies = dateRow.Currencies.Skip(startIndex).Take(numberOfCurrenciesRemain).ToList(),
            };

            pagingReport.Rows.Add(newDate);
        }
    }
}
