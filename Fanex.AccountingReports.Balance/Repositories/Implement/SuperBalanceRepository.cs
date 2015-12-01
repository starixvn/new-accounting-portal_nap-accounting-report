// -----------------------------------------------------------------------
// <copyright file="SuperBalanceRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="" date="2013.30.10">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Balance.Criteria.SuperBalance;
    using Fanex.AccountingReports.Balance.Models.SuperBalance;
    using Fanex.AccountingReports.Balance.Repositories.Interface;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SuperBalanceRepository : ISuperBalanceRepository
    {
        /// <summary>
        /// Gets the super balance.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The super balance parameter.</returns>
        public List<dynamic> GetSuperBalance(GetSuperBalanceCriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            var superBalance = new List<dynamic>();
            if (criteria.IsViewTotalOnly)
            {
                superBalance.Add(outputParam["total"]);
                return superBalance;
            }

            if (result == null || result.Tables.Count == 0)
            {
                return null;
            }

            var currencies = ReportService.ExchangeCurrencies;
            var superBalanceTable = result.Tables[0];
            foreach (DataRow row in superBalanceTable.Rows)
            {
                var currencyId = Converter.ToInt(row["currency"]);

                superBalance.Add(new
                {
                    CustId = Converter.ToString2(row["custid"]),
                    Username = row["username"],
                    Currency = currencies[currencyId],
                    Balance = Converter.ToDecimal(row["Balance"]),
                    BaseBalance = Converter.ToDecimal(row["BaseBalance"])
                });
            }

            return superBalance;
        }

        /// <summary>
        /// Gets the super balance down line.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The super balance down line parameter.</returns>
        public List<SuperBalance> GetSuperBalanceDownline(ICriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            if (result == null || result.Tables.Count == 0)
            {
                return null;
            }

            var superBalanceTable = result.Tables[0];
            var superBalance = new List<SuperBalance>();
            var currencies = ReportService.ExchangeCurrencies;
            foreach (DataRow row in superBalanceTable.Rows)
            {
                var currencyId = Converter.ToInt(row["currency"]);

                superBalance.Add(new SuperBalance()
                {
                    CustId = Converter.ToString2(row["custid"]),
                    Username = Converter.ToString2(row["username"]),
                    Currency = currencies[currencyId],
                    Balance = Converter.ToDecimal(row["Balance"]),
                    BaseBalance = Converter.ToDecimal(row["BaseBalance"])
                });
            }

            return superBalance;
        }

        /// <summary>
        /// Gets the super balance master down line.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The super balance master down line.</returns>
        public List<SuperBalance> GetSuperBalanceMasterDownline(ICriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            if (result == null || result.Tables.Count == 0)
            {
                return null;
            }

            var superBalanceTable = result.Tables[0];
            var superBalance = new List<SuperBalance>();
            var currencies = ReportService.ExchangeCurrencies;
            foreach (DataRow row in superBalanceTable.Rows)
            {
                var currencyId = Converter.ToInt(row["currency"]);

                superBalance.Add(new SuperBalance()
                {
                    CustId = Converter.ToString2(row["custid"]),
                    Username = Converter.ToString2(row["username"]),
                    Currency = currencies[currencyId],
                    Balance = Converter.ToDecimal(row["bal"]),
                    BaseBalance = Converter.ToDecimal(row["baseBalance"])
                });
            }

            return superBalance;
        }
    }
}