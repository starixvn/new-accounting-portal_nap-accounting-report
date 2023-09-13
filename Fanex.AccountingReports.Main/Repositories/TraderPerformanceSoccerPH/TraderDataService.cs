using System.Data;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Main.Repositories.TraderPerformanceSoccerPH
{
    public class TraderDataServiceImp
    {
        public DataTable Query(ICriteria criteria)
        {
            var dataTable = new DataTable();
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var dataSet = db.ExecuteDataSet(criteria);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    dataTable = dataSet.Tables[0];
                }
            }

            return dataTable;
        }
    }
}