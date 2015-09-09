namespace Fanex.AccountingReports.Special.Models.PunterAnalysis
{
    using System;

    public class Punter
    {
        public int SportType { get; set; }

        public string BetType { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Turnover { get; set; }

        public decimal Margin { get; set; }

        public short IsLive { get; set; }

        public int BetCount { get; set; }

        public decimal SortASC { get; set; }

        public decimal SortDESC { get; set; }
    }
}
