// --------------------------------------------------------------------------
// <copyright file="GetSitesCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.15">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetSitesCriteria : GetTotalSMAMemberActiveCriteria, ICriteria
    {
        public override string GetSettingKey()
        {
            return "Misc_TotalSMAMemberActive_GetSites";
        }
    }
}
