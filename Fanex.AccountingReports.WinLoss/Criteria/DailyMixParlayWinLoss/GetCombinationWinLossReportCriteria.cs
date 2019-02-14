namespace Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss
{
    using System;
    using Data;
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
