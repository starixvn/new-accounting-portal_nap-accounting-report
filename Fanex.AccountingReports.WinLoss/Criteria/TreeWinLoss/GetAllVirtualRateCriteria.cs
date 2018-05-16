namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetAllVirtualRateCriteria : CriteriaBase
    {
        public string CustIDs { get; set; }

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
            return "WinLoss_TreeWinLoss_GetAllVirtualRate";
        }
    }
}
