
namespace Fanex.AccountingReports.Core.Web
{
    using System;
    using Couchbase;
    using Enyim.Caching.Memcached;

    public static class CacheExtension
    {
        private static CouchbaseClient _couchbaseClient = new CouchbaseClient();

        public static object Get(string key)
        {
            return _couchbaseClient.Get(key);
        }

        public static T Get<T>(string key)
        {
            return _couchbaseClient.Get<T>(key);
        }

        /// <summary>
        /// Function to set cache info.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Cache value.</param>
        /// <param name="expireTime">Expire time [Hour].</param>
        public static void Set(string key, object value, int expireTime = 2)
        {
            _couchbaseClient.ExecuteStore(
                StoreMode.Set,
                key,
                value,
                DateTime.Now.AddHours(expireTime),
                Couchbase.Operations.PersistTo.One,
                Couchbase.Operations.ReplicateTo.Two);
        }

        public static void Remove(string key)
        {
            _couchbaseClient.Remove(key);
        }
    }
}
