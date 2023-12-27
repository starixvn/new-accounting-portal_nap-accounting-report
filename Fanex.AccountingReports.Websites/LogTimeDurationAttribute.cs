using Fanex.Logging;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Fanex.AccountingReports.Websites
{
	public class LogTimeDurationAttribute : ActionFilterAttribute
    {
        private const string StopwatchKey = "StopwatchFilter.Value";

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            actionContext.HttpContext.Items["timer"] = Stopwatch.StartNew();
            string controllerName = actionContext.RouteData.Values["controller"].ToString();
            string actionName = actionContext.RouteData.Values["action"].ToString();
			LogService.Info($"{controllerName}/{actionName} time : {DateTime.Now}");
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            Stopwatch stopwatch = actionExecutedContext.HttpContext.Items["timer"] as Stopwatch;
            string controllerName = actionExecutedContext.RouteData.Values["controller"].ToString();
            string actionName = actionExecutedContext.RouteData.Values["action"].ToString();
            // TODO something useful with stopwatch.Elapsed
            LogService.Info($"{controllerName}/{actionName} time elapsed: {stopwatch.Elapsed}");
        }
    }
}