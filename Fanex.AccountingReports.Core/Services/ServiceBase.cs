namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ServiceBase
    {
        protected readonly IDynamicRepository DynamicRepository;

        public ServiceBase()
        {
            DynamicRepository = new DynamicRepository();
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <value>
        /// The repository.
        /// </value>
        protected IDynamicRepository Repository
        {
            get { return DynamicRepository; }
        }

        /// <summary>
        /// Handler Exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public void HandleException(Exception ex)
        {
            LoggingHandler.Log(ex);
        }

        /// <summary>
        /// Get Report Data with Criteria.
        /// </summary>
        /// <typeparam name="T">Type of Object return.</typeparam>
        /// <param name="criteria">Criteria to Query DB.</param>
        /// <returns>Enumerable of Type T.</returns>
        public virtual IEnumerable<T> GetReportData<T>(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            try
            {
                if (criteria.IsValid())
                {
                    return DynamicRepository.Fetch<T>(criteria);
                }

                return new List<T>();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }
    }
}