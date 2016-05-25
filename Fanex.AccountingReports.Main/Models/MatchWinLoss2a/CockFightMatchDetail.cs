using Fanex.AccountingReports.Core.Resources;

namespace Fanex.AccountingReports.Main.Models.MatchWinLoss2a
{
    public class CockFightMatchDetail
    {
        public int FightNo { get; set; }

        public int ArenaId { get; set; }

        public string HomeOwner { get; set; }

        public string AwayOwner { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Position { get; set; }

        public decimal Commission { get; set; }

        public decimal VirtualCommission
        {
            get
            {
                return this.Commission;
            }
        }

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

        public string Margin_Str
        {
            get
            {
                var margin_Str = this.Margin == 0 ? Labels.NotAvailable : this.Margin.ToString();
                return margin_Str;
            }
        }
    }
}