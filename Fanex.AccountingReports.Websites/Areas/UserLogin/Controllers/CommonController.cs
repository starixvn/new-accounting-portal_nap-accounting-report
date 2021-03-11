namespace Fanex.AccountingReports.Websites.Areas.UserLogin.Controllers
{
    using App_Start;
    using ONEbook.UM;
    using ONEbook.UM.Services;
    using System.Web.Mvc;
    using System.Web.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CommonController : Controller
    {
        public CommonController()
        {
        }

        /// <summary>
        /// Goes offline.
        /// </summary>
        /// <returns>Goes offline result.</returns>
        public ActionResult Offline()
        {
            var maintenanceService = new MaintenanceService(UMClientManager.RestClient,
                UMClientManager.CacheService,
                UMClientManager.UMClientConfigs,
                UMClientManager.HttpContext);

            var downTime = maintenanceService.GetUnderMaintenanceTime();

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