namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameMonthlyWinLoss
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;

    public interface INumberGameMonthlyWinLossExportDataService
    {
        void ExportNumberGameMonthlyWinLossReport(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria);
    }
}
