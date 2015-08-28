// --------------------------------------------------------------------------
// <copyright file="GetTopPunter_ByLeagueCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetTopPunter_ByLeagueCriteria : CriteriaBase
    {
        public int LeagueID { get; set; }

        public int Sporttype { get; set; }

        public string Bettype { get; set; }

        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public int TopRow { get; set; }

        public int OrderBy { get; set; }

        public bool OrderType { get; set; }

        public int Reporttype { get; set; }

        public override string GetSettingKey()
        {
            return "Special_PunterAnalysis_GetTopPunter_ByLeague";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
