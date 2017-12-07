namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccer;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;

    public interface IDailyWinLoss2hSoccerService
    {
        WinLossInfo GetMain2hReport(GetMain2hReportCriteria criteria);

        void ExportMain2hReport(GetMain2hReportCriteria criteria);

        void ExportReport_DirectFilter(GetMain2hReportCriteria criteria);
    }
}
