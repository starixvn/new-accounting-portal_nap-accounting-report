namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameWinLossByBetTypeMatch
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    public interface INumberGameWinLossByBetTypeMatchService
    {
        List<EntityBase<int, string>> GetCurrencyList();
    }
}
