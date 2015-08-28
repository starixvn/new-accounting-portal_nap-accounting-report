// --------------------------------------------------------------------------
// <copyright file="HandleAjaxErrorAttribute.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Steven.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Exceptions
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HandleAjaxErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param>
        public override void OnException(ExceptionContext filterContext)
        {
            // if the request is AJAX return JSON else view.
            var requestHeaderType = filterContext.HttpContext.Request.Headers["X-Requested-With"];
            switch (requestHeaderType)
            {
                case RequestHeaderTypeConstants.XMLHttpRequest:
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            error = HttpStatusCodeConstants.ServerError,
                            message = Errors.SystemError,
                            innerException = filterContext.Exception.ToString()
                        }
                    };

                    break;
                default:
                    var controllerName = (string)filterContext.RouteData.Values["controller"];
                    var actionName = (string)filterContext.RouteData.Values["action"];
                    var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                    filterContext.Result = new ViewResult
                    {
                        ViewName = View,
                        MasterName = Master,
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = filterContext.Controller.TempData
                    };

                    break;
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = HttpStatusCodeConstants.ServerError;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}