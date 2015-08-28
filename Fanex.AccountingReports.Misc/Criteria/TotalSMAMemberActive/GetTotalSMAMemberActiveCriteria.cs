// --------------------------------------------------------------------------
// <copyright file="GetTotalSMAMemberActiveCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetTotalSMAMemberActiveCriteria : CriteriaBase, ICriteria
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string SiteList { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_TotalSMAMemberActive_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
