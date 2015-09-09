// --------------------------------------------------------------------------
// <copyright file="GetCombinationWinLossReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCombinationWinLossReportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Site { get; set; }

        [SpParam(Ignored = true)]
        public bool IsHistoryReport { get; set; }

        public override string GetSettingKey()
        {
            var suffix = string.Empty;
            if (this.IsHistoryReport)
            {
                suffix = ReportService.WarehouseSPSuffix;
            }

            return "WinLoss_DailyMixParlayWinLoss_GetCombinationWinLoss" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
