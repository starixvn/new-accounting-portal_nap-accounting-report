// --------------------------------------------------------------------------
// <copyright file="GetReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetReportByHourlyCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "fromdate")]
        public string FromDate { get; set; }

        [SpParam(SqlParamName = "todate")]
        public string ToDate { get; set; }

        [SpParam(SqlParamName = "MoveBetsDay")]
        public DateTime MoveBetsDay
        {
            get
            {
                return ReportService.BalanceUp.MoveBetsDay;
            }
        }

        [SpParam(SqlParamName = "username")]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "excludeTest")]
        public bool ExcludeTestData { get; set; }

        [SpParam(SqlParamName = "SiteID")]
        public string LicenseeId { get; set; }

        [SpParam(SqlParamName = "GameTypeID")]
        public string GameTypeId { get; set; }

        [SpParam(SqlParamName = "GameTableID")]
        public string GameTableId { get; set; }

        [SpParam(SqlParamName = "currency")]
        public string Currency { get; set; }

        [SpParam(Ignored = true)]
        public bool ShowRefundColumn { get; set; }

        [SpParam(SqlParamName = "ProductID")]
        public string ProductId { get; set; }

        [SpParam(Ignored = true)]
        public string Product { get; set; }

        [SpParam(Ignored = true)]
        public bool IsExportExcel { get; set; }

        [SpParam(Ignored = true)]
        public int ViewReport { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_CasinoGameStatistics_Hourly";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}