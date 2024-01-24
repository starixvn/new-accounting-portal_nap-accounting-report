namespace Fanex.AccountingReports.Licensee.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllLicenseeCriteria : CriteriaBase, ICriteria
    {
        public override string GetSettingKey()
        {
            return "Licensee_Core_GetAllLicensees";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
