namespace Fanex.AccountingReports.Main.Models.MatchWinLoss2a
{
    using System.Collections.Generic;

    public class CockFighting
    {
        public List<CockFightingDetail> ReportRows { get; set; }

        public CockFightingDetail GrandTotal { get; set; }
    }
}