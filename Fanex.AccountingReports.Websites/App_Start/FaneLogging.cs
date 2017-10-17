using System.Web;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Extensions.Web;

namespace Fanex.AccountingReports.Websites.App_Start
{
    public static class FanexLogging
    {
        public static void Register()
        {
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
