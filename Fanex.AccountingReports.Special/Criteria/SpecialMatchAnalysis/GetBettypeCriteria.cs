namespace Fanex.AccountingReports.Special.Criteria.SpecialMatchAnalysis
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetBetTypeCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public DateTime MoveBetsDay
        {
            get
            {
                return ReportService.BalanceUp.MoveBetsDay;
            }
        }

        public DateTime BalanceUpDay
        {
            get
            {
                return ReportService.BalanceUp.BalanceUpDay;
            }
        }

        public int MatchId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Special_SpecialMatchAnalysis_GetBetTypeByMatch";
        }
    }
}
