namespace Fanex.AccountingReports.Special.Controllers
{
    using System.IO;
    using System.Net;
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.SaveBetReport;
    using Fanex.AccountingReports.Special.Services.Implement.SaveBetReport;
    using Fanex.Common;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.ServiceManagement.Utility;

    /// <summary>
    /// Save bet report controller.
    /// </summary>
    public class SaveBetReportController : BaseController
    {
        private readonly SaveBetReportService _saveBetReportService;
        private NetworkCredential _networkCredential;
        private string ip = AppSettings.IP;
        private string username = AppSettings.UserName;
        private string password = AppSettings.Password;

        public SaveBetReportController()
        {
            if (AppSettings.IsAuthenticate)
            {
                if (AppSettings.IsEncryptPassword)
                {
                    password = Cryptography.CfsDeCrypt(password);
                }

                _networkCredential = new NetworkCredential(username, password);
            }

            this._saveBetReportService = new SaveBetReportService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.SaveBet, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Special/Views/SaveBetReport/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetSaveBetReport(GetMatchsCriteria criteria)
        {
            var result = this._saveBetReportService.GetMatchs(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetSportsByDate(GetSportsCriteria criteria)
        {
            var result = this._saveBetReportService.GetSports(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetLeaguesBySport(GetLeaguesCriteria criteria)
        {
            var result = this._saveBetReportService.GetLeagues(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        /// <summary>
        /// Checks the file exist.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>File is exist or not.</returns>
        [HttpGet]
        [HandleAjaxError]
        public ActionResult CheckFileExist(DownloadReportCriteria criteria)
        {
            string filePath = _saveBetReportService.GetFilePath(criteria);
            bool exists = false;

            if (AppSettings.IsAuthenticate)
            {
                using (new NetworkConnection(@"\\" + ip, _networkCredential))
                {
                    exists = System.IO.File.Exists(filePath);
                }
            }
            else
            {
                exists = System.IO.File.Exists(filePath);
            }

            if (exists)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Down load save bet report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// Save bet report excel file.
        /// </returns>
        [HttpGet]
        [HandleAjaxError]
        public ActionResult DownLoadSaveBetReport(DownloadReportCriteria criteria)
        {
            if (AppSettings.IsAuthenticate)
            {
                using (new NetworkConnection(@"\\" + ip, _networkCredential))
                {
                    return DownloadExcelFiles(criteria);
                }
            }
            else
            {
                return DownloadExcelFiles(criteria);
            }
        }

        private ActionResult DownloadExcelFiles(DownloadReportCriteria criteria)
        {
            string filePath = _saveBetReportService.GetFilePath(criteria);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileExtension = Path.GetExtension(filePath);
            var mimeType = this._saveBetReportService.GetMimeType(fileExtension);
            string fileName = _saveBetReportService.GetFileName(criteria, fileExtension);
            var contentResult = File(fileBytes, mimeType, fileName);
            return contentResult;
        }
    }
}