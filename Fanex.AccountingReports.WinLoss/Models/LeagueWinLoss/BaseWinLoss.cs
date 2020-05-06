using System;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
    [Serializable]
    public class BaseWinLoss
    {
        public decimal MemberWinLoss { get; set; }

        public decimal SMAWinLoss { get; set; }

        public decimal ExtraPT { get; set; }

        public decimal Commission { get; set; }

        public decimal RealIncome { get; set; }

        public decimal VirtualWinLoss { get; set; }

        public decimal VarianceIncome
        {
            get
            {
                return VirtualWinLoss - RealIncome;
            }
        }

        public decimal RealTurnover { get; set; }

        public decimal VirtualTurnover { get; set; }

        public decimal VirtualMargin
        {
            get
            {
                return VirtualTurnover != 0 ? VirtualWinLoss / VirtualTurnover : 0;
            }
        }

        public decimal RealMargin
        {
            get
            {
                return RealTurnover != 0 ? RealIncome / RealTurnover : 0;
            }
        }
    }
}
