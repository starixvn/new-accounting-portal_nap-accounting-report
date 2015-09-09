// --------------------------------------------------------------------------
// <copyright file="GetVirtualSportBySporttypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.11.27">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.VirtualSportWinLoss
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetVirtualSportBySporttypeCriteria : CriteriaBase, ICriteria
    {
        public override string GetSettingKey()
        {
            return "Licensee_VirtualSportWinLoss_GetReportBySport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}