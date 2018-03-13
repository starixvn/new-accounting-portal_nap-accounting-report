namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetWinLossByBetTypeDetailReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "siteid")]
        public string CompanyList { get; set; }

        [SpParam(SqlParamName = "currency")]
        public string CurrencyList { get; set; }

        [SpParam(SqlParamName = "liveindicator")]
        public int? LiveStatus { get; set; }

        [SpParam(SqlParamName = "winlostdate")]
        public DateTime ReportDate { get; set; }

        [SpParam(SqlParamName = "bettype")]
        public int BetTypeId { get; set; }

        public bool TrialData { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinLoss_WinLossByBetTypeDetailReport" + ReportService.GetStoreMappingSuffix(ReportDate);
        }
    }
}
