namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    public class GetCockFightingCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetCockFightingReport" + ReportService.GetStoreMappingSuffix(this.WinlostDate);
        }
    }
}