namespace Fanex.AccountingReports.Balance.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Balance.Criteria.CompanyBalanceDifference;
    using Fanex.AccountingReports.Balance.Models.CompanyBalanceDifference;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CompanyBalanceDifferenceService : ServiceBase
    {
        /// <summary>
        /// Gets the company balance difference.
        /// </summary>
        /// <returns>The company balance difference.</returns>
        public List<DifferenceInfo> GetCompanyBalanceDifference()
        {
            var rawData = GetReportData<dynamic>(new GetCompanyBalanceDifferenceCriteria());
            var differences = new List<DifferenceInfo>();
            foreach (var row in rawData)
            {
                differences.Add(new DifferenceInfo
                {
                    BalanceUpDate = Converter.ToDateTime(row.BalanceUpDay).ToString("MMM dd, yyyy"),
                    BalanceBeforeBalanceUpDate = Converter.ToDecimal(row.CompBalBefore),
                    BalanceAfterBalanceUpDate = Converter.ToDecimal(row.CompBalAfter),
                    Difference = Converter.ToDecimal(row.DIFFERENCE)
                });
            }

            return differences;
        }
    }
}