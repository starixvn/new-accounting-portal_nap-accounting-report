namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByDate
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetNumberGameWinLossByDateCriteria : CriteriaBase
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string Currency { get; set; }

        public string Bettype { get; set; }

        public string SiteID { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinlossByDate_GetReport";
        }
    }
}
