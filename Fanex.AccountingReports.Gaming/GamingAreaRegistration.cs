namespace Fanex.AccountingReports.Gaming
{
    using System.Web.Mvc;

    public class GamingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get 
            { 
                return "Gaming"; 
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Gaming_default",
                "Gaming/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.Gaming.Controllers" });
        }
    }
}
