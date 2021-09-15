using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BettypeWinLossBySite : BaseWinLoss
    {
        public int Bettype { get; set; }

        public string BettypeName { get; set; }

        public string SiteType { get; set; }

        public bool IsHedge { get; set; }

        public int TraderID { get; set; }

        public string TraderName { get; set; }
    }
}