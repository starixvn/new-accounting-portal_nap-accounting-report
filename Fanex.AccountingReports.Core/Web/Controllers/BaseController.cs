// --------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.12">Created</change>
//   <change who="Ricky.Nguyen" date="2012.11.14">Update Maintenance Time</change>
// </history>

namespace Fanex.AccountingReports.Core.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Web;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using NAP.Core.Context;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly ICurrentContext CurrentContext;

        protected readonly ICurrentSession CurrentSession;

        protected readonly string Environment = NapSettings.Settings.Environment;

        protected readonly int SiteId = NapSettings.Settings.SiteId;

        private bool _issiteLogin = NapSettings.Settings.EnableDirectLogin;

        private ViewFlags _viewFlags;

        private MenuService _menuService;

        public BaseController()
        {
            this.CurrentContext = new CurrentContext();
            ViewBag.Session = this.CurrentSession = new CurrentSession();

            ViewFlags = new ViewFlags
            {
                IsLive = this.Environment.Equals(Constant.LIVE),
                HasEditRight = false
            };
            this._menuService = new MenuService();
        }

        /// <summary>
        /// Gets Indicates whether the current site allows users login directly.
        /// If "0", users must login through NAP Hub.
        /// </summary>
        /// <value>
        /// The is site login.
        /// </value>
        public bool IsSiteLogin
        {
            get
            {
                return this._issiteLogin;
            }
        }

        protected ViewFlags ViewFlags
        {
            get
            {
                return ViewBag.ViewFlags;
            }

            set
            {
                ViewBag.ViewFlags = this._viewFlags = value;
            }
        }

        /// <summary>
        /// Execute before each request, if Downtime then go to Downtime page
        /// Or not Authenticate will go to Login Page.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AppSession.CurrentActionName = filterContext.ActionDescriptor.ActionName;
            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Get current working on menu for getting display name,... on webpage
            if (filterContext.Result is ViewResult && string.Equals(AppSession.CurrentActionName, "Index", StringComparison.OrdinalIgnoreCase))
            {
                var menuPath = MenuService.GetMenuPathFromRequestFilePath(filterContext.HttpContext.Request.FilePath, Request.ApplicationPath);
                string controllerName = (string)filterContext.RouteData.Values["controller"];
                AppSession.CurrentWorkingOnMenu = this._menuService.GetMenuByURL(menuPath);
            }
        }
    }
}