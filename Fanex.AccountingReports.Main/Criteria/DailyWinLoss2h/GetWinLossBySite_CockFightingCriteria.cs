namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    public class GetWinLossBySite_CockFightingCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public int Site { get; set; }

        public override string GetSettingKey()
        {
            var winLossDateSuffix = ReportService.GetStoreMappingSuffix(this.WinLostDate);

            return string.Format("Main_DailyWinLoss2h_WinLossBySiteBetType_CockFighting{0}", winLossDateSuffix);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}