namespace Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2hSoccerMinorBettype
{
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.Excel.Models;

    public interface IBettypeReportService
    {
        GridOptions GetReport(GetReportByBetTypeCriteria criteria, bool isCached = false);
    }
}
