// --------------------------------------------------------------------------
// <copyright file="GetCurrenciesByLicenseeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrenciesByLicenseeCriteria: CriteriaBase, ICriteria
    {
        public int LicenseeID { get; set; }

        public bool ExcludeTest
        {
            get
            {
                return true;
            }
        }

        public override string GetSettingKey()
        {
            return "Licensee_Core_GetCurrencies";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}