using Fanex.Data.Repository;

namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    public class GetSportCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "WinLoss_TreeWinLoss_GetSport";
        }
    }
}
