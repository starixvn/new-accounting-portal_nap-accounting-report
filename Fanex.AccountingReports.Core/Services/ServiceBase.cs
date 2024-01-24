namespace Fanex.AccountingReports.Core.Services.Impl
{
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;

    public class ServiceBase
    {
        private readonly IDynamicRepository dynamicRepository = new DynamicRepository();

        public void HandleException(Exception ex)
        {
            LoggingHandler.Log(ex);
        }

        public virtual IEnumerable<T> GetReportData<T>(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                return dynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }
    }
}
