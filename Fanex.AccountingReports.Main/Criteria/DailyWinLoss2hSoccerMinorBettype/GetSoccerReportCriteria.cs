// --------------------------------------------------------------------------
// <copyright file="GetSoccerReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetSoccerReportCriteria : GetMainReportCriteria
    {
        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetSoccerReport" + ReportService.GetStoreMappingSuffix(this.WinlostDate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
