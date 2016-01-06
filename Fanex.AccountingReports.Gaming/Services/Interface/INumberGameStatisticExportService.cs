namespace Fanex.AccountingReports.Gaming.Services.Interface
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic;

    public interface INumberGameStatisticExportService
    {
        void ExportReportByDaily(GetNumberGameStatisticCriteria criteria);

        void ExportReportByUserName(GetNumberGameStatisticCriteria criteria);

        void ExportReportByLeague(GetNumberGameStatisticCriteria criteria);

        void ExportReportByBettype(GetNumberGameStatisticCriteria criteria);
    }
}