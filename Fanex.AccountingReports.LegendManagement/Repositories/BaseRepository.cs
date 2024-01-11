namespace Fanex.AccountingReports.LegendManagement.Repositories.Impls
{
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class BaseRepository
    {
        public virtual Hashtable ExecuteCommand(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
                return db.GetOutputParams();
            }
        }
    }
}
