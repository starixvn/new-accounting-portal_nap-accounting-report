// --------------------------------------------------------------------------
// <copyright file="GetDailyCreatedCustomersDetailsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.DailyCreatedCustomers
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetDailyCreatedCustomersDetailsCriteria : CriteriaBase
    {
        /// <summary>
        /// Gets or sets the Date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the RoleID.
        /// </summary>
        /// <value>The role id of customer.</value>
        public int RoleID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether.
        /// </summary>
        /// <value>The boolean value to identify Direct or not.</value>
        public bool IsDirect { get; set; }

        /// <summary>
        /// Gets or sets the RoleName.
        /// </summary>
        /// <value>The role name.</value>
        [SpParam(Ignored = true)]
        public string RoleName { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>The key to access database.</returns>
        public override string GetSettingKey()
        {
            return "Misc_DailyCreatedCustomers_GetDetailReport";
        }

        /// <summary>
        /// Check valid of criteria.
        /// </summary>
        /// <returns>True or false.</returns>
        public override bool IsValid()
        {
            return true;
        }
    }
}