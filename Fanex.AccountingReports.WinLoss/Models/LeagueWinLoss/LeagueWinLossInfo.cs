namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.WinLoss.Resources;
    using Fanex.AccountingReports.WinLoss.Services.Implement;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class LeagueWinLossInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Event
        {
            get
            {
                if (this.BetType != null &&
                    this.HomeTeam != null &&
                    this.AwayTeam != null)
                {
                    var awayTeamName = this.BetType.Id == LeagueWinLossService.BetTypeContants.MixParlay2 ?
                        string.Empty :
                        (" -" + Labels.Symbol_Vs + "- " + this.AwayTeam.Name.Trim(' '));
                    return string.Format(
                        "{0} {1}",
                        this.HomeTeam.Name.Trim(' '),
                        awayTeamName);
                }

                return string.Empty;
            }
        }

        public decimal WinLoss { get; set; }

        public decimal Position { get; set; }

        public decimal Commission { get; set; }

        public decimal LineTotal { get; set; }

        public decimal TurnOver { get; set; }

        public string Margin { get; set; }

        public string Hedge { get; set; }

        public string MatchCode { get; set; }

        public EntityBase<int, string> HomeTeam { get; set; }

        public EntityBase<int, string> AwayTeam { get; set; }

        public EntityBase<int, string> BetType { get; set; }

        public EntityBase<int, string> League { get; set; }

        public bool IsLive { get; set; }

        public bool IsNeutral { get; set; }

        public List<LeagueWinLossInfo> ChildsInfo { get; set; }

        public LeagueWinLossInfo EightRM { get; set; }

        public LeagueWinLossInfo BARM { get; set; }

        public LeagueWinLossInfo Total { get; set; }

        public LeagueWinLossInfo GrandTotal { get; set; }

        /// <summary>
        /// Adds to total.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void AddToTotal(LeagueWinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            this.Total.WinLoss += winLoss.WinLoss;
            this.Total.Position += winLoss.Position;
            this.Total.Commission += winLoss.Commission;
            this.Total.LineTotal += winLoss.LineTotal;
            this.Total.TurnOver += winLoss.TurnOver;
        }

        /// <summary>
        /// Adds to grand total.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void AddToGrandTotal(LeagueWinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            this.GrandTotal.WinLoss += winLoss.WinLoss;
            this.GrandTotal.Position += winLoss.Position;
            this.GrandTotal.Commission += winLoss.Commission;
            this.GrandTotal.LineTotal += winLoss.LineTotal;
            this.GrandTotal.TurnOver += winLoss.TurnOver;
        }
    }
}
