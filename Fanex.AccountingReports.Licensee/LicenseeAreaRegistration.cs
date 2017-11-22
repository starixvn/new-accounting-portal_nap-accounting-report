namespace Fanex.AccountingReports.Licensee
{
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LicenseeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Licensee";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Licensee_default",
                "Licensee/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.Licensee.Controllers" });
        }
    }
}
