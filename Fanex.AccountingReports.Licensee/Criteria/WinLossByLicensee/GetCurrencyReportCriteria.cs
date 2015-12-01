namespace Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrencyReportCriteria : WinLossByLicenseeCriteria
    {
        [SpParam(Ignored = true)]
        public int CustID { get; set; }

        public int ReportType { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_WinLossByLicensee_GetCurrencyReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
