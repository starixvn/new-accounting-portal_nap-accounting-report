namespace Fanex.AccountingReports.Core.Services.Impl
{
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using NAP.Core.Configuration;
    using NAP.Core.Web.Session;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PermissionService
    {
        private static readonly IUserService UserService;

        private static readonly ICurrentSession CurrentSession;

        static PermissionService()
        {
            UserService = new UserService();
            CurrentSession = new CurrentSession();
        }

        /// <summary>
        /// HasPermission: check that user has permission to access one menu link.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>
        /// Has Read Permission indicator.
        /// </returns>
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
