namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System.Collections.Generic;

    public class NumberGameStatistic
    {
        public NumberGameStatistic()
        {
            ReportRows = new List<NumberGameStatistic>();
        }

        public string LeagueName { get; set; }

        public int Players { get; set; }

        public int BetCount { get; set; }

        public decimal TotalBets { get; set; }

        public decimal TotalWins { get; set; }

        public decimal GameIncome { get; set; }

        public decimal GamePayout { get; set; }

        public decimal CompanyTurnover { get; set; }

        public decimal NetIncome { get; set; }

        public List<NumberGameStatistic> ReportRows { get; set; }

        public NumberGameStatistic GrandTotal { get; set; }

        public void CalCulateGrandTotal(NumberGameStatistic item)
        {
            this.GrandTotal.BetCount += item.BetCount;
            this.GrandTotal.TotalBets += item.TotalBets;
            this.GrandTotal.TotalWins += item.TotalWins;
            this.GrandTotal.GameIncome += item.GameIncome;
            this.GrandTotal.CompanyTurnover += item.CompanyTurnover;
            this.GrandTotal.NetIncome += item.NetIncome;
        }
    }
}