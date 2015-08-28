// --------------------------------------------------------------------------
// <copyright file="WinLossAreaRegistration.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ethan.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss
{
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
   public class WinLossAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WinLoss";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WinLoss_default",
                "WinLoss/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.AccountingReports.WinLoss.Controllers" });
        }
    }
}
