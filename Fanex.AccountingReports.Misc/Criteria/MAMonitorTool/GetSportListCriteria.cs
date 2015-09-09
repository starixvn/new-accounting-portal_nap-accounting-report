// --------------------------------------------------------------------------
// <copyright file="GetSportListCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.MAMonitorTool
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Sport List Criteria.
    /// </summary>
    public class GetSportListCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Misc_MAMonitorTool_GetSportList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}