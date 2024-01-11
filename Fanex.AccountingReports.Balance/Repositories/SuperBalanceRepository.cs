namespace Fanex.AccountingReports.Balance.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Balance.Criteria.SuperBalance;
    using Fanex.AccountingReports.Balance.Models.SuperBalance;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SuperBalanceRepository
    {
        public List<dynamic> GetSuperBalance(GetSuperBalanceCriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
                outputParam = db.GetOutputParams();
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
                    BaseBalance = Converter.ToDecimal(row["BaseBalance"]),
                    HeadlineEnabled = Converter.ToBool(row["HeadlineEnabled"])
                });
            }

            return superBalance;
        }

        public List<SuperBalance> GetSuperBalanceDownline(ICriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
                outputParam = db.GetOutputParams();
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

        public List<SuperBalance> GetSuperBalanceMasterDownline(ICriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
                outputParam = db.GetOutputParams();
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
                    BaseBalance = Converter.ToDecimal(row["baseBalance"])
                });
            }

            return superBalance;
        }
    }
}
