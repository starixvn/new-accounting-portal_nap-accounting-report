namespace Fanex.AccountingReports.Special.Services.Interface.DailyTurnover
{
    using Fanex.AccountingReports.Special.Criteria.DailyTurnover;

    /// <summary>
    /// The interface of export report service of detail turnover.
    /// </summary>
    public interface IDailyDetailChangedTurnoverExportReportService
    {
        void ExportDetailReport(GetDailyTurnoverChangedDetailCriteria criteria);

        void ExportDetailDifferenceReport(string index, string fromDate);
    }
}
