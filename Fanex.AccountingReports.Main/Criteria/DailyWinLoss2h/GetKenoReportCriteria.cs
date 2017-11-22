namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetKenoReportCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetKenoReport" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
