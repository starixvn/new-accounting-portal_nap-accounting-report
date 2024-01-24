namespace Fanex.AccountingReports.Balance
{
    using System.Web.Mvc;

    public class BalanceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Balance";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Balance_default",
                "Balance/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.Balance.Controllers" });
        }
    }
}
