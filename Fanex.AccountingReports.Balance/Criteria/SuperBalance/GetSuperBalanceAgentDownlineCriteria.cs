namespace Fanex.AccountingReports.Balance.Criteria.SuperBalance
{
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetSuperBalanceAgentDownlineCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public int BalanceType { get; set; }

        [SpParam(Ignored = true)]
        public string CustomerType { get; set; }

        public int AgentId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            var storeProcedureName = "Balance_SuperBalance_Member";

            if (!string.IsNullOrEmpty(storeProcedureName) && BalanceType == 2)
            {
                storeProcedureName += "Yesterday";
            }

            return storeProcedureName;
        }
    }
}
