using System;
using System.Data;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Main.Repositories.TraderPerformanceSoccerPH
{
    public interface TraderDataService
    {
        DataTable Query(ICriteria criteria);
    }

    public class TraderDataServiceImp : TraderDataService
    {
        public DataTable Query(ICriteria criteria)
        {
            var dataTable = new DataTable();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var dataSet = db.ExecuteDataSet();
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    dataTable = dataSet.Tables[0];
                }
            }

            return dataTable;
        }
    }
}