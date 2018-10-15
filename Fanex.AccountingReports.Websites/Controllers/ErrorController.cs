using System.Web.Mvc;

namespace Fanex.AccountingReports.Websites.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}