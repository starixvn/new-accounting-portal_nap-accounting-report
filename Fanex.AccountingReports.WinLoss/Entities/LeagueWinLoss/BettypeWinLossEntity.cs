using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    public class BettypeWinLossEntity : BaseEntity
    {
        public bool LiveIndicator { get; set; }

        public int Bettype { get; set; }

        public string BettypeName { get; set; }

        public bool IsHedge { get; set; }
    }
}
