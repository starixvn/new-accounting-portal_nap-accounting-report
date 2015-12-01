// --------------------------------------------------------------------------
// <copyright file="GetSoccerReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype
{
    using System;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetSoccerReportCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public int SoccerType
        {
            get
            {
                return 2;
            }
        }

        public string IncludeBetType
        {
            get
            {
                return AppSettings.Main2hSoccerMinorBettypes;
            }
        }

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
