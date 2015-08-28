// --------------------------------------------------------------------------
// <copyright file="LegendManagementBundleConfig.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement
{
    using System.Web.Optimization;

    /// <summary>
    /// Legend Management Bundle Configuration.
    /// </summary>
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
