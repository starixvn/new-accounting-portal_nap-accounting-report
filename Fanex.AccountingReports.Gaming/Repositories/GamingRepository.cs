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
            var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.ExecuteReader(criteria);
        }
    }
}