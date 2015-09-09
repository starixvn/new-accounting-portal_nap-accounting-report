namespace Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByAgentCriteria : WinLossByLicenseeCriteria
    {
        [SpParam(Ignored = true)]
        public int CustID { get; set; }

        [SpParam(SqlParamName = "mrecommend")]
        public int MRecommend
        {
            get { return CustID; }
        }

        public override string GetSettingKey()
        {
            return "Licensee_WinLossByLicensee_GetWinLossAgent";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
