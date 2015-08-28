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
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Sport List Criteria.
    /// </summary>
    public class GetSportListCriteria : CriteriaBase, ICriteria
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
