// --------------------------------------------------------------------------
// <copyright file="LegendManagementAreaRegistration.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ethan.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee
{
    using System.Web.Mvc;

    /// <summary>
    /// Legend Management Area Registration.
    /// </summary>
    public class LegendManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LegendManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LegendManagement_default",
                "LegendManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.LegendManagement.Controllers" });
        }
    }
}
