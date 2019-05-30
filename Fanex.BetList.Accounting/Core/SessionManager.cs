namespace Fanex.BetList.Accounting.Core
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
        /// <param name="sessionKey">The session Key.</param>
        /// <param name="value">Value to session with specific type.</param>
        public static void Set<T>(string sessionKey, T value)
        {
            HttpContext.Current.Session[sessionKey] = value;
        }

        /// <summary>
        /// Get Value from Session with specific Type.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="sessionKey">The session Key.</param>
        /// <returns>Value from Session with specific Type.</returns>
        public static T Get<T>(string sessionKey)
        {
            T value = (T)HttpContext.Current.Session[sessionKey];
            return value;
        }

        public static void Set(string sessionKey, object value)
        {
            SessionManager.Set<object>(sessionKey, value);
        }

        public static object Get(string sessionKey)
        {
            return SessionManager.Get<object>(sessionKey);
        }
    }
}