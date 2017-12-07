namespace Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee
{
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;

    /// <summary>
    /// Export report service.
    /// </summary>
    public interface IRacingWinLossByLicenseeExportReportService
    {
        void ExportReport(GetRacingDailyReportCriteria criteria);
    }
}
