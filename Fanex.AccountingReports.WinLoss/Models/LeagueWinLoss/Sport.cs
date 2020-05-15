using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class Sport
    {
        public int SportGroupType { get; set; }

        public string SportGroupName { get; set; }

        public int SportType { get; set; }

        public string SportName { get; set; }
    }
}
