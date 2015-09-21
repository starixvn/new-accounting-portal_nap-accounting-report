namespace Fanex.AccountingReports.Misc.Models.MemberWinLossOnRNGCasino
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemberWinLoss
    {
        public MemberWinLoss()
        {
            Member = string.Empty;
            CurrencyName = string.Empty;
            TransactionTime = string.Empty;
            Agent = string.Empty;
        }

        public string Member { get; set; }

        public int Currency { get; set; }

        public string CurrencyName { get; set; }

        public string Agent { get; set; }

        public decimal WinLoss { get; set; }

        public decimal WinLossRM { get; set; }

        public int NumberOfBet { get; set; }

        public string TransactionTime { get; set; }
    }
}
