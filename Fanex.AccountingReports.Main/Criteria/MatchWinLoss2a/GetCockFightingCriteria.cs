namespace Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    public class GetCockFightingCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinLoss2a_GetCockFightingReport" + ReportService.GetStoreMappingSuffix(this.WinLostDate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}