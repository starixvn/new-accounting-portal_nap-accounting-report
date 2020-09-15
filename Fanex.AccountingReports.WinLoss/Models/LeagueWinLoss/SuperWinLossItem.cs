using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class SuperWinLossItem : BaseWinLoss
    {
        public int SuperId { get; set; }

        public string SuperName { get; set; }

        public bool HasVR { get; set; }
    }
}