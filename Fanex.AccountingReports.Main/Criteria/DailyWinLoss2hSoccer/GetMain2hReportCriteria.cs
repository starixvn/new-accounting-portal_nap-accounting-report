namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccer
{
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMain2hReportCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2hSoccer_Main2hReport" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
