namespace Fanex.AccountingReports.Main.Models.MatchWinLoss2a
{
    using Fanex.AccountingReports.Core.Services.Impl;

    public class CockFightingDetail
    {
        public int ArenaId { get; set; }

        public string ArenaName
        {
            get
            {
                return CacheRef.GetLeagueName(this.ArenaId);
            }
        }

        public decimal WinLoss { get; set; }

        public decimal Position { get; set; }

        public decimal Commission { get; set; }

        public decimal VirtualCommission { get; set; }

        public decimal RealLineTotal { get; set; }

        public decimal VirtualLineTotal { get; set; }

        public decimal RealTurnover { get; set; }

        public decimal Variance
        {
            get
            {
                return this.RealLineTotal - this.VirtualLineTotal;
            }
        }

        public decimal Margin
        {
            get
            {
                var margin = this.RealTurnover == 0 ? 0 : (this.WinLoss / this.RealTurnover);
                return margin;
            }
        }
    }
}