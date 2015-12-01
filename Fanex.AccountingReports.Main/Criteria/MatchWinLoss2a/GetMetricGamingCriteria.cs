// -----------------------------------------------------------------------
// <copyright file="GetMetricGamingCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get metric gaming criteria.
    /// </summary>
    public class GetMetricGamingCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public string SubProduct { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinLoss2a_GetMetricGamingReport" + ReportService.GetStoreMappingSuffix(this.WinlostDate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}