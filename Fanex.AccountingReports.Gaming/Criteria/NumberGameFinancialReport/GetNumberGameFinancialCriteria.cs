// -----------------------------------------------------------------------
// <copyright file="GetNumberGameFinancialCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.04">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Number Game Financial Criteria.
    /// </summary>
    public class GetNumberGameFinancialCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "datestart")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "dateend")]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Number_Game_Financial_Report";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}