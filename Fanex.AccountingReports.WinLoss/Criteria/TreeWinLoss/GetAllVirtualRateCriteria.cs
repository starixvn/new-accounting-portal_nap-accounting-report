namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using Fanex.Data.Repository;

    public class GetAllVirtualRateCriteria : CriteriaBase
    {
        public string CustIDs { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "WinLoss_TreeWinLoss_GetAllVirtualRate";
        }
    }
}
