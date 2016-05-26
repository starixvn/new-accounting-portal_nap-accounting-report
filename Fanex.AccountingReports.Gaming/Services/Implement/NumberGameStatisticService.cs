namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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

            CacheExtension.Set(GamingConstants.NumberGameStatisticReportByBettypeCachedDataKey, report);
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

            CacheExtension.Set(GamingConstants.NumberGameStatisticReportByDailyCachedDataKey, report);
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

            CacheExtension.Set(GamingConstants.NumberGameStatisticReportByLeagueCachedDataKey, report);
            return report;
        }

        public NumberGameStatisticByUserName GetNumberGameStatisticFullReportByUserName(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByUserName fullReport;
            try
            {
                fullReport = _repository.GetNumberGameStatisticByUserName(criteria);
                CacheExtension.Set(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey, fullReport);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return fullReport;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
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
                    fullReport = CacheExtension.Get<NumberGameStatisticByUserName>(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey) ?? GetNumberGameStatisticFullReportByUserName(criteria);
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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
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
                    fullReport = CacheExtension.Get<NumberGameStatisticByUserName>(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey) ?? GetNumberGameStatisticFullReportByUserName(criteria);
                }
                else
                {
                    fullReport = GetNumberGameStatisticFullReportByUserName(criteria);
                }

                ListExtensions.SortDataByProperty<NumberGameStatisticByUserNameDetail>(fullReport.ReportRows, criteria.SortDirection, criteria.SortColumnName, criteria.SortDataType);
                CacheExtension.Set(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey, fullReport);

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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        public NumberGameStatisticByDaily GetNumberGameStatisticSortByDaily(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByDaily report;

            try
            {
                report = CacheExtension.Get<NumberGameStatisticByDaily>(GamingConstants.NumberGameStatisticReportByDailyCachedDataKey) ?? _repository.GetNumberGameStatisticByDaily(criteria);
                ListExtensions.SortDataByProperty<NumberGameStatisticByDailyDetail>(report.ReportRows, criteria.SortDirection, criteria.SortColumnName, criteria.SortDataType);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheExtension.Set(GamingConstants.NumberGameStatisticReportByDailyCachedDataKey, report);
            return report;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        public NumberGameStatisticByLeague GetNumberGameStatisticSortByLeague(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByLeague report;

            try
            {
                report = CacheExtension.Get<NumberGameStatisticByLeague>(GamingConstants.NumberGameStatisticReportByLeagueCachedDataKey) ?? _repository.GetNumberGameStatisticByLeague(criteria);
                ListExtensions.SortDataByProperty<NumberGameStatisticByLeagueDetail>(report.ReportRows, criteria.SortDirection, criteria.SortColumnName, criteria.SortDataType);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheExtension.Set(GamingConstants.NumberGameStatisticReportByLeagueCachedDataKey, report);
            return report;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        public NumberGameStatisticByBettype GetNumberGameStatisticSortByBettype(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByBettype report;

            try
            {
                report = CacheExtension.Get<NumberGameStatisticByBettype>(GamingConstants.NumberGameStatisticReportByBettypeCachedDataKey) ?? _repository.GetNumberGameStatisticByBettype(criteria);
                ListExtensions.SortDataByProperty<NumberGameStatisticByBettypeDetail>(report.ReportRows, criteria.SortDirection, criteria.SortColumnName, criteria.SortDataType);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheExtension.Set(GamingConstants.NumberGameStatisticReportByBettypeCachedDataKey, report);
            return report;
        }
    }
}