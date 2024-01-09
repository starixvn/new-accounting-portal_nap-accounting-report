namespace Fanex.AccountingReports.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class AppSettings
    {
        private static List<string> _excludedControllers;

        private static List<string> _specialIncludedViews;

        private static List<string> _multiAgentLicensses;

        private static int _topRateMostVisitedPages;

        private static DateTime _mixParlayAdvanceSupportDate;

        private static bool _isUsingPort = false;

        private static string _saveBetReportDataDirectory;

        private static string _oldSaveBetReportDataDirectory;

        private static DateTime _newSaveBetReportDate;

        private static DateTime _saveBetDateWithZipFolder;
        private static string _dockerFolders;

        private static string _smaReportDataDirectory;

        private static DateTime _casinoSigleWalletLaunchDate;

        private static string _dockerFileApi;

        static AppSettings()
        {
            _excludedControllers = GetExcludedControllers();
            _specialIncludedViews = GetSpecialIncludedViews();
            _multiAgentLicensses = GetMultiAgentLicensses();
            IntervalGetMostVisitedPage = Converter.ToInt(ConfigurationManager.AppSettings["IntervalGetMostVisitedPage"]);
            _topRateMostVisitedPages = Converter.ToInt(ConfigurationManager.AppSettings["MostVisitedPages_MaxPages"]);
            _mixParlayAdvanceSupportDate = Converter.ToDateTime(ConfigurationManager.AppSettings["MixParlayAdvanceSupportDate"]);

            _isUsingPort = ConfigurationManager.AppSettings["IsUsingPort"] == "true";

            _saveBetReportDataDirectory = ConfigurationManager.AppSettings["SaveBetReportDataDirectory"];
            _oldSaveBetReportDataDirectory = ConfigurationManager.AppSettings["OldSaveBetReportDataDirectory"];
            _newSaveBetReportDate = Convert.ToDateTime(ConfigurationManager.AppSettings["NewSaveBetReportDate"]);
            _saveBetDateWithZipFolder = Convert.ToDateTime(ConfigurationManager.AppSettings["SaveBetDateWithZipFolder"]);
            _dockerFolders = ConfigurationManager.AppSettings["DockerFolders"];

            _smaReportDataDirectory = ConfigurationManager.AppSettings["SMAReportDataDirectory"];
            Converter.To<int>(ConfigurationManager.AppSettings["AutoGenerateReportFunctionId"]);

            _casinoSigleWalletLaunchDate = Converter.ToDateTime(ConfigurationManager.AppSettings["CasinoSingleWalletLaunchDate"]);

            _dockerFileApi = ConfigurationManager.AppSettings["DockerFileApi"];
        }

        /// <summary>
        /// Gets the excluded controllers.
        /// </summary>
        /// <value>
        /// The excluded controllers.
        /// </value>
        public static List<string> ExcludedControllers
        {
            get
            {
                return _excludedControllers;
            }
        }

        public static List<string> SpecialIncludedViews
        {
            get
            {
                return _specialIncludedViews;
            }
        }

        public static List<string> MultiAgentLicensses
        {
            get
            {
                return _multiAgentLicensses;
            }
        }

        public static DateTime MixParlayAdvanceSupportDate
        {
            get
            {
                return _mixParlayAdvanceSupportDate;
            }
        }

        public static int IntervalGetMostVisitedPage { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [is disable most visited page].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is disable most visited page]; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDisableMostvisitedPage
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsDisableMostvisitedPage"]);
            }
        }

        public static int TopRateMostVisitedPages
        {
            get
            {
                return _topRateMostVisitedPages;
            }
        }

        public static int MaxPagesOfMostVisitedPage
        {
            get
            {
                return Converter.ToInt(ConfigurationManager.AppSettings["MostVisitedPages_MaxPages"]);
            }
        }

        public static bool IsUsingPort
        {
            get { return _isUsingPort; }
        }

        public static string SaveBetReportDataDirectory
        {
            get { return _saveBetReportDataDirectory; }
        }

        public static string OldSaveBetReportDataDirectory
        {
            get { return _oldSaveBetReportDataDirectory; }
        }

        public static DateTime NewSaveBetReportDate
        {
            get
            {
                return _newSaveBetReportDate;
            }
        }

        public static string DockerFolders
        {
            get
            {
                return _dockerFolders;
            }
        }

        public static DateTime SaveBetDateWithZipFolder
        {
            get
            {
                return _saveBetDateWithZipFolder;
            }
        }

        public static string IP
        {
            get
            {
                var val = ConfigurationManager.AppSettings["ServerFile.IP"];
                return val == null ? string.Empty : Convert.ToString(ConfigurationManager.AppSettings["ServerFile.IP"]);
            }
        }

        public static string UserName
        {
            get
            {
                var val = ConfigurationManager.AppSettings["ServerFile.UserName"];
                return val == null ? string.Empty : Convert.ToString(ConfigurationManager.AppSettings["ServerFile.UserName"]);
            }
        }

        public static string Password
        {
            get
            {
                var val = ConfigurationManager.AppSettings["ServerFile.Password"];
                return val == null ? string.Empty : Convert.ToString(ConfigurationManager.AppSettings["ServerFile.Password"]);
            }
        }

        public static bool IsEncryptPassword
        {
            get
            {
                var val = ConfigurationManager.AppSettings["ServerFile.IsEncryptPassword"];
                return val == null ? false : Convert.ToString(ConfigurationManager.AppSettings["ServerFile.IsEncryptPassword"]) == "true";
            }
        }

        public static bool IsAuthenticate
        {
            get
            {
                var val = ConfigurationManager.AppSettings["ServerFile.IsAuthenticate"];
                return val == null ? false : Convert.ToString(ConfigurationManager.AppSettings["ServerFile.IsAuthenticate"]) == "true";
            }
        }

        public static string SMAReportDataDirectory
        {
            get
            {
                return _smaReportDataDirectory;
            }
        }

        public static string DockerFileApi
        {
            get
            {
                return _dockerFileApi;
            }
        }

        public static DateTime CasinoSingleWalletLaunchDate
        {
            get
            {
                return _casinoSigleWalletLaunchDate;
            }
        }

        private static List<string> GetExcludedControllers()
        {
            List<string> excludedControllers =
                new List<string>(ConfigurationManager
                .AppSettings["PageTracker_ExcludedControllers"]
                .ToString()
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            return excludedControllers;
        }

        private static List<string> GetSpecialIncludedViews()
        {
            List<string> specialIncludedViews =
                new List<string>(ConfigurationManager
                .AppSettings["PageTracker_SpecialIncludedViews"]
                .ToString()
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            return specialIncludedViews;
        }

        private static List<string> GetMultiAgentLicensses()
        {
            List<string> multiAgentLicensses =
                new List<string>(ConfigurationManager
                .AppSettings["MultiAgentLicensses"]
                .ToString()
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            return multiAgentLicensses;
        }
    }
}
