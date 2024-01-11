namespace Fanex.AccountingReports.Misc.Criteria.TotalCreditAccountByProduct
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    public class GetBetCountDetailCriteria : CriteriaBase
    {
        public DateTime MoveBetsDay { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public int CustID { get; set; }

        public int SportType { get; set; }

        [SpParam(Ignored = true)]
        public string SuperUserName { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        public override bool IsValid()
        {
            return this.MoveBetsDay <= this.BalanceUpDay;
        }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_BetCount_ByProduct_Super";
        }
    }
}
