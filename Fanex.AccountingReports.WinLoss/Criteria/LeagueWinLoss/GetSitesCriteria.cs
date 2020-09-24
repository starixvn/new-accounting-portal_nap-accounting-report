using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public bool IsNoDraw { get; set; }

        public string SiteName { get; set; }

        public override string GetSettingKey()
        {
            var suffix = IsNormalMode ? string.Empty : "_WH";

            return "WinLoss_LeagueWinLoss_GetSites" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}