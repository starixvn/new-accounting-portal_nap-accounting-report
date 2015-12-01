// --------------------------------------------------------------------------
// <copyright file="GetReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.BingoWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetReportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(Ignored = true)]
        public bool IsDirectFromFilter { get; set; }

        public override string GetSettingKey()
        {
            var suffix = string.Empty;
            if (this.ToDate <= ReportService.BalanceUp.MoveBetsDay)
            {
                suffix = ReportService.WarehouseSPSuffix;
            }

            return "Gaming_BingoWinLoss_GetReport" + suffix;
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}