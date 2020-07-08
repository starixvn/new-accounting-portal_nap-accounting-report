namespace Fanex.AccountingReports.Core.Repository.Impl
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReportHelperRepository
    {
        /// <summary>
        /// Gets the username suggestion.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The username suggestion.</returns>
        public List<dynamic> UserNameSuggestion(UserNameSuggestionCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        UserName = row["UserName"]
                    });
                }
            }

            return report;
        }

        /// <summary>
        /// Gets the currency by licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The currency by licensee.</returns>
        public List<dynamic> GetCurrencyByLicensee(GetCurrencyByLicenseeCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        ExchangeID = row["ExchangeID"],
                        Currency = row["Currency"]
                    });
                }
            }

            return report;
        }

        /// <summary>
        /// Gets the licensee support by product.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The licensee support by product.</returns>
        public List<dynamic> GetLicensee_SupportByProduct(GetLicensee_SupportByProductCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        LicenseeID = row["LicenseeID"],
                        Licenseename = row["Licenseename"]
                    });
                }
            }

            return report;
        }

        /// <summary>
        /// Gets the DWH date range.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>DWH date range.</returns>
        public DWHDateRange GetDWHDateRange(GetDWHDateRangeCriteria criteria)
        {
            var result = new DWHDateRange();

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objectDb.ExecuteReader(criteria.GetSpParams());

                while (reader.Read())
                {
                    result.MinDate = Converter.ToDateTime(reader["FirstDate"]);
                    result.MaxDate = Converter.ToDateTime(reader["LastDate"]);
                }
            }

            return result;
        }

        public List<dynamic> LeagueSuggestion(LeagueSuggestionCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        LeagueName = row["leaguename"]
                    });
                }
            }

            return report;
        }
    }
}