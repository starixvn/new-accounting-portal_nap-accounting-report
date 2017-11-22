namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetParlayReportCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetParlayReport" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }
    }
}
