namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.Common;
    using RemotingRef;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CacheRef
    {
        private static RemotingRefService remotingRefService;

        static CacheRef()
        {
            remotingRefService = new RemotingRefService();
        }

        public static string GetBettype(object betTypeId)
        {
            return remotingRefService.GetBetTypeName(betTypeId.ToString(), LanguageTypeConstants.En);
        }

        public static string GetSportName(object sportId)
        {
            if (sportId.ToString() == "-38")
            {
                return "Bet Builder";
            }

            if (sportId.ToString() == "96")
            {
                return "SB Adjustment";
            }

            return remotingRefService.GetSportName(Converter.ToString2(sportId), LanguageTypeConstants.En);
        }

        public static string GetTeamName(object teamId)
        {
            return remotingRefService.GetTeamName(Converter.ToString2(teamId), LanguageTypeConstants.En);
        }

        public static string GetLeagueName(object leagueId)
        {
            return remotingRefService.GetLeagueName(Converter.ToString2(leagueId), LanguageTypeConstants.En);
        }

        public static string GetHorseTeamName(object codeID)
        {
            return remotingRefService.GetHorseTeamName(Converter.ToString2(codeID), LanguageTypeConstants.En);
        }

        public static string GetTeamName3rd(object teamId)
        {
            return remotingRefService.GetTeamName3rd(Converter.ToString2(teamId), LanguageTypeConstants.En);
        }

        public static string GetLeagueName3rd(object leagueId)
        {
            return remotingRefService.GetLeagueName3rd(Converter.ToString2(leagueId), LanguageTypeConstants.En);
        }

        public static string GetResourceData(string refId, string team)
        {
            return remotingRefService.GetResourceDataByRefIdAndResourceId(refId, team, LanguageTypeConstants.En, string.Empty);
        }

        public static Dictionary<object, string> SaveCacheRefResponse(
            List<object> identityList,
            Func<object, string> cacheRefFunction,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var identityInfo in identityList)
            {
                if (!currentStorage.ContainsKey(identityInfo))
                {
                    currentStorage.Add(identityInfo, cacheRefFunction(identityInfo).ToString());
                }
            }

            return currentStorage;
        }
    }
}