// --------------------------------------------------------------------------
// <copyright file="WinLossSummaryRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Main.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossSummaryRepository : IWinLossSummaryRepository
    {
        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The report.</returns>
        public List<dynamic> GetReport(ICriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;
            List<dynamic> report = new List<dynamic>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            if (result != null && result.Tables.Count > 0)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        SportType = Converter.ToInt(row["SportType"]),
                        ibc = Converter.ToInt(row["ibc"]),
                        liveindicator = Converter.ToInt(row["liveindicator"]),
                        bType = Converter.ToInt(row["bType"]),
                        Status = Converter.ToString2(row["Status"]),
                        WinLost = Converter.ToDecimal(row["WinLost"]),
                        Position = Converter.ToDecimal(row["Position"]),
                        Commission = Converter.ToDecimal(row["Commission"]),
                        Admin2 = Converter.ToDecimal(row["Admin2"])
                    });
                }
            }

            return report;
        }
    }
}
