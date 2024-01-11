namespace Fanex.AccountingReports.LegendManagement.Services.Impls
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.AccountingReports.LegendManagement.Criteria;
    using Fanex.AccountingReports.LegendManagement.Models;
    using Fanex.Common;
    using Fanex.NAP.Core.Business;
    using NAP.Core.Service.Impl;

    public class MenuService : ServiceBase
    {
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<MenuItem> GetMenuList(GetSystemMenuCriteria criteria)
        {
            var menuList = new SiteService().GetMenuItemBySiteId(WebSettings.NAPSiteId, Constant.UAT, true);
            return this.MappingNapMenuToAccountingMenu(menuList);
        }

        private List<MenuItem> RecursionMenu(List<dynamic> rawData, int parentId)
        {
            var menuList = new List<MenuItem>();
            if (rawData == null)
            {
                return menuList;
            }

            if (rawData.Count > 0)
            {
                var rows = rawData.Where(menuItem => menuItem.ParentId == parentId);
                menuList = new List<MenuItem>();
                foreach (var row in rows)
                {
                    var menuItem = new MenuItem
                    {
                        Id = Converter.ToInt(row.MenuId),
                        Name = Converter.ToString2(row.Menu),
                        Href = Converter.ToString2(row.Url),
                        System = row.System,
                        ParentId = Converter.ToInt(row.ParentId),
                        FunctionName = Converter.ToString2(row.FuncName),
                        Target = Converter.ToString2(row.Target),
                        IsHide = Converter.ToBool(row.IsHide),
                        Order = Converter.ToInt(row.Order),
                        SubMenu = this.RecursionMenu(rawData, row.MenuId)
                    };

                    menuItem.SubMenu = this.RecursionMenu(rawData, menuItem.Id);
                    menuList.Add(menuItem);
                }
            }

            return menuList;
        }

        private List<MenuItem> MappingNapMenuToAccountingMenu(IEnumerable<NAP.Core.Models.MenuItem> rawData, int? parentId = null)
        {
            var menuList = new List<MenuItem>();
            if (rawData == null)
            {
                return menuList;
            }

            if (rawData.Any())
            {
                var rows = rawData.Where(menuItem =>
                    (parentId == null && menuItem.ParentId == 0) ||
                    (menuItem.ParentId != 0 && menuItem.ParentId == parentId));

                if (rows != null && rows.Any())
                {
                    menuList = new List<MenuItem>();
                    foreach (var row in rows)
                    {
                        var menuItem = new MenuItem
                        {
                            Id = Converter.ToInt(row.Id),
                            Name = Converter.ToString2(row.Name),
                            Href = Converter.ToString2(row.Url),
                            System = Constants.SystemName.AccNapReport,
                            ParentId = Converter.ToInt(parentId),
                            FunctionName = Converter.ToString2(row.FunctionName),
                            IsHide = !Converter.ToBool(row.IsActive),
                            Order = Converter.ToInt(row.DisplayOrder),
                            SubMenu = this.MappingNapMenuToAccountingMenu(rawData, row.Id)
                        };

                        menuItem.SubMenu = this.MappingNapMenuToAccountingMenu(rawData, menuItem.Id);
                        menuList.Add(menuItem);
                    }
                }
            }

            return menuList;
        }
    }
}
