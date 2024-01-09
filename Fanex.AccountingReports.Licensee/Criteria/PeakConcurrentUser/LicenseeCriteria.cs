using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Licensee.Criteria.PeakConcurrentUser
{
    public class LicenseeCriteria : CriteriaBase, ICriteria
    {
        public override string GetSettingKey()
        {
            return "Licensee_PeakConcurrentUser_GetLicensee";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
