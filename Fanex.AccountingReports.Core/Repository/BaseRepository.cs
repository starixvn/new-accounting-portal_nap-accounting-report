namespace Fanex.AccountingReports.Core.Repository.Impl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class BaseRepository<T>
    {
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
