// --------------------------------------------------------------------------
// <copyright file="WebSettings.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Configurations
{
    using System.Configuration;
    using Fanex.Common;
    using Fanex.NAP.Core.Configuration;

    /// <summary>
    /// Web Settings.
    /// </summary>
    public static class WebSettings
    {
        static WebSettings()
        {
            Version = ConfigurationManager.AppSettings["version"];
            System = ConfigurationManager.AppSettings["System"];
            DefaultReportLegendTemplate = Converter.ToInt(ConfigurationManager.AppSettings["DefaultReportLegendTemplate"]);
            NAPSiteId = Converter.ToInt(NapSettings.Settings.SiteId);
        }

        public static string Version { get; set; }

        public static string System { get; set; }

        public static int DefaultReportLegendTemplate { get; set; }

        public static int NAPSiteId { get; set; }
    }
}
