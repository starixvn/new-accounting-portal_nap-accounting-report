namespace Fanex.AccountingReports.Websites
{
    using System.Web.Mvc;
    using ONEbook.UM.Filters;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckUMFilter());
            filters.Add(new UserActivityDetailFilter());
        }
    }
}
