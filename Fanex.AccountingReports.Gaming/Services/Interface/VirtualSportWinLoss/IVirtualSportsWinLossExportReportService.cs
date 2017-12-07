namespace Fanex.AccountingReports.Gaming.Services.Interface.VirtualSportWinLoss
{
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;

    /// <summary>
    /// The interface to export report service.
    /// </summary>
    public interface IVirtualSportsWinLossExportReportService
    {
        void ExportReport(GetVirtualSportsWinLossCriteria criteria);
    }
}
