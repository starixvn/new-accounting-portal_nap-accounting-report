using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.AccountingReports.WinLoss.Entities.LeagueWinLoss
{
    public class SportEntity
    {
        public int SportGroupType { get; set; }

        public string SportGroupName { get; set; }

        public int SportType { get; set; }

        public string SportName { get; set; }

        public int Bettype { get; set; }

        public string BettypeName { get; set; }
    }
}
