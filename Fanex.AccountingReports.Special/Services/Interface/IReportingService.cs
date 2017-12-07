namespace Fanex.AccountingReports.Special.Services.Interface
{
    using Fanex.AccountingReports.Special.Criteria.ReportingServices;
    using Fanex.AccountingReports.Special.Models.ReportingServices;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IReportingService
    {
        Report GetReport(GetReportCriteria criteria);

        string GetMimeType(string fileExtension);
    }
}
