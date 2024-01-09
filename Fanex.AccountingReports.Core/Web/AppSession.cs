namespace Fanex.AccountingReports.Core.Web
{
    using Fanex.AccountingReports.Core.Constants;
    using NAP.Core.Models;
    using NAP.Core.Web.Session;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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

        /// <summary>
        /// Gets or sets current login user.
        /// </summary>
        /// <value>
        /// The current user.
        /// </value>
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

        /// <summary>
        /// Gets or sets menu of current login user.
        /// </summary>
        /// <value>
        /// The current user menu.
        /// </value>
        public static IList<MenuItem> CurrentUserMenu
        {
            get
            {
                return currentSession.MenuItemList;
            }
        }

        /// <summary>
        /// Gets or sets current working menu url.
        /// </summary>
        /// <value>
        /// The current working on menu.
        /// </value>
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

        /// <summary>
        /// Gets or sets IsTrackedMenu: determine current working on menu is
        /// tracked for page tracking or not.
        /// </summary>
        /// <value>
        /// The name of the current action.
        /// </value>
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
