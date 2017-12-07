namespace Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByCurrency
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NumberGameWinLossByCurrencyMain
    {
        public string Currency { get; set; }

        public decimal Turnover { get; set; }

        public decimal MemberWinLoss { get; set; }

        public decimal GamePayout { get; set; }

        public decimal NetTurnover { get; set; }

        public decimal PTWinLoss { get; set; }

        public decimal CommExpense { get; set; }

        public decimal NetIncome { get; set; }

        public decimal Margin { get; set; }
    }
}
