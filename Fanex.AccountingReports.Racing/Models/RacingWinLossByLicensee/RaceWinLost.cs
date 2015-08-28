// -----------------------------------------------------------------------
// <copyright file="RaceWinLost.cs" company="Nexcel Solutions VietNam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee
{
    using System;

    /// <summary>
    /// The win lost information of race.
    /// </summary>
    [Serializable]
    public class RaceWinLost
    {
        public decimal MemberWinTurnover { get; set; }

        public decimal MemberPlaceTurnover { get; set; }

        public decimal MemberWinPlaceTurnover { get; set; }

        public decimal MemberTotalTurnover { get; set; }

        public decimal MemberDividendWon { get; set; }

        public decimal Site12BetWinTurnover { get; set; }

        public decimal Site12BetPlaceTurnover { get; set; }

        public decimal Site12BetWinPlaceTurnover { get; set; }

        public decimal Site12BetTotalTurnover { get; set; }

        public decimal Site12BetPositionTakingWinLoss { get; set; }

        public decimal Site12BetCommissionIncome { get; set; }

        public decimal IBCPositionTakingWinLoss { get; set; }

        public decimal IBCCommissionIncome { get; set; }

        public decimal AmountDueToIBC { get; set; }

        /// <summary>
        /// Calculate the total win lost.
        /// </summary>
        /// <param name="race">The RaceWinLost.</param>
        public void SumTotal(RaceWinLost race)
        {
            this.MemberWinTurnover += race.MemberWinTurnover;
            this.MemberPlaceTurnover += race.MemberPlaceTurnover;
            this.MemberWinPlaceTurnover += race.MemberWinPlaceTurnover;
            this.MemberTotalTurnover += race.MemberTotalTurnover;
            this.MemberDividendWon += race.MemberDividendWon;
            this.Site12BetWinTurnover += race.Site12BetWinTurnover;
            this.Site12BetPlaceTurnover += race.Site12BetPlaceTurnover;
            this.Site12BetWinPlaceTurnover += race.Site12BetWinPlaceTurnover;
            this.Site12BetTotalTurnover += race.Site12BetTotalTurnover;
            this.Site12BetPositionTakingWinLoss += race.Site12BetPositionTakingWinLoss;
            this.Site12BetCommissionIncome += race.Site12BetCommissionIncome;
            this.IBCPositionTakingWinLoss += race.IBCPositionTakingWinLoss;
            this.IBCCommissionIncome += race.IBCCommissionIncome;
            this.AmountDueToIBC += race.AmountDueToIBC;
        }
    }
}
