// --------------------------------------------------------------------------
// <copyright file="SpecialAreaRegistration.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Faisall.Tran" date="2012.12.17">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee
{
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SpecialAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Special";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Special_default",
                "Special/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.Special.Controllers" });
        }
    }
}
