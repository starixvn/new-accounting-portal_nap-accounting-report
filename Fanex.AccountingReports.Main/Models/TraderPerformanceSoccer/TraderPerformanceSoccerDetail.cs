// -----------------------------------------------------------------------
// <copyright file="TraderPerformanceSoccerDetail.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.01.28">Created</change>
// </history>
namespace Fanex.AccountingReports.Main.Models.TraderPerformanceSoccer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Trader performance soccer detail model.
    /// </summary>
    [Serializable]
    public class TraderPerformanceSoccerDetail
    {
        public TraderPerformanceSoccerDetail()
        {
            ReportRows = new List<TraderPerformanceSoccerDetail>();
            ChildInfo = new List<TraderPerformanceSoccerDetail>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string EventDate { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public int HomeId { get; set; }

        public string HomeName { get; set; }

        public int AwayId { get; set; }

        public string AwayName { get; set; }

        public bool IsNeutral { get; set; }

        public string HTHomeScore { get; set; }

        public string HTAwayScore { get; set; }

        public string FTHomeScore { get; set; }

        public string FTAwayScore { get; set; }

        public decimal FirstHalfOU { get; set; }

        public decimal OU { get; set; }

        public decimal FifteenMinOU { get; set; }

        public decimal TenMinOU { get; set; }

        public decimal SingleTeamOU { get; set; }

        public decimal FirstHalfOE { get; set; }

        public decimal OE { get; set; }

        public decimal FirstHalfHdp { get; set; }

        public decimal Hdp { get; set; }

        public decimal FifteenMinHdp { get; set; }

        public decimal FirstHalfOneXTwo { get; set; }

        public decimal OneXTwo { get; set; }

        public decimal FifteenMinOneXTwo { get; set; }

        public decimal FirstHalfNoOfCorners { get; set; }

        public decimal NoOfCorners { get; set; }

        public decimal FifteenMinNoOfCorners { get; set; }

        public decimal NextCorner { get; set; }

        public decimal FirstHalfTotalBookings { get; set; }

        public decimal TotalBookings { get; set; }

        public decimal TotalBookingsFifteenMinOU { get; set; }

        public decimal NextBookings { get; set; }

        public decimal Overall { get; set; }

        public decimal Maxbet { get; set; }

        public decimal TenMinHdp { get; set; }

        public decimal TenMinOneXTwo { get; set; }

        public List<TraderPerformanceSoccerDetail> ChildInfo { get; set; }

        public List<TraderPerformanceSoccerDetail> ReportRows { get; set; }

        public TraderPerformanceSoccerDetail GrandTotal { get; set; }

        public int ReportLength { get; set; }

        /// <summary>
        /// Sums the specified trader information.
        /// </summary>
        /// <param name="traderInfo">The trader information.</param>
        public void CalculateTotal(TraderPerformanceSoccerDetail traderInfo)
        {
            this.FirstHalfHdp += traderInfo.FirstHalfHdp;
            this.Hdp += traderInfo.Hdp;
            this.FifteenMinHdp += traderInfo.FifteenMinHdp;
            this.FirstHalfOU += traderInfo.FirstHalfOU;
            this.OU += traderInfo.OU;
            this.FirstHalfOneXTwo += traderInfo.FirstHalfOneXTwo;
            this.OneXTwo += traderInfo.OneXTwo;
            this.FifteenMinOneXTwo += traderInfo.FifteenMinOneXTwo;
            this.FirstHalfNoOfCorners += traderInfo.FirstHalfNoOfCorners;
            this.FifteenMinNoOfCorners += traderInfo.FifteenMinNoOfCorners;
            this.NoOfCorners += traderInfo.NoOfCorners;
            this.FifteenMinOU += traderInfo.FifteenMinOU;
            this.TenMinOU += traderInfo.TenMinOU;
            this.SingleTeamOU += traderInfo.SingleTeamOU;
            this.FirstHalfTotalBookings += traderInfo.FirstHalfTotalBookings;
            this.TotalBookings += traderInfo.TotalBookings;
            this.TotalBookingsFifteenMinOU += traderInfo.TotalBookingsFifteenMinOU;
            this.NextBookings += traderInfo.NextBookings;
            this.FirstHalfOE += traderInfo.FirstHalfOE;
            this.OE += traderInfo.OE;
            this.NextCorner += traderInfo.NextCorner;
            this.Overall += traderInfo.Overall;
            this.TenMinHdp += traderInfo.TenMinHdp;
            this.TenMinOneXTwo += traderInfo.TenMinOneXTwo;
        }

        /// <summary>
        /// Calculates the margin total.
        /// </summary>
        /// <param name="winLossInfo">The win loss information.</param>
        /// <param name="turnOverInfo">The turn over information.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void CalculateMarginTotal(TraderPerformanceSoccerDetail winLossInfo, TraderPerformanceSoccerDetail turnOverInfo)
        {
            this.FirstHalfHdp = turnOverInfo.FirstHalfHdp == 0 ?
                0 :
                (winLossInfo.FirstHalfHdp / turnOverInfo.FirstHalfHdp) * 100;
            this.Hdp = turnOverInfo.Hdp == 0 ?
                0 :
                (winLossInfo.Hdp / turnOverInfo.Hdp) * 100;
            this.FifteenMinHdp = turnOverInfo.FifteenMinHdp == 0 ?
                0 :
                (winLossInfo.FifteenMinHdp / turnOverInfo.FifteenMinHdp) * 100;
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
            this.FifteenMinOneXTwo = turnOverInfo.FifteenMinOneXTwo == 0 ?
                0 :
                (winLossInfo.FifteenMinOneXTwo / turnOverInfo.FifteenMinOneXTwo) * 100;
            this.FirstHalfNoOfCorners = turnOverInfo.FirstHalfNoOfCorners == 0 ?
                0 :
                (winLossInfo.FirstHalfNoOfCorners / turnOverInfo.FirstHalfNoOfCorners) * 100;
            this.FifteenMinNoOfCorners = turnOverInfo.FifteenMinNoOfCorners == 0 ?
                0 :
                (winLossInfo.FifteenMinNoOfCorners / turnOverInfo.FifteenMinNoOfCorners) * 100;
            this.NoOfCorners = turnOverInfo.NoOfCorners == 0 ?
                0 :
                (winLossInfo.NoOfCorners / turnOverInfo.NoOfCorners) * 100;
            this.FifteenMinOU = turnOverInfo.FifteenMinOU == 0 ?
                0 :
                (winLossInfo.FifteenMinOU / turnOverInfo.FifteenMinOU) * 100;
            this.TenMinOU = turnOverInfo.TenMinOU == 0 ?
                0 :
                (winLossInfo.TenMinOU / turnOverInfo.TenMinOU) * 100;
            this.SingleTeamOU = turnOverInfo.SingleTeamOU == 0 ?
                0 :
                (winLossInfo.SingleTeamOU / turnOverInfo.SingleTeamOU) * 100;
            this.FirstHalfTotalBookings = turnOverInfo.FirstHalfTotalBookings == 0 ?
                0 :
                (winLossInfo.FirstHalfTotalBookings / turnOverInfo.FirstHalfTotalBookings) * 100;
            this.TotalBookings = turnOverInfo.TotalBookings == 0 ?
                0 :
                (winLossInfo.TotalBookings / turnOverInfo.TotalBookings) * 100;
            this.TotalBookingsFifteenMinOU = turnOverInfo.TotalBookingsFifteenMinOU == 0 ?
                0 :
                (winLossInfo.TotalBookingsFifteenMinOU / turnOverInfo.TotalBookingsFifteenMinOU) * 100;
            this.NextBookings = turnOverInfo.NextBookings == 0 ?
                0 :
                (winLossInfo.NextBookings / turnOverInfo.NextBookings) * 100;
            this.FirstHalfOE = turnOverInfo.FirstHalfOE == 0 ?
                0 :
                (winLossInfo.FirstHalfOE / turnOverInfo.FirstHalfOE) * 100;
            this.OE = turnOverInfo.OE == 0 ?
                0 :
                (winLossInfo.OE / turnOverInfo.OE) * 100;
            this.NextCorner = turnOverInfo.NextCorner == 0 ?
                0 :
                (winLossInfo.NextCorner / turnOverInfo.NextCorner) * 100;
            this.Overall = turnOverInfo.Overall == 0 ?
                0 :
                (winLossInfo.Overall / turnOverInfo.Overall) * 100;
            this.TenMinHdp = turnOverInfo.TenMinHdp == 0 ?
                0 :
                (winLossInfo.TenMinHdp / turnOverInfo.TenMinHdp) * 100;
            this.TenMinOneXTwo = turnOverInfo.TenMinOneXTwo == 0 ?
                0 :
                (winLossInfo.TenMinOneXTwo / turnOverInfo.TenMinOneXTwo) * 100; 
        }

        /// <summary>
        /// Formats the two decimal number data.
        /// </summary>
        /// <param name="traderInfo">The trader information.</param>
        public void FormatTwoDecimalNumberData(TraderPerformanceSoccerDetail traderInfo)
        {
            this.FirstHalfHdp = Math.Round(traderInfo.FirstHalfHdp, 2, MidpointRounding.AwayFromZero);
            this.Hdp = Math.Round(traderInfo.Hdp, 2, MidpointRounding.AwayFromZero);
            this.FifteenMinHdp = Math.Round(traderInfo.FifteenMinHdp, 2, MidpointRounding.AwayFromZero);
            this.FirstHalfOU = Math.Round(traderInfo.FirstHalfOU, 2, MidpointRounding.AwayFromZero);
            this.OU = Math.Round(traderInfo.OU, 2, MidpointRounding.AwayFromZero);
            this.FirstHalfOneXTwo = Math.Round(traderInfo.FirstHalfOneXTwo, 2, MidpointRounding.AwayFromZero);
            this.OneXTwo = Math.Round(traderInfo.OneXTwo, 2, MidpointRounding.AwayFromZero);
            this.FifteenMinOneXTwo = Math.Round(traderInfo.FifteenMinOneXTwo, 2, MidpointRounding.AwayFromZero);
            this.FirstHalfNoOfCorners = Math.Round(traderInfo.FirstHalfNoOfCorners, 2, MidpointRounding.AwayFromZero);
            this.FifteenMinNoOfCorners = Math.Round(traderInfo.FifteenMinNoOfCorners, 2, MidpointRounding.AwayFromZero);
            this.NoOfCorners = Math.Round(traderInfo.NoOfCorners, 2, MidpointRounding.AwayFromZero);
            this.FifteenMinOU = Math.Round(traderInfo.FifteenMinOU, 2, MidpointRounding.AwayFromZero);
            this.TenMinOU = Math.Round(traderInfo.TenMinOU, 2, MidpointRounding.AwayFromZero);
            this.SingleTeamOU = Math.Round(traderInfo.SingleTeamOU, 2, MidpointRounding.AwayFromZero);
            this.FirstHalfTotalBookings = Math.Round(traderInfo.FirstHalfTotalBookings, 2, MidpointRounding.AwayFromZero);
            this.TotalBookings = Math.Round(traderInfo.TotalBookings, 2, MidpointRounding.AwayFromZero);
            this.TotalBookingsFifteenMinOU = Math.Round(traderInfo.TotalBookingsFifteenMinOU, 2, MidpointRounding.AwayFromZero);
            this.NextBookings = Math.Round(traderInfo.NextBookings, 2, MidpointRounding.AwayFromZero);
            this.FirstHalfOE = Math.Round(traderInfo.FirstHalfOE, 2, MidpointRounding.AwayFromZero);
            this.OE = Math.Round(traderInfo.OE, 2, MidpointRounding.AwayFromZero);
            this.NextCorner = Math.Round(traderInfo.NextCorner, 2, MidpointRounding.AwayFromZero);
            this.Overall = Math.Round(traderInfo.Overall, 2, MidpointRounding.AwayFromZero);
            this.TenMinHdp = Math.Round(traderInfo.TenMinHdp, 2, MidpointRounding.AwayFromZero);
            this.TenMinOneXTwo = Math.Round(traderInfo.TenMinOneXTwo, 2, MidpointRounding.AwayFromZero); 
        }

        /// <summary>
        /// Gets the margin value.
        /// </summary>
        /// <param name="marginInfo">The margin information.</param>
        public void GetMarginValue(TraderPerformanceSoccerDetail marginInfo)
        {
            this.FirstHalfHdp = 100 * marginInfo.FirstHalfHdp;
            this.Hdp = 100 * marginInfo.Hdp;
            this.FifteenMinHdp = 100 * marginInfo.FifteenMinHdp;
            this.FirstHalfOU = 100 * marginInfo.FirstHalfOU;
            this.OU = 100 * marginInfo.OU;
            this.FirstHalfOneXTwo = 100 * marginInfo.FirstHalfOneXTwo;
            this.OneXTwo = 100 * marginInfo.OneXTwo;
            this.FifteenMinOneXTwo = 100 * marginInfo.FifteenMinOneXTwo;
            this.FirstHalfNoOfCorners = 100 * marginInfo.FirstHalfNoOfCorners;
            this.FifteenMinNoOfCorners = 100 * marginInfo.FifteenMinNoOfCorners;
            this.NoOfCorners = 100 * marginInfo.NoOfCorners;
            this.FifteenMinOU = 100 * marginInfo.FifteenMinOU;
            this.TenMinOU = 100 * marginInfo.TenMinOU;
            this.SingleTeamOU = 100 * marginInfo.SingleTeamOU;
            this.FirstHalfTotalBookings = 100 * marginInfo.FirstHalfTotalBookings;
            this.TotalBookings = 100 * marginInfo.TotalBookings;
            this.TotalBookingsFifteenMinOU = 100 * marginInfo.TotalBookingsFifteenMinOU;
            this.NextBookings = 100 * marginInfo.NextBookings;
            this.FirstHalfOE = 100 * marginInfo.FirstHalfOE;
            this.OE = 100 * marginInfo.OE;
            this.NextCorner = 100 * marginInfo.NextCorner;
            this.Overall = 100 * marginInfo.Overall;
            this.TenMinOneXTwo = 100 * marginInfo.TenMinOneXTwo;
            this.TenMinHdp = 100 * marginInfo.TenMinHdp; 
        }
    }
}
