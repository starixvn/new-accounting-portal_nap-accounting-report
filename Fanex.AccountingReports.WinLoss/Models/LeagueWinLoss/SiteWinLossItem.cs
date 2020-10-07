using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class SiteWinLossItem : BaseWinLoss
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; }
    }
}