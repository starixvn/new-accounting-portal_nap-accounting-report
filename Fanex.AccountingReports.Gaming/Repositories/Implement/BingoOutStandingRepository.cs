// --------------------------------------------------------------------------
// <copyright file="BingoOutStandingRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Gaming.Criteria.BingoOutStanding;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BingoOutStandingRepository : IBingoOutStandingRepository
    {
        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParams">The output parameters.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetReport(
            GetReportCriteria criteria,
             ref Hashtable outputParams)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
                outputParams = objectDb.GetOuputParams();
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null)
            {
                if (result.Tables.Count >= 1)
                {
                    var winLossInfoTable = result.Tables[0];
                    var winLossInfo = new List<dynamic>();
                    foreach (DataRow row in winLossInfoTable.Rows)
                    {
                        var info = new
                        {
                            UserName = row["UserName"],
                            Currency = row["Currency"],
                            Company = row["Company"],
                            OutStanding = row["OutStanding"],
                            Date = row["Date"]
                        };

                        winLossInfo.Add(info);
                    }

                    report.Add(winLossInfo);
                }

                if (result.Tables.Count >= 2)
                {
                    var grandTotalInfoTable = result.Tables[1];
                    var grandTotalInfo = new List<dynamic>();
                    foreach (DataRow row in grandTotalInfoTable.Rows)
                    {
                        grandTotalInfo.Add(new
                        {
                            Currency = row["Currency"],
                            OutStanding = row["OutStanding"]
                        });
                    }

                    report.Add(grandTotalInfo);
                }

                if (result.Tables.Count >= 3)
                {
                    var grandTotalRMInfoTable = result.Tables[2];
                    var grandTotalInfo = new List<dynamic>();
                    foreach (DataRow row in grandTotalRMInfoTable.Rows)
                    {
                        grandTotalInfo.Add(new
                        {
                            OutStanding = row["Grand Total RM"]
                        });
                    }

                    report.Add(grandTotalInfo);
                }
            }

            return report;
        }
    }
}
