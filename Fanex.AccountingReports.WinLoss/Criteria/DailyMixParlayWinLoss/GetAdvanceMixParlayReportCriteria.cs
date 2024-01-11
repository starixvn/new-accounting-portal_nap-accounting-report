namespace Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss
{
    using System;
    using Data;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    public class GetAdvanceMixParlayReportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Site { get; set; }

        [SpParam(Ignored = true)]
        public int Bettype
        {
            get
            {
                return 9;
            }
        }

        [SpParam(Ignored = true)]
        public string BetName { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_DailyMixParlayWinLoss_GetAdvanceMixParlayReport_WH";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
