namespace Fanex.AccountingReports.Websites.App_Start
{
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Web.Auth;
    using Fanex.NAP.Core.Web.Session;
    using NAP.Core.Auth;
    using System.Web;
    using System.Web.Security;

    public static class AuthConfig
    {
        public static void RegisterAuth(HttpContext context)
        {
            var user = HttpContext.Current.User;

            if (user != null)
            {
                if (user.Identity is FormsIdentity && user.Identity.IsAuthenticated)
                {
                    string cookie = FormsAuthentication.FormsCookieName;
                    HttpCookie httpCookie = context.Request.Cookies[cookie];

                    if (httpCookie == null)
                    {
                        return;
                    }

                    var ticket = FormsAuthentication.Decrypt(httpCookie.Value);
                    if (ticket == null || ticket.Expired)
                    {
                        return;
                    }

                    var identity = new FormsIdentity(ticket);
                    if (string.IsNullOrEmpty(ticket.UserData))
                    {
                        return;
                    }

                    var udata = AuthInfo.CreateUserData(ticket.UserData);
                    var principal = new NapPrincipal(identity, udata);
                    context.User = principal;
                }
            }
        }

        public static void SignOut()
        {
            var _currentSession = new CurrentSession();
            var securityService = new SecurityService();
            securityService.Logout(_currentSession.OnUserId);
            _currentSession.ClearAll();
        }
    }
}
