namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetLicensee_SupportByProductCriteria : CriteriaBase, ICriteria
    {
        public string ProductPrefix { get; set; }

        public override string GetSettingKey()
        {
            return "Core_ReportService_GetLicenseeSupportByProduct";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
