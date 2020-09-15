using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    public class GetSuperCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool LiveIndicator { get; set; }

        public int MatchID { get; set; }

        public int SiteID { get; set; }

        public int BetType { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public bool IsNoDraw { get; set; }

        public override string GetSettingKey()
        {
            var suffix = IsNormalMode ? string.Empty : "_WH";

            return "WinLoss_LeagueWinLoss_GetSuper" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}