namespace Fanex.AccountingReports.Websites
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Optimization;
    using Fanex.AccountingReports.LegendManagement;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class BundleConfig
    {
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/scripts/_lib/fanex.result/style")
              .Include("~/Content/Fanex.Result/fanex.result.css")
              .Include("~/Content/Fanex.Result/fanex.result-customized.css"));
            
            bundles.Add(new StyleBundle("~/scripts/_lib/Tooltip/style").Include(
            "~/scripts/_lib/Tooltip/tipsy.css"));
            bundles.Add(new StyleBundle("~/themes/base/jquery/style").Include(
            "~/themes/base/jquery/jquery-ui-1.9.1.custom.css"));
            bundles.Add(new StyleBundle("~/themes/style").Include(
            "~/themes/reset.css"));
            bundles.Add(new StyleBundle("~/scripts/_lib/Fanex.Grid/style").Include(
            "~/scripts/_lib/Fanex.Grid/Fanex.Pagination.css"));
            bundles.Add(new ScriptBundle("~/scripts-bundle/jquery").Include(
                       "~/scripts/_lib/jquery-*",
                       "~/scripts/_lib/BlockUI/jquery.blockUI.1.33.js",
                       "~/scripts/_lib/FormatLibs/NumberFormat/numeral.js",
                       "~/scripts/_lib/Fanex.Grid/Fanex.Grid.js",
                       "~/scripts/_lib/Fanex.Grid/Fanex.Pagination.js",
                       "~/scripts/_lib/Tooltip/jquery.tipsy.js",
                       "~/scripts/NapAccReports.js"));

            bundles.Add(new ScriptBundle("~/scripts/_lib/msgbox/JS").Include(
                "~/scripts/_lib/msgbox/msgbox-0.0.1.js"));

            bundles.Add(new StyleBundle("~/scripts/_lib/msgbox/CSS").Include(
                 "~/scripts/_lib/msgbox/msgbox-0.0.1.css"));

            bundles.Add(new StyleBundle("~/scripts/_lib/MultipleSelect/style").Include(
            "~/scripts/_lib/MultipleSelect/jquery.multiselect.css"));
            bundles.Add(new ScriptBundle("~/scripts/_lib/multiselect").Include(
                      "~/scripts/_lib/MultipleSelect/jquery.multiselect.js"));

            bundles.Add(new StyleBundle("~/scripts/_lib/DatePicker/style").Include(
             "~/scripts/_lib/DatePicker/jquery.nexDatePicker.css"));
            bundles.Add(new ScriptBundle("~/scripts/_lib/nexDatePicker").Include(
                      "~/scripts/_lib/DatePicker/jquery.nexDatePicker.js"));

            bundles.Add(new ScriptBundle("~/scripts/_lib/template").Include(
                     "~/scripts/_lib/TemplateRender/doT/doT.js"));

            bundles.Add(new StyleBundle("~/scripts/_lib/MonthPicker/style").Include(
            "~/scripts/_lib/MonthPicker/monPickr-0.0.3.css"));

            bundles.Add(new ScriptBundle("~/scripts/_lib/monPickr").Include(
                      "~/scripts/_lib/MonthPicker/monPickr-0.0.3.js"));

            bundles.Add(new ScriptBundle("~/scripts-bundle/knockout").Include(
                        "~/scripts/_lib/knockout*"));

            LegendManagementBundleConfig.RegisterBundles(bundles);
        }
    }
}
