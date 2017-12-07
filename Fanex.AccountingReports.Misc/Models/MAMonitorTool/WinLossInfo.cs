namespace Fanex.AccountingReports.Misc.Models.MAMonitorTool
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Win Loss Info.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public int HomeId { get; set; }

        public string HomeName { get; set; }

        public int AwayId { get; set; }

        public string AwayName { get; set; }

        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public int BetCount { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Turnover { get; set; }

        public int NumberUsers { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        /// <summary>
        /// Formats the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The formatted win loss.</returns>
        public static WinLossInfo Format(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.WinLoss = Math.Round(winLoss.WinLoss, 2, MidpointRounding.AwayFromZero);
            winLoss.Turnover = Math.Round(winLoss.Turnover, 2, MidpointRounding.AwayFromZero);
            if (winLoss.ChildsInfo == null)
            {
                return winLoss;
            }

            for (var i = 0; i < winLoss.ChildsInfo.Count; i++)
            {
                winLoss.ChildsInfo[i] = WinLossInfo.Format(winLoss.ChildsInfo[i]);
            }

            return winLoss;
        }

        /// <summary>
        /// Sums the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void Sum(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            BetCount += winLoss.BetCount;
            WinLoss += winLoss.WinLoss;
            Turnover += winLoss.Turnover;
            if (winLoss.ChildsInfo == null || ChildsInfo == null)
            {
                return;
            }

            for (var i = 0; i < ChildsInfo.Count &&
                i < winLoss.ChildsInfo.Count; i++)
            {
                ChildsInfo[i].Sum(winLoss.ChildsInfo[i]);
            }
        }
    }
}
