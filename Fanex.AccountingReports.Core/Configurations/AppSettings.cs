namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Collections.Generic;
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class AppSettings
    {
        private static bool _isUsingPort = false;

        private static string _dockerFolders;

        private static string _dockerFileApi;

        static AppSettings()
        {
            _isUsingPort = ConfigurationManager.AppSettings["IsUsingPort"] == "true";
            _dockerFolders = ConfigurationManager.AppSettings["DockerFolders"];
            _dockerFileApi = ConfigurationManager.AppSettings["DockerFileApi"];
        }

        public static List<string> MultiAgentLicensses => new List<string> { "12Bet" };

        public static int IntervalGetMostVisitedPage => 10;

        public static bool IsDisableMostvisitedPage => Converter.ToBool(ConfigurationManager.AppSettings["IsDisableMostvisitedPage"]);

        public static int TopRateMostVisitedPages => 9;

        public static int MaxPagesOfMostVisitedPage => 9;

        public static bool IsUsingPort
        {
            get { return _isUsingPort; }
        }

        public static string DockerFolders
        {
            get
            {
                return _dockerFolders;
            }
        }

        public static string DockerFileApi
        {
            get
            {
                return _dockerFileApi;
            }
        }
    }
}
