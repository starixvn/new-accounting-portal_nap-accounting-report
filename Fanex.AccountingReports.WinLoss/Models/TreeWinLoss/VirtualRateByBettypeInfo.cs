namespace Fanex.AccountingReports.WinLoss.Models.TreeWinLoss
{
    using System;

    [Serializable]
    public class VirtualRateByBettypeInfo
    {
        public string BettypeName { get; set; }

        public decimal? VirtualRate { get; set; }
    }
}
