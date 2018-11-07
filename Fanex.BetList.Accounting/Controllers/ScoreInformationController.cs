namespace Fanex.BetList.Accounting.Controllers
{
    using System.Web.Mvc;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Services;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ScoreInformationController : Controller
    {
        private readonly ScoreInformationService _service;

        public ScoreInformationController()
        {
            this._service = new ScoreInformationService();
        }

        [HttpPost]
        public JsonResult GetScoreInformation(GetScoreInformationCriteria criteria)
        {
            return Json(this._service.GetScoreInfo(criteria));
        }
    }
}
