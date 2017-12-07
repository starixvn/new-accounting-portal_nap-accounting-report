namespace Fanex.AccountingReports.Misc.Criteria.TotalCreditAccountByProduct
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get total credit account by product criteria.
    /// </summary>
    public class GetTotalCreditAccountByProductCriteria : CriteriaBase
    {
        public string ListCurrency { get; set; }

        public bool ShowEnabled { get; set; }

        public bool ShowDisabled { get; set; }

        public int SportType { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_EnableDisableAccount_ByProduct";
        }
    }
}
