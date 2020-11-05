namespace Fanex.AccountingReports.Balance.Criteria.DirectBalance
{
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetDirectBalanceMasterDownlineCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public int BalanceType { get; set; }

        [SpParam(Ignored = true)]
        public string CustomerType { get; set; }

        public int MasterId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            var storeProcedureName = "Balance_SuperBalance_Agent";

            if (BalanceType == 2)
            {
                storeProcedureName += "Yesterday";
            }

            return storeProcedureName;
        }
    }
}