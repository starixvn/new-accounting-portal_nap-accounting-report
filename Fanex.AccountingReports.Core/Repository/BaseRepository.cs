namespace Fanex.AccountingReports.Core.Repository.Impl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    public class BaseRepository<T>
    {
        /// <summary>
        /// Fetches the specified criteria.
        /// </summary>
        /// <typeparam name="S">The parameter type.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="mapObjectFunction">The map object function.</param>
        /// <returns>The data value.</returns>
        public virtual List<S> Fetch<S>(
            ICriteria criteria,
            Func<IDataReader, S> mapObjectFunction) where S : class
        {
            List<S> listObject = new List<S>();
            if (!criteria.IsValid())
            {
                return listObject;
            }

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria))
                {
                    while (reader.Read())
                    {
                        listObject.Add(mapObjectFunction(reader));
                    }
                }
            }

            return listObject;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The data value.</returns>
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
