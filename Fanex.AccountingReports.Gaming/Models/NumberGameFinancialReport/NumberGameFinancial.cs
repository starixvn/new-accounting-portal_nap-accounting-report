namespace Fanex.AccountingReports.Gaming.Models.NumberGameFinancial
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class NumberGameFinancial
    {
        public string ReportDate { get; set; }

        public string Bettype { get; set; }

        public bool IsLive { get; set; }

        public string Status { get; set; }
        
        public int NumberPlayer { get; set; }
        
        public int NumberGame { get; set; }

        public decimal TotalBet { get; set; }

        public decimal TotalWin { get; set; }

        public decimal GameIncome { get; set; }

        public string Payout { get; set; }

        public List<NumberGameFinancial> ReportRows { get; set; }

        public void CalculateGrandTotal(NumberGameFinancial reportInfo)
        {
            if (reportInfo == null)
            {
                return;
            }

            this.TotalBet += reportInfo.TotalBet;
            this.TotalWin += reportInfo.TotalWin;
        }
    }
}
