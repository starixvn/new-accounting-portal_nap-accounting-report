// --------------------------------------------------------------------------
// <copyright file="GetTopPunterReportCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetTopPunterReportCriteria : CriteriaBase
    {
        public string UserName { get; set; }

        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public string Sportbettype { get; set; }

        public int TopRow { get; set; }

        public int OrderBy { get; set; }

        [SpParam(Ignored = true)]
        public string OrderByName { get; set; }

        public int OrderType { get; set; }

        [SpParam(Ignored = true)]
        public string OrderTypeName { get; set; }

        public int Reporttype { get; set; }

        [SpParam(Ignored = true)]
        public int PeriodType { get; set; }

        public override string GetSettingKey()
        {
            return "Special_PunterAnalysis_GetTopPunterReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}