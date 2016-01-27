// -----------------------------------------------------------------------
// <copyright file="TreeWinLossService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Services.Implement.TreeWinLoss
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml.Linq;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Resources;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.TreeWinLoss;
    using Fanex.AccountingReports.WinLoss.Repositories.Implement;
    using Fanex.AccountingReports.WinLoss.Repositories.Interface;
    using Fanex.AccountingReports.WinLoss.Services.Interface.TreeWinLoss;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// Create service for tree win loss.
    /// </summary>
    public class TreeWinLossService : ServiceBase, ITreeWinLossService
    {
        private const string TreeWinLossProductPath = @"Web.Configuration\TreeWinLossProduct.xml";
        private ITreeWinLossRepository _treeWinLossRepository;
        private Dictionary<int, int> _bettype = new Dictionary<int, int>();

        public TreeWinLossService()
            : this(null)
        {
        }

        public TreeWinLossService(ITreeWinLossRepository treeWinLossRepository)
        {
            this._treeWinLossRepository = treeWinLossRepository ?? new TreeWinLossRepository();
            InitBettype();
        }

        public List<TreeWinLossProduct> GetProduct()
        {
            var product = new List<TreeWinLossProduct>();
            var root = AppDomain.CurrentDomain.BaseDirectory;
            var productFilePath = root + TreeWinLossProductPath;
            XDocument productDocument = XDocument.Load(productFilePath);

            foreach (var profileElement in productDocument.Descendants("Product").ToList())
            {
                product.Add(new TreeWinLossProduct()
                {
                    ProductId = Converter.ToInt(profileElement.Attribute("ProductID").Value),
                    ProductName = Converter.ToString2(profileElement.Attribute("ProductName").Value)
                });
            }

            return product;
        }

        /// <summary>
        /// Get Tree Win Loss By Product.
        /// </summary>
        /// <param name="criteria">Tree Win Loss By Product Criteria.</param>
        /// <returns>The Tree Win Loss.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public TreeWinLoss GetTreeWinLossByProduct(GetTreeWinLossByProductCriteria criteria)
        {
            try
            {
                var currencies = ReportService.ExchangeCurrencies;
                var isShowNotAvailableTurnover = IsShowAvailableTurnoverByProduct(criteria);
                criteria.Products = GetProductParams(criteria.ProductSelection);
                var treeWinLoss = new TreeWinLoss
                {
                    ChildsInfo = new List<TreeWinLossInfo>(),
                    TotalInfo = new List<TreeWinLossInfo>(),
                    IsShowNotAvailableTurnover = isShowNotAvailableTurnover
                };

                DateTime moveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                if (criteria.FromDate <= moveBetsDay && criteria.ToDate > moveBetsDay)
                {
                    DateTime fromDate = criteria.FromDate;
                    var accountWinLoss = new TreeWinLoss();
                    var wareHouseWinLoss = new TreeWinLoss();
                    criteria.FromDate = moveBetsDay.AddDays(1);
                    criteria.ToDate = criteria.ToDate;
                    accountWinLoss = GetTreeWinLossDateRangeByProduct(criteria);

                    criteria.FromDate = fromDate;
                    criteria.ToDate = moveBetsDay;
                    wareHouseWinLoss = GetTreeWinLossDateRangeByProduct(criteria);
                    var treeWinLossTmp = new List<TreeWinLossInfo>();
                    treeWinLossTmp = accountWinLoss.ChildsInfo.Concat(wareHouseWinLoss.ChildsInfo).ToList();
                    treeWinLoss.ChildsInfo = treeWinLossTmp.GroupBy(x => x.Id)
                        .Select(y => new TreeWinLossInfo
                        {
                            Id = y.Key,
                            Name = currencies[Converter.ToInt(y.Key)],
                            Turnover = y.Sum(x => x.Turnover),
                            WinLoss = y.Sum(x => x.WinLoss),
                            Comm = y.Sum(x => x.Comm),
                            WinLossTotal = y.Sum(x => x.WinLossTotal),
                            BaseRMTurnover = y.Sum(x => x.BaseRMTurnover),
                            BaseRMTotal = y.Sum(x => x.BaseRMTotal),
                            BaseRMVirtualTotal = y.Sum(x => x.BaseRMVirtualTotal),
                            BaseRMVirtualTurnover = y.Sum(x => x.BaseRMVirtualTurnover),
                            BaseRMTOVarianceMinus = y.Sum(x => x.BaseRMTOVarianceMinus),
                            BaseRMTOVariancePlus = y.Sum(x => x.BaseRMTOVariancePlus),
                            BaseRMWLVarianceMinus = y.Sum(x => x.BaseRMWLVarianceMinus),
                            BaseRMWLVariancePlus = y.Sum(x => x.BaseRMWLVariancePlus),
                        }).ToList();

                    for (int j = 0; j < accountWinLoss.TotalInfo.Count; j++)
                    {
                        var totalAccountWinLoss = accountWinLoss.TotalInfo[j];
                        var totalWarehouseWinLoss = wareHouseWinLoss.TotalInfo[j];
                        var totalWinLoss = new TreeWinLossInfo()
                        {
                            Name = totalAccountWinLoss.Name,
                            BaseRMTurnover = totalAccountWinLoss.BaseRMTurnover + totalWarehouseWinLoss.BaseRMTurnover,
                            BaseRMVirtualTurnover = totalAccountWinLoss.BaseRMVirtualTurnover + totalWarehouseWinLoss.BaseRMVirtualTurnover,
                            BaseRMTOVarianceMinus = totalAccountWinLoss.BaseRMTOVarianceMinus + totalWarehouseWinLoss.BaseRMTOVarianceMinus,
                            BaseRMTOVariancePlus = totalAccountWinLoss.BaseRMTOVariancePlus + totalWarehouseWinLoss.BaseRMTOVariancePlus,
                            BaseRMWLVarianceMinus = totalAccountWinLoss.BaseRMWLVarianceMinus + totalWarehouseWinLoss.BaseRMWLVarianceMinus,
                            BaseRMWLVariancePlus = totalAccountWinLoss.BaseRMWLVariancePlus + totalWarehouseWinLoss.BaseRMWLVariancePlus,
                            BaseRMTotal = totalAccountWinLoss.BaseRMTotal + totalWarehouseWinLoss.BaseRMTotal,
                            BaseRMVirtualTotal = totalAccountWinLoss.BaseRMVirtualTotal + totalWarehouseWinLoss.BaseRMVirtualTotal
                        };
                        treeWinLoss.TotalInfo.Add(totalWinLoss);
                    }
                }
                else
                {
                    treeWinLoss = GetTreeWinLossDateRangeByProduct(criteria);
                }

                SessionManager.Set<TreeWinLoss>(TreeWinLossUtil.TreeWinLossByProductSession, treeWinLoss);
                return treeWinLoss;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<TreeWinLoss>(TreeWinLossUtil.TreeWinLossByProductSession, null);
                throw;
            }
        }

        /// <summary>
        /// Get Data Tree Win Loss by level.
        /// </summary>
        /// <param name="criteria">The Tree win loss by level.</param>
        /// <returns>The tree win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public TreeWinLossVirtualRate GetTreeWinLossByLevel(GetTreeWinLossByLevelCriteria criteria)
        {
            try
            {
                bool isShowNotAvailableTurnover = IsShowAvailableTurnoverByLevel(criteria);
                criteria.Products = GetProductParams(criteria.ProductSelection);
                TreeWinLossVirtualRate treeWinLossVirtualRate = new TreeWinLossVirtualRate();
                var winLossByComm = new TreeWinLoss
                {
                    ChildsInfo = new List<TreeWinLossInfo>(),
                    TotalInfo = new List<TreeWinLossInfo>(),
                    IsShowNotAvailableTurnover = isShowNotAvailableTurnover
                };
                DateTime moveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                if (criteria.FromDate <= moveBetsDay && criteria.ToDate > moveBetsDay)
                {
                    DateTime fromDate = criteria.FromDate;
                    criteria.FromDate = moveBetsDay.AddDays(1);
                    var accountWinLoss = GetTreeWinLossDateRangeByLevel(criteria);

                    criteria.FromDate = fromDate;
                    criteria.ToDate = moveBetsDay;
                    var wareHouseWinLoss = GetTreeWinLossDateRangeByLevel(criteria);

                    var winLossByCommTmp = new List<TreeWinLossInfo>();
                    winLossByCommTmp = accountWinLoss.TreeWinLoss.ChildsInfo.Concat(wareHouseWinLoss.TreeWinLoss.ChildsInfo).ToList();
                    winLossByComm.ChildsInfo = winLossByCommTmp.GroupBy(x => new { x.Id, x.Name, x.VirtualRate })
                        .Select(y => new TreeWinLossInfo
                        {
                            Id = y.Key.Id,
                            Name = y.Key.Name,
                            Turnover = y.Sum(x => x.Turnover),
                            WinLoss = y.Sum(x => x.WinLoss),
                            Comm = y.Sum(x => x.Comm),
                            WinLossTotal = y.Sum(x => x.WinLossTotal),
                            BaseRMTurnover = y.Sum(x => x.BaseRMTurnover),
                            BaseRMTotal = y.Sum(x => x.BaseRMTotal),
                            BaseRMVirtualTotal = y.Sum(x => x.BaseRMVirtualTotal),
                            BaseRMVirtualTurnover = y.Sum(x => x.BaseRMVirtualTurnover),
                            BaseRMTOVarianceMinus = y.Sum(x => x.BaseRMTOVarianceMinus),
                            BaseRMTOVariancePlus = y.Sum(x => x.BaseRMTOVariancePlus),
                            BaseRMWLVarianceMinus = y.Sum(x => x.BaseRMWLVarianceMinus),
                            BaseRMWLVariancePlus = y.Sum(x => x.BaseRMWLVariancePlus),
                            VirtualRate = y.Key.VirtualRate
                        }).ToList();

                    for (int j = 0; j < accountWinLoss.TreeWinLoss.TotalInfo.Count; j++)
                    {
                        var totalAccountWinLoss = accountWinLoss.TreeWinLoss.TotalInfo[j];
                        var totalWarehouseWinLoss = wareHouseWinLoss.TreeWinLoss.TotalInfo[j];
                        var totalWinLoss = new TreeWinLossInfo()
                        {
                            Name = totalAccountWinLoss.Name,
                            BaseRMTurnover = totalAccountWinLoss.BaseRMTurnover + totalWarehouseWinLoss.BaseRMTurnover,
                            BaseRMVirtualTurnover = totalAccountWinLoss.BaseRMVirtualTurnover + totalWarehouseWinLoss.BaseRMVirtualTurnover,
                            BaseRMTOVarianceMinus = totalAccountWinLoss.BaseRMTOVarianceMinus + totalWarehouseWinLoss.BaseRMTOVarianceMinus,
                            BaseRMTOVariancePlus = totalAccountWinLoss.BaseRMTOVariancePlus + totalWarehouseWinLoss.BaseRMTOVariancePlus,
                            BaseRMWLVarianceMinus = totalAccountWinLoss.BaseRMWLVarianceMinus + totalWarehouseWinLoss.BaseRMWLVarianceMinus,
                            BaseRMWLVariancePlus = totalAccountWinLoss.BaseRMWLVariancePlus + totalWarehouseWinLoss.BaseRMWLVariancePlus,
                            BaseRMTotal = totalAccountWinLoss.BaseRMTotal + totalWarehouseWinLoss.BaseRMTotal,
                            BaseRMVirtualTotal = totalAccountWinLoss.BaseRMVirtualTotal + totalWarehouseWinLoss.BaseRMVirtualTotal
                        };
                        winLossByComm.TotalInfo.Add(totalWinLoss);
                    }

                    winLossByComm.ChildsInfo = winLossByComm.ChildsInfo.OrderBy(c => c.Name).ToList();
                    treeWinLossVirtualRate.TreeWinLoss = winLossByComm;
                    treeWinLossVirtualRate.SMAVirtualRate.ReportRows = this.GetEnabledVirtualRateUserFromIBCAndWH(
                        accountWinLoss.SMAVirtualRate.ReportRows,
                        wareHouseWinLoss.SMAVirtualRate.ReportRows);
                }
                else
                {
                    treeWinLossVirtualRate = GetTreeWinLossDateRangeByLevel(criteria);
                    treeWinLossVirtualRate.TreeWinLoss.ChildsInfo.OrderBy(c => c.Name).ToList();
                }

                treeWinLossVirtualRate.NumberUserEnabledVirtualRate = treeWinLossVirtualRate.SMAVirtualRate.ReportRows.Count;
                treeWinLossVirtualRate.EnabledVirtualRateUser = MapEnabledVirtualRateUserData(treeWinLossVirtualRate.TreeWinLoss.ChildsInfo);
                treeWinLossVirtualRate.TreeWinLoss.IsShowNotAvailableTurnover = isShowNotAvailableTurnover;

                switch (criteria.RoleId)
                {
                    case 5:
                        SessionManager.Set<TreeWinLossVirtualRate>(TreeWinLossUtil.TreeWinLossByCommSession, treeWinLossVirtualRate);
                        break;

                    case 4:
                        SessionManager.Set<TreeWinLossVirtualRate>(TreeWinLossUtil.TreeWinLossBySuperSession, treeWinLossVirtualRate);
                        break;

                    case 3:
                        SessionManager.Set<TreeWinLossVirtualRate>(TreeWinLossUtil.TreeWinLossByMasterSession, treeWinLossVirtualRate);
                        break;

                    case 2:
                        SessionManager.Set<TreeWinLossVirtualRate>(TreeWinLossUtil.TreeWinLossByAgentSession, treeWinLossVirtualRate);
                        break;
                }

                return treeWinLossVirtualRate;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets the virtual rate.
        /// </summary>
        /// <param name="custId">The customer identifier.</param>
        /// <returns>Virtual rate information.</returns>
        public VirtualRateInfo GetVirtualRate(int custId)
        {
            VirtualRateInfo virtualRateInfo = new VirtualRateInfo();
            VirtualRateInfo virtualRateItem = new VirtualRateInfo
            {
                Date = "Current"
            };
            GetVirtualRateCriteria getVirtualRateCriteria = new GetVirtualRateCriteria
            {
                CustId = custId
            };
            GetVirtualRateHistoryCriteria criteria = new GetVirtualRateHistoryCriteria
            {
                CustId = custId
            };

            var virtualRateBySportAndLiveStatus = GetVirtualRateBySportAndLiveStatus(getVirtualRateCriteria);
            virtualRateItem.ReportRows = virtualRateBySportAndLiveStatus.ReportRows;
            virtualRateItem.CalculateNumberMergeRows();
            virtualRateInfo.ReportRows.Add(virtualRateItem);

            var virtualRateHistoryLog = GetVirtualRateHistory(criteria);
            foreach (var historyLogItem in virtualRateHistoryLog.ReportRows)
            {
                historyLogItem.CalculateNumberMergeRows();
                virtualRateInfo.ReportRows.Add(historyLogItem);
            }

            return virtualRateInfo;
        }

        /// <summary>
        /// Get Tree Win Loss By Product for specified time.
        /// </summary>
        /// <param name="criteria">The tree win loss by product criteria.</param>
        /// <returns>The tree win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private TreeWinLoss GetTreeWinLossDateRangeByProduct(GetTreeWinLossByProductCriteria criteria)
        {
            var isShowNotAvailableTurnover = IsShowAvailableTurnoverByProduct(criteria);
            var treeWinLoss = new TreeWinLoss
            {
                ChildsInfo = new List<TreeWinLossInfo>(),
                TotalInfo = new List<TreeWinLossInfo>(),
                IsShowNotAvailableTurnover = isShowNotAvailableTurnover
            };
            try
            {
                var currencies = ReportService.ExchangeCurrencies;
                var outputParam = new Hashtable();
                var rawData = this._treeWinLossRepository.GetTreeWinLossByProduct(criteria, ref outputParam);
                if (rawData != null)
                {
                    var totalWinLoss = new TreeWinLossInfo
                    {
                        Name = string.Empty
                    };
                    var grandTotalWinLoss = new TreeWinLossInfo
                    {
                        Name = Labels.Total
                    };
                    var total8RMWinLoss = new TreeWinLossInfo
                    {
                        Name = Labels._8RM
                    };
                    var totalBARMWinLoss = new TreeWinLossInfo
                    {
                        Name = Labels.BARM
                    };
                    foreach (TreeWinLossInfo row in rawData)
                    {
                        var winLoss = new TreeWinLossInfo()
                        {
                            Id = row.Id,
                            Name = currencies[row.Id],
                            Turnover = row.Turnover,
                            WinLoss = row.WinLoss,
                            Comm = row.Comm,
                            WinLossTotal = row.WinLossTotal,
                            BaseRMTurnover = row.BaseRMTurnover,
                            BaseRMVirtualTurnover = row.BaseRMVirtualTurnover,
                            BaseRMTOVariancePlus = row.BaseRMTOVariancePlus,
                            BaseRMTOVarianceMinus = row.BaseRMTOVarianceMinus,
                            BaseRMWLVariancePlus = row.BaseRMWLVariancePlus,
                            BaseRMWLVarianceMinus = row.BaseRMWLVarianceMinus,
                            BaseRMTotal = row.BaseRMTotal,
                            BaseRMVirtualTotal = row.BaseRMVirtualTotal
                        };

                        winLoss = TreeWinLossUtil.RoundTreeWinLossInfo(winLoss);
                        treeWinLoss.ChildsInfo.Add(winLoss);
                        TreeWinLossUtil.AddToTotal(winLoss, ref totalWinLoss);
                        TreeWinLossUtil.AddToTotal(winLoss, ref grandTotalWinLoss);
                    }

                    treeWinLoss.TotalInfo.Add(totalWinLoss);
                    total8RMWinLoss.BaseRMTurnover = Converter.ToDecimal(outputParam["turnover_8rm"]);
                    total8RMWinLoss.BaseRMVirtualTurnover = Converter.ToDecimal(outputParam["turnover_8rm"]);
                    total8RMWinLoss.BaseRMTotal = Converter.ToDecimal(outputParam["total_8rm"]);
                    total8RMWinLoss.BaseRMVirtualTotal = Converter.ToDecimal(outputParam["virtualtotal_8rm"]);
                    TreeWinLossUtil.AddToTotal(total8RMWinLoss, ref grandTotalWinLoss);
                    treeWinLoss.TotalInfo.Add(total8RMWinLoss);

                    totalBARMWinLoss.BaseRMTurnover = Converter.ToDecimal(outputParam["turnover_barm"]);
                    totalBARMWinLoss.BaseRMVirtualTurnover = totalBARMWinLoss.BaseRMTurnover;
                    totalBARMWinLoss.BaseRMTotal = Converter.ToDecimal(outputParam["total_barm"]);
                    totalBARMWinLoss.BaseRMVirtualTotal = Converter.ToDecimal(outputParam["virtualtotal_barm"]);
                    TreeWinLossUtil.AddToTotal(totalBARMWinLoss, ref grandTotalWinLoss);
                    treeWinLoss.TotalInfo.Add(totalBARMWinLoss);
                    treeWinLoss.TotalInfo.Add(grandTotalWinLoss);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return treeWinLoss;
        }

        /// <summary>
        /// Get tree win loss by level for specified time.
        /// </summary>
        /// <param name="criteria">The tree win loss by level criteria.</param>
        /// <returns>The tree win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private TreeWinLossVirtualRate GetTreeWinLossDateRangeByLevel(GetTreeWinLossByLevelCriteria criteria)
        {
            switch (criteria.RoleId)
            {
                case 5:
                    return GetTreeWinlossData(
                        new GetTreeWinLossByComCriteria
                        {
                            Currency = criteria.Currency,
                            FromDate = criteria.FromDate,
                            ToDate = criteria.ToDate,
                            ProductSelection = criteria.ProductSelection,
                            Products = criteria.Products
                        },
                    criteria.RoleId);
                case 4:
                    return GetTreeWinlossData(
                        new GetTreeWinLossBySuperCriteria
                        {
                            SRecommend = criteria.SRecommend,
                            FromDate = criteria.FromDate,
                            ToDate = criteria.ToDate,
                            ProductSelection = criteria.ProductSelection,
                            Products = criteria.Products
                        },
                    criteria.RoleId);
                case 3:
                    return GetTreeWinlossData(
                        new GetTreeWinLossByMasterCriteria
                        {
                            MRecommend = criteria.MRecommend,
                            FromDate = criteria.FromDate,
                            ToDate = criteria.ToDate,
                            ProductSelection = criteria.ProductSelection,
                            Products = criteria.Products
                        },
                    criteria.RoleId);
                case 2:
                    return GetTreeWinlossData(
                        new GetTreeWinLossByAgentCriteria
                        {
                            Recommend = criteria.Recommend,
                            FromDate = criteria.FromDate,
                            ToDate = criteria.ToDate,
                            ProductSelection = criteria.ProductSelection,
                            Products = criteria.Products
                        },
                    criteria.RoleId);
            }

            return new TreeWinLossVirtualRate();
        }

        private TreeWinLossVirtualRate GetTreeWinlossData(CriteriaBase criteria, int roleId)
        {
            TreeWinLossVirtualRate treeWinLossVirtualRate = new TreeWinLossVirtualRate();
            var treeWinLoss = new TreeWinLoss
            {
                ChildsInfo = new List<TreeWinLossInfo>(),
                TotalInfo = new List<TreeWinLossInfo>()
            };

            SMAVirtualRate virtualRate = null;

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                IDataReader reader = objectDb.ExecuteReader(criteria.GetSpParams());
                var totalWinLoss = new TreeWinLossInfo
                {
                    Name = Labels.Total
                };

                while (reader.Read())
                {
                    var winLoss = MapTreeWinLossData(reader, roleId);
                    winLoss = TreeWinLossUtil.RoundTreeWinLossInfo(winLoss);
                    treeWinLoss.ChildsInfo.Add(winLoss);
                    TreeWinLossUtil.AddToTotal(winLoss, ref totalWinLoss);
                }

                treeWinLoss.TotalInfo.Add(totalWinLoss);
                treeWinLossVirtualRate.TreeWinLoss = treeWinLoss;

                if (reader.NextResult())
                {
                    virtualRate = MapSMAVirtualRateData(reader);
                }

                treeWinLossVirtualRate.SMAVirtualRate = virtualRate;
                reader.Close();
            }

            return treeWinLossVirtualRate;
        }

        /// <summary>
        /// Gets the virtual rate by sport and live status.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Virtual rate by sport and live status.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private VirtualRateInfo GetVirtualRateBySportAndLiveStatus(GetVirtualRateCriteria criteria)
        {
            VirtualRateInfo virtualRateBySportAndLiveStatus = new VirtualRateInfo();
            bool enableVirtualRate = false;
            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                IDataReader reader = objectDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    enableVirtualRate = Converter.ToDecimal(reader["EnableVirtualrate"]) == 1;
                }

                if (enableVirtualRate)
                {
                    if (reader.NextResult())
                    {
                        VirtualRateInfo virtualRateItem = null;
                        string preLiveAndSportType = string.Empty;
                        virtualRateBySportAndLiveStatus.EnableVirtualRate = true;
                        while (reader.Read())
                        {
                            int sportId = Converter.ToInt(reader["sporttype"]);
                            bool isLive = Converter.ToBool(reader["isLive"]);
                            int bettypeId = Converter.ToInt(reader["bettype"]);
                            decimal virtualRate = Converter.ToDecimal(reader["virtualMultiplier"]);
                            string liveAndSportType = string.Join(
                                string.Empty,
                                Converter.ToString2(reader["sporttype"]),
                                Converter.ToString2(reader["isLive"]));

                            if (liveAndSportType != preLiveAndSportType)
                            {
                                if (virtualRateItem != null)
                                {
                                    virtualRateBySportAndLiveStatus.ReportRows.Add(virtualRateItem);
                                }

                                string liveStatusTitle = isLive ? Labels.Live : Labels.Non_Live;
                                virtualRateItem = new VirtualRateInfo
                                {
                                    Sport = GetSportName(sportId),
                                    LiveStatus = liveStatusTitle
                                };
                                preLiveAndSportType = liveAndSportType;
                            }

                            virtualRateItem.VirtualRateByBettype.Add(new VirtualRateByBettypeInfo
                            {
                                BettypeName = CacheRef.GetBettype(bettypeId),
                                VirtualRate = virtualRate - 1
                            });
                        }

                        if (virtualRateItem != null)
                        {
                            virtualRateBySportAndLiveStatus.ReportRows.Add(virtualRateItem);
                        }
                    }
                }
                else
                {
                    virtualRateBySportAndLiveStatus.ReportRows.Add(new VirtualRateInfo
                    {
                        Sport = "Virtual Rate: Disabled",
                        EnableVirtualRate = false
                    });
                }

                reader.Close();
            }

            return virtualRateBySportAndLiveStatus;
        }

        /// <summary>
        /// Gets the virtual rate history.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Virtual rate history.</returns>
        private VirtualRateInfo GetVirtualRateHistory(GetVirtualRateHistoryCriteria criteria)
        {
            VirtualRateInfo virtualRateInfo = new VirtualRateInfo();

            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                IDataReader reader = objectDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    int actionId = Converter.ToInt(reader["ActionID"]);
                    string trackingValue = Converter.ToString2(reader["TrackingValue"]);
                    XElement xmlTrackingValue = XElement.Parse(trackingValue);
                    var virturalRateHistoryItem = GetVirtualRateLogHistory(actionId, xmlTrackingValue);
                    if (virturalRateHistoryItem.ReportRows.Count != 0)
                    {
                        virturalRateHistoryItem.ChangedBy = Converter.ToString2(reader["Username"]);
                        virturalRateHistoryItem.Date = Converter.ToDateTime(reader["logdate"]).ToLongFormatedDateWithSlash();
                        virtualRateInfo.ReportRows.Add(virturalRateHistoryItem);
                    }
                }

                reader.Close();
            }

            return virtualRateInfo;
        }

        /// <summary>
        /// Initializes the bet type data.
        /// </summary>
        private void InitBettype()
        {
            this._bettype.Add(0, BetTypeConstants.Handicap);
            this._bettype.Add(1, BetTypeConstants.OverUnder2);
            this._bettype.Add(2, BetTypeConstants.OddEvenBetType);
            this._bettype.Add(3, BetTypeConstants._1stHandicap);
            this._bettype.Add(4, BetTypeConstants._1stOverUnder);
            this._bettype.Add(5, BetTypeConstants._1stOddEvenBetType);
            this._bettype.Add(6, BetTypeConstants.MoneyLineBettype);
            this._bettype.Add(7, BetTypeConstants._1stMoneyLineBettype);
            this._bettype.Add(8, BetTypeConstants.SetXGameYWinner);
        }

        private string GetSportName(int sportId)
        {
            string sportName = sportId == 99 ? "Other Sports" : CacheRef.GetSportName(sportId);
            return sportName;
        }

        /// <summary>
        /// Initializes the virtual rate by bet type information.
        /// </summary>
        /// <returns>The virtual rate by bet type.</returns>
        private List<VirtualRateByBettypeInfo> InitVirtualRateByBettypeInfo()
        {
            var virtualRateByBettype = new List<VirtualRateByBettypeInfo>(this._bettype.Count);
            for (int i = 0; i < _bettype.Count; i++)
            {
                var virtualRateByBettypeItem = new VirtualRateByBettypeInfo
                {
                    BettypeName = CacheRef.GetBettype(_bettype[i])
                };

                virtualRateByBettype.Add(virtualRateByBettypeItem);
            }

            return virtualRateByBettype;
        }

        /// <summary>
        /// Gets the virtual rate log history.
        /// </summary>
        /// <param name="actionId">The action identifier.</param>
        /// <param name="xmlTrackingValue">The XML tracking value.</param>
        /// <returns>The virtual rate log history.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private VirtualRateInfo GetVirtualRateLogHistory(int actionId, XElement xmlTrackingValue)
        {
            string preLiveAndSportType = string.Empty;
            VirtualRateInfo virtualRateHistory = new VirtualRateInfo();
            VirtualRateInfo virtualRateUpdatedItem = null;
            if (actionId == 806 || actionId == 2802)
            {
                bool enabledVirtualRate = Converter.ToInt(xmlTrackingValue.Attribute("EnableVirtualrate").Value) == 1;
                virtualRateUpdatedItem = new VirtualRateInfo
                {
                    Sport = string.Format("Virtual Rate: {0}", enabledVirtualRate ? "Enabled" : "Disabled")
                };
            }
            else
            {
                int userId = Converter.ToInt(xmlTrackingValue.Attribute("UserID").Value);
                foreach (XElement element in xmlTrackingValue.Nodes())
                {
                    int sportId = Converter.ToInt(element.Attribute("SportType").Value);
                    int bettypeId = Converter.ToInt(element.Attribute("BetType").Value);
                    bool isLive = Converter.ToInt(element.Attribute("IsLive").Value) == 1;
                    decimal virtualRate = Converter.ToDecimal(element.Attribute("VirtualMultiplier").Value);
                    string liveAndSportType = string.Join(string.Empty, sportId, isLive);
                    int bettypeIndex = this._bettype.FirstOrDefault(x => x.Value == bettypeId).Key;

                    if (liveAndSportType != preLiveAndSportType)
                    {
                        if (virtualRateUpdatedItem != null)
                        {
                            virtualRateHistory.ReportRows.Add(virtualRateUpdatedItem);
                        }

                        string liveStatusTitle = isLive ? Labels.Live : Labels.Non_Live;
                        virtualRateUpdatedItem = new VirtualRateInfo
                        {
                            Sport = GetSportName(sportId),
                            LiveStatus = liveStatusTitle
                        };
                        preLiveAndSportType = liveAndSportType;
                    }

                    virtualRateUpdatedItem.VirtualRateByBettype.Add(new VirtualRateByBettypeInfo
                    {
                        BettypeName = CacheRef.GetBettype(bettypeId),
                        VirtualRate = virtualRate - 1
                    });
                }
            }

            if (virtualRateUpdatedItem != null)
            {
                virtualRateHistory.ReportRows.Add(virtualRateUpdatedItem);
            }

            return virtualRateHistory;
        }

        /// <summary>
        /// Maps the SMA virtual rate data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>SMA virtual rate.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2101:MethodMustNotContainMoreLinesThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private SMAVirtualRate MapSMAVirtualRateData(IDataReader reader)
        {
            SMAVirtualRate report = new SMAVirtualRate();
            string preUserName = string.Empty;
            string preSportAndLiveStatus = string.Empty;
            SMAVirtualRate virtualRateByUser = null;
            SMAVirtualRate virtualRateBySportAndLiveStatus = null;
            while (reader.Read())
            {
                string userName = Converter.ToString2(reader["username"]);
                int sportId = Converter.ToInt(reader["sporttype"]);
                int bettypeId = Converter.ToInt(reader["bettype"]);
                bool isLive = Converter.ToBool(reader["islive"]);
                decimal virtualRate = Converter.ToDecimal(reader["virtualMultiplier"]);
                string sportAndLiveStatus = string.Join(string.Empty, sportId, isLive);
                string liveStatusTitle = isLive ? "Live" : "None-Live";
                int bettypeIndex = _bettype.FirstOrDefault(x => x.Value == bettypeId).Key;

                if (userName != preUserName)
                {
                    if (virtualRateBySportAndLiveStatus != null)
                    {
                        virtualRateByUser.ReportRows.Add(virtualRateBySportAndLiveStatus);
                        virtualRateBySportAndLiveStatus = null;
                    }

                    if (virtualRateByUser != null)
                    {
                        report.ReportRows.Add(virtualRateByUser);
                    }

                    virtualRateByUser = new SMAVirtualRate
                    {
                        UserName = userName
                    };

                    preUserName = userName;
                }

                if (sportAndLiveStatus != preSportAndLiveStatus)
                {
                    if (virtualRateBySportAndLiveStatus != null)
                    {
                        virtualRateByUser.ReportRows.Add(virtualRateBySportAndLiveStatus);
                    }

                    virtualRateBySportAndLiveStatus = new SMAVirtualRate
                    {
                        Sport = string.Format("{0} ({1})", GetSportName(sportId), liveStatusTitle),
                        VirtualRateByBettypeInfo = InitVirtualRateByBettypeInfo()
                    };
                    preSportAndLiveStatus = sportAndLiveStatus;
                }

                virtualRateBySportAndLiveStatus.VirtualRateByBettypeInfo[bettypeIndex].VirtualRate = virtualRate - 1;
            }

            if (virtualRateBySportAndLiveStatus != null)
            {
                virtualRateByUser.ReportRows.Add(virtualRateBySportAndLiveStatus);
            }

            if (virtualRateByUser != null)
            {
                report.ReportRows.Add(virtualRateByUser);
            }

            return report;
        }

        /// <summary>
        /// Maps the tree win loss data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>Tree win loss data.</returns>
        private TreeWinLossInfo MapTreeWinLossData(IDataReader reader, int roleId)
        {
            string recommend = string.Empty;
            switch (roleId)
            {
                case 2:
                    recommend = "custid";
                    break;

                case 3:
                    recommend = "recommend";
                    break;

                case 4:
                    recommend = "mrecommend";
                    break;

                case 5:
                    recommend = "srecommend";
                    break;

                default:
                    break;
            }

            var winLoss = new TreeWinLossInfo()
            {
                Id = Converter.ToInt(reader[recommend]),
                Name = Converter.ToString2(reader["username"]),
                Turnover = Converter.ToDecimal(reader["turnover"]),
                WinLoss = Converter.ToDecimal(reader["winloss"]),
                Comm = Converter.ToDecimal(reader["commission"]),
                WinLossTotal = Converter.ToDecimal(reader["total"]),
                BaseRMTurnover = Converter.ToDecimal(reader["turnoverRM"]),
                BaseRMVirtualTurnover = Converter.ToDecimal(reader["Virtual_Turnover"]),
                BaseRMTotal = Converter.ToDecimal(reader["totalRM"]),
                BaseRMVirtualTotal = Converter.ToDecimal(reader["virtual_total"]),
                BaseRMTOVariancePlus = Converter.ToDecimal(reader["TOVariance+"]),
                BaseRMTOVarianceMinus = Converter.ToDecimal(reader["TOVariance-"]),
                BaseRMWLVarianceMinus = Converter.ToDecimal(reader["WLVariance-"]),
                BaseRMWLVariancePlus = Converter.ToDecimal(reader["WLVariance+"]),
                VirtualRate = Converter.ToDecimal(reader["EnableVirtualrate"])
            };

            return winLoss;
        }

        /// <summary>
        /// Maps the enabled virtual rate user data.
        /// </summary>
        /// <param name="enabledVirtualRateUser">The enabled virtual rate user.</param>
        /// <returns>Enable virtual rate users. </returns>
        private Dictionary<string, int> MapEnabledVirtualRateUserData(List<TreeWinLossInfo> enabledVirtualRateUser)
        {
            int index = 0;
            Dictionary<string, int> enabledVirtualRateUserDictionary = new Dictionary<string, int>();
            foreach (var user in enabledVirtualRateUser)
            {
                enabledVirtualRateUserDictionary.Add(user.Name, index);
                index++;
            }

            return enabledVirtualRateUserDictionary;
        }

        /// <summary>
        /// Gets the enabled virtual rate user from IBC and WH.
        /// </summary>
        /// <param name="enabledVirtualRateUserFromIBC">The enabled virtual rate user from IBC.</param>
        /// <param name="enabledVirtualRateUserFromWH">The enabled virtual rate user from WH.</param>
        /// <returns>Enable virtual rate user from IBC and WH.</returns>
        private List<SMAVirtualRate> GetEnabledVirtualRateUserFromIBCAndWH(
            List<SMAVirtualRate> enabledVirtualRateUserFromIBC,
            List<SMAVirtualRate> enabledVirtualRateUserFromWH)
        {
            Dictionary<string, string> enabledVirtualRateUserDictionary = new Dictionary<string, string>();
            List<SMAVirtualRate> enabledVirtualRateUser = new List<SMAVirtualRate>();
            foreach (var user in enabledVirtualRateUserFromIBC)
            {
                enabledVirtualRateUserDictionary.Add(user.UserName, user.UserName);
                enabledVirtualRateUser.Add(user);
            }

            foreach (var user in enabledVirtualRateUserFromWH)
            {
                bool userIsExist = enabledVirtualRateUserDictionary.ContainsKey(user.UserName);
                if (userIsExist)
                {
                    continue;
                }

                enabledVirtualRateUser.Add(user);
            }

            return enabledVirtualRateUser;
        }

        private string GetProductParams(string productSelection)
        {
            XDocument productDocument = new XDocument(new XElement("ROOT"));
            var root = AppDomain.CurrentDomain.BaseDirectory;
            var productFilePath = root + TreeWinLossProductPath;
            XDocument treeWinLossProductDocument = XDocument.Load(productFilePath);

            var products = productSelection.Split(',');

            foreach (var item in products)
            {
                foreach (var profileElement in treeWinLossProductDocument.Descendants("Product").ToList())
                {
                    if (Converter.ToInt(profileElement.Attribute("ProductID").Value) == Converter.ToInt(item))
                    {
                        productDocument.Root.Add(profileElement);
                        break;
                    }
                }
            }

            return productDocument.ToString();
        }

        private bool IshowNotAvailableTurnover(string productSelection)
        {
            bool isShowNotAvailable = false;
            List<bool> isShowTurnover = new List<bool>();

            var products = productSelection.Split(',');
            var root = AppDomain.CurrentDomain.BaseDirectory;
            var productFilePath = root + TreeWinLossProductPath;
            XDocument productDocument = XDocument.Load(productFilePath);

            foreach (var item in products)
            {
                foreach (var profileElement in productDocument.Descendants("Product").ToList())
                {
                    if (Converter.ToInt(profileElement.Attribute("ProductID").Value) == Converter.ToInt(item))
                    {
                        isShowTurnover.Add(Converter.ToInt(profileElement.Attribute("IsNoTurnover").Value) == 1);
                        break;
                    }
                }
            }

            isShowNotAvailable = isShowTurnover.All(x => x == true);
            return isShowNotAvailable;
        }

        private bool IsShowAvailableTurnoverByProduct(GetTreeWinLossByProductCriteria criteria)
        {
            bool isShowNotAvailableTurnover = false;
            int compareCasinoSingleWalletLaunchDate = DateTime.Compare(criteria.FromDate, AppSettings.CasinoSingleWalletLaunchDate);
            if (criteria.ProductSelection == "6" && compareCasinoSingleWalletLaunchDate < 0)
            {
                isShowNotAvailableTurnover = true;
            }
            else if (criteria.ProductSelection == "7,6" && compareCasinoSingleWalletLaunchDate < 0)
            {
                isShowNotAvailableTurnover = true;
            }
            else if (criteria.ProductSelection == "6,9" && compareCasinoSingleWalletLaunchDate < 0)
            {
                isShowNotAvailableTurnover = true;
            }
            else if (criteria.ProductSelection == "7,6,9" && compareCasinoSingleWalletLaunchDate < 0)
            {
                isShowNotAvailableTurnover = true;
            }
            else
            {
                isShowNotAvailableTurnover = IshowNotAvailableTurnover(criteria.ProductSelection);
            }

            return isShowNotAvailableTurnover;
        }

        private bool IsShowAvailableTurnoverByLevel(GetTreeWinLossByLevelCriteria criteria)
        {
            bool isShowNotAvailableTurnover = false;
            int compareCasinoSingleWalletLaunchDate = DateTime.Compare(criteria.FromDate, AppSettings.CasinoSingleWalletLaunchDate);
            if (criteria.ProductSelection == "6" && compareCasinoSingleWalletLaunchDate < 0)
            {
                isShowNotAvailableTurnover = true;
            }
            else if (criteria.ProductSelection == "7,6" && compareCasinoSingleWalletLaunchDate < 0)
            {
                isShowNotAvailableTurnover = true;
            }
            else if (criteria.ProductSelection == "6,9" && compareCasinoSingleWalletLaunchDate < 0)
            {
                isShowNotAvailableTurnover = true;
            }
            else if (criteria.ProductSelection == "7,6,9" && compareCasinoSingleWalletLaunchDate < 0)
            {
                isShowNotAvailableTurnover = true;
            }
            else
            {
                isShowNotAvailableTurnover = IshowNotAvailableTurnover(criteria.ProductSelection);
            }

            return isShowNotAvailableTurnover;
        }
    }
}