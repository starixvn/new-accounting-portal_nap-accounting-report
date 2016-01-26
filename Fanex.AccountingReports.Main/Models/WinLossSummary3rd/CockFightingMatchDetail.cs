namespace Fanex.AccountingReports.Main.Models.WinLossSummary3rd
{
    using System;

    [Serializable]
    public class CockFightingMatchDetail
    {
        public int ArenaId { get; set; }

        public int FightNo { get; set; }

        public string HomeOwner { get; set; }

        public string AwayOwner { get; set; }

        public decimal RealWinLoss { get; set; }

        public decimal RealPosition { get; set; }

        public decimal RealCommission { get; set; }

        public decimal RealTurnover { get; set; }

        public decimal RealLineTotal { get; set; }

        public decimal VirtualTurnover { get; set; }

        public decimal VirtualLineTotal { get; set; }

        public decimal Variance
        {
            get
            {
                return this.RealLineTotal - this.VirtualLineTotal;
            }
        }

        public decimal RealMargin
        {
            get
            {
                var margin = this.RealTurnover == 0 ? 0 : (this.RealLineTotal / this.RealTurnover);
                return margin;
            }
        }

        public decimal VirtualMargin
        {
            get
            {
                var margin = this.VirtualTurnover == 0 ? 0 : (this.VirtualLineTotal / this.VirtualTurnover);
                return margin;
            }
        }
    }
}