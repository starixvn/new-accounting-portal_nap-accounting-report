using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    public class SiteGroup
    {
        public string GroupName { get; set; }

        public List<Site> Sites { get; set; }
    }
}