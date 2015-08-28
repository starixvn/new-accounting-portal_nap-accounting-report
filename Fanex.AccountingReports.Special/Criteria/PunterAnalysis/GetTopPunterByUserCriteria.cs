// --------------------------------------------------------------------------
// <copyright file="GetTopPunterByUserCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetTopPunterByUserCriteria : CriteriaBase
    {
        public int CustID { get; set; }

        public string UserName { get; set; }

        public string Sportbettype { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int OrderBy { get; set; }

        [SpParam(Ignored = true)]
        public string OrderByName { get; set; }

        public int OrderType { get; set; }

        [SpParam(Ignored = true)]
        public string OrderTypeName { get; set; }

        public int Reporttype { get; set; }

        public override string GetSettingKey()
        {
            return "Special_PunterAnalysis_GetTopPunterByUser";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
