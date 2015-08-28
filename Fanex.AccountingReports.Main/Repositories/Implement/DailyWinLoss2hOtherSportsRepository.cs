// --------------------------------------------------------------------------
// <copyright file="DailyWinLoss2hOtherSportsRepository.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hOtherSports;
    using Fanex.AccountingReports.Main.Repositories.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyWinLoss2hOtherSportsRepository : IDailyWinLoss2hOtherSportsRepository
    {
        /// <summary>
        /// Gets the main2h report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The main2h report.</returns>
        public List<dynamic> GetMain2hReport(
            GetMain2hReportCriteria criteria,
            ref Hashtable outputParam)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
                outputParam = objectDb.GetOuputParams();
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        SportType = row["SportType"],
                        SportName = row["SportName"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        RealLineTotal = row["RealLineTotal"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        RealTurnover = row["RealTurnover"],
                        VirtualTurnover = row["VirtualTurnover"],
                        ibc = row["ibc"],
                        Site = row["Site"]
                    });
                }
            }

            return report;
        }
    }
}
