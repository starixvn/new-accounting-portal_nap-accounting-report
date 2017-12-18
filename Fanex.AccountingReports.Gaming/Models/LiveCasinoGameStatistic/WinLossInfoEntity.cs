namespace Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfoEntity
    {
        public string Id { get; set; }

        public string CustId { get; set; }

        public int NumberOfPlayers { get; set; }

        public int NumberOfGames { get; set; }

        public string CurrencyName { get; set; }

        public decimal TotalBets { get; set; }

        public decimal TotalWins { get; set; }

        public decimal TotalRefund { get; set; }

        public decimal GameIncome { get; set; }

        public decimal GamePayout { get; set; }

        public decimal CompanyTurnover { get; set; }

        public decimal CompanyNetIncome { get; set; }

        public decimal CompanyCommission { get; set; }

        public decimal TotalBetsRM { get; set; }

        public decimal TotalWinsRM { get; set; }

        public decimal GameIncomeRM { get; set; }

        public decimal GamePayoutRM { get; set; }

        public decimal TotalRefundRM { get; set; }

        public decimal CompanyTurnoverRM { get; set; }

        public decimal CompanyNetIncomeRM { get; set; }

        public decimal CompanyCommissionRM { get; set; }
    }
}