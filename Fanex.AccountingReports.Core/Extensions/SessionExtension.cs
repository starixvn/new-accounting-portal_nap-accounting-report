// --------------------------------------------------------------------------
// <copyright file="SessionExtension.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.6">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Extensions
{
    using System.Web.SessionState;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class SessionExtension
    {
        public delegate object UponCreate();

        /// <summary>
        /// Gets the session class.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The session key.</param>
        /// <param name="uponCreate">The upon create.</param>
        /// <returns>The session class.</returns>
        public static T GetSessionClass<T>(
            this HttpSessionState session,
            string key,
            UponCreate uponCreate) where T : class
        {
            var sessionValue = session[key];
            if (sessionValue != null)
            {
                return sessionValue as T;
            }

            var item = uponCreate() as T;
            session[key] = item;
            return item;
        }

        /// <summary>
        /// Gets the session value.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The session key.</param>
        /// <param name="uponCreate">The upon create.</param>
        /// <returns>The session value.</returns>
        public static T GetSessionValue<T>(
            this HttpSessionState session,
            string key,
            UponCreate uponCreate) where T : struct
        {
            var sessionValue = session[key];
            if (sessionValue != null)
            {
                return (T)sessionValue;
            }

            var item = uponCreate();
            session[key] = item;
            return (T)item;
        }

        public static object GetSession(HttpSessionState sessionState, string key)
        {
            return sessionState[key];
        }

        public static void AddSession(HttpSessionState sessionState, string key, object value)
        {
            sessionState[key] = value;
        }
    }
}
