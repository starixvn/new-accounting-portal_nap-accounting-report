namespace Fanex.AccountingReports.Websites.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Common;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Models;
    using Fanex.NAP.Core.Web.Auth;
    using Fanex.NAP.Core.Web.Session;
    using NAP.Core.Service.Impl;

    public class DebugController : Controller
    {
        public ActionResult Index(string username)
        {
            var userService = new UserService();
            var user = userService.Get(username);
            var currentSession = new CurrentSession();
            currentSession.UserId = user.Id;
            currentSession.Username = user.Username;
            currentSession.OnUserId = 123456;
            currentSession.SessionId = new Guid().ToString();
            currentSession.Level = userService.GetUserLevel(user.Id, NapSettings.Settings.SiteId);

            currentSession.MenuItemList = userService.GetMenuItem(
                user.Id,
                NapSettings.Settings.SiteId,
                NapSettings.Settings.Environment,
                true) as IList<MenuItem>;

            CreateAuthInfo(user, 123456);

            return RedirectToPropagationAuth(user.Id, 123456);
        }

        private void CreateAuthInfo(User user, int onUserId)
        {
            var authInfo = new AuthInfo();
            authInfo.UserId = user.Id;
            authInfo.OnUserId = onUserId;
            var authTicket = new FormsAuthenticationTicket(
                1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(20),
                false,
                authInfo.ToXml(),
                "/");

            var encrypted = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                HttpOnly = true,
                Path = "/",
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        private ActionResult RedirectToPropagationAuth(int userId, int onUserId)
        {
            var homeUrl = Fanex.NAP.Core.SignIn.Helper.GetHomeUrl();
            var propagationAuthUrl = ConfigurationManager.AppSettings["PropagationAuth"];
            SecurityService securityService = new SecurityService();
            var time = securityService.GetTime();
            var encodedRedirectUrl = HttpUtility.UrlEncode(homeUrl);
            var parameters = $"?userId={Cryptography.CfsEnCrypt(userId.ToString())}"
                             + $"&onuserId={Cryptography.CfsEnCrypt(onUserId.ToString())}"
                             + $"&time={Cryptography.CfsEnCrypt(time.ToString())}"
                             + $"&redirectUrl={encodedRedirectUrl}";

            var redirectAuthUrl = propagationAuthUrl + parameters;

            return Redirect(redirectAuthUrl);
        }
    }
}