// --------------------------------------------------------------------------
// <copyright file="TotalSMAMemberActiveRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.15">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive;
    using Fanex.AccountingReports.Misc.Repositories.Interface;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TotalSMAMemberActiveRepository : ITotalSMAMemberActiveRepository
    {
        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="totalSMA">The total SMA.</param>
        /// <param name="activeDetails">The active details.</param>
        public void GetReport(ICriteria criteria, ref dynamic totalSMA, ref IList<dynamic> activeDetails)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.ExecuteDataSet(criteria.GetSpParams());
                if (result != null && result.Tables != null && result.Tables.Count == 2)
                {
                    // Total
                    if (result.Tables[0].Rows.Count > 0)
                    {
                        var totalRow = result.Tables[0].Rows[0];
                        totalSMA = new
                        {
                            NewCreatedSuper = totalRow["NewCreatedSuper"],
                            NewCreatedMaster = totalRow["NewCreatedMaster"],
                            NewCreatedAgent = totalRow["NewCreatedAgent"],

                            ActiveSuper = totalRow["ActiveSuper"],
                            ActiveMaster = totalRow["ActiveMaster"],
                            ActiveAgent = totalRow["ActiveAgent"],

                            SuspendedSuper = totalRow["SuspendedSuper"],
                            SuspendedMaster = totalRow["SuspendedMaster"],
                            SuspendedAgent = totalRow["SuspendedAgent"],

                            ClosedSuper = totalRow["ClosedSuper"],
                            ClosedMaster = totalRow["ClosedMaster"],
                            ClosedAgent = totalRow["ClosedAgent"]
                        };
                    }

                    // Active
                    if (result.Tables[1].Rows.Count > 0)
                    {
                        activeDetails = activeDetails ?? new List<dynamic>();
                        foreach (DataRow drow in result.Tables[1].Rows)
                        {
                            activeDetails.Add(new
                            {
                                Id = drow["Id"],
                                SiteName = drow["SiteName"],
                                SMA = drow["SMA"]
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the status detail.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The status detail.</returns>
        public IList<dynamic> GetStatusDetail(GetStatusDetailsCriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;
            List<dynamic> detail = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                // Starting of PageIndex of SP is 1
                criteria.PageIndex++;
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            if (result != null && result.Tables.Count >= 1)
            {
                Status status = (Status)criteria.Status;
                detail = new List<dynamic>();

                dynamic value;
                DataTable dataTable = result.Tables[0];
                foreach (DataRow drow in dataTable.Rows)
                {
                    value = this.GetStatusDetail(drow, status);
                    detail.Add(value);
                }
            }

            return detail;
        }

        /// <summary>
        /// Gets the status detail.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <param name="status">The status.</param>
        /// <returns>The status detail.</returns>
        private dynamic GetStatusDetail(DataRow dataRow, Status status)
        {
            if (status == Status.NewCreated)
            {
                return new
                {
                    date = dataRow["date"],
                    groupHeadline = dataRow["groupHeadline"],
                    username = dataRow["username"],
                    status = dataRow["status"],
                    site = dataRow["site"],
                    currency = dataRow["currency"],
                    position = dataRow["position"],
                    createddate = dataRow["creatdate"],
                    creator = dataRow["creator"]
                };
            }

            return new
            {
                date = dataRow["date"],
                groupHeadline = dataRow["groupHeadline"],
                username = dataRow["username"],
                status = dataRow["status"],
                site = dataRow["site"],
                currency = dataRow["currency"],
                position = dataRow["position"]
            };
        }
    }
}