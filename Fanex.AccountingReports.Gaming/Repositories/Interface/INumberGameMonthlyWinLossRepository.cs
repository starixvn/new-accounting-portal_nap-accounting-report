namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Models.NumberGameMonthlyWinLoss;

    public interface INumberGameMonthlyWinLossRepository
    {
        List<NumberGameMonthlyWinLoss> GetNumberGameMonthlyWinLoss(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria);
    }
}
