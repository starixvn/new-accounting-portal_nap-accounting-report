namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Data;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GamingRepository : IGamingRepository
    {
        public IDataReader GetLicenseesByProduct(Criteria.GetLicenseesByProductCriteria criteria)
        {
            ObjectDb db = new ObjectDb(criteria.GetSettingKey());
            db.AssignParamValues(criteria.GetSpParams());
            return db.ExecuteReader();
        }
    }
}
