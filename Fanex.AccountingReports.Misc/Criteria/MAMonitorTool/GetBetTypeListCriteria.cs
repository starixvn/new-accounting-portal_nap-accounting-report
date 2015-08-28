// --------------------------------------------------------------------------
// <copyright file="GetBetTypeListCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.MAMonitorTool
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Models.MAMonitorTool;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Bet Type List Criteria.
    /// </summary>
    public class GetBetTypeListCriteria : CriteriaBase, ICriteria
    {
        [SpParam(SqlParamName = "sporttype")]
        public int SportType { get; set; }

        public string Products { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_MAMonitorTool_GetBetTypeList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
