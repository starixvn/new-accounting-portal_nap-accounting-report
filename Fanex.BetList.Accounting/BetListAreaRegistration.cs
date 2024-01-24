namespace Fanex.BetList.Accounting
{
    using System.Web.Mvc;

    public class BetListAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BetList";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BetList_default",
                "BetList/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.BetList.Accounting.Controllers" });
        }
    }
}
