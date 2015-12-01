// --------------------------------------------------------------------------
// <copyright file="GetTopPunterBetCountByUserCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetTopPunterBetCountByUserCriteria : CriteriaBase
    {
        public int CustID { get; set; }

        public int LeagueID { get; set; }

        public int Sporttype { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public string Bettype { get; set; }

        public string Sportbettype { get; set; }

        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public int Reporttype { get; set; }

        public bool ByLeague { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        [SpParam(Ignored = true)]
        public string UserName { get; set; }

        public override string GetSettingKey()
        {
            return "Special_PunterAnalysis_GetTopPunterBetCountByUser";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}