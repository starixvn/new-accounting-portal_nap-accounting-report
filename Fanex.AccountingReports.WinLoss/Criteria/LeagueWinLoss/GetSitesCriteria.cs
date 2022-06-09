using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    public class GetSitesCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool LiveIndicator { get; set; }

        public int MatchID { get; set; }

        [SpParam(Ignored = true)]
        public string MatchEvent { get; set; }

        public string SiteType { get; set; }

        public int BetType { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public string SportName { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public bool IsNoDraw { get; set; }

        public string SiteName { get; set; }

        [SpParam(Ignored = true)]
        public string SortColumnName { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        public int SportType { get; set; }

        public int PermissionProfileID { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetSites";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}