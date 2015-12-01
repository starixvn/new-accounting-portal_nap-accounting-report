// -----------------------------------------------------------------------
// <copyright file="GetNumberOfActiveMemberCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetNumberOfActiveMemberCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public string ProductList { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>The key to access database.</returns>
        public override string GetSettingKey()
        {
            return "Gaming_NumberOfActiveMember_GetReport";
        }

        /// <summary>
        /// Check criteria is valid or not.
        /// </summary>
        /// <returns>The boolean.</returns>
        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}