namespace Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetAdvanceMixParlayReportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Site { get; set; }

        [SpParam(Ignored = true)]
        public int Bettype
        {
            get
            {
                return 9;
            }
        }

        [SpParam(Ignored = true)]
        public string BetName { get; set; }

        [SpParam(Ignored = true)]
        public bool IsHistoryReport { get; set; }

        public override string GetSettingKey()
        {
            var suffix = string.Empty;
            if (this.IsHistoryReport)
            {
                suffix = ReportService.WarehouseSPSuffix;
            }

            return "WinLoss_DailyMixParlayWinLoss_GetAdvanceMixParlayReport" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
