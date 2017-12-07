namespace Fanex.AccountingReports.Licensee.Criteria.CompanyWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrenciesByLicenseeCriteria : CriteriaBase
    {
        public int LicenseeID { get; set; }

        public bool ExcludeTest
        {
            get;
            set;
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
