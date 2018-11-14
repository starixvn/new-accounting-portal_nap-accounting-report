namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Data;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GamingRepository
    {
        public IDataReader GetLicenseesByProduct(Criteria.GetLicenseesByProductCriteria criteria)
        {
            ObjectDb db = new ObjectDb(criteria.GetSettingKey());
            db.AssignParamValues(criteria.GetSpParams());
            return db.ExecuteReader();
        }
    }
}