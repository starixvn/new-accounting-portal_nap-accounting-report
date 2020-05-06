using System;

namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    [Serializable]
    public class LeagueWinLossEntity
    {
        public int SportType { get; set; }

        public string Sport { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public decimal MemberWinLoss { get; set; }

        public decimal SMAWinLoss { get; set; }

        public decimal ExtraPT { get; set; }

        public decimal Commission { get; set; }

        public decimal RealIncome { get; set; }

        public decimal VirtualWinLoss { get; set; }

        public decimal RealTurnover { get; set; }

        public decimal VirtualTurnover { get; set; }
    }
}
