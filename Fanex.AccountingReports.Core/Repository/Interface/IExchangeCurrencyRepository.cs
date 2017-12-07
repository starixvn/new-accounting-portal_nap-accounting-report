namespace Fanex.AccountingReports.Core.Repository.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IExchangeCurrencyRepository
    {
        List<Currency> GetExchangeCurrencies(ICriteria criteria);
    }
}
