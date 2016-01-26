namespace Fanex.AccountingReports.Main.Models.WinLossSummary3rd
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