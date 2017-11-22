namespace Fanex.AccountingReports.WinLoss.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.Data.Repository;

    public interface ILeagueWinLossRepository
    {
        List<dynamic> GetLeagueWinLoss(ICriteria criteria, ref Hashtable outputParam);

        List<dynamic> GetLeagueWinLossBySport(
            ICriteria criteria,
            out List<dynamic> eightRMTable, 
            out List<dynamic> barmTable);

        List<dynamic> GetLeagueWinLossByBetType(ICriteria criteria, ref Hashtable outputParam);
    }
}
