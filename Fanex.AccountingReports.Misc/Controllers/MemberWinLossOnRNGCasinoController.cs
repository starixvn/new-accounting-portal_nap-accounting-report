// -----------------------------------------------------------------------
// <copyright file="MemberWinLossOnRNGCasinoController.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemberWinLossOnRNGCasinoController : BaseController
    {
        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MemberWinLossOnRNGCasino, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {       
            return View("~/Areas/Misc/Views/MemberWinLossOnRNGCasino/Index.cshtml");
        }
    }
}
