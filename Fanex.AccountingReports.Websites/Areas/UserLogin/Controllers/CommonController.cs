// --------------------------------------------------------------------------
// <copyright file="CommonController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2013.1.4">Created</change>
// </history>

namespace Fanex.AccountingReports.Websites.Areas.UserLogin.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CommonController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public CommonController()
            : this(null)
        {
        }

        public CommonController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        /// <summary>
        /// Goes offline.
        /// </summary>
        /// <returns>Goes offline result.</returns>
        public ActionResult Offline()
        {
            var downTime = _maintenanceService.GetDowntimeInfo();

            if (!downTime.IsDowntime)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            ViewBag.UmtFrom = downTime.From;
            ViewBag.UmtTo = downTime.To;

            return View();
        }

        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        public ActionResult DownloadReport(string time, string reportName)
        {
            return RedirectToAction("DownloadReport", "DownloadReport", new { time = time, reportName = reportName, area = string.Empty });
        }

        public ActionResult ExpiredNotification(int type)
        {
            string message = "Sorry, This file has been expried. Please contact Starix IT for more details.";
            if (type == 1)
            {
                message = "Sorry, This file is not found. Please contact Starix IT for more details.";
            }

            ViewBag.Message = message;
            return View("ExpiredNotification");
        }
    }
}