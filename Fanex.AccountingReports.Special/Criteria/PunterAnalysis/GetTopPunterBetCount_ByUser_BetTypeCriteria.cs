// --------------------------------------------------------------------------
// <copyright file="GetTopPunterBetCount_ByUser_BetTypeCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetTopPunterBetCount_ByUser_BetTypeCriteria : CriteriaBase
    {
        public int CustID { get; set; }

        [SpParam(Ignored = true)]
        public string UserName { get; set; }

        [SpParam(Ignored = true)]
        public bool ByLeague { get; set; }

        public int? LeagueID { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public int? Status { get; set; }

        [SpParam(Ignored = true)]
        public string LiveTypeName { get; set; }

        public int Bettype { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public int Sporttype { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public int Reporttype { get; set; }

        public override string GetSettingKey()
        {
            return "Special_PunterAnalysis_GetTopPunterBetCount_ByUser_BetType";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}