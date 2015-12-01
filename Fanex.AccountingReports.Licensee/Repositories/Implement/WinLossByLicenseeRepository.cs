// -----------------------------------------------------------------------
// <copyright file="WinLossByLicenseeRepository.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Repositories.Implement
{
    using System.Data;
    using Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee;
    using Fanex.AccountingReports.Licensee.Repositories.Interface;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByLicenseeRepository : IWinLossByLicenseeRepository
    {
        public DataTable GetCurrencyReport(GetCurrencyReportCriteria criteria)
        {
            DataTable dataTable = new DataTable();
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

        public DataTable GetLevelReport(CriteriaBase criteria)
        {
            DataTable dataTable = new DataTable();
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
