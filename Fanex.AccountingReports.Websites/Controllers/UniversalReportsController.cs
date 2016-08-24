using Fanex.AccountingReports.Core.Web.Controllers;
using System.Web.Mvc;

namespace Fanex.AccountingReports.Websites.Controllers
{
    public class UniversalReportsController : BaseController
    {
        public ActionResult HouseWinLossByProducts()
        {
            return Redirect("../../UniversalReports/HouseWinLossByProducts");
        }
    }
}
