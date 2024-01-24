namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    public class GetLeagueWinLossCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string SportBettype { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public bool IsNoDraw { get; set; }

        public string LeagueName { get; set; }

        [SpParam(Ignored = true)]
        public string SortColumnName { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        public string SiteName { get; set; }

        public int LiveIndicator { get; set; }

        public string GlobalShowTime { get; set; }

        public int PermissionProfileID { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetLeagueWinLoss";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
