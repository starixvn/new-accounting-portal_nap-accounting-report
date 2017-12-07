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
