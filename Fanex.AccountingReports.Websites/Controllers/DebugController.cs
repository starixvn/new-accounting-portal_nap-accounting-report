namespace Fanex.AccountingReports.Websites.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
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

            return Content("Done");
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
    }
}