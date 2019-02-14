using Fanex.Data;
using Fanex.NAP.Core.Configuration;

namespace Fanex.AccountingReports.Websites.App_Start
{
    public static class FanexData
    {
        public static void Register()
        {
            var resourceFolder = NapSettings.Settings.Environment == "LIVE"
                ? "LIVE"
                : "TEST";
            DbSettingProviderManager
             .StartNewSession()
             .UseDefaultConnectionStringProvider()
             .UseDefaultDbSettingProvider(
               $"~/App_Data/{resourceFolder}")
             .Run();
        }
    }
}