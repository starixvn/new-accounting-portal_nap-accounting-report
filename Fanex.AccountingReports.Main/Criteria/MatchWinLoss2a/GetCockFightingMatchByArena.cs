namespace Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    public class GetCockFightingMatchByArena : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public int ArenaId { get; set; }

        [SpParam(Ignored = true)]
        public string ArenaName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2a_GetCockFightingByArenaReport" + ReportService.GetStoreMappingSuffix(this.WinLostDate);
        }
    }
}