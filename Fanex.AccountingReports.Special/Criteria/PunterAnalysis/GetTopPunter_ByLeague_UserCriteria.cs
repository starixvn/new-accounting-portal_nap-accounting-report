// --------------------------------------------------------------------------
// <copyright file="GetTopPunter_ByLeague_UserCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Criteria.PunterAnalysis
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetTopPunter_ByLeague_UserCriteria : CriteriaBase
    {
        public int CustID { get; set; }

        public string UserName { get; set; }

        public int Sporttype { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public string Bettype { get; set; }

        public int LeagueID { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Reporttype { get; set; }

        public int OrderBy { get; set; }

        [SpParam(Ignored = true)]
        public string OrderByName { get; set; }

        public int OrderType { get; set; }

        [SpParam(Ignored = true)]
        public string OrderTypeName { get; set; }

        public override string GetSettingKey()
        {
            return "Special_PunterAnalysis_GetTopPunter_ByLeague_User";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
