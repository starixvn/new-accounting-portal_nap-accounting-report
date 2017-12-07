using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;

namespace Fanex.AccountingReports.WinLoss.Services.Interface.LeagueWinLoss
{
    public interface ILeagueWinLossExportExcel
    {
        void ExportLeagueWinLoss(GetLeagueWinLossCriteria criteria);

        void ExportLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria);

        void ExportLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria);
    }
}
