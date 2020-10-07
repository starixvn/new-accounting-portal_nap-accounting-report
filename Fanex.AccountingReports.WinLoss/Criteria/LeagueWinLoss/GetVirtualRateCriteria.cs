using Fanex.Data.Repository;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    public class GetVirtualRateCriteria : CriteriaBase
    {
        public int CustID { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetVirtualRate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}