// --------------------------------------------------------------------------
// <copyright file="CacheManager.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.19">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Core
{
    using System;
    using Fanex.Caching;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CacheManager
    {
        private static readonly ICache CacheManagerInstance;

        static CacheManager()
        {
            CacheManagerInstance = CacheService.CreateInstance(CacheType.MemCache);
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <returns>The specified key.</returns>
        public static object Get(string key)
        {
            try
            {
                return CacheManagerInstance.Get<object>(key);
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="value">The value.</param>
        public static void Set(string key, object value)
        {
            try
            {
                CacheManagerInstance.Set(key, value);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="key">The key value.</param>
        /// <returns>The specified key.</returns>
        public static T Get<T>(string key)
        {
            try
            {
                return CacheManagerInstance.Get<T>(key);
            }
            catch (Exception)
            {
            }

            return default(T);
        }

        public static void Set<T>(string key, T value)
        {
            CacheManagerInstance.Set<T>(key, value);
        }

        public static void Set(string key, object value, TimeSpan timeSpanExpiration)
        {
            CacheManagerInstance.Set(key, value, timeSpanExpiration);
        }

        public static void Set<T>(string key, T value, TimeSpan timeSpanExpiration)
        {
            CacheManagerInstance.Set<T>(key, value, timeSpanExpiration);
        }

        public static void Set(string key, object value, DateTime absoluteExpiration)
        {
            CacheManagerInstance.Set(key, value, absoluteExpiration);
        }

        public static void Set<T>(string key, T value, DateTime absoluteExpiration)
        {
            CacheManagerInstance.Set<T>(key, value, absoluteExpiration);
        }

        public static void Remove(string key)
        {
            CacheManagerInstance.Remove(key);
        }
    }
}