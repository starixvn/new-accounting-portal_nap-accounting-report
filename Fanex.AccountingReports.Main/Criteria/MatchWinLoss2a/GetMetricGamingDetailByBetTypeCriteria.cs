using Fanex.AccountingReports.Core.Services.Impl;
using Fanex.Data.Repository;
using System;

namespace Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a
{
    public class GetMetricGamingDetailByBetTypeCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public string Sporttype { get; set; }

        public int Bettype { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public bool LiveIndicator { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinLoss2a_GetMetricGamingDetailByBetType" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
