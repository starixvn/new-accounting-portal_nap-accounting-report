namespace Fanex.AccountingReports.Main.Models.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CockFightingArena
    {
        public List<CockFightingArenaDetail> ReportRows { get; set; }

        public CockFightingMatchDetail GrandTotal { get; set; }
    }
}