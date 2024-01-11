namespace Fanex.AccountingReports.Misc.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TotalSMAMemberActiveRepository
    {
        public void GetReport(ICriteria criteria, ref dynamic totalSMA, ref IList<dynamic> activeDetails)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.ExecuteDataSet(criteria);
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

        public IList<dynamic> GetStatusDetail(GetStatusDetailsCriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;
            List<dynamic> detail = null;
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                // Starting of PageIndex of SP is 1
                criteria.PageIndex++;
                result = db.ExecuteDataSet(criteria);
                outputParam = db.GetOutputParams();
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
