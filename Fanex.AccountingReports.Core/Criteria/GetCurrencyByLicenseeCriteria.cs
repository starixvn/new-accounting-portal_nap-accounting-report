// --------------------------------------------------------------------------
// <copyright file="GetCurrencyByLicenseeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrencyByLicenseeCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public string LicenseeList { get; set; }

        public bool ExcludeTest { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_ExchangeCurrencySel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}