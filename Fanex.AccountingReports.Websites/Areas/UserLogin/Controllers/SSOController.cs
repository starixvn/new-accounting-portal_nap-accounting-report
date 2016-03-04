// --------------------------------------------------------------------------
// <copyright file="SSOController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Websites.Areas.UserLogin.Controllers
{
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.Common;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Criteria.User;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Auth;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SSOController : BaseController
    {
        #region members

        private readonly int _timeExipration = 30;
        private IUserService _userService;
        private IMenuItemService _menuItemService;
        private ICurrentSession _currentSession;

        #endregion members

        #region ctors

        public SSOController()
            : this(null, null, null)
        {
        }

        public SSOController(
            IUserService userService,
            IMenuItemService menuItemService,
            ICurrentSession currentSession)
        {
            _userService = userService ?? new UserService();
            _menuItemService = menuItemService ?? new MenuItemService();
            _currentSession = currentSession ?? new CurrentSession();
        }

        #endregion ctors

        /// <summary>
        /// Index method.
        /// </summary>
        /// <param name="userId">User identify.</param>
        /// <param name="onuserId">Online User identify.</param>
        /// <param name="time">Time request.</param>
        /// <param name="redirectUrl">Redirect Url.</param>
        /// <returns>Return result.</returns>
        [AllowAnonymous]
        [HttpGet]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Index(string userId, string onuserId, string time, Uri redirectUrl = null)
        {
            string currentTime = this._userService.UserSSOGetTime();
            string forwardLink = redirectUrl == null ? FormsAuthentication.DefaultUrl : redirectUrl.ToString();
            TimeSpan expiration = new TimeSpan();

            userId = Cryptography.CfsDeCrypt(userId.Replace(" ", "+"));
            onuserId = Cryptography.CfsDeCrypt(onuserId.Replace(" ", "+"));
            time = Cryptography.CfsDeCrypt(time.Replace(" ", "+"));
            expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(time);

            if (expiration.TotalSeconds <= this._timeExipration)
            {
                // do SSO check-in
                var criteria = new UserSSOCheckInUserCriteria
                {
                    UserId = Convert.ToInt32(userId),
                    SiteId = SiteId,
                    CheckOnUserId = Convert.ToInt32(onuserId),
                    OnUserId = 0,
                    IP = CurrentContext.IP,
                    NetSessionId = Session.SessionID,
                    LoginDate = DateTime.Now,
                    BrowserName = CurrentContext.BrowserName,
                    CountryCode = CurrentContext.CountryCode,
                    Domain = CurrentContext.DomainName,
                    Platform = CurrentContext.Platform
                };

                this._userService.UserSSOCheckIn(criteria);

                if (criteria.OnUserId != 0)
                {
                    var user = this._userService.GetById(criteria.UserId) as User;

                    // saves user session & auth info
                    this._currentSession.UserId = user.Id;
                    this._currentSession.Username = user.Username;
                    this._currentSession.OnUserId = criteria.OnUserId;
                    this._currentSession.SessionId = Session.SessionID.ToString();
                    this._currentSession.Level = this._userService.GetUserSiteLevel(user.Id, this.SiteId);

                    AppSession.CurrentUser = user;
                    AppSession.RoleAccess = PermissionService.GetRoleAccess((new FunctionService()).GetByUserAndSiteIncludeGroup(
                        user.Id,
                        NapSettings.Settings.SiteId,
                        NapSettings.Settings.Environment));
                    AppSession.ViewIPVN = PermissionService.HasPermission(AccReportFunctionKeys.ViewIPVN);
                    if (this.SiteId != 0)
                    {
                        this._currentSession.MenuItemList = this._menuItemService.GetByUserAndSiteIncludeGroup(
                                                                                user.Id,
                                                                                SiteId,
                                                                                this.Environment,
                                                                                true) as IList<MenuItem>;
                        AppSession.CurrentUserMenu = _currentSession.MenuItemList;
                    }

                    AppSession.IsAccountingLogin = false;
                    this.CreateAuthInfo(user);
                    return this.Redirect(forwardLink);
                }
            }

            return this.Redirect(FormsAuthentication.LoginUrl);
        }

        /// <summary>
        /// Accountings the login.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="onlineUserId">The on user identifier.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns>The login result page.</returns>
        [AllowAnonymous]
        [HttpGet]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult AccountingLogin(string username, string onlineUserId, string redirectUrl)
        {
            try
            {
                Session.Clear();
                username = Cryptography.CfsDeCrypt(username.Replace(" ", "+"));

                var user = _userService.GetByUsername(username) as User;
                if (user != null)
                {
                    //// saves user session & auth info
                    var currentContext = new CurrentContext();

                    var criteria = new UserCheckInUserCriteria
                    {
                        UserId = user.Id,
                        SiteId = 100,
                        NetSessionId = Session.SessionID,
                        Ip = currentContext.IP,
                        LoginDate = DateTime.Now,
                        BrowserName = currentContext.BrowserName,
                        CountryCode = currentContext.CountryCode,
                        Domain = currentContext.DomainName,
                        Platform = currentContext.Platform
                    };

                    _userService.UserCheckIn(criteria);
                    _currentSession.UserId = user.Id;
                    _currentSession.Username = user.Username;
                    _currentSession.OnUserId = Convert.ToInt32(criteria.OnUserId);
                    _currentSession.SessionId = Session.SessionID.ToString();
                    _currentSession.Level = _userService.GetUserSiteLevel(user.Id, this.SiteId);

                    AppSession.CurrentUser = user;
                    AppSession.RoleAccess = PermissionService.GetRoleAccess((new FunctionService()).GetByUserAndSiteIncludeGroup(
                        user.Id,
                        NapSettings.Settings.SiteId,
                        NapSettings.Settings.Environment));

                    if (this.SiteId != 0)
                    {
                        _currentSession.MenuItemList = _menuItemService.GetByUserAndSiteIncludeGroup(
                                                                                user.Id,
                                                                                SiteId,
                                                                                Environment,
                                                                                true) as IList<MenuItem>;
                        AppSession.CurrentUserMenu = _currentSession.MenuItemList;
                    }

                    AppSession.IsAccountingLogin = true;

                    CreateAuthInfo(user);
                    return Redirect(redirectUrl);
                    //// saves user session & auth info
                }

                return Redirect(FormsAuthentication.LoginUrl);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
                return Redirect(FormsAuthentication.LoginUrl);
            }
        }

        #region utilities

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
                Path = "/",
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        #endregion utilities
    }
}