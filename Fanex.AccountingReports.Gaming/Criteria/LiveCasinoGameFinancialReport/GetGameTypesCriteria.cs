namespace Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTypesCriteria : CriteriaBase
    {
        public string ProductID { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_LiveCasinoGameFinancial_GetGameType";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(this.ProductID);
        }
    }
}
