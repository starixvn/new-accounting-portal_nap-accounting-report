namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Web;
    using NAP.Core.Models;

    public class MenuService : ServiceBase
    {
        public const char UrlSeparator = '/';

        public static string GetMenuPathFromRequestFilePath(
            string filePath,
            string applicationPath)
        {
            filePath = filePath.Trim();
            applicationPath = applicationPath.Trim();
            var menuPath = filePath;
            if (!string.IsNullOrEmpty(applicationPath) &&
                menuPath.StartsWith(applicationPath, StringComparison.CurrentCultureIgnoreCase) &&
                AppSession.CurrentUserMenu != null)
            {
                menuPath = menuPath.Remove(0, applicationPath.Length);
            }

            if (!menuPath.StartsWith(UrlSeparator.ToString()))
            {
                menuPath = UrlSeparator + menuPath;
            }

            return menuPath;
        }

        public MenuItem GetMenuById(int menuId)
        {
            if (AppSession.CurrentUserMenu != null)
            {
                MenuItem menu = AppSession.CurrentUserMenu
                    .FirstOrDefault<MenuItem>(m => m.Id == menuId);
                return menu;
            }

            return null;
        }

        public MenuItem GetMenuByURL(string menuURL)
        {
            if (!string.IsNullOrEmpty(menuURL) &&
                !string.Equals(menuURL, UrlSeparator.ToString()) &&
                AppSession.CurrentUserMenu != null)
            {
                MenuItem menu = AppSession.CurrentUserMenu
                    .FirstOrDefault<MenuItem>(m => m.Url.ToLower().StartsWith(menuURL.ToLower()));
                return menu;
            }

            return null;
        }

        public MenuItem GetMenuByURL(string menuURL, IList<MenuItem> menuList)
        {
            if (!string.IsNullOrEmpty(menuURL) &&
                !string.Equals(menuURL, UrlSeparator.ToString()) &&
                AppSession.CurrentUserMenu != null)
            {
                MenuItem menu = menuList.FirstOrDefault<MenuItem>(m => m.Url.StartsWith(menuURL));
                return menu;
            }
            else
            {
                return null;
            }
        }

        public string GetMenuNameByURL(string menuURL)
        {
            if (!string.IsNullOrEmpty(menuURL) &&
                !string.Equals(menuURL, UrlSeparator.ToString()) &&
                AppSession.CurrentUserMenu != null)
            {
                MenuItem menu = AppSession.CurrentUserMenu
                    .FirstOrDefault<MenuItem>(m => m.Url.StartsWith(menuURL));
                return menu.Name;
            }

            return null;
        }
    }
}
