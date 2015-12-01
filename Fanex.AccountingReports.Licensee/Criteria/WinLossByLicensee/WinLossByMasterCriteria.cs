namespace Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByMasterCriteria : WinLossByLicenseeCriteria
    {
        [SpParam(Ignored = true)]
        public int CustID { get; set; }

        public int Srecommend
        {
            get { return CustID; }
        }

        public override string GetSettingKey()
        {
            return "Licensee_WinLossByLicensee_GetWinLossMaster";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
