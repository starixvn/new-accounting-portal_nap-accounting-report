namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// BalanceUpCriteria: criteria for getting balance up day and move bet day.
    /// </summary>
    public class BalanceUpCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Core_ReportService_GetBalanceUpDay";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
