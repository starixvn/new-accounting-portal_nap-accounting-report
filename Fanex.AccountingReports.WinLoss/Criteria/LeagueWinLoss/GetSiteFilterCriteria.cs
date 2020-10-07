using Fanex.Data.Repository;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    public class GetSiteFilterCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetSiteFilter";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}