namespace Fanex.AccountingReports.Licensee.Repositories.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Core.Repository.Impl;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;
    using Fanex.Common.Extensions;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NegativeBalanceRepository : BaseRepository<NegativeBalanceUser>
    {
        /// <summary>
        /// Gets the negative balance user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The negative balance user.</returns>
        public List<NegativeBalanceUser> GetNegativeBalanceUser(ICriteria criteria)
        {
            //// Create Function will map Data from DataReader to Schedule Object
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