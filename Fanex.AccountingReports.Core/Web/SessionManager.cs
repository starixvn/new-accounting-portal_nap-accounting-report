namespace Fanex.AccountingReports.Core.Web
{
    using System.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SessionManager
    {
        /// <summary>
        /// Set Value to session with specific type.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="value">The value.</param>
        public static void Set<T>(string sessionKey, T value)
        {

            HttpContext.Current.Session[sessionKey] = value;
        }

        /// <summary>
        /// Get Value from Session with specific Type.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="sessionKey">The session key.</param>
        /// <returns>Value from Session with specific Type.</returns>
        public static T Get<T>(string sessionKey)
        {

            T value = (T)HttpContext.Current.Session[sessionKey];
            return value;

        }

        /// <summary>
        /// Set object to Session.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="value">The value.</param>
        public static void Set(string sessionKey, object value)
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
        public static object Get(string sessionKey)
        {
            return SessionManager.Get<object>(sessionKey);
        }
    }
}
