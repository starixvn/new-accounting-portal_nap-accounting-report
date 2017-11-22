namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetVirtualRateCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Get store procedure.
        /// </summary>
        /// <returns>The store procedure name.</returns>
        public override string GetSettingKey()
        {
            return "WinLoss_TreeWinLoss_GetVirtualRate";
        }
    }
}
