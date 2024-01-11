namespace Fanex.AccountingReports.Core.Web
{
    using System;
    using System.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class SessionManager
    {
        /// <summary>
        /// Set Value to session with specific type.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="value">The value.</param>
        internal static void Set<T>(string sessionKey, T value)
        {

            HttpContext.Current.Session[sessionKey] = value;
        }

        /// <summary>
        /// Get Value from Session with specific Type.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="sessionKey">The session key.</param>
        /// <returns>Value from Session with specific Type.</returns>
        internal static T Get<T>(string sessionKey)
        {
            try
            {
                T value = (T)HttpContext.Current.Session[sessionKey];
                return value;
            }
            catch (Exception)
            {
                return default(T);
            }

        }

        /// <summary>
        /// Set object to Session.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="value">The value.</param>
        internal static void Set(string sessionKey, object value)
        {
            SessionManager.Set<object>(sessionKey, value);
        }

        /// <summary>
        /// Gets value object from Session.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <returns>
        /// Value object from Session.
        /// </returns>
        internal static object Get(string sessionKey)
        {
            return SessionManager.Get<object>(sessionKey);
        }
    }
}
