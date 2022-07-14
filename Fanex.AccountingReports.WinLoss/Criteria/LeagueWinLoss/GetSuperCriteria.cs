using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    public class GetSuperCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public bool LiveIndicator { get; set; }

        public int MatchID { get; set; }

        [SpParam(Ignored = true)]
        public string MatchEvent { get; set; }

        public int SiteID { get; set; }

        [SpParam(Ignored = true)]
        public string SiteType { get; set; }

        [SpParam(Ignored = true)]
        public string SiteName { get; set; }

        public int BetType { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public string SportName { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public bool IsNoDraw { get; set; }

        public int SportType { get; set; }

        public int PermissionProfileID { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetSuper";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}