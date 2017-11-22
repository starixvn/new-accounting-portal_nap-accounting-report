namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceSoccer;

    public interface ITraderPerformanceSoccerRepository
    {
        TraderPerformanceSoccer GetTraderPerformanceSoccerReport(GetReportCriteria criteria);

        TraderPerformanceSoccerDetail GetTraderPerformanceSoccerDetailReport(GetDetailReportCriteria criteria);
    }
}
