namespace Fanex.AccountingReports.Racing.Services.Interface.RacingGroupDaily
{
    using Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily;

    /// <summary>
    /// The interface of export Racing Group Daily report.
    /// </summary>
    public interface IRacingGroupDailyExportReportServices
    {
        void ExportReport(GetWinLostRacingGroupCriteria criteria);
    }
}
