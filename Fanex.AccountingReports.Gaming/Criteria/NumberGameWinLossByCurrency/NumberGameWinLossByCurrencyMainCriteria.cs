namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByCurrency
{
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinLossByCurrencyMainCriteria : CriteriaBase
    {
        public string SiteID { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        [SpParam(SqlParamName = "isLive")]
        public int IsLive { get; set; }

        public string Currency { get; set; }

        public string Bettype { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        public bool TrialData { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinlossByCurrency_GetReport";
        }
    }
}
