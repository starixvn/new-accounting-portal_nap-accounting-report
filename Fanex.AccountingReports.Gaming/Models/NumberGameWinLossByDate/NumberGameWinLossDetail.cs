namespace Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByDate
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class NumberGameWinLossDetail
    {
        public string Bettype { get; set; }

        public decimal CustomerWinloss { get; set; }

        public decimal AgencyWinloss { get; set; }

        public decimal Comm { get; set; }

        public decimal Turnover { get; set; }

        public decimal LineTotal { get; set; }

        public decimal VirtualTotal { get; set; }

        public decimal Variance { get; set; }

        public string Margin { get; set; }

        public string Status { get; set; }

        public string WinLossDate { get; set; }

        public NumberGameWinLossDetail GrandTotal { get; set; }

        public List<NumberGameWinLossDetail> ChildInfo { get; set; }

        public void AddToGrandTotal(NumberGameWinLossDetail winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            this.GrandTotal.AgencyWinloss += winLoss.AgencyWinloss;
            this.GrandTotal.CustomerWinloss += winLoss.CustomerWinloss;
            this.GrandTotal.Comm += winLoss.Comm;
            this.GrandTotal.LineTotal += winLoss.LineTotal;
            this.GrandTotal.VirtualTotal += winLoss.VirtualTotal;
            this.GrandTotal.Turnover += winLoss.Turnover;
            this.GrandTotal.Variance += winLoss.Variance;
        }
    }
}
