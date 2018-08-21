namespace Fanex.AccountingReports.Websites
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using BetList.Core;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Services.Impl.Worker;
    using Fanex.AccountingReports.Websites.App_Start;
    using Fanex.Data.Configuration;
    using RemotingRef;
    using Result.Core.Base;
    using Result.Core.RemotingRef;

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

            RefDataTypeContainer
               .ConnectTo(new Uri(ConfigurationManager.AppSettings["Fanex.RemotingRef.Service.Url"]))
               .FromApplication("Nap - Reports")
               .SupportOnlyDefaultLanguage();

            BetListManager.RegisterRemotingRef(new BetList.Core.RemotingRef.CacheRef());

            ResultCoreManager.RegisterRefDataHelper(new RefDataHelper());

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

            FanexLogging.Register();
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

                FanexLogging.HandleApplicationError(Server);
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
