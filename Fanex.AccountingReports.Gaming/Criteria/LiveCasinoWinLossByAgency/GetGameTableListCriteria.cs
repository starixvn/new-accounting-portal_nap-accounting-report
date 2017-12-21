namespace Fanex.AccountingReports.Gaming.Criteria.LiveCasinoWinLossByAgency
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTableListCriteria : CriteriaBase, ICriteria
    {
        [SpParam(SqlParamName = "GameTypeID")]
        public int GameTypeId { get; set; }

        public bool ExcludeTest { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_LiveCasinoGameStatistics_GetGameTableList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}