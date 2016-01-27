namespace Fanex.AccountingReports.Main.Models.WinLossSummary3rd
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CockFightingArenaDetail
    {
        public string ArenaName { get; set; }

        public List<CockFightingMatchDetail> CockFightingMatchs { get; set; }

        public CockFightingMatchDetail ArenaTotal { get; set; }
    }
}