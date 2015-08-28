// --------------------------------------------------------------------------
// <copyright file="IMenuService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.LegendManagement.Criteria;
    using Fanex.AccountingReports.LegendManagement.Models;

    public interface IMenuService
    {
        List<MenuItem> GetMenuList(GetSystemMenuCriteria criteria);
    }
}
