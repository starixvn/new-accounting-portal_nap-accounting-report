// --------------------------------------------------------------------------
// <copyright file="GetResellerCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2013.08.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetResellerCriteria : CriteriaBase
    {
        public int ResellerID { get; set; }

        public string ProductCode { get; set; }

        public bool ExcludeBA { get; set; }

        public bool ExcludeTest { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_Core_GetReseller";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}