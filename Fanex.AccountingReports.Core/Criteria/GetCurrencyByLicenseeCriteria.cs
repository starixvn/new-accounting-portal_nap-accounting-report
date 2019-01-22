namespace Fanex.AccountingReports.Core.Criteria
{
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrencyByLicenseeCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public string LicenseeList { get; set; }

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
