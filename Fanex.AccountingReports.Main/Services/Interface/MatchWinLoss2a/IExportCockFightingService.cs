namespace Fanex.AccountingReports.Main.Services.Interface.MatchWinLoss2a
{
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;

    public interface IExportCockFightingService
    {
        void ExportCockFightingReport(GetCockFightingCriteria criteria);

        void ExportCockFightingMatchByArenaReport(GetCockFightingMatchByArena criteria);
    }
}