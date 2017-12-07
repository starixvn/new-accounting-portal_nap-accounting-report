namespace Fanex.AccountingReports.WinLoss.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss;

    public interface IDailyMixParlayWinLossRepository
    {
        List<dynamic> GetCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria);
    }
}
