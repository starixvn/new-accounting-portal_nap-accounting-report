namespace Fanex.AccountingReports.Websites.Areas.UserControl
{
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserControlAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UserControl";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "UserControl_default",
                "UserControl/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
