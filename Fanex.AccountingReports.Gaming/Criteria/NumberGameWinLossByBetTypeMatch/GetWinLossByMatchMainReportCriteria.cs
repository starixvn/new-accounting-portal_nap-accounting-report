namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    public class GetWinLossByMatchMainReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "siteid")]
        public string CompanyList { get; set; }

        [SpParam(SqlParamName = "currency")]
        public string CurrencyList { get; set; }

        [SpParam(SqlParamName = "bettype")]
        public string BetTypeList { get; set; }

        [SpParam(SqlParamName = "date")]
        public DateTime ReportDate { get; set; }

        public bool TrialData { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinLoss_WinLossByMatchMainReport";
        }
    }
}
