namespace Fanex.AccountingReports.Websites
{
    using ONEbook.UM;
    using ONEbook.UM.AspNetMVC.Filters;
    using ONEbook.UM.Services;
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var umConfig = UMClientManager.UMClientConfigs;

            var maintenanceService = new MaintenanceService(
                                  UMClientManager.RestClient,
                                  UMClientManager.CacheService,
                                  UMClientManager.UMClientConfigs,
                                  UMClientManager.HttpContext);

            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckUMFilterAttribute(maintenanceService, umConfig));
            filters.Add(new UserActivityDetailFilter());
        }
    }
}