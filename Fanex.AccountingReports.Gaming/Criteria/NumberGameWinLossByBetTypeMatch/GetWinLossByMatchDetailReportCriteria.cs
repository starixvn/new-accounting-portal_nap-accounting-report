namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetWinLossByMatchDetailReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "siteid")]
        public string CompanyList { get; set; }

        [SpParam(SqlParamName = "currency")]
        public string CurrencyList { get; set; }

        [SpParam(SqlParamName = "islive")]
        public int LiveStatus { get; set; }

        [SpParam(SqlParamName = "matchid")]
        public int MatchId { get; set; }

        [SpParam(SqlParamName = "balanceupday")]
        public DateTime BalanceUpDay
        {
            get
            {
                return ReportService.BalanceUp.BalanceUpDay;
            }
        }

        [SpParam(SqlParamName = "date")]
        public DateTime ReportDate { get; set; }

        [SpParam(SqlParamName = "bettype")]
        public int BetTypeId { get; set; }

        [SpParam(Ignored = true)]
        public string MatchCode { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinLoss_WinLossByMatchDetailReport";
        }
    }
}
