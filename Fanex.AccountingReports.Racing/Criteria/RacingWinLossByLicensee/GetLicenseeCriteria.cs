// -----------------------------------------------------------------------
// <copyright file="GetLicenseeCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// The criteria to get list licensee.
    /// </summary>
    public class GetLicenseeCriteria : CriteriaBase
    {
        public GetLicenseeCriteria()
        {
            ProductPrefix = string.Empty;
            LangKey = string.Empty;
        }

        public int LicenseeID { get; set; }

        public string ProductPrefix { get; set; }

        public string LangKey { get; set; }

        public override string GetSettingKey()
        {
            return "WinlostRacingLicensee_SiteSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
