namespace Fanex.AccountingReports.Licensee.Repositories.Implement
{
    using System.Data;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CompanyWinLossRepository
    {
        public IDataReader GetDataFromDB(ICriteria criteria)
        {
            var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());

            return db.ExecuteReader(criteria);
        }
    }
}