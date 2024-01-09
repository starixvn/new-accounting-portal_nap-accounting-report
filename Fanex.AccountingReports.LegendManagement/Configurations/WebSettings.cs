namespace Fanex.AccountingReports.LegendManagement.Configurations
{
    using Fanex.Common;
    using Fanex.NAP.Core.Configuration;

    /// <summary>
    /// Web Settings.
    /// </summary>
    public static class WebSettings
    {
        static WebSettings()
        {
            NAPSiteId = Converter.ToInt(NapSettings.Settings.SiteId);
        }

        public static string System => "ACC-NAP";

        public static int DefaultReportLegendTemplate => -1;

        public static int NAPSiteId { get; set; }
    }
}
