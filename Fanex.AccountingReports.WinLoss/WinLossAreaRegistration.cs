namespace Fanex.AccountingReports.WinLoss
{
    using System.Web.Mvc;

   public class WinLossAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WinLoss";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WinLoss_default",
                "WinLoss/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.WinLoss.Controllers" });
        }
    }
}
