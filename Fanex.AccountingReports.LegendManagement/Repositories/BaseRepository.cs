namespace Fanex.AccountingReports.LegendManagement.Repositories.Impls
{
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// Base Repository.
    /// </summary>
    public class BaseRepository
    {
        /// <summary>
        /// Execute command to DB.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// Execute result.
        /// </returns>
        public virtual Hashtable ExecuteCommand(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                return db.GetOuputParams();
            }
        }
    }
}
