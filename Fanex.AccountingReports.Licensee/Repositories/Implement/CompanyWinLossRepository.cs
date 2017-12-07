namespace Fanex.AccountingReports.Licensee.Repositories.Implement
{
    using System.Data;
    using Fanex.AccountingReports.Licensee.Repositories.Interface;
    using Fanex.Data;
    using Fanex.Data.Repository;

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
