// -----------------------------------------------------------------------
// <copyright file="CasinoWinLossRepository.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Data;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoWinLossRepository : ICasinoWinLossRepository
    {
        /// <summary>
        /// Get Data from Database.
        /// </summary>
        /// <param name="criteria">Parameters of method.</param>
        /// <returns>Return result.</returns>
        public DataTable GetData(GetCasinoWinLossCriteria criteria)
        {
            DataTable dataTable = new DataTable();

            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var result = db.ExecuteDataSet();
                if (result != null && result.Tables.Count > 0)
                {
                    dataTable = result.Tables[0];
                }               
            }

            return dataTable;
        }
    }
}
