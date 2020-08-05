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

        public ActionResult NG2Statistics()
        {
            return Redirect("../../UniversalReports/NG2Statistics");
        }

        public ActionResult PlayForFunStatistics()
        {
            return Redirect("../../UniversalReports/PlayForFunStatistics");
        }

        public ActionResult PunterAnalysis()
        {
            return Redirect("../../UniversalReports/PunterAnalysis");
        }

        public ActionResult UpdatedUsernameAndGivenCredit()
        {
            return Redirect("../../UniversalReports/UpdatedUsernameAndGivenCredit");
        }

        public ActionResult Exchange()
        {
            return Redirect("../../UniversalReports/Exchange");
        }

        public ActionResult ActualMemberBet()
        {
            return Redirect("../../UniversalReports/ActualMemberBet");
        }

        public ActionResult DailyWinLoss2hSoccerMinor()
        {
            return Redirect("../../UniversalReports/DailyWinLoss2hSoccerMinor");
        }

        public ActionResult CN88WinLossByProducts()
        {
            return Redirect("../../UniversalReports/CN88WinLossByProducts");
        }

        public ActionResult AbnormalBet()
        {
            return Redirect("../../UniversalReports/AbnormalBet");
        }

        public ActionResult SBPerformanceMonitor()
        {
            return Redirect("../../UniversalReports/SBPerformanceMonitor");
        }

        public ActionResult SMAWinLoss()
        {
            return Redirect("../../UniversalReports/SMAWinLoss");
        }
    }
}