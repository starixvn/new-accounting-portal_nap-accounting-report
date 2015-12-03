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
    public class GetReportByDailyCriteria : CriteriaBase
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
        public int ReportType { get; set; }

        [SpParam(Ignored = true)]
        public bool IsChangedValue { get; set; }

        [SpParam(Ignored = true)]
        public bool ShowRefundColumn { get; set; }

        [SpParam(Ignored = true)]
        public string TotalBetLessThan { get; set; }

        [SpParam(Ignored = true)]
        public string TotalBetMoreThan { get; set; }

        [SpParam(Ignored = true)]
        public string TotalWinLessThan { get; set; }

        [SpParam(Ignored = true)]
        public string TotalWinMoreThan { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        [SpParam(Ignored = true)]
        public string SortColumnName { get; set; }

        [SpParam(Ignored = true)]
        public string SortDataType { get; set; }

        [SpParam(SqlParamName = "ProductID")]
        public string ProductId { get; set; }

        [SpParam(Ignored = true)]
        public string Product { get; set; }

        [SpParam(Ignored = true)]
        public bool IsDrillDown { get; set; }

        [SpParam(Ignored = true)]
        public bool IsExportExcel { get; set; }

        [SpParam(Ignored = true)]
        public int ViewReport { get; set; }

        public override string GetSettingKey()
        {
            string spame = string.Empty;
            switch (ReportType)
            {
                case 0:
                case 7:
                case 8:
                    spame = "Acc_Rpt_CasinoGameStatistics_Daily";
                    break;
                case 1:
                    spame = "Acc_Rpt_CasinoGameStatistics_Monthly";
                    break;
                case 2:
                    spame = "Acc_Rpt_CasinoGameStatistics_UserName";
                    break;
            }

            return spame;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}