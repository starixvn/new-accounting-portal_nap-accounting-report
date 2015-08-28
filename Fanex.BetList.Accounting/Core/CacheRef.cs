// --------------------------------------------------------------------------
// <copyright file="CacheRef.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Core
{
    using System;
    using System.Collections.Generic;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Resources;
    using Fanex.Common;
    using RemotingRef;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CacheRef
    {
        public static string GetBettype(object bettypeId)
        {
            return GetBetType(bettypeId, null);
        }

        /// <summary>
        /// Gets the type of the bet.
        /// </summary>
        /// <param name="betTypeId">The bet type identifier.</param>
        /// <param name="betId">The bet identifier.</param>
        /// <returns>The type of the bet.</returns>
        public static string GetBetType(object betTypeId, object betId)
        {
            // resourceId remove first character
            string resourceId = string.Empty;
            if (betId != null && betId.ToString().Length > 1)
            {
                resourceId = betId.ToString().Substring(1);
            }

            return RefData.GetBetTypeName(
                betTypeId.ToString(),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty,
                resourceId);
        }

        public static string GetSportName(object sportId)
        {
            return RefData.GetSportName(
                Converter.ToString2(sportId),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty);
        }

        public static string GetTeamName(object teamId)
        {
            return RefData.GetTeamName(
                Converter.ToString2(teamId),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty);
        }

        public static string GetLeagueName(object leagueId)
        {
            return RefData.GetLeagueName(
                Converter.ToString2(leagueId),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty);
        }

        public static string GetFncIndexName(object indexId)
        {
            return RefData.GetFncIndexName(
                Converter.ToString2(indexId),
                GetSimpleLanguage(LanguageTypeConstants.EnUS));
        }

        public static string GetHorseTeamName(object codeID)
        {
            return RefData.GetHorseTeamName(
                Converter.ToString2(codeID),
                GetSimpleLanguage(LanguageTypeConstants.EnUS));
        }

        public static string GetOddsTypeName(object oddsType)
        {
            return RefData.GetOddsTypeName(
                Converter.ToString2(oddsType),
                GetSimpleLanguage(LanguageTypeConstants.EnUS));
        }

        /// <summary>
        /// Gets the name of the combination.
        /// </summary>
        /// <param name="betTeam">The bet team.</param>
        /// <returns>The name of the combination.</returns>
        public static string GetCombinationName(string betTeam)
        {
            string name = string.Empty;
            switch (betTeam)
            {
                case CombinationTypeConstants.Trixie:
                    name = Labels.Trixie;
                    break;
                case CombinationTypeConstants.Yankee:
                    name = Labels.Yankee;
                    break;
                case CombinationTypeConstants.Canadian:
                    name = Labels.Canadian;
                    break;
                case CombinationTypeConstants.Heinz:
                    name = Labels.Heinz;
                    break;
                case CombinationTypeConstants.SuperHeinz:
                    name = Labels.Super_Heinz;
                    break;
                case CombinationTypeConstants.Goliath:
                    name = Labels.Goliath;
                    break;
            }

            return name;
        }

        public static string GetRouletteBetItem(int itemId)
        {
            return RefData.GetRouletteBetItem(
                Converter.ToString2(itemId),
                GetSimpleLanguage(LanguageTypeConstants.EnUS));
        }

        public static string GetBettypeNameSportingSolution(object betypeId, string resourceId)
        {
            string bettypeName = string.Empty;
            bettypeName = RefData.GetBetTypeName(
                Converter.ToString2(betypeId),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty,
                resourceId);
            return bettypeName;
        }

        public static string GetCasinoBetItem(object betStatusId)
        {
            return RefData.GetCasinoBetItem(
                betStatusId.ToString(),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty);
        }

        public static string GetRouletteBetItem(object itemId)
        {
            return RefData.GetRouletteBetItem(
                itemId.ToString(),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty);
        }

        /// <summary>
        /// Saves the cache reference response.
        /// </summary>
        /// <param name="identityList">The identifier list.</param>
        /// <param name="cacheRefFunction">The cache reference function.</param>
        /// <param name="currentStorage">The current storage.</param>
        /// <returns>The cache reference response.</returns>
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

        /// <summary>
        /// Gets the simple language.
        /// </summary>
        /// <param name="lang">The language.</param>
        /// <returns>The simple language.</returns>
        private static string GetSimpleLanguage(string lang)
        {
            switch (lang)
            {
                case LanguageTypeConstants.EnUS:
                    return LanguageTypeConstants.En;
                case LanguageTypeConstants.ZhTW:
                    return LanguageTypeConstants.Ch; // Traditional
                case LanguageTypeConstants.ZhCn:
                    return LanguageTypeConstants.Cs; // Simplified
                case LanguageTypeConstants.JaJp:
                    return LanguageTypeConstants.Jp;
                case LanguageTypeConstants.ThTh:
                    return LanguageTypeConstants.Th; // Thai
                default:
                    return LanguageTypeConstants.En;
            }
        }
    }
}
