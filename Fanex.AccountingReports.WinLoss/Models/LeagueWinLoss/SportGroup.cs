using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    public class SportGroup
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public List<Sport> ChildItems { get; set; }
    }
}
