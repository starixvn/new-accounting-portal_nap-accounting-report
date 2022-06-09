using Fanex.Data.Repository;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    public class GetSportCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetSports";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
