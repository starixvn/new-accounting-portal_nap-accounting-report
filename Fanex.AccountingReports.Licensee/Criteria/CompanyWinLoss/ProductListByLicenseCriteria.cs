// -----------------------------------------------------------------------
// <copyright file="GetProductListByLicenseCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Criteria.CompanyWinLoss
{
    using Fanex.NAP.Framework.Data;

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
