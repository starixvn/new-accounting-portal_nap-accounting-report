namespace Fanex.AccountingReports.Core.Services.Impl
{
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using NAP.Core.Configuration;
    using NAP.Core.Web.Session;

    public class PermissionService
    {
        private static readonly IUserService UserService;

        private static readonly ICurrentSession CurrentSession;

        static PermissionService()
        {
            UserService = new UserService();
            CurrentSession = new CurrentSession();
        }

        public static bool HasReadPermission(string functionName)
        {
            return UserService.GetPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, functionName) >= (int)Constant.Policy.Read;
        }

        public static bool HasHostPermission(string functionName)
        {
            return UserService.GetPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, functionName) >= (int)Constant.Policy.Host;
        }

        public static bool HasPermission(string functionName)
        {
            return HasReadPermission(functionName);
        }
    }
}
