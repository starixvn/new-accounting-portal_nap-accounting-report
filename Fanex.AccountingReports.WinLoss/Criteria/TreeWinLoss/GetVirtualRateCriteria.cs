namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using Fanex.Data.Repository;

    public class GetVirtualRateCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "WinLoss_TreeWinLoss_GetVirtualRate";
        }
    }
}
