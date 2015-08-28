// --------------------------------------------------------------------------
// <copyright file="GetCurrencyReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrencyReportCriteria : WinLossByLicenseeCriteria
    {
        public override string GetSettingKey()
        {
            return "Licensee_WinLossByLicensee_GetCurrencyReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
