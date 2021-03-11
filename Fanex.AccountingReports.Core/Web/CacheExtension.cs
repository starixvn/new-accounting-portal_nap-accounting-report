namespace Fanex.AccountingReports.Core.Web
{
    using System;
    using Caching;

    public static class CacheExtension
    {
        private static CacheService cacheService = new CacheService();

        public static T Get<T>(string key)
        {
            return cacheService.Get<T>(key);
        }


        public static void Set<T>(string key, T value, int expireTime = 2)
        {
            cacheService.Set(key, value, new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(expireTime)));
        }
    }
}