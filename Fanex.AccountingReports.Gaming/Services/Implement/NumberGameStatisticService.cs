namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Services.Interface;

    public class NumberGameStatisticService : ServiceBase, INumberGameStatisticService
    {
        private const int ExtendCachedTimeOut = 2;
        private INumberGameStatisticRepository _repository;

        public NumberGameStatisticService()
            : this(null)
        {
        }

        public NumberGameStatisticService(INumberGameStatisticRepository repository)
        {
            this._repository = repository ?? new NumberGameStatisticRepository();
        }

        public NumberGameStatisticByBettype GetNumberGameStatisticByBettype(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByBettype report;

            try
            {
                report = _repository.GetNumberGameStatisticByBettype(criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<NumberGameStatisticByBettype>(GamingConstants.NumberGameStatisticReportByBettypeCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }

        public NumberGameStatisticByDaily GetNumberGameStatisticByDaily(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByDaily report;

            try
            {
                report = _repository.GetNumberGameStatisticByDaily(criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<NumberGameStatisticByDaily>(GamingConstants.NumberGameStatisticReportByDailyCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }

        public NumberGameStatisticByLeague GetNumberGameStatisticByLeague(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByLeague report;

            try
            {
                report = _repository.GetNumberGameStatisticByLeague(criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<NumberGameStatisticByLeague>(GamingConstants.NumberGameStatisticReportByLeagueCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }

        public NumberGameStatisticByUserName GetNumberGameStatisticFullReportByUserName(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByUserName fullReport;
            try
            {
                fullReport = _repository.GetNumberGameStatisticByUserName(criteria);
                CacheManager.Set<NumberGameStatisticByUserName>(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey, fullReport, DateTime.Now.AddHours(ExtendCachedTimeOut));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return fullReport;
        }

        public NumberGameStatisticByUserName GetNumberGameStatisticByUserName(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByUserName report = new NumberGameStatisticByUserName();

            try
            {
                NumberGameStatisticByUserName fullReport;
                int pageIndex = criteria.PageIndex;
                int pageSize = criteria.PageSize;

                if (criteria.IsPagingCallBack)
                {
                    fullReport = CacheManager.Get<NumberGameStatisticByUserName>(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey) ?? GetNumberGameStatisticFullReportByUserName(criteria);
                }
                else
                {
                    fullReport = GetNumberGameStatisticFullReportByUserName(criteria);
                }

                report.ReportRows = fullReport.ReportRows.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                report.GrandTotal = fullReport.GrandTotal;
                report.TotalItems = fullReport.ReportRows.Count;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return report;
        }

        public NumberGameStatisticByUserName GetNumberGameStatisticSortByUserName(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByUserName report = new NumberGameStatisticByUserName();

            try
            {
                NumberGameStatisticByUserName fullReport;
                int pageIndex = criteria.PageIndex;
                int pageSize = criteria.PageSize;

                if (criteria.IsPagingCallBack)
                {
                    fullReport = CacheManager.Get<NumberGameStatisticByUserName>(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey) ?? GetNumberGameStatisticFullReportByUserName(criteria);
                }
                else
                {
                    fullReport = GetNumberGameStatisticFullReportByUserName(criteria);
                }

                ListExtensions.SortDataByProperty<NumberGameStatisticByUserNameDetail>(fullReport.ReportRows, criteria.SortDirection, criteria.SortColumnName, criteria.SortDataType);
                CacheManager.Set<NumberGameStatisticByUserName>(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey, fullReport);

                report.ReportRows = fullReport.ReportRows.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                report.GrandTotal = fullReport.GrandTotal;
                report.TotalItems = fullReport.ReportRows.Count;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return report;
        }
        public NumberGameStatisticByDaily GetNumberGameStatisticSortByDaily(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByDaily report;

            try
            {
                report = CacheManager.Get<NumberGameStatisticByDaily>(GamingConstants.NumberGameStatisticReportByDailyCachedDataKey) ?? _repository.GetNumberGameStatisticByDaily(criteria);
                ListExtensions.SortDataByProperty<NumberGameStatisticByDailyDetail>(report.ReportRows, criteria.SortDirection, criteria.SortColumnName, criteria.SortDataType);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<NumberGameStatisticByDaily>(GamingConstants.NumberGameStatisticReportByDailyCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }
        public NumberGameStatisticByLeague GetNumberGameStatisticSortByLeague(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByLeague report;

            try
            {
                report = CacheManager.Get<NumberGameStatisticByLeague>(GamingConstants.NumberGameStatisticReportByLeagueCachedDataKey) ?? _repository.GetNumberGameStatisticByLeague(criteria);
                ListExtensions.SortDataByProperty<NumberGameStatisticByLeagueDetail>(report.ReportRows, criteria.SortDirection, criteria.SortColumnName, criteria.SortDataType);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<NumberGameStatisticByLeague>(GamingConstants.NumberGameStatisticReportByLeagueCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }
        public NumberGameStatisticByBettype GetNumberGameStatisticSortByBettype(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByBettype report;

            try
            {
                report = CacheManager.Get<NumberGameStatisticByBettype>(GamingConstants.NumberGameStatisticReportByBettypeCachedDataKey) ??_repository.GetNumberGameStatisticByBettype(criteria);
                ListExtensions.SortDataByProperty<NumberGameStatisticByBettypeDetail>(report.ReportRows, criteria.SortDirection, criteria.SortColumnName, criteria.SortDataType);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<NumberGameStatisticByBettype>(GamingConstants.NumberGameStatisticReportByBettypeCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }
    }
}