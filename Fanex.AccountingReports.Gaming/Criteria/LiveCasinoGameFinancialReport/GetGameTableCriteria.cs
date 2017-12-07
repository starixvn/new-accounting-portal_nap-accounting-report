namespace Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTableCriteria : CriteriaBase
    {
        public string GameType { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_LiveCasinoGameFinancial_GetGameTable";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(this.GameType);
        }
    }
}
