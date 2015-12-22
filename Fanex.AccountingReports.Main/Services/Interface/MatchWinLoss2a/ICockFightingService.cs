namespace Fanex.AccountingReports.Main.Services.Interface.MatchWinLoss2a
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;

    public interface ICockFightingService
    {
        CockFighting GetCockFightingReport(GetCockFightingCriteria criteria);

        CockFightingDetail CalculateGrandTotal(List<CockFightingDetail> report);

        CockFightingMatchByArena GetCockFightingMatchByArenaReport(GetCockFightingMatchByArena criteria);

        CockFightMatchDetail CalculateGrandTotalByArena(List<CockFightMatchDetail> report);
    }
}