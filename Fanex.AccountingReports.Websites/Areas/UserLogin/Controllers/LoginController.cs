namespace Fanex.AccountingReports.Websites.Areas.UserLogin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.AccountingReports.Core.Resources;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web;
    using Fanex.NAP.Core.Web.Auth;
    using Fanex.NAP.Core.Web.Session;
    using NAP.Core.Models;
    using NAP.Core.Models.ParameterObject;
    using NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LoginController : BaseController
    {
        #region members

        private readonly IUserService _userService = new UserService();
        private readonly IGroupService groupService;
        private readonly ICurrentSession currentSession;
        private readonly ISecurityService securityService = new SecurityService();

        private readonly HashAlgorithm hashAlgorithm;

        #endregion members

        #region ctors

        public LoginController()
        {
            groupService = new GroupService();
            currentSession = new CurrentSession();
            hashAlgorithm = new SHA256Managed();
        }

        #endregion ctors

        #region sign in

        /// <summary>
        /// Indexes the specified username.
        /// </summary>
        /// <returns>The index page.</returns>
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Index(string username, string password, string returnUrl)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var user = _userService.Get(username.Trim()) as User;

                if (user != null)
                {
                    string hashPassword = GetHash(user.Password);

                    if (password == hashPassword)
                    {
                        var group = groupService.Get(user.GroupId);
                        var siteLevelList = _userService.GetSiteLevel(user.Id, Environment); // gets all sites of logging-in user

                        if ((group == null || group.IsActive) &&
                            siteLevelList.Any(x => x.IsActive))
                        {
                            //// allows user login

                            //// check-in user

                            var criteria = new LoginParameter
                            {
                                UserId = user.Id,
                                SiteId = this.SiteId,
                                NetSessionId = Session.SessionID,
                                Ip = CurrentContext.IP,
                                LoginDate = DateTime.Now,
                                BrowserName = CurrentContext.BrowserName,
                                CountryCode = CurrentContext.CountryCode,
                                Domain = CurrentContext.DomainName,
                                Platform = CurrentContext.Platform
                            };

                            var onuserId = securityService.Login(criteria);

                            //// check-in user

                            //// saves user session

                            currentSession.Level = _userService.GetUserLevel(user.Id, this.SiteId);
                            currentSession.UserId = user.Id;
                            currentSession.Username = username;
                            currentSession.OnUserId = onuserId;
                            currentSession.SessionId = Session.SessionID.ToString();

                            // current site is not NAP Hub
                            if (this.SiteId != 0)
                            {
                                CreateUserMenu(user);
                            }

                            CreateAuthInfo(user);
                            AppSession.CurrentUser = user;

                            //// saves user session

                            if (AppSession.CurrentUserMenu != null &&
                                AppSession.CurrentUserMenu.Where(m => m.ParentId <= 0).ToList().Count > 0)
                            {
                                bool userIsActiveOnThisSite = false;

                                foreach (var siteLevel in siteLevelList)
                                {
                                    if (siteLevel.SiteId == this.SiteId && siteLevel.IsActive)
                                    {
                                        userIsActiveOnThisSite = true;
                                    }
                                }

                                if (userIsActiveOnThisSite)
                                {
                                    AppSession.ViewIPVN = PermissionService.HasPermission(AccReportFunctionKeys.ViewIPVN);
                                    if (string.IsNullOrEmpty(returnUrl))
                                    {
                                        return Redirect(FormsAuthentication.DefaultUrl);
                                    }
                                    else
                                    {
                                        returnUrl = Server.UrlDecode(returnUrl);
                                        return Redirect(returnUrl);
                                    }
                                }
                                else
                                {
                                    DoSignOut();
                                    ViewBag.Message = ViewResources.Your_account_was_suspended;
                                }
                            }
                            else
                            {
                                // user has no menu permission
                                DoSignOut();
                                ViewBag.Message = Labels.You_have_no_menu_permission;
                            }

                            //// allows user login
                        }
                        else
                        {
                            // user account was suspended on all sites
                            if (siteLevelList.Any())
                            {
                                ViewBag.Message = ViewResources.Your_account_was_suspended;
                            }
                            else
                            {
                                // logging-in user belongs to no site
                                ViewBag.Message = ViewResources.You_have_no_authorization;
                            }
                        }
                    }
                    else
                    {
                        // wrong password
                        ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                    }
                }
                else
                {
                    // user doesn't exist
                    ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                }
            }
            else
            {
                ViewBag.Message = "Invalid input!";
            }

            return View();
        }

        #endregion
        #region sign out

        public ActionResult LogOut()
        {
            DoSignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }

        #endregion sign out

        #region re check in

        /// <summary>
        /// Recheck login.
        /// </summary>
        /// <returns>Recheck login result.</returns>
        [NapSuperAuth]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        public ActionResult ReCheckLogin()
        {
            var recheckIn = 0;

            if (currentSession.OnUserId > 0)
            {
                recheckIn = securityService.ReCheckIn(currentSession.OnUserId, NapSettings.Settings.SiteId);
            }

            var isKickedOut = currentSession.OnUserId <= 0 || NapSettings.Settings.EnableReCheckIn && recheckIn > 0;
            return Json(isKickedOut);
        }

        #endregion re check in

        private void CreateUserMenu(User user)
        {
            currentSession.MenuItemList = (IList<MenuItem>)_userService.GetMenuItem(
                                                                                    user.Id,
                                                                                    SiteId,
                                                                                    Environment,
                                                                                    true);
            AppSession.CurrentUserMenu = currentSession.MenuItemList;
        }

        /// <summary>
        /// Creates the authentication information.
        /// </summary>
        /// <param name="user">The user value.</param>
        private void CreateAuthInfo(User user)
        {
            var currentDomain = CurrentContext.CurrentDomain;

            var authInfo = new AuthInfo();

            authInfo.UserId = user.Id;
            authInfo.OnUserId = this.currentSession.OnUserId;

            var authTicket = new FormsAuthenticationTicket(
                 1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(20000),
                false,
                authInfo.ToXml(),
                currentDomain.Path);

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Path = "/",
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        private void DoSignOut()
        {
            if (currentSession != null && currentSession.OnUserId != 0)
            {
                securityService.Logout(currentSession.OnUserId);
            }

            FormsAuthentication.SignOut();
            currentSession.ClearAll();

            var requestCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Expires = DateTime.Now.AddYears(-1);
            cookie.Path = requestCookie.Path;
            Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Gets the hash value of input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Hash value of input.</returns>
        private string GetHash(string input)
        {
            var encrypted = string.Empty;

            if (!string.IsNullOrWhiteSpace(input))
            {
                var hashData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLower();
            }

            return encrypted;
        }
    }
}