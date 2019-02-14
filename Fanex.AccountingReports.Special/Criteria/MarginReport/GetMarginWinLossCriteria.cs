namespace Fanex.AccountingReports.Special.Criteria.MarginReport
{
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMarginWinLossCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "actual_stake")]
        public decimal ActualStake { get; set; }

        public override string GetSettingKey()
        {
            return "Special_MarginReport_BetRatioByUser";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}