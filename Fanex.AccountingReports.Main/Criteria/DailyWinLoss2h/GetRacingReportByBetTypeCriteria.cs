// --------------------------------------------------------------------------
// <copyright file="GetRacingReportByBetTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetRacingReportByBetTypeCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public int Sporttype { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public int Bettype { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetRacingReportByBetType" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
