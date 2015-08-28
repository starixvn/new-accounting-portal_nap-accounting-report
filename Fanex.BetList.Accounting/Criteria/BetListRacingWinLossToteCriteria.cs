// -----------------------------------------------------------------------
// <copyright file="BetListRacingWinLossToteCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Criteria
{
    using System;
    using Fanex.BetList.Accounting.Services;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Create Bet list racing win loss tote criteria.
    /// </summary>
    public class BetListRacingWinLossToteCriteria : CriteriaBase
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
