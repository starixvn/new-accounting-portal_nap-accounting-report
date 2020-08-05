using System;

namespace Fanex.AccountingReports.Websites
{
    public partial class WhoAmI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GC.Collect();
        }
    }
}