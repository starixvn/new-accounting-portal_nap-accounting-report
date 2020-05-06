namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.WinLoss.Resources;
    using Fanex.AccountingReports.WinLoss.Services.Implement;
    using System;
    using System.Collections.Generic;

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

        public decimal MemberWinLoss { get; set; }

        public decimal SMAWinLoss { get; set; }

        public decimal ExtraPT { get; set; }

        public decimal Commission { get; set; }

        public decimal RealIncome { get; set; }

        public decimal VirtualWinLoss { get; set; }

        public decimal VarianceIncome { get; set; }

        public decimal RealTurnover { get; set; }

        public decimal VirtualTurnover { get; set; }

        public decimal VirtualMargin
        {
            get
            {
                return this.VirtualWinLoss / this.VirtualTurnover;
            }
        }

        public decimal RealMargin
        {
            get
            {
                return this.RealIncome / this.RealTurnover;
            }
        }

        //public decimal WinLoss { get; set; }

        //public decimal Position { get; set; }

        //public decimal Commission { get; set; }

        //public decimal LineTotal { get; set; }

        //public decimal TurnOver { get; set; }

        //public string Margin { get; set; }

        public string Hedge { get; set; }

        public string MatchCode { get; set; }

        public EntityBase<int, string> HomeTeam { get; set; }

        public EntityBase<int, string> AwayTeam { get; set; }

        public EntityBase<int, string> BetType { get; set; }

        public EntityBase<int, string> League { get; set; }

        public EntityBase<int, string> Sport { get; set; }

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
        //public void AddToTotal(LeagueWinLossInfo winLoss)
        //{
        //    if (winLoss == null)
        //    {
        //        return;
        //    }

        //    this.Total.WinLoss += winLoss.WinLoss;
        //    this.Total.Position += winLoss.Position;
        //    this.Total.Commission += winLoss.Commission;
        //    this.Total.LineTotal += winLoss.LineTotal;
        //    this.Total.TurnOver += winLoss.TurnOver;
        //}

        /// <summary>
        /// Adds to grand total.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
    }
}
