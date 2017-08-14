// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Websites
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Services.Impl.Worker;
    using Fanex.AccountingReports.Websites.App_Start;
    using Fanex.Data.Configuration;
    using RemotingRef;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Starts application.
        /// </summary>
        protected void Application_Start()
        {
            ChangeApplicationName();

            // Initialize RefData. It always returns empty for first time. Please contact CTO if you have any question
            RefData.GetBetTypeName("1", "en");

            DbSettingProviderManager.Current.Start();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                MostVisitedPageSavingWorker mostVisitedPageSavingWorker = new MostVisitedPageSavingWorker("MostVisitedPage Saving Worker");
                mostVisitedPageSavingWorker.Start();
            }
            catch (Exception)
            {
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            AuthConfig.SignOut();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            AuthConfig.RegisterAuth(Context);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                DbSettingProviderManager.Current.Restart();

                Exception ex = Server.GetLastError().GetBaseException();

                if (ex is HttpException)
                {
                    return;
                }

                var formVariable = string.Empty;
                int length = Request.Form.Count;

                for (int i = 0; i < length; i++)
                {
                    if (!Request.Form.Keys[i].Equals("password", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (i != 0)
                        {
                            formVariable += "&";
                        }

                        formVariable += string.Format("{0}={1}", Request.Form.Keys[i], Request.Form[Request.Form.Keys[i]]);
                    }
                }

                var stackTrace = new StackTrace(ex);
                var methodname = stackTrace.GetFrames().Select(f => f.GetMethod()).FirstOrDefault();

                string message = ex.Message +
                    "\r\nType: " + ex.GetType().ToString() +
                    "\r\nSource: " + ex.Source +
                    "\r\nURL: " + Request.Url +
                    "\r\nDomain: " + HttpContext.Current.Request.Headers["Host"] +
                    "\r\nForm Variable: " + formVariable +
                    "\r\nQuery String: " + Request.QueryString.ToString() +
                    (methodname != null ? "<br />\r\nMethodName: " + methodname.Name : string.Empty) +
                    "\r\nTarget Site: " + ex.TargetSite +
                    "\r\nStack Trace: " + ex.StackTrace;

                if (ex is SqlException)
                {
                    message += "\r\nProcedure: " + ((SqlException)ex).Procedure +
                        "\r\nLine Number: " + ((SqlException)ex).LineNumber +
                        "\r\nServer: " + ((SqlException)ex).Server;
                }

                LoggingHandler.Log(message);
            }
            catch (Exception ex2)
            {
                LoggingHandler.Log(ex2);

                if (!Response.IsRequestBeingRedirected)
                {
                    Response.Redirect("~");
                }
            }
        }

        private void ChangeApplicationName()
        {
            string applicationName = "NAP_ACC_REPORT";

            // Change the Application Name in runtime.
            var runtimeInfo = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.Static | BindingFlags.NonPublic);
            var appNameInfo = typeof(HttpRuntime).GetField("_appDomainAppId", BindingFlags.Instance | BindingFlags.NonPublic);
            var theRuntime = runtimeInfo.GetValue(null);

            appNameInfo.SetValue(theRuntime, applicationName);
        }
    }
}