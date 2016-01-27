namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    public class GetCockFightingArenaCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public int ArenaId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetCockFightingByArenaReport" + ReportService.GetStoreMappingSuffix(this.WinLostDate);
        }
    }
}