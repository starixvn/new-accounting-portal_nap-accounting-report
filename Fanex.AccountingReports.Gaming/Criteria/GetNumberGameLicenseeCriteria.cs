namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByDate
{
    using Fanex.Data.Repository;

    public class GetNumberGameLicenseeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinLossByDate_GetLicensee";
        }
    }
}
