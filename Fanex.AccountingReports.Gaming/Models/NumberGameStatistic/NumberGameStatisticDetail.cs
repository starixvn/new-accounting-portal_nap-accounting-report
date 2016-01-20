namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System;

    [Serializable]
    public class NumberGameStatisticDetail
    {
        public int BetCount { get; set; }

        public decimal TotalBets { get; set; }

        public decimal TotalWins { get; set; }

        public decimal GameIncome { get; set; }

        public decimal GamePayout { get; set; }

        public decimal CompanyTurnover { get; set; }

        public decimal NetIncome { get; set; }
    }
}