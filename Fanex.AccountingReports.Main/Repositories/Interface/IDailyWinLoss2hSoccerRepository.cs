namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccer;

    public interface IDailyWinLoss2hSoccerRepository
    {
        List<dynamic> GetMain2hReport(GetMain2hReportCriteria criteria, ref Hashtable outputParam);
    }
}
