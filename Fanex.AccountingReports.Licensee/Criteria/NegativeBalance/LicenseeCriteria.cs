using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Licensee.Criteria.NegativeBalance
{
    public class LicenseeCriteria : CriteriaBase, ICriteria
    {
        public override string GetSettingKey()
        {
            return "Licensee_NegativeBalance_GetLicensee";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
