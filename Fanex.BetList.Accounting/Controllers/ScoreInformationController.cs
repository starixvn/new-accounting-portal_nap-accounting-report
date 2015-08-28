// --------------------------------------------------------------------------
// <copyright file="ScoreInformationController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

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
        private readonly IScoreInformationService _service;
        private readonly IResultService _resultService;

        public ScoreInformationController()
            : this(null, null)
        {
        }

        public ScoreInformationController(IScoreInformationService service, IResultService resultService)
        {
            this._service = service ?? new ScoreInformationService();
            this._resultService = resultService ?? new ResultService();
        }

        [HttpPost]
        public JsonResult GetScoreInformation(GetScoreInformationCriteria criteria)
        {
            return Json(this._service.GetScoreInfo(criteria));
        }

        [HttpPost]
        public ActionResult GetTennisResult(GetScoreInformationCriteria criteria)
        {
            criteria.TraderName = string.Empty;
            var result = _resultService.GetTennisResult(criteria, criteria.MatchId.ToString(), 0);
            ViewBag.criteria = criteria;
            return PartialView("~/Areas/Betlist/Views/ScoreInformation/TennisSporting/_TennisSportting.cshtml", result);
        }
    }
}