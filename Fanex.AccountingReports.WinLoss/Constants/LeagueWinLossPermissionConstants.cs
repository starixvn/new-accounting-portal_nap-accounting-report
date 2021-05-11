using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.AccountingReports.WinLoss.Constants
{
    public static class LeagueWinLossPermissionConstants
    {
        public static Dictionary<string, string> Products => new Dictionary<string, string>
        {
            { "LeagueWinLoss_AllProducts", "All Product" },
            { "LeagueWinLoss_Soccer", "Soccer" },
            { "LeagueWinLoss_SoccerLive", "Soccer" },
            { "LeagueWinLoss_SoccerNonLive", "Soccer" },
            { "LeagueWinLoss_Parlay", "Parlay" },
            { "LeagueWinLoss_SBAdjustment", "SB Adjustment" },
            { "LeagueWinLoss_Basketball", "Basketball" },
            { "LeagueWinLoss_E_Sports", "E-Sports" },
            { "LeagueWinLoss_OtherSports", "Others" }
        };

        public const string AllProducts = "LeagueWinLoss_AllProducts";
        public const string LeagueWinLoss_SoccerLive = "LeagueWinLoss_SoccerLive";
        public const string LeagueWinLoss_SoccerNonLive = "LeagueWinLoss_SoccerNonLive";
        public const string LeagueWinLoss_NormalMode = "LeagueWinLoss_NormalMode";
    }
}
