namespace Fanex.AccountingReports.Main.Models.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CockFightingArenaDetail
    {
        public string ArenaName { get; set; }

        public List<CockFightingMatchDetail> CockFightingMatchs { get; set; }
    }
}