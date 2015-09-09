// --------------------------------------------------------------------------
// <copyright file="GetReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.BingoStatistic
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "fromdate")]
        public string FromDate { get; set; }

        [SpParam(SqlParamName = "todate")]
        public string ToDate { get; set; }

        [SpParam(SqlParamName = "username")]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "excludeTest")]
        public bool ExcludeTestAccounts { get; set; }

        [SpParam(SqlParamName = "SiteID")]
        public string LicenseeList { get; set; }

        [SpParam(SqlParamName = "currency")]
        public string Currency { get; set; }

        [SpParam(SqlParamName = "MoveBetDay")]
        public DateTime MoveBetsDay
        {
            get
            {
                return ReportService.BalanceUp.MoveBetsDay;
            }
        }

        [SpParam(SqlParamName = "ReportBy")]
        public int ReportType { get; set; }

        [SpParam(Ignored = true)]
        public bool IsChangedValue { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_BingoStatistic_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}