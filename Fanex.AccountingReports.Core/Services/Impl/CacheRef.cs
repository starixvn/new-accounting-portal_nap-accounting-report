// --------------------------------------------------------------------------
// <copyright file="CacheRef.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.17">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Resources;
    using Fanex.Common;
    using RemotingRef;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CacheRef
    {
        public static string GetBettype(object bettypeId)
        {
            return RefData.GetBetTypeName(bettypeId.ToString(), LanguageTypeConstants.En);
        }
        public static string GetSportName(object sportId)
        {
            return RefData.GetSportName(Converter.ToString2(sportId), LanguageTypeConstants.En);
        }

        public static string GetTeamName(object teamId)
        {
            return RefData.GetTeamName(Converter.ToString2(teamId), LanguageTypeConstants.En);
        }

        public static string GetLeagueName(object leagueId)
        {
            return RefData.GetLeagueName(Converter.ToString2(leagueId), LanguageTypeConstants.En);
        }

        public static string GetHorseTeamName(object codeID)
        {
            return RefData.GetHorseTeamName(Converter.ToString2(codeID), LanguageTypeConstants.En);
        }

        public static string GetBettypeNameSportingSolution(object betypeId, string resourceId)
        {
            return RefData.GetBetTypeName(Converter.ToString2(betypeId), LanguageTypeConstants.En, string.Empty, resourceId);
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
