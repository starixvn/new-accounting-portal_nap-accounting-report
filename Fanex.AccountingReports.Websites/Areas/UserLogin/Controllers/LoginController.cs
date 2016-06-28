// --------------------------------------------------------------------------
// <copyright file="LoginController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Websites.Areas.UserLogin.Controllers
{
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Resources;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Criteria.User;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web;
    using Fanex.NAP.Core.Web.Auth;
    using Fanex.NAP.Core.Web.Session;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using ONEbookUMService = ONEbook.UM.Services;
    using ONEbookUMServiceImp = ONEbook.UM.Services.Impl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LoginController : BaseController
    {
        #region members

        private const string LoginFormToken = "_SecureLoginFormToken";
        private IUserService _userService;
        private IGroupService _groupService;
        private ISiteService _siteService;
        private IMenuItemService _menuItemService;
        private ICurrentSession _currentSession;
        private IFunctionService _functionService;
        private ONEbookUMService.IMaintenanceService _maintenanceService;

        private HashAlgorithm _hashAlgorithm;

        #endregion members

        #region ctors

        public LoginController()
            : this(null, null, null, null, null, null, null)
        {
        }

        public LoginController(
            IUserService userService,
            IGroupService groupService,
            ISiteService siteService,
            IMenuItemService menuItemService,
            ICurrentSession currentSession,
            IFunctionService functionService,
            ONEbookUMService.IMaintenanceService maintenanceService)
        {
            _userService = userService ?? new UserService();
            _groupService = groupService ?? new GroupService();
            _siteService = siteService ?? new SiteService();
            _menuItemService = menuItemService ?? new MenuItemService();
            _currentSession = currentSession ?? new CurrentSession();
            _functionService = functionService ?? new FunctionService();
            _maintenanceService = maintenanceService ?? new ONEbookUMServiceImp.MaintenanceService();
            _hashAlgorithm = new SHA256Managed();
        }

        #endregion ctors

        public string FormToken
        {
            get
            {
                if (Session[LoginFormToken] == null)
                {
                    return string.Empty;
                }

                return Session[LoginFormToken].ToString();
            }

            set
            {
                Session[LoginFormToken] = value;
            }
        }

        #region sign in

        /// <summary>
        /// Indexes the specified username.
        /// </summary>
        /// <returns>The index page.</returns>
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            GenerateToken();
            return View();
        }

        /// <summary>
        /// Indexes the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="loginToken">The loginToken.</param>
        /// <returns>The index page.</returns>
        [HttpPost]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Index(string username, string password, string loginToken, string returnUrl)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && CheckValidToken(loginToken))
            {
                var user = _userService.GetByUsername(username.Trim()) as User;

                try
                {
                    if (user != null)
                    {
                        string hashPassword = GetHash(user.Password + loginToken);

                        if (password == hashPassword)
                        {
                            ClearToken();

                            var group = user.Group == null ? null : _groupService.GetById(user.Group.Id);
                            var siteLevelList = _siteService.GetSiteLevelByUser(user.Id, Environment); // gets all sites of logging-in user

                            if ((group == null || group.IsActive) &&
                                siteLevelList.Where(x => x.IsActive).Any())
                            {
                                //// allows user login

                                //// check-in user

                                var criteria = new UserCheckInUserCriteria
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

                                _userService.UserCheckIn(criteria);

                                //// check-in user

                                //// saves user session

                                _currentSession.Level = _userService.GetUserSiteLevel(user.Id, this.SiteId);
                                _currentSession.UserId = user.Id;
                                _currentSession.Username = username;
                                _currentSession.OnUserId = criteria.OnUserId;
                                _currentSession.SessionId = Session.SessionID.ToString();

                                // current site is not NAP Hub
                                if (this.SiteId != 0)
                                {
                                    CreateUserMenu(user);
                                }

                                CreateAuthInfo(user);
                                AppSession.CurrentUser = user;
                                AppSession.RoleAccess = PermissionService.GetRoleAccess(_functionService.GetByUserAndSiteIncludeGroup(
                                    user.Id,
                                    NapSettings.Settings.SiteId,
                                    NapSettings.Settings.Environment));

                                //// saves user session

                                if (AppSession.CurrentUserMenu != null &&
                                    AppSession.CurrentUserMenu.Where(m => m.Parent == null).ToList().Count > 0)
                                {
                                    //// redirects user

                                    // if the user login directly, not through NAP Hub
                                    if (this.IsSiteLogin)
                                    {
                                        //// checks if logging-in user is active on this site

                                        bool userIsActiveOnThisSite = false;

                                        foreach (var siteLevel in siteLevelList)
                                        {
                                            if (siteLevel.Site.Id == this.SiteId && siteLevel.IsActive)
                                            {
                                                userIsActiveOnThisSite = true;
                                            }
                                        }

                                        #endregion sign in

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
                                        AppSession.ViewIPVN = PermissionService.HasPermission(AccReportFunctionKeys.ViewIPVN);
                                        //// user has access to multiple sites
                                        if (siteLevelList.Count() > 1)
                                        {
                                            return RedirectToAction("Site", "UserSite");
                                        }

                                        return RedirectToAction("GoToSite", "UserSite", new { id = siteLevelList.First().Site.Id });
                                    }

                                    //// redirects user
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
                                if (siteLevelList.Count() > 0)
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

                    GenerateToken();
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message, ex);
                    throw;
                }
            }
            else
            {
                ViewBag.Message = "Invalid input!";
                GenerateToken();
            }

            return View();
        }

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
        [HttpPost]
        public ActionResult ReCheckLogin()
        {
            if (AppSettings.IsLocalTest)
            {
                return Json(new
                {
                    IsKickedOut = "0"
                });
            }

            try
            {
                AppSession.RoleAccess = PermissionService.GetRoleAccess(_functionService.GetByUserAndSiteIncludeGroup(
                                    AppSession.CurrentUser.Id,
                                    NapSettings.Settings.SiteId,
                                    NapSettings.Settings.Environment));

                var siteStatus = _siteService.GetStatusById(this.SiteId);
                if (!siteStatus)
                {
                    return Json(new
                    {
                        IsKickedOut = "1"
                    });
                }

                UserReCheckInUserCriteria criteria = new UserReCheckInUserCriteria();
                criteria.OnUserId = Convert.ToInt32(_currentSession.OnUserId);
                _userService.ReUserCheckIn(criteria);

                return Json(new
                {
                    OnUserId = Convert.ToInt32(_currentSession.OnUserId),
                    IsKickedOut = criteria.IsKickedOut ? "1" : "0"
                });
            }
            catch
            {
            }

            return Json(new
            {
                IsKickedOut = "0"
            });
        }

        #endregion re check in

        private void CreateUserMenu(User user)
        {
            _currentSession.MenuItemList = (IList<MenuItem>)_menuItemService.GetByUserAndSiteIncludeGroup(
                                                                                    user.Id,
                                                                                    SiteId,
                                                                                    Environment,
                                                                                    true);
            AppSession.CurrentUserMenu = _currentSession.MenuItemList;
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
            authInfo.OnUserId = this._currentSession.OnUserId;

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
                Path = currentDomain.Path,
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        private void DoSignOut()
        {
            _userService.UserLogOut(new LogOutUserCriteria
            {
                OnUserId = _currentSession.OnUserId
            });
            FormsAuthentication.SignOut();
            _currentSession.ClearAll();

            var requestCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Expires = DateTime.Now.AddYears(-1);
            cookie.Path = requestCookie.Path;
            Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Adds the login token to cookie.
        /// </summary>
        /// <param name="loginToken">The login token.</param>
        private void AddTokenToCookie(string loginToken)
        {
            // Create the non-persistent secure form token cookie
            var responseCookie = new HttpCookie(LoginFormToken)
            {
                HttpOnly = true,
                Value = loginToken
            };

            // If we are using SSL, the cookie should be set to secure to
            // prevent it from being sent over HTTP connections
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }

            // Add the cookie to the response
            Response.Cookies.Set(responseCookie);
        }

        /// <summary>
        /// Generates the token for login form.
        /// </summary>
        private void GenerateToken()
        {
            string loginToken = Guid.NewGuid().ToString("N");

            FormToken = loginToken;
            ViewBag.LoginToken = loginToken;
            AddTokenToCookie(loginToken);
        }

        /// <summary>
        /// Checks if the token is valid or not.
        /// </summary>
        /// <param name="loginToken">The login token.</param>
        /// <returns>Boolean value indicates if the login token is valid or not.</returns>
        private bool CheckValidToken(string loginToken)
        {
            return !string.IsNullOrWhiteSpace(loginToken)
                && FormToken == loginToken
                && Request.Cookies[LoginFormToken] != null
                && loginToken == Request.Cookies[LoginFormToken].Value;
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
                var hashData = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLower();
            }

            return encrypted;
        }

        /// <summary>
        /// Clears the token.
        /// </summary>
        private void ClearToken()
        {
            Session.Remove(LoginFormToken);

            if (Request.Cookies[LoginFormToken] != null)
            {
                HttpCookie cookie = new HttpCookie(LoginFormToken);

                cookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookie);
            }
        }
    }
}