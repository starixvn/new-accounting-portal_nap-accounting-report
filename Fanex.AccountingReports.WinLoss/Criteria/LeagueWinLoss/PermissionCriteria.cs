using Fanex.Data.Repository;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    public class PermissionCriteria : CriteriaBase
    {
        public string FunctionKey { get; set; }

        public override string GetSettingKey() => $"WinLoss_LeagueWinLoss_GetPermisionProfileID";

        public override bool IsValid() => true;
    }
}
