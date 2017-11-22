namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get metric gaming bet type criteria.
    /// </summary>
    public class GetMetricGamingBettypeCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public string SubProduct { get; set; }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetMetricGamingBettypeReport" + ReportService.GetStoreMappingSuffix(this.WinlostDate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
