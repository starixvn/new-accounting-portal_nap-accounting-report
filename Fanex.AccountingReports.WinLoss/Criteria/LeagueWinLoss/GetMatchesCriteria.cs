namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMatchesCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string LeagueName { get; set; }

        public string SportBettype { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public string SportName { get; set; }

        public bool IsNoDraw { get; set; }

        public string SiteName { get; set; }

        public int LiveIndicator { get; set; }

        public bool ShowHedgeMatchOnly { get; set; }

        public int BetType { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public string GlobalShowTime { get; set; }

        public int SportType { get; set; }

        public override string GetSettingKey()
        {
            var suffix = IsNormalMode ? string.Empty : "_WH";

            return "WinLoss_LeagueWinLoss_GetMatches" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}