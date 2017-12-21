namespace Fanex.AccountingReports.Racing
{
    using System.Web.Mvc;

    /// <summary>
    /// Register Racing Area.
    /// </summary>
    public class RacingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Racing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Racing_default",
                "Racing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.Racing.Controllers" });
        }
    }
}