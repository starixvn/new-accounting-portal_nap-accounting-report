namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceOtherSports;

    public interface ITraderPerformanceOtherSportsService
    {
        TraderPerformanceOtherSports GetReport(GetReportCriteria criteria);

        TraderPerformanceOtherSports SortReportData(GetReportCriteria criteria);

        void ExportReport(GetReportCriteria criteria);

        TraderPerformanceOtherSportsDetail GetDetailReport(GetDetailReportCriteria criteria);

        TraderPerformanceOtherSportsDetail SortDetailReportData(GetDetailReportCriteria criteria);

        void ExportDetailReport(GetDetailReportCriteria criteria);
    }
}
