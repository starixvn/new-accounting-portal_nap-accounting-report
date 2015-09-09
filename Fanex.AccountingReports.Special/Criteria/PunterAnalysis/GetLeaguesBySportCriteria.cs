// --------------------------------------------------------------------------
// <copyright file="GetLeaguesBySportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Criteria.PunterAnalysis
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLeaguesBySportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Sporttype { get; set; }

        public override string GetSettingKey()
        {
            return "Special_PunterAnalysis_GetLeaguesBySport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}