using Fanex.Caching;
using ONEbook.UM;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.NETFramework;
using ONEbook.UM.Transport;
using System.Configuration;

namespace Fanex.AccountingReports.Websites.App_Start
{
    public static class OnebookUMInitializer
    {
        public static void InitializeUMClientManager()
        {
            var umConfig = new UMClientConfiguration().Initialize();

            UMClientManager
              .UseConfig(umConfig)
              .UseCacheService(new UMCacheService(new CacheService()))
              .UseRestClient(new UMRestClient(umConfig))
              .UseHttpContext(new HttpContextHelper());
        }
    }
}