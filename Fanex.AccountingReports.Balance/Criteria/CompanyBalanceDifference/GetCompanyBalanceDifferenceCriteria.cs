namespace Fanex.AccountingReports.Balance.Criteria.CompanyBalanceDifference
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCompanyBalanceDifferenceCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Balance_CompanyBalanceDifference_GetReport";
        }
    }
}
