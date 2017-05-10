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
    using Common.Extensions;
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Models;
    using Fanex.Data;
    using System;
    using System.Collections.Generic;

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

        #endregion StaticProperties

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
                        throw ex;
                    }
                }

                return ipVnList;
            }
        }
    }
}