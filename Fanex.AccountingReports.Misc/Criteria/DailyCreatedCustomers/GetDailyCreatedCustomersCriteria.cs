// --------------------------------------------------------------------------
// <copyright file="GetDailyCreatedCustomersCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetDailyCreatedCustomersCriteria : CriteriaBase
    {
        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_DailyCreatedCustomers_GetReport";
        }

        public override bool IsValid()
        {
            return this.Fromdate <= this.Todate;
        }
    }
}