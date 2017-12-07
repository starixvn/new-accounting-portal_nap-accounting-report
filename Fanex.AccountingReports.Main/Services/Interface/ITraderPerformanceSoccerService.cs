namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceSoccer;

    public interface ITraderPerformanceSoccerService
    {
        TraderPerformanceSoccer GetReport(GetReportCriteria criteria);

        TraderPerformanceSoccer SortReportData(GetReportCriteria criteria);

        void ExportReport(GetReportCriteria criteria);

        TraderPerformanceSoccerDetail GetDetailReport(GetDetailReportCriteria criteria);

        TraderPerformanceSoccerDetail SortDetailReportData(GetDetailReportCriteria criteria);

        void ExportDetailReport(GetDetailReportCriteria criteria);
    }
}
