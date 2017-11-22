namespace Fanex.AccountingReports.LegendManagement.Services.Impls
{
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.AccountingReports.LegendManagement.Criteria;
    using Fanex.AccountingReports.LegendManagement.Models;
    using Fanex.AccountingReports.LegendManagement.Services.Interfaces;
    using Fanex.Common;
    using Fanex.NAP.Core.Business;
    using NAP.Core.Service.Impl;
    using NAP.Core.Service.Interface;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Menu Service.
    /// </summary>
    public class MenuService : ServiceBase, IMenuService
    {
        private ISiteService _accNapMenuService;

        public MenuService()
            : this(null)
        {
        }

        public MenuService(ISiteService accNapMenuService)
        {
            this._accNapMenuService = accNapMenuService ?? new SiteService();
        }

        /// <summary>
        /// Gets the menu list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The menu list.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<MenuItem> GetMenuList(GetSystemMenuCriteria criteria)
        {
            try
            {
                switch (criteria.System)
                {
                    case Constants.SystemName.AccNapReport:
                        var menuList = this._accNapMenuService.GetMenuItemBySiteId(WebSettings.NAPSiteId, Constant.UAT);
                        return this.MappingNapMenuToAccountingMenu(menuList);
                    case Constants.SystemName.AccReport:
                        var rawData = GetReportData<dynamic>(criteria)
                            .OrderBy(menuItem => menuItem.Menu)
                            .ToList();

                        var listMenu = this.RecursionMenu(rawData, 0);
                        return listMenu;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Recursions the menu.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns>The menu list.</returns>
        private List<MenuItem> RecursionMenu(List<dynamic> rawData, int parentId)
        {
            List<MenuItem> menuList = null;
            if (rawData != null || rawData.Count > 0)
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

        /// <summary>
        /// Mappings the nap menu to accounting menu.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns>Mappings result.</returns>
        private List<MenuItem> MappingNapMenuToAccountingMenu(IEnumerable<NAP.Core.Models.MenuItem> rawData, int? parentId = null)
        {
            List<MenuItem> menuList = null;
            if (rawData != null || rawData.Count() > 0)
            {
                var rows = rawData.Where(menuItem =>
                    (parentId == null && menuItem.Parent == null) ||
                    (menuItem.Parent != null && menuItem.Parent.Id == parentId));

                if (rows != null && rows.Count() > 0)
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
                            FunctionName = Converter.ToString2(row.Function.Name),
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
