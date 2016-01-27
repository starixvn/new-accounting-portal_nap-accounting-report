namespace Fanex.AccountingReports.Main.Models.DailyWinLoss2h
{
    using System;

    [Serializable]
    public class CockFightingMatchDetail
    {
        public int FightNo { get; set; }

        public int ArenaId { get; set; }

        public string HomeOwner { get; set; }

        public string AwayOwner { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Position { get; set; }

        public decimal Commission { get; set; }

        public decimal VirtualCommission { get; set; }

        public decimal RealLineTotal { get; set; }

        public decimal VirtualLineTotal { get; set; }

        public decimal Variance
        {
            get
            {
                return this.RealLineTotal - this.VirtualLineTotal;
            }
        }

        public decimal RealTurnover { get; set; }

        public decimal Margin
        {
            get
            {
                var margin = this.RealTurnover == 0 ? 0 : (this.RealLineTotal / this.RealTurnover);
                return margin;
            }
        }
    }
}