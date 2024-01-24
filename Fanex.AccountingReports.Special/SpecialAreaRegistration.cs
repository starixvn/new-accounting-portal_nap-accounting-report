namespace Fanex.AccountingReports.Licensee
{
    using System.Web.Mvc;

    public class SpecialAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Special";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Special_default",
                "Special/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.Special.Controllers" });
        }
    }
}
