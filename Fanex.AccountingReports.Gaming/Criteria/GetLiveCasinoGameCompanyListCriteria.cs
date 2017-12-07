namespace Fanex.AccountingReports.Gaming.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLiveCasinoGameCompanyListCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_LiveCasinoGameStatistics_GetLicensees";
        }
    }
}
