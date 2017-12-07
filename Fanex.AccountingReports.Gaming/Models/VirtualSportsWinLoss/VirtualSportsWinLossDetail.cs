namespace Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss
{
    using System;

    /// <summary>
    /// The model of Virtual Sports Win Loss detail report.
    /// </summary>
    [Serializable]
    public class VirtualSportsWinLossDetail
    {
        public string Name { get; set; }

        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public decimal MemberWinLoss { get; set; }

        public decimal PositionWinLoss { get; set; }

        public decimal CompanyWinLoss { get; set; }

        public decimal AmountDueInspired { get; set; }

        public void SumTotal(VirtualSportsWinLossDetail currency)
        {
            this.BetCount += currency.BetCount;
            this.Turnover += currency.Turnover;
            this.MemberWinLoss += currency.MemberWinLoss;
            this.PositionWinLoss += currency.PositionWinLoss;
            this.CompanyWinLoss += currency.CompanyWinLoss;
            this.AmountDueInspired += currency.AmountDueInspired;
        }
    }
}
