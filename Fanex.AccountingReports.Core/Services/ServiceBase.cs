namespace Fanex.AccountingReports.Core.Services.Impl
{
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;

    public class ServiceBase
    {
        protected readonly IDynamicRepository DynamicRepository;

        public ServiceBase()
        {
            DynamicRepository = new DynamicRepository();
        }

        protected IDynamicRepository Repository
        {
            get { return DynamicRepository; }
        }

        public void HandleException(Exception ex)
        {
            LoggingHandler.Log(ex);
        }

        public virtual IEnumerable<T> GetReportData<T>(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                return DynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }
    }
}
