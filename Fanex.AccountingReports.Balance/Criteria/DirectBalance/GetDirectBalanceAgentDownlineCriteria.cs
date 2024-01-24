namespace Fanex.AccountingReports.Balance.Criteria.DirectBalance
{
    using Data;
    using Fanex.Data.Repository;

    public class GetDirectBalanceAgentDownlineCriteria : CriteriaBase
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

            if (BalanceType == 2)
            {
                storeProcedureName += "Yesterday";
            }

            return storeProcedureName;
        }
    }
}
