namespace Fanex.AccountingReports.Core.Repository.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.Common.Extensions;
    using Fanex.Data.Repository;

    public class ExchangeCurrencyRepository : BaseRepository<Currency>
    {
        public List<Currency> GetExchangeCurrencies(ICriteria criteria)
        {
            // Create Function will map Data from DataReader to Schedule Object
            Func<IDataReader, Currency> mapObjectFunc = (reader) =>
            {
                return new Currency
                {
                    CurrencyId = reader.ToInt("exchangeid"),
                    CurrencyName = reader.To<string>("currency")
                };
            };

            return Fetch<Currency>(criteria, mapObjectFunc);
        }
    }
}
