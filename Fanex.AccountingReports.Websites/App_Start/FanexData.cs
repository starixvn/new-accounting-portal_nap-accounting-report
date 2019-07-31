using Fanex.Data;

namespace Fanex.AccountingReports.Websites.App_Start
{
    public static class FanexData
    {
        public static void Register()
        {
            DbSettingProviderManager
             .StartNewSession()
             .UseDefaultConnectionStringProvider()
             .UseDefaultDbSettingProvider($"~/App_Data/Fanex.Data")
             .Run();
        }
    }
}