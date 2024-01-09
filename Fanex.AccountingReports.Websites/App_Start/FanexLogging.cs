using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Extensions.Web;
using Fanex.Logging.Sentry.RavenClient;
using SharpRaven;

namespace Fanex.AccountingReports.Websites.App_Start
{
    public static class FanexLogging
    {
        public static void Register()
        {
            LogManager
            .SetDefaultLogCategory(ConfigurationManager.AppSettings["SiteCategory"])
            .Use(new List<ILogEngine>
            {
                 new SentryLogging(new SentryEngineOptions
                 {
                     Dsn = new Dsn(ConfigurationManager.AppSettings["SentryToken"]),
                 })
        });

            var configuration = new LoggingConfiguration()
                 .GetHttpHeaderInfo()
                 .GetHttpSessionInfo()
                 .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);
            ExceptionHandler.UseConfig(configuration);
        }

        public static void HandleApplicationError(HttpServerUtility server)
        {
            ExceptionHandler.Handle(server.GetLastError());
        }
    }
}
