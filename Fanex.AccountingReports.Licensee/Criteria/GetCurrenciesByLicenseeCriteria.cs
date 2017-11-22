namespace Fanex.AccountingReports.Licensee.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrenciesByLicenseeCriteria : CriteriaBase, ICriteria
    {
        public int LicenseeID { get; set; }

        public bool ExcludeTest
        {
            get
            {
                return true;
            }
        }

        public override string GetSettingKey()
        {
            return "Licensee_Core_GetCurrencies";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
