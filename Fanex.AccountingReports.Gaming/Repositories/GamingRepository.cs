namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Data;
    using Fanex.Data;

    public class GamingRepository
    {
        public IDataReader GetLicenseesByProduct(Criteria.GetLicenseesByProductCriteria criteria)
        {
            var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.ExecuteReader(criteria);
        }
    }
}
