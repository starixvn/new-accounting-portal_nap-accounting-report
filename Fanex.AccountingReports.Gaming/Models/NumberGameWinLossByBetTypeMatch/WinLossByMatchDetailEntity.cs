namespace Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossByMatchDetailEntity
    {
        public int CustomerId { get; set; }

        public string UserName { get; set; }

        public double CompanyTurnover { get; set; }

        public double GrossCommission { get; set; }

        public double MemberWinLoss { get; set; }

        public double MemberCommission { get; set; }

        public double MemberTotal { get; set; }

        public double AgentWinLoss { get; set; }

        public double AgentCommission { get; set; }

        public double AgentTotal { get; set; }

        public double MasterWinLoss { get; set; }

        public double MasterCommission { get; set; }

        public double MasterTotal { get; set; }

        public double SuperWinLoss { get; set; }

        public double SuperCommission { get; set; }

        public double SuperTotal { get; set; }

        public double CompanyTotal { get; set; }
    }
}
