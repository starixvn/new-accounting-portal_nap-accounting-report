namespace Fanex.AccountingReports.Core.Web
{
    using Fanex.AccountingReports.Core.Constants;
    using NAP.Core.Models;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class AppSession
    {
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
                return SessionManager.Get<User>(CommonConstants.CurrentLogInUser);
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
                return SessionManager.Get<IList<MenuItem>>(CommonConstants.CurrentUserMenu);
            }

            set
            {
                SessionManager.Set<IList<MenuItem>>(CommonConstants.CurrentUserMenu, value);
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
        /// Gets or sets a value indicating whether [is tracked menu].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is tracked menu]; otherwise, <c>false</c>.
        /// </value>
        public static bool IsTrackedMenu
        {
            get
            {
                return SessionManager.Get<bool>(CommonConstants.IsTrackedMenu);
            }

            set
            {
                SessionManager.Set<bool>(CommonConstants.IsTrackedMenu, value);
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

        public static bool ViewIPVN
        {
            get
            {
                return SessionManager.Get<bool>(CommonConstants.ViewIPVN);
            }

            set
            {
                SessionManager.Set<bool>(CommonConstants.ViewIPVN, value);
            }
        }

        public static bool IsAccountingLogin
        {
            get
            {
                return SessionManager.Get<bool>("IsAccountingLogin");
            }

            set
            {
                SessionManager.Set<bool>("IsAccountingLogin", value);
            }
        }

        public static bool IsU3Account
        {
            get
            {
                return SessionManager.Get<bool>("IsU3Account");
            }

            set
            {
                SessionManager.Set<bool>("IsU3Account", value);
            }
        }

        public static bool IsU3SpecialAccount
        {
            get
            {
                return SessionManager.Get<bool>("IsU3SpecialAccount");
            }

            set
            {
                SessionManager.Set<bool>("IsU3SpecialAccount", value);
            }
        }

        public static bool IsITAccount
        {
            get
            {
                return SessionManager.Get<bool>("IsITAccount");
            }

            set
            {
                SessionManager.Set<bool>("IsITAccount", value);
            }
        }
    }
}
