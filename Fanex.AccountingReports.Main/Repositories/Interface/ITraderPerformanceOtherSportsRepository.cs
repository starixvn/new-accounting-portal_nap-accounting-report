namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceOtherSports;

    public interface ITraderPerformanceOtherSportsRepository
    {
        TraderPerformanceOtherSports GetTraderPerformanceOtherSportsReport(GetReportCriteria criteria);

        TraderPerformanceOtherSportsDetail GetTraderPerformanceOtherSportsDetailReport(GetDetailReportCriteria criteria);
    }
}
