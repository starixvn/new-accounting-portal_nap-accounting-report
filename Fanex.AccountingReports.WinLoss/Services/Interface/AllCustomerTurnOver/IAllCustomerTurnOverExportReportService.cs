namespace Fanex.AccountingReports.WinLoss.Services.Interface.AllCustomerTurnOver
{
    using Fanex.AccountingReports.WinLoss.Criteria.GetAllCustomerTurnOver;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IAllCustomerTurnOverExportReportService
    {
        void ExportReport(GetAllCustomerTurnOverCriteria criteria);
    }
}