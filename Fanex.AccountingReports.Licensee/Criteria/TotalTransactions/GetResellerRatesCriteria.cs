// --------------------------------------------------------------------------
// <copyright file="GetResellerRatesCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.TotalTransactions
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetResellerRatesCriteria : CriteriaBase
    {
        public DateTime InputDate { get; set; }

        public int ResellerID { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_Core_GetResellerRates";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}