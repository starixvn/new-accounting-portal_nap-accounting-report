namespace Fanex.AccountingReports.Websites.Areas.UserLogin.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CommonController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public CommonController()
        {
            _maintenanceService = new MaintenanceService();
        }

        /// <summary>
        /// Goes offline.
        /// </summary>
        /// <returns>Goes offline result.</returns>
        public ActionResult Offline()
        {
            var downTime = _maintenanceService.GetUnderMaintenanceTime();

            if (!downTime.IsUnderMaintenanceTime)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            ViewBag.UmtFrom = downTime.From;
            ViewBag.UmtTo = downTime.To;

            return View();
        }

        [Authorize]
        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        public ActionResult DownloadReport(string time, string reportName)
        {
            return RedirectToAction("DownloadReport", "DownloadReport", new { time, reportName, area = string.Empty });
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
