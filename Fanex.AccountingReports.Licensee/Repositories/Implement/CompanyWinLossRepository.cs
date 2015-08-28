// -----------------------------------------------------------------------
// <copyright file="CompanyWinLossRepository.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Repositories.Implement
{
    using System.Data;
    using Fanex.AccountingReports.Licensee.Repositories.Interface;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CompanyWinLossRepository : ICompanyWinLossRepository
    {
        public IDataReader GetDataFromDB(ICriteria criteria)
        {
            ObjectDb db = new ObjectDb(criteria.GetSettingKey());
            db.AssignParamValues(criteria.GetSpParams());

            return db.ExecuteReader();
        }
    }
}
