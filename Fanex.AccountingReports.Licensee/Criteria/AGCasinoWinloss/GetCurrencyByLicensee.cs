// -----------------------------------------------------------------------
// <copyright file="GetCurrencyByLicensee.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Criteria.AGCasinoWinloss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrencyByLicensee : CriteriaBase
    {
        public int LicenseeID { get; set; }

        public bool ExcludeTest { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Licensee_AGCasinoWinloss_GetCurrencyByLicensee";
        }
    }
}
