// --------------------------------------------------------------------------
// <copyright file="GetLicenseeByResellerCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLicenseeByResellerCriteria : CriteriaBase, ICriteria
    {
        public int ResellerID { get; set; }

        public string ProductCode { get; set; }

        public bool ExcludeBA { get; set; }

        public bool ExcludeTest { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Licensee_Core_GetReseller";
        }
    }
}
