namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLeagueWinLossBySportCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int LeagueId { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public string SportBettype { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public bool IsNoDraw { get; set; }

        public override string GetSettingKey()
        {
            var suffix = IsNormalMode ? string.Empty : "_WH";

            return "WinLoss_LeagueWinLoss_GetLeagueWinLossBySport" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}