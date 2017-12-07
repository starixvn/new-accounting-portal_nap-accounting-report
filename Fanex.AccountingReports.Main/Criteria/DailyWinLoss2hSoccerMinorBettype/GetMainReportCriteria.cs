namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype
{
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMainReportCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public int SoccerType
        {
            get
            {
                return 2;
            }
        }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2hSoccer_Main2hReport" + ReportService.GetStoreMappingSuffix(this.WinlostDate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
