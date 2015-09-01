// -----------------------------------------------------------------------
// <copyright file="MatchWinLoss2aUtilityService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MatchWinLoss2aUtilityService
    {
        private Dictionary<object, string> _betTypeNameSuffixCache;

        public MatchWinLoss2aUtilityService()
        {
            _betTypeNameSuffixCache = new Dictionary<object, string>();
        }

        public Dictionary<object, string> BetTypeNameSuffixCache
        {
            get
            {
                return _betTypeNameSuffixCache;
            }
        }

        public Dictionary<object, string> SaveBetTypeCacheRef(
            List<dynamic> betTypeIdList,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var betTypeIdInfo in betTypeIdList)
            {
                if (!currentStorage.ContainsKey(betTypeIdInfo))
                {
                    currentStorage.Add(betTypeIdInfo, this.GetBetTypeName(Converter.ToInt(betTypeIdInfo)));
                }
            }

            return currentStorage;
        }

        public Dictionary<object, string> CachedBetTypeRef(
            List<int> betTypeIdList,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var betTypeIdInfo in betTypeIdList)
            {
                if (!currentStorage.ContainsKey(betTypeIdInfo))
                {
                    currentStorage.Add(betTypeIdInfo, this.GetBetTypeName(Converter.ToInt(betTypeIdInfo)));
                }
            }

            return currentStorage;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public string GetBetTypeName(int betTypeId)
        {
            string betTypeName = string.Empty;
            var nameSuffix = string.Empty;
            switch (betTypeId)
            {
                case RowType.EightRM:
                    betTypeName = Labels.EightRM;
                    break;

                case RowType.BARM:
                    betTypeName = Labels.BARM;
                    break;

                case CoreConstants.BetTypeConstants.CasinoGames:
                    betTypeName = Labels.Casino.ToUpper();
                    break;

                case CoreConstants.BetTypeConstants.KickOff:
                    betTypeName = Labels.Kick_Off;
                    break;

                case CoreConstants.BetTypeConstants.P2PGames:
                    betTypeName = Labels.P2P;
                    break;

                case CoreConstants.BetTypeConstants.SystemParlay:
                    betTypeName = Labels.System_Parlay;
                    break;

                case CoreConstants.BetTypeConstants.OverUnder:
                    betTypeName = CacheRef.GetBettype(betTypeId);
                    nameSuffix = string.Format("({0})", Labels.NumberGame);
                    break;

                case CoreConstants.BetTypeConstants.GoldDeluxeBaccarat:
                case CoreConstants.BetTypeConstants.GoldDeluxeRoulete:
                case CoreConstants.BetTypeConstants.GoldDeluxeParlayBaccarat:
                    betTypeName = CacheRef.GetBettype(betTypeId);
                    nameSuffix = string.Format("({0})", Labels.Gold_Deluxe);
                    break;

                default:
                    betTypeName = CacheRef.GetBettype(betTypeId);
                    break;
            }

            if (this.IsVirtualSport(betTypeId))
            {
                nameSuffix = string.Format("({0})", Labels.Virtual_Sports);
            }

            if (this.IsMicroGamingBetType(betTypeId))
            {
                nameSuffix = string.Format("({0})", Labels.Microgaming);
            }

            if (!this._betTypeNameSuffixCache.ContainsKey(betTypeId))
            {
                this._betTypeNameSuffixCache.Add(betTypeId, nameSuffix);
            }

            if (this.IsBetTrade(betTypeId))
            {
                betTypeName = Labels.Bettrade;
            }

            return betTypeName;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public bool IsExcludeVirtualLineTotal(int betType)
        {
            var result = false;

            switch (betType)
            {
                case CoreConstants.BetTypeConstants._5MinOU:
                case CoreConstants.BetTypeConstants._15MinOU:
                case CoreConstants.BetTypeConstants._30MinOU:
                case CoreConstants.BetTypeConstants._45MinOU:
                case CoreConstants.BetTypeConstants._60MinOU:
                case CoreConstants.BetTypeConstants.EndOfDayOU:

                case CoreConstants.BetTypeConstants._5MinOE:
                case CoreConstants.BetTypeConstants._15MinOE:
                case CoreConstants.BetTypeConstants._30MinOE:
                case CoreConstants.BetTypeConstants._45MinOE:
                case CoreConstants.BetTypeConstants._60MinOE:
                case CoreConstants.BetTypeConstants.EndOfDayOE:

                case CoreConstants.BetTypeConstants.WinToteOdds:
                case CoreConstants.BetTypeConstants.PlateToteOdds:
                case CoreConstants.BetTypeConstants.WinPlaceToteOdds:

                case CoreConstants.BetTypeConstants.WinFixedOdds:
                case CoreConstants.BetTypeConstants.PlaceFixedOdds:
                case CoreConstants.BetTypeConstants.WinPlaceFixedOdds:

                case CoreConstants.BetTypeConstants.CasinoGames:
                case CoreConstants.BetTypeConstants.P2P:
                case CoreConstants.BetTypeConstants.P2PGames:
                case CoreConstants.BetTypeConstants.Baccarat:
                case CoreConstants.BetTypeConstants.Roulette:
                case CoreConstants.BetTypeConstants.VirtualSportHandicap:
                case CoreConstants.BetTypeConstants.VirtualSportOverUnder:
                case CoreConstants.BetTypeConstants.VirtualSportCorrectScore:
                case CoreConstants.BetTypeConstants._1X2VirtualSport:
                case CoreConstants.BetTypeConstants.VirtualSportTotalGoal:
                case CoreConstants.BetTypeConstants.VirtualSportDoubleChance:
                case CoreConstants.BetTypeConstants.Bingo:
                case CoreConstants.BetTypeConstants.LiveBaccarat:
                case CoreConstants.BetTypeConstants.LiveRoulette:
                case CoreConstants.BetTypeConstants.GoldDeluxeBaccarat:
                case CoreConstants.BetTypeConstants.GoldDeluxeParlayBaccarat:
                case CoreConstants.BetTypeConstants.GoldDeluxeRoulete:
                case CoreConstants.BetTypeConstants.GoldDeluxeDeposit:
                case CoreConstants.BetTypeConstants.AGCasinoDeposit:
                case CoreConstants.BetTypeConstants.VegasGameDeposit:
                case RowType.ThirdParty:
                    result = true;
                    break;

                default:
                    result = false;
                    break;
            }

            if (this.IsVirtualSport(betType))
            {
                result = true;
            }

            if (this.IsKenoBetType(betType))
            {
                result = true;
            }

            if (this.IsMetricGamingSuperLiveBetType(betType))
            {
                result = true;
            }
            
            if (this.IsMicroGamingBetType(betType))
            {
                result = true;
            }

            if (this.IsBetTrade(betType))
            {
                result = true;
            }

            return result;
        }

        public bool IsVirtualSport(int betType)
        {
            var result = false;
            if (CoreConstants.BetTypeConstants.MinVirtualSportBetType <= betType &&
                  betType <= CoreConstants.BetTypeConstants.MaxVirtualSportBetType)
            {
                result = true;
            }

            return result;
        }

        public string GetThirdPartyBetTypeName(int betType)
        {
            var result = string.Empty;
            switch (betType)
            {
                case CoreConstants.BetTypeConstants.CasinoGames:
                    result = Labels.Casino;
                    break;

                case CoreConstants.BetTypeConstants.P2P:
                    result = Labels.P2P;
                    break;

                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        public bool IsExcludeVirtualLineTotal(string betTypeName)
        {
            return !string.IsNullOrEmpty(betTypeName) &&
                   (string.Equals(betTypeName, Labels.Casino, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Racing, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.LiveCasino, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Virtual_Sports, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Bingo, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.P2P, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Keno, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Metric_Gaming_Super_Live, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Gold_Deluxe, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Microgaming, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Macau, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.AG_Casino, StringComparison.OrdinalIgnoreCase));
        }

        public decimal CalculateMargin(decimal lineTotal, decimal turnover)
        {
            decimal result = 0;
            if (turnover != 0)
            {
                result = (lineTotal / turnover) * 100;
            }

            return result;
        }

        public bool IsKenoBetType(int betTypeId)
        {
            if (betTypeId < CoreConstants.BetTypeConstants.MinKenoBetType ||
                betTypeId > CoreConstants.BetTypeConstants.MaxKenoBetType)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsMetricGamingSuperLiveBetType(int betTypeId)
        {
            if (betTypeId < CoreConstants.BetTypeConstants.MinMetricGamingSuperLiveBetType ||
                betTypeId > CoreConstants.BetTypeConstants.MaxMetricGamingSuperLiveBetType)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsMicroGamingBetType(int betTypeId)
        {
            if (betTypeId < CoreConstants.BetTypeConstants.MinMicroGamingBetType ||
                betTypeId > CoreConstants.BetTypeConstants.MaxMicroGamingBetType)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsBetTrade(int betType)
        {
            var result = false;
            if (CoreConstants.BetTypeConstants.MinBetTradeBetType <= betType &&
                  betType <= CoreConstants.BetTypeConstants.MaxBetTradeBetType)
            {
                result = true;
            }

            return result;
        }
    }
}