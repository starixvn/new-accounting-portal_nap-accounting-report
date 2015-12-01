// --------------------------------------------------------------------------
// <copyright file="GetWinLossByMatchMainReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetWinLossByMatchMainReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "siteid")]
        public string CompanyList { get; set; }

        [SpParam(SqlParamName = "currency")]
        public string CurrencyList { get; set; }

        [SpParam(SqlParamName = "bettype")]
        public string BetTypeList { get; set; }

        [SpParam(SqlParamName = "date")]
        public DateTime ReportDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinLoss_WinLossByMatchMainReport";
        }
    }
}