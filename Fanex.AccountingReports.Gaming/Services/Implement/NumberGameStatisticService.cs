namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System;
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

        public NumberGameStatisticByUserName GetNumberGameStatisticByUserName(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatisticByUserName report;

            try
            {
                report = _repository.GetNumberGameStatisticByUserName(criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<NumberGameStatisticByUserName>(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }
    }
}