namespace Fanex.AccountingReports.Main.Models.DailyWinLoss2h
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
   public class LiveAndNoneLiveReport
    {
        public WinLossInfo NoneLiveReport { get; set; }

        public WinLossInfo LiveReport { get; set; }

        public WinLossInfo MixLiveAndNoneLiveReport { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        public WinLossInfo Total { get; set; }
    }
}
