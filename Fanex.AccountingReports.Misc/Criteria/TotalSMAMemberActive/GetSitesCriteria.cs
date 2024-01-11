namespace Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive
{
    using Fanex.Data.Repository;

    public class GetSitesCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Misc_TotalSMAMemberActive_GetSites";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
