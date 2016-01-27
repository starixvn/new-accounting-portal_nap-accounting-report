namespace Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2h
{
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;

    public interface ICockFightingService
    {
        WinLossInfo GetCockFightingReport(GetCockFightingCriteria criteria);

        void ExportCockFightingReport(GetCockFightingCriteria criteria);

        CockFightingArena GetCockFightingArenaReport(GetCockFightingArenaCriteria criteria);

        void ExportCockFightingArenaReport(GetCockFightingArenaCriteria criteria);
    }
}