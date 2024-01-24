namespace Fanex.AccountingReports.Licensee
{
    using System.Web.Mvc;

    public class LegendManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LegendManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LegendManagement_default",
                "LegendManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.LegendManagement.Controllers" });
        }
    }
}
