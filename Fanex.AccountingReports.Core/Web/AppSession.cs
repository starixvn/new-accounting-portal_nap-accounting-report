namespace Fanex.AccountingReports.Core.Web
{
    using Fanex.AccountingReports.Core.Constants;
    using NAP.Core.Models;
    using NAP.Core.Web.Session;
    using System;
    using System.Collections.Generic;

    public static class AppSession
    {

        private static CurrentSession currentSession = new CurrentSession();

        private static readonly User UnauthorizedUser = new User
        {
            Email = string.Empty,
            FirstName = string.Empty,
            Id = 0,
            Username = string.Empty,
            LastName = string.Empty,
            Name = string.Empty,
            Password = string.Empty
        };

        public static User CurrentUser
        {
            get
            {
                return SessionManager.Get<User>(CommonConstants.CurrentLogInUser)?? UnauthorizedUser;
                
            }

            set
            {
                SessionManager.Set<User>(CommonConstants.CurrentLogInUser, value);
            }
        }

        public static IList<MenuItem> CurrentUserMenu
        {
            get
            {
                return currentSession.MenuItemList;
            }
        }

        public static MenuItem CurrentWorkingOnMenu
        {
            get
            {
                return SessionManager.Get<MenuItem>(CommonConstants.CurrentWorkingOnMenu);
            }

            set
            {
                SessionManager.Set<MenuItem>(CommonConstants.CurrentWorkingOnMenu, value);
            }
        }

        public static string CurrentActionName
        {
            get
            {
                return SessionManager.Get<string>(CommonConstants.CurrentActionName);
            }

            set
            {
                SessionManager.Set<string>(CommonConstants.CurrentActionName, value);
            }
        }
    }
}
