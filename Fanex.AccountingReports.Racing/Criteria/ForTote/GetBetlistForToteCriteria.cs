namespace Fanex.AccountingReports.Racing.Criteria.ForTote
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// Create Racing for Tote Bet list criteria.
    /// </summary>
    public class GetBetlistForToteCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "leagueid")]
        public int LeagueId { get; set; }

        [SpParam(SqlParamName = "raceNo")]
        public string RaceNo { get; set; }

        [SpParam(SqlParamName = "currency")]
        public int Currency { get; set; }

        [SpParam(SqlParamName = "winlostdate")]
        public DateTime WinLostDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            if (WinLostDate > ReportService.BalanceUp.MoveBetsDay)
            {
                return "Racing_HorseRacingDailyReportForTote_Betlist";
            }
            else
            {
                return "Racing_HorseRacingDailyReportForTote_Betlist_14";
            }
        }
    }
}
