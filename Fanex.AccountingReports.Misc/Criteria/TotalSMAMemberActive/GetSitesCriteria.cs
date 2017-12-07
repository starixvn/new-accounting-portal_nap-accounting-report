namespace Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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
