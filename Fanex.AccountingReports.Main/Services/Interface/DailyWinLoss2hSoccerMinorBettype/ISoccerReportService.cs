namespace Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2hSoccerMinorBettype
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.Excel.Models;

    public interface ISoccerReportService
    {
        List<GridOptions> GetReport(GetSoccerReportCriteria criteria, bool isCached = false);
    }
}
