namespace Fanex.AccountingReports.LegendManagement
{
    using System.Web.Optimization;

    public static class LegendManagementBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts-bundle/legendmanagement")
                .Include("~/Areas/LegendManagement/Views/LegendManagement/Scripts/GuideManMessage.js")
                .Include("~/Areas/LegendManagement/Views/LegendManagement/Scripts/GuideMan.js"));
        }
    }
}
