namespace Fanex.AccountingReports.Licensee.Repositories.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Core.Repository.Impl;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;
    using Fanex.Common.Extensions;
    using Fanex.Data.Repository;

    public class NegativeBalanceRepository : BaseRepository<NegativeBalanceUser>
    {
        public List<NegativeBalanceUser> GetNegativeBalanceUser(ICriteria criteria)
        {
            Func<IDataReader, NegativeBalanceUser> mapObjectFunc = (reader) =>
            {
                return new NegativeBalanceUser
                {
                    Balance = reader.ToDouble("balance"),
                    UserName = reader.To<string>("username"),
                    Currency = reader.To<string>("currencyname"),
                    Site = reader.To<string>("site")
                };
            };

            return this.Fetch<NegativeBalanceUser>(criteria, mapObjectFunc);
        }
    }
}
