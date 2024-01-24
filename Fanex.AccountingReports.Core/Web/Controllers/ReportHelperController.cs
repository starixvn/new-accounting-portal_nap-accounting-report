namespace Fanex.AccountingReports.Core.Web.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Services.Impl;

    public class ReportHelperController : BaseController
    {
        [HandleAjaxError]
        public JsonResult UserNameSuggestion(bool excludeTestAccounts, string query)
        {
            var criteria = new UserNameSuggestionCriteria
            {
                Username = query,
                ExcludeTestAccounts = excludeTestAccounts
            };

            var result = ReportService.UserNameSuggestion(criteria);
            var jsonResult = Json(new
            {
                query = result.QueryValue,
                suggestions = result.Suggestions,
                data = result.DataInfo
            });

            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyByLicensee(GetCurrencyByLicenseeCriteria criteria)
        {
            var result = ReportService.GetCurrencyByLicensee(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HandleAjaxError]
        public JsonResult LeagueSuggestion(string sportBettype, string query)
        {
            var criteria = new LeagueSuggestionCriteria
            {
                SportBettype = sportBettype,
                SearchText = query
            };

            var result = ReportService.LeagueSuggestion(criteria);
            var jsonResult = Json(new
            {
                query = result.QueryValue,
                suggestions = result.Suggestions,
                data = result.DataInfo
            });

            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }
    }
}
