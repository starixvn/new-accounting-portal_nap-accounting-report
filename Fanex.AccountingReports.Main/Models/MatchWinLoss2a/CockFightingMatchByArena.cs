namespace Fanex.AccountingReports.Main.Models.MatchWinLoss2a
{
    using System.Collections.Generic;

    public class CockFightingMatchByArena
    {
        public List<CockFightMatchDetail> ReportRows { get; set; }

        public CockFightMatchDetail GrandTotal { get; set; }
    }
}