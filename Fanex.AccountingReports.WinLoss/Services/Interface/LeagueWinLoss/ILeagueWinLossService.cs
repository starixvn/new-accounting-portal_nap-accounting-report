namespace Fanex.AccountingReports.WinLoss.Services.Interface
{
    using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss;

    public interface ILeagueWinLossService
    {
        LeagueWinLossInfo GetLeagueWinLoss(GetLeagueWinLossCriteria criteria);

        LeagueWinLossInfo GetLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria);

        LeagueWinLossInfo GetLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria);        
    }
}
