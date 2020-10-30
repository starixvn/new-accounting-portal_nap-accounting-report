namespace Fanex.AccountingReports.Balance.Criteria.DirectBalance
{
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetDirectBalanceAgentDownlineCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public string CustomerType { get; set; }

        public int AgentId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Balance_MasterBalance_MemberDownline";
        }
    }
}