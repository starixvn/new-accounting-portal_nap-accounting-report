namespace Fanex.BetList.Accounting.Services
{
    using System.Collections.Generic;
    using AccountingReports.Core.Web;
    using Common.Extensions;
    using Fanex.BetList.Accounting.Models;
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

                using (ObjectDb objDb = new ObjectDb("Core_ReportService_GetBalanceUpDay"))
                {
                    balanceUp = objDb.QueryEntity<BalanceUp>();
                }

                return balanceUp;
            }
        }

        #endregion StaticProperties

        public static List<IPVNBean> IPVN
        {
            get
            {
                var ipVnList = CacheExtension.Get<List<IPVNBean>>(CacheIPVN);
                if (ipVnList == null)
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

                    CacheExtension.Set(CacheIPVN, ipVnList);
                }

                return ipVnList;
            }
        }
    }
}