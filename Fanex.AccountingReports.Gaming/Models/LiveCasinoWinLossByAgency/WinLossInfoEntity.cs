namespace Fanex.AccountingReports.Gaming.Models.LiveCasinoWinLossByAgency
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfoEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CurrencyName { get; set; }

        public decimal RollingTurnover { get; set; }

        public decimal CompanyTurnover { get; set; }

        public decimal GrossCommission { get; set; }

        public decimal TotalBets { get; set; }

        public decimal MemberWinLoss { get; set; }

        public decimal MemberCommission { get; set; }

        public decimal AgentCommission { get; set; }

        public decimal AgentWinLoss { get; set; }

        public decimal MasterWinLoss { get; set; }

        public decimal MasterCommission { get; set; }

        public decimal SuperWinLoss { get; set; }

        public decimal SuperCommission { get; set; }

        public decimal CompanyWinLoss { get; set; }
    }
}
