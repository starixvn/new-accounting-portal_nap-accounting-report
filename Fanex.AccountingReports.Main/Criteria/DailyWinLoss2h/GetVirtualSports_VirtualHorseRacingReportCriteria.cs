namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetVirtualSports_VirtualHorseRacingReportCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public int Sporttype { get; set; }

        [SpParam(Ignored = true)]
        public string SportTypeName { get; set; }

        public int Bettype { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetVirtualSports_VirtualHorseRacingReport" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
