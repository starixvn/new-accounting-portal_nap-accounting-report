namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetExchangeCurrenciesCriteria : CriteriaBase, ICriteria
    {
        public bool ExcludeTest { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_ExchangeCurrencySel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
