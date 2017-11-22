namespace Fanex.AccountingReports.Test
{
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Test";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Test_default",
                "Test/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.Test.Controllers" });
        }
    }
}
