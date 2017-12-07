namespace Fanex.AccountingReports.Main.Models.TraderPerformanceOtherSports
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Trader performance for other sports model..
    /// </summary>
    [Serializable]
    public class TraderPerformanceOtherSports
    {
        public TraderPerformanceOtherSports()
        {
            ReportRows = new List<TraderPerformanceOtherSports>();
            ChildInfo = new List<TraderPerformanceOtherSports>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int MatchNumber { get; set; }

        public int Major { get; set; }

        public int Minor1 { get; set; }

        public int Minor2 { get; set; }

        public int Minor3 { get; set; }

        public int Others { get; set; }

        public decimal FirstHalfOU { get; set; }

        public decimal OU { get; set; }

        public decimal FirstHalfOE { get; set; }

        public decimal OE { get; set; }

        public decimal FirstHalfHdp { get; set; }

        public decimal Hdp { get; set; }

        public decimal FirstHalfOneXTwo { get; set; }

        public decimal OneXTwo { get; set; }

        public decimal Overall { get; set; }

        public List<TraderPerformanceOtherSports> ChildInfo { get; set; }

        public List<TraderPerformanceOtherSports> ReportRows { get; set; }

        public TraderPerformanceOtherSports GrandTotal { get; set; }

        /// <summary>
        /// Sums the specified trader information.
        /// </summary>
        /// <param name="traderInfo">The trader information.</param>
        public void CalculateTotal(TraderPerformanceOtherSports traderInfo)
        {
            this.FirstHalfHdp += traderInfo.FirstHalfHdp;
            this.Hdp += traderInfo.Hdp;
            this.FirstHalfOU += traderInfo.FirstHalfOU;
            this.OU += traderInfo.OU;
            this.FirstHalfOneXTwo += traderInfo.FirstHalfOneXTwo;
            this.OneXTwo += traderInfo.OneXTwo;
            this.FirstHalfOE += traderInfo.FirstHalfOE;
            this.OE += traderInfo.OE;
            this.Overall += traderInfo.Overall;
        }

        /// <summary>
        /// Calculates the margin total.
        /// </summary>
        /// <param name="winLossInfo">The win loss information.</param>
        /// <param name="turnOverInfo">The turn over information.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void CalculateMarginTotal(TraderPerformanceOtherSports winLossInfo, TraderPerformanceOtherSports turnOverInfo)
        {
            this.FirstHalfHdp = turnOverInfo.FirstHalfHdp == 0 ?
                0 :
                (winLossInfo.FirstHalfHdp / turnOverInfo.FirstHalfHdp) * 100;
            this.Hdp = turnOverInfo.Hdp == 0 ?
                0 :
                (winLossInfo.Hdp / turnOverInfo.Hdp) * 100;
            this.FirstHalfOU = turnOverInfo.FirstHalfOU == 0 ?
                0 :
                (winLossInfo.FirstHalfOU / turnOverInfo.FirstHalfOU) * 100;
            this.OU = turnOverInfo.OU == 0 ?
                0 :
                (winLossInfo.OU / turnOverInfo.OU) * 100;
            this.FirstHalfOneXTwo = turnOverInfo.FirstHalfOneXTwo == 0 ?
                0 :
                (winLossInfo.FirstHalfOneXTwo / turnOverInfo.FirstHalfOneXTwo) * 100;
            this.OneXTwo = turnOverInfo.OneXTwo == 0 ?
                0 :
                (winLossInfo.OneXTwo / turnOverInfo.OneXTwo) * 100;
            this.FirstHalfOE = turnOverInfo.FirstHalfOE == 0 ?
                0 :
                (winLossInfo.FirstHalfOE / turnOverInfo.FirstHalfOE) * 100;
            this.OE = turnOverInfo.OE == 0 ?
                0 :
                (winLossInfo.OE / turnOverInfo.OE) * 100;
            this.Overall = turnOverInfo.Overall == 0 ?
                0 :
                (winLossInfo.Overall / turnOverInfo.Overall) * 100;
        }

        /// <summary>
        /// Calculates the total match and league.
        /// </summary>
        /// <param name="matchNumber">The match number.</param>
        /// <param name="major">The major.</param>
        /// <param name="minor1">The minor1.</param>
        /// <param name="minor2">The minor2.</param>
        /// <param name="minor3">The minor3.</param>
        /// <param name="others">The others.</param>
        public void CalculateTotalMatchAndLeague(
            int matchNumber,
            int major,
            int minor1,
            int minor2,
            int minor3,
            int others)
        {
            this.MatchNumber += matchNumber;
            this.Major += major;
            this.Minor1 += minor1;
            this.Minor2 += minor2;
            this.Minor3 += minor3;
            this.Others += others;
        }

        /// <summary>
        /// Formats the two decimal number data.
        /// </summary>
        /// <param name="traderInfo">The trader information.</param>
        public void FormatTwoDecimalNumberData(TraderPerformanceOtherSports traderInfo)
        {
            this.FirstHalfHdp = Math.Round(traderInfo.FirstHalfHdp, 2, MidpointRounding.AwayFromZero);
            this.Hdp = Math.Round(traderInfo.Hdp, 2, MidpointRounding.AwayFromZero);
            this.FirstHalfOU = Math.Round(traderInfo.FirstHalfOU, 2, MidpointRounding.AwayFromZero);
            this.OU = Math.Round(traderInfo.OU, 2, MidpointRounding.AwayFromZero);
            this.FirstHalfOneXTwo = Math.Round(traderInfo.FirstHalfOneXTwo, 2, MidpointRounding.AwayFromZero);
            this.OneXTwo = Math.Round(traderInfo.OneXTwo, 2, MidpointRounding.AwayFromZero);
            this.FirstHalfOE = Math.Round(traderInfo.FirstHalfOE, 2, MidpointRounding.AwayFromZero);
            this.OE = Math.Round(traderInfo.OE, 2, MidpointRounding.AwayFromZero);
            this.Overall = Math.Round(traderInfo.Overall, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Gets the margin value.
        /// </summary>
        /// <param name="marginInfo">The margin information.</param>
        public void GetMarginValue(TraderPerformanceOtherSports marginInfo)
        {
            this.FirstHalfHdp = 100 * marginInfo.FirstHalfHdp;
            this.Hdp = 100 * marginInfo.Hdp;
            this.FirstHalfOU = 100 * marginInfo.FirstHalfOU;
            this.OU = 100 * marginInfo.OU;
            this.FirstHalfOneXTwo = 100 * marginInfo.FirstHalfOneXTwo;
            this.OneXTwo = 100 * marginInfo.OneXTwo;
            this.FirstHalfOE = 100 * marginInfo.FirstHalfOE;
            this.OE = 100 * marginInfo.OE;
            this.Overall = 100 * marginInfo.Overall;
        }
    }
}
