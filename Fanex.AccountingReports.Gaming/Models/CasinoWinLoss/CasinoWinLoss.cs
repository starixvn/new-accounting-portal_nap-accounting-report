namespace Fanex.AccountingReports.Gaming.Models.CasinoWinLoss
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CasinoWinLoss
    {
        public CasinoWinLoss()
        {
            Name = string.Empty;
            Rows = new List<CasinoWinLoss>();
        }

        public string Name { get; set; }

        public decimal Turnover { get; set; }

        public decimal WinLost { get; set; }

        public decimal Position { get; set; }

        public decimal Commission { get; set; }

        public decimal LineTotal { get; set; }

        public List<CasinoWinLoss> Rows { get; set; }

        public void SumTotal(CasinoWinLoss winLost)
        {
            Turnover += winLost.Turnover;
            WinLost += winLost.WinLost;
            Position += winLost.Position;
            Commission += winLost.Commission;
            LineTotal += winLost.LineTotal;
        }
    }
}
