namespace Fanex.AccountingReports.Main.Models.TraderPerformanceOtherSports
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [Serializable]
    public class TraderPerformanceOtherSportsDetail
    {
        public TraderPerformanceOtherSportsDetail()
        {
            ReportRows = new List<TraderPerformanceOtherSportsDetail>();
            ChildInfo = new List<TraderPerformanceOtherSportsDetail>();
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

        public decimal FirstHalfOE { get; set; }

        public decimal OE { get; set; }

        public decimal FirstHalfHdp { get; set; }

        public decimal Hdp { get; set; }

        public decimal FirstHalfOneXTwo { get; set; }

        public decimal OneXTwo { get; set; }

        public decimal Overall { get; set; }

        public decimal Maxbet { get; set; }

        public List<TraderPerformanceOtherSportsDetail> ChildInfo { get; set; }

        public List<TraderPerformanceOtherSportsDetail> ReportRows { get; set; }

        public TraderPerformanceOtherSportsDetail GrandTotal { get; set; }

        public int ReportLength { get; set; }

        public void CalculateTotal(TraderPerformanceOtherSportsDetail traderInfo)
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

        
        public void CalculateMarginTotal(TraderPerformanceOtherSportsDetail winLossInfo, TraderPerformanceOtherSportsDetail turnOverInfo)
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

        public void FormatTwoDecimalNumberData(TraderPerformanceOtherSportsDetail traderInfo)
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

        public void GetMarginValue(TraderPerformanceOtherSportsDetail marginInfo)
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
