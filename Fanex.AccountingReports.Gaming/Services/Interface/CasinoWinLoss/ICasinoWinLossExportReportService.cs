namespace Fanex.AccountingReports.Gaming.Services.Interface.CasinoWinLoss
{
    using Fanex.AccountingReports.Gaming.Criteria.CasinoWinLoss;

    /// <summary>
    /// The export report service..
    /// </summary>
    public interface ICasinoWinLossExportReportService
    {
        void ExportReport(GetCasinoWinLossCriteria criteria);
    }
}
