using System.Web.Mvc;
using Fanex.AccountingReports.Core.Web.Controllers;
using Fanex.NAP.Core.Business;
using Fanex.NAP.Core.Web.Attribute;

namespace Fanex.AccountingReports.Special.Controllers
{
    public class PunterAnalysisController : BaseController
    {
        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.PunterAnalysis, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return Redirect("../../../UniversalReports/PunterAnalysis");
        }
    }
}