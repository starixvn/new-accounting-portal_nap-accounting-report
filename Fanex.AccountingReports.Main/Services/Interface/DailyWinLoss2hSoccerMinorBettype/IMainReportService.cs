namespace Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2hSoccerMinorBettype
{
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.Excel.Models;

    public interface IMainReportService
    {
        GridOptions GetReport(GetMainReportCriteria criteria, bool isCached = false);
    }
}
