namespace Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByMemberCriteria : WinLossByLicenseeCriteria
    {
        public int CustID { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_WinLossByLicensee_GetWinLossMember";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
