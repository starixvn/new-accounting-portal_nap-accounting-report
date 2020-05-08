namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLeagueWinLossByBetTypeCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int LeagueID { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public int Sporttype { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public int Bettype { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public bool LiveIndicator { get; set; }

        public bool IsHedge { get; set; }
        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public override string GetSettingKey()
        {
            var suffix = IsNormalMode ? string.Empty : "_WH";

            return "WinLoss_LeagueWinLoss_GetLeagueWinLossByBetType" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
