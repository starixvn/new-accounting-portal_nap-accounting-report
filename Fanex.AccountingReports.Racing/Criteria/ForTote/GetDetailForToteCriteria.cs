namespace Fanex.AccountingReports.Racing.Criteria.ForTote
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// Create Get racing for tote detail.
    /// </summary>
    public class GetDetailForToteCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "winlostdate")]
        public DateTime WinlostDate { get; set; }

        [SpParam(SqlParamName = "partnerID")]
        public int PartnerId { get; set; }

        [SpParam(SqlParamName = "poolCurrency")]
        public int PoolCurrency { get; set; }

        [SpParam(Ignored = true)]
        public string PartnerName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            if (WinlostDate > ReportService.BalanceUp.MoveBetsDay)
            {
                return "Racing_HorseRacingDailyReportForTote_Detail";
            }
            else
            {
                return "Racing_HorseRacingDailyReportForTote_Detail_14";
            }
        }
    }
}
