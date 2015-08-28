// -----------------------------------------------------------------------
// <copyright file="RacingGroupDailyRepository.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Repositories.Implement
{
    using System;
    using System.Data;
    using Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily;
    using Fanex.AccountingReports.Racing.Repositories.Interface;
    using Fanex.Data;

    /// <summary>
    /// The implement of Racing Group Daily repository.
    /// </summary>
    public class RacingGroupDailyRepository : IRacingGroupDailyRepository
    {
        /// <summary>
        /// Get data table return from database.
        /// </summary>
        /// <param name="criteria">The criteria to get racing group.</param>
        /// <returns>The data table.</returns>
        public DataTable GetDataTable(GetWinLostRacingGroupCriteria criteria)
        {
            DataTable groupTable = new DataTable();
            DateTime fromDate = criteria.FromDate;
            DateTime toDate = criteria.ToDate;

            if (fromDate == toDate && toDate == DateTime.Today)
            {
                groupTable = this.GetRacingGroupDailyDataTable(groupTable, fromDate);
            }
            else
            {
                using (ObjectDb objectDB = new ObjectDb(criteria.GetSettingKey(), 600))
                {
                    objectDB.AssignParamValues(criteria.GetSpParams());
                    var groupDatasetResult = objectDB.ExecuteDataSet();

                    if (groupDatasetResult != null && groupDatasetResult.Tables.Count > 0)
                    {
                        groupTable = groupDatasetResult.Tables[0];
                    }
                }

                if (toDate >= DateTime.Today)
                {
                    DataTable dailyTable = new DataTable();
                    dailyTable = this.GetRacingGroupDailyDataTable(dailyTable, toDate);
                    groupTable = this.AppendDailyIntoGroupTable(groupTable, dailyTable);
                }
            }

            return groupTable;
        }

        /// <summary>
        /// GEt racing group daily data table.
        /// </summary>
        /// <param name="groupTable">The data table is got from database.</param>
        /// <param name="winLostDate">The win lost date.</param>
        /// <returns>The data table.</returns>
        private DataTable GetRacingGroupDailyDataTable(DataTable groupTable, DateTime winLostDate)
        {
            var dailyCriteria = new GetWinLostRacingGroupDailyCriteria();
            dailyCriteria.WinLostDate = winLostDate;

            using (ObjectDb objectDB = new ObjectDb(dailyCriteria.GetSettingKey(), 600))
            {
                objectDB.AssignParamValues(dailyCriteria.GetSpParams());
                var datasetResult = objectDB.ExecuteDataSet();

                if (datasetResult != null && datasetResult.Tables.Count > 0)
                {
                    groupTable = datasetResult.Tables[0];
                }
            }

            return groupTable;
        }

        /// <summary>
        /// Append daily data table into group data table.
        /// </summary>
        /// <param name="groupTable">The group data table.</param>
        /// <param name="dailyTable">The daily data table.</param>
        /// <returns>The new data table after append daily table.</returns>
        private DataTable AppendDailyIntoGroupTable(DataTable groupTable, DataTable dailyTable)
        {
            foreach (DataRow dailyRow in dailyTable.Rows)
            {
                groupTable.ImportRow(dailyRow);
            }

            return groupTable;
        }
    }
}
