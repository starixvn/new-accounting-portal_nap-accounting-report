// --------------------------------------------------------------------------
// <copyright file="GetLicenseesByProductCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLicenseesByProductCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "GetIsActiveOnly")]
        public bool GetIsActiveOnly { get; set; }

        [SpParam(SqlParamName = "ExcludeLicenseeID")]
        public int ExcludeLicenseeID { get; set; }

        [SpParam(SqlParamName = "SystemID")]
        public int SystemID { get; set; }

        [SpParam(SqlParamName = "ProductPrefix")]
        public string ProductPrefix { get; set; }

        public override bool IsValid()
        {
            return true;
        }        

        public override string GetSettingKey()
        {
            return "Acc_Rpt_GetLicenseesByProduct";
        }
    }
}
