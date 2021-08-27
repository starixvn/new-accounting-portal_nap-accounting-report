namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetBetTypesCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int LiveIndicator { get; set; }

        public int MatchID { get; set; }

        [SpParam(Ignored = true)]
        public string MatchEvent { get; set; }

        public string SportBettype { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public string SportName { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public bool IsNoDraw { get; set; }

        public string SiteName { get; set; }

        [SpParam(Ignored = true)]
        public bool IsHedge { get; set; }

        public bool IsIncludeSiteType { get; set; }

        public bool IsIncludeHedge { get; set; }

        public string GlobalShowTime { get; set; }

        public int SportType { get; set; }

        public string LeagueId { get; set; }

        public override string GetSettingKey()
        {
            var suffix = IsNormalMode ? string.Empty : "_WH";

            return "WinLoss_LeagueWinLoss_GetBetTypes" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}