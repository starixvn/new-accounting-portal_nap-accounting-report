using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Licensee.Criteria.CompanyWinLoss
{
    public class LicenseeCriteria : CriteriaBase, ICriteria
    {
        public override string GetSettingKey()
        {
            return "Licensee_CompanyWinloss_GetLicensee";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
