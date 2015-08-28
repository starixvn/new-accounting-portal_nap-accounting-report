// -----------------------------------------------------------------------
// <copyright file="GamingAreaRegistration.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.12.11">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming
{
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GamingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get 
            { 
                return "Gaming"; 
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Gaming_default",
                "Gaming/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.Gaming.Controllers" });
        }
    }
}
