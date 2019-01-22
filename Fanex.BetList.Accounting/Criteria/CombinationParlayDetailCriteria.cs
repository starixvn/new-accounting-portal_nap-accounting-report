namespace Fanex.BetList.Accounting.Criteria
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CombinationParlayDetailCriteria : CriteriaBase, ICriteria
    {
        [SpParam(SqlParamName = "refno")]
        public long RefNo { get; set; }

        [SpParam(SqlParamName = "transid")]
        public long TransId { get; set; }

        [SpParam(SqlParamName = "betid")]
        public long BetId { get; set; }

        [SpParam(SqlParamName = "custid")]
        public int CustId { get; set; }

        public DateTime MoveBetDay { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public override string GetSettingKey()
        {
            return "BetList_CombinationParlay_Details";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
