namespace Fanex.AccountingReports.Core.Web
{
    using System;
    using System.Web;

    internal class SessionManager
    {
        internal static void Set<T>(string sessionKey, T value)
        {

            HttpContext.Current.Session[sessionKey] = value;
        }

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

        internal static void Set(string sessionKey, object value)
        {
            SessionManager.Set<object>(sessionKey, value);
        }

        internal static object Get(string sessionKey)
        {
            return SessionManager.Get<object>(sessionKey);
        }
    }
}
