namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hOtherSports;

    public interface IDailyWinLoss2hOtherSportsRepository
    {
        List<dynamic> GetMain2hReport(GetMain2hReportCriteria criteria, ref Hashtable outputParam);
    }
}
