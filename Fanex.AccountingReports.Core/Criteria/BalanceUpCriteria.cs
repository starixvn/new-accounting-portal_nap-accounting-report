namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.Data.Repository;

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
