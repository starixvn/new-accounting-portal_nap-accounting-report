namespace Fanex.AccountingReports.Licensee.Criteria.CompanyWinLoss
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ProductListByLicenseCriteria : CriteriaBase
    {
        public int LicenseeId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Acc_GetProductListByLicenseId";
        }
    }
}
