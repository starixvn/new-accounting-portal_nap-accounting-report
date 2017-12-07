namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameMonthlyWinLoss
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Models.NumberGameMonthlyWinLoss;

    public interface INumberGameMonthlyWinLossService
    {
        NumberGameMonthlyWinLoss GetNumberGameMonthlyWinLoss(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria);
    }
}
