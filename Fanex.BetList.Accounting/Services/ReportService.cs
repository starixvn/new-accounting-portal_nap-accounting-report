// --------------------------------------------------------------------------
// <copyright file="ReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Resources;
    using Fanex.Common.Extensions;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ReportService
    {
        public const string CacheIPVN = "Fanex.BetList.Accounting.Services.Ipvn";
        public const string BetTrans14SPSuffix = "_14";
        public const string WarehouseSPSuffix = "_WH";

        #region StaticProperties

        public static BalanceUp BalanceUp
        {
            get
            {
                BalanceUp balanceUp = null;
                try
                {
                    using (ObjectDb objDb = new ObjectDb("Core_ReportService_GetBalanceUpDay"))
                    {
                        balanceUp = objDb.QueryEntity<BalanceUp>();
                    }
                }
                catch (Exception ex)
                {
                    LoggingHandler.Log(ex);
                }

                return balanceUp;
            }
        }

        public static List<IPVNBean> IPVN
        {
            get
            {
                var ipVnList = CacheManager.Get<List<IPVNBean>>(CacheIPVN);
                if (ipVnList == null)
                {
                    try
                    {
                        ipVnList = new List<IPVNBean>();
                        using (ObjectDb objDb = new ObjectDb("BetList_GetIPVN"))
                        {
                            using (var reader = objDb.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ipVnList.Add(new IPVNBean
                                        {
                                            IPFrom = reader.ToDouble("IPFrom"),
                                            IPTo = reader.ToDouble("IPTo")
                                        });
                                }
                            }
                        }

                        CacheManager.Set<List<IPVNBean>>(CacheIPVN, ipVnList);
                    }
                    catch (Exception ex)
                    {
                        LoggingHandler.Log(ex);
                    }
                }

                return ipVnList;
            }
        }

        #endregion StaticProperties

        #region StaticMethods

        /// <summary>
        /// Gets the store mapping suffix.
        /// </summary>
        /// <param name="reportDate">The report date.</param>
        /// <returns>The store mapping suffix.</returns>
        public static string GetStoreMappingSuffix(DateTime reportDate)
        {
            if (reportDate > BalanceUp.MoveBetsDay)
            {
                return string.Empty;
            }
            else if (reportDate >= BalanceUp.BalanceUpDay)
            {
                return BetTrans14SPSuffix;
            }
            else
            {
                return WarehouseSPSuffix;
            }
        }

        /// <summary>
        /// Gets the name of the currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns>The name of the currency.</returns>
        public static string GetCurrencyName(CurrencyContants currency)
        {
            switch (currency)
            {
                case CurrencyContants.EUR:
                    return Labels.EUR;
                case CurrencyContants.MYR:
                    return Labels.RM;
                case CurrencyContants.USD:
                    return Labels.USD;
                default:
                    return string.Empty;
            }
        }

        public static bool IsVirtualSportBetType(int betTypeId)
        {
            return BetTypeContants.MinVirtualSportBetType <= betTypeId &&
                betTypeId <= BetTypeContants.MaxVirtualSportBetType;
        }

        public static bool IsGoldDeluxe(int betTypeId)
        {
            return betTypeId == BetTypeContants.GoldDeluxeBaccarat || 
                betTypeId == BetTypeContants.GoldDeluxeParlayBaccarat || 
                betTypeId == BetTypeContants.GoldDeluxeRoulette;
        }

        #endregion StaticMethods
    }
}