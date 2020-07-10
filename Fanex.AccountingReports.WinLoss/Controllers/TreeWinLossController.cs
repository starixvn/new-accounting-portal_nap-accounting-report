namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss;
    using Fanex.AccountingReports.WinLoss.Services.Implement.TreeWinLoss;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TreeWinLossController : BaseController
    {
        public readonly TreeWinLossService TreeWinLossService;
        public readonly TreeWinLossExcelService TreeWinLossExcelService;

        public TreeWinLossController()
        {
            this.TreeWinLossService = new TreeWinLossService();
            this.TreeWinLossExcelService = new TreeWinLossExcelService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.TreeWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/WinLoss/Views/TreeWinLoss/index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetProduct()
        {
            var result = this.TreeWinLossService.GetProduct();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSport()
        {
            var result = this.TreeWinLossService.GetSport();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetTreeWinLossByProduct(GetTreeWinLossByProductCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.SportType))
            {
                criteria.SportType = string.Empty;
            }

            var report = this.TreeWinLossService.GetTreeWinLossByProduct(criteria);
            var jsonResult = Json(report, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetTreeWinLossByLevel(GetTreeWinLossByLevelCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.SportType))
            {
                criteria.SportType = string.Empty;
            }

            var report = this.TreeWinLossService.GetTreeWinLossByLevel(criteria);
            var jsonResult = Json(report, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetVirtualRate(int custId)
        {
            var report = this.TreeWinLossService.GetVirtualRate(custId);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportTreeWinLossByProduct(GetTreeWinLossByProductCriteria criteria)
        {
            TreeWinLossExcelService.ExportExcelTreeWinLossByProduct(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTreeWinLossByLevel(GetTreeWinLossByLevelCriteria criteria)
        {
            TreeWinLossExcelService.ExportExcelTreeWinLossByLevel(criteria);
            return new EmptyResult();
        }
    }
}