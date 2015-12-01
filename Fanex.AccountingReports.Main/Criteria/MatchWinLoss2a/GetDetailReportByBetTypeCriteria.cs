// --------------------------------------------------------------------------
// <copyright file="GetDetailReportByBetTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get detail report by bet type criteria.
    /// </summary>
    public class GetDetailReportByBetTypeCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public string Sporttype { get; set; }

        public int Bettype { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public bool LiveIndicator { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinLoss2a_GetWinLossDetailReportByBetType" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}