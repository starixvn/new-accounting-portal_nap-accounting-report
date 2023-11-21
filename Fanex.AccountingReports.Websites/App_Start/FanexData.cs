using Fanex.Data;

namespace Fanex.AccountingReports.Websites.App_Start
{
    using Data.SqlServer;

    public static class FanexData
    {
        public static void Register()
        {
            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer()
                .UseDefaultDbSettingProvider(resourcePath: "~/App_Data/Fanex.Data")
                .UseDefaultConnectionStringProvider()
                .Run();
        }
    }
}