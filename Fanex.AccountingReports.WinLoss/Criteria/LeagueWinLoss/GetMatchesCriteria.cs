namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMatchesCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public string SportBettype { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public string SportName { get; set; }

        public bool IsNoDraw { get; set; }

        public string SiteName { get; set; }

        public int LiveIndicator { get; set; }

        public int ShowHedgeOrNormalMatchOnly { get; set; }

        public int BetType { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public string GlobalShowTime { get; set; }

        public int SportType { get; set; }

        public string LeagueId { get; set; }

        public int PermissionProfileID { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetMatches";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
