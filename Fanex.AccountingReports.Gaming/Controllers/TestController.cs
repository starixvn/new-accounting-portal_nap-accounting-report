using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fanex.AccountingReports.Gaming.Controllers
{
    public class TestController: Controller
    {
        public ActionResult Index()
        {
            Logging.LogService.Info(DateTime.Now.ToString());
            return this.View();
        }
    }
}
