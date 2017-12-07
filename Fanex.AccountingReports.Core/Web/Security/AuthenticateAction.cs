namespace Fanex.AccountingReports.Core.Web.Security
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Models;
    using NAP.Core.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AuthenticateAction : ActionFilterAttribute
    {
        private readonly AuthenticationType _authenticationType;

        private readonly string _mainAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateAction"/> class.
        /// </summary>
        /// <param name="authenticationType">Read or Host.</param>
        /// <param name="mainAction">Main action of page ex: Index.</param>
        public AuthenticateAction(AuthenticationType authenticationType, string mainAction)
        {
            _authenticationType = authenticationType;
            _mainAction = mainAction;
        }

        public enum AuthenticationType
        {
            /// <summary>
            /// The read type.
            /// </summary>
            Read,

            /// <summary>
            /// The host type.
            /// </summary>
            Host
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controlName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var pathActionName = controlName + "/" + _mainAction;
            var actionPrivilege = CheckPrivilege(pathActionName);

            switch (_authenticationType)
            {
                case AuthenticationType.Read:
                    if (actionPrivilege < 1)
                    {
                        ActionDenied(filterContext);
                        return;
                    }

                    break;
                case AuthenticationType.Host:
                    if (actionPrivilege < 2)
                    {
                        ActionDenied(filterContext);
                        return;
                    }

                    break;
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Actions the denied.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        private static void ActionDenied(ActionExecutingContext filterContext)
        {
            var requestHeaderType = filterContext.HttpContext.Request.Headers["X-Requested-With"];
            switch (requestHeaderType)
            {
                case RequestHeaderTypeConstants.XMLHttpRequest:
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new Result
                        {
                            ErrorCode = -500,
                            ErrorMessage = Resources.Errors._500
                        }
                    };
                    break;
                default:
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        area = string.Empty,
                        controller = "Common",
                        action = "AccessDenied"
                    }));
                    break;
            }
        }

        /// <summary>
        /// Checks the privilege.
        /// </summary>
        /// <param name="fullActionPath">The full action path.</param>
        /// <returns>The checked result.</returns>
        private int CheckPrivilege(string fullActionPath)
        {
            var privilege = 0;
            if (string.IsNullOrEmpty(fullActionPath))
            {
                return 0;
            }

            var useMenu = AppSession.CurrentUserMenu;
            if (useMenu == null || useMenu.Count == 0)
            {
                return 0;
            }

            var result = useMenu.Where(menuItem =>
                menuItem.Url.Trim().ToLower().Contains(fullActionPath.Trim().ToLower()));
            var s = result as List<MenuItem> ?? result.ToList();
            if (s.Any())
            {
                var firstOrDefault = s.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    privilege = firstOrDefault.Policy;
                }
            }

            return privilege;
        }
    }
}
