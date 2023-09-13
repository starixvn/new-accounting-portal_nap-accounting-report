namespace Fanex.AccountingReports.Misc.Services.Implement.MAMonitorTool
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Entities.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Models.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.Common;
    using Fanex.Data;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// MA Monitor Tool Service.
    /// </summary>
    public class MAMonitorToolService : ServiceBase
    {
        public readonly string ReportSessionKey = "Fanex.AccountingReports.Misc.MAMonitorTool.Report" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private string _pagingReportSessionKey = "Fanex.AccountingReports.Misc.MAMonitorTool.PagingReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private string _productSessionKey = "Fanex.AccountingReports.Misc.MAMonitorTool.Products" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        /// <summary>
        /// Gets the report type list.
        /// </summary>
        /// <returns>The report type list.</returns>
        public List<EntityBase<int, string>> GetReportTypeList()
        {
            List<EntityBase<int, string>> result = new List<EntityBase<int, string>>();
            MAMonitorCollection collection = MAMonitorSection.Instance.ReportBy;
            foreach (MAMonitorElement element in collection)
            {
                if (!element.IsActive)
                {
                    continue;
                }

                result.Add(new EntityBase<int, string>()
                {
                    Id = Converter.ToInt(element.Id),
                    Name = Converter.ToString2(element.Name)
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the product list.
        /// </summary>
        /// <returns>The product list.</returns>
        public List<BaseModel> GetProductList()
        {
            List<BaseModel> result = new List<BaseModel>();
            MAMonitorCollection collection = MAMonitorSection.Instance.Products;
            Dictionary<int, string> cachedProducts = new Dictionary<int, string>();
            foreach (MAMonitorElement element in collection)
            {
                if (!element.IsActive)
                {
                    continue;
                }

                int productId = Converter.ToInt(element.Id);
                string productName = Converter.ToString2(element.Name);

                result.Add(new BaseModel()
                {
                    Id = productId,
                    Name = productName,
                    AliasName = Converter.ToString2(element.AliasName)
                });

                cachedProducts.Add(productId, productName);
            }

            SessionManager.Set<Dictionary<int, string>>(_productSessionKey, cachedProducts);
            return result;
        }

        /// <summary>
        /// Gets the sport list.
        /// </summary>
        /// <returns>The sport list.</returns>
        public List<EntityBase<int, string>> GetSportList()
        {
            List<EntityBase<int, string>> result = new List<EntityBase<int, string>>();
            result.Add(new EntityBase<int, string>()
            {
                Id = SportConstants.AllSport,
                Name = Labels.All
            });

            var rawData = GetReportData<Sport>(new GetSportListCriteria());
            foreach (var row in rawData)
            {
                result.Add(new EntityBase<int, string>()
                {
                    Id = row.SportType,
                    Name = row.SportName
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the bet type list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// The bet type list.
        /// </returns>
        public List<EntityBase<int, string>> GetBetTypeList(GetBetTypeListCriteria criteria)
        {
            List<EntityBase<int, string>> result = new List<EntityBase<int, string>>();
            result.Add(new EntityBase<int, string>()
            {
                Id = SportConstants.AllSport,
                Name = Labels.All
            });

            var rawData = GetReportData<ProductEntity>(criteria);
            foreach (var row in rawData)
            {
                result.Add(new EntityBase<int, string>()
                {
                    Id = row.TypeId,
                    Name = row.TypeName
                });
            }

            return result;
        }

        public List<ProductBettype> GetProductBettype(GetBetTypeListCriteria criteria)
        {
            var productBettypes = new List<ProductBettype>();
            var rawData = GetReportData<ProductBettypeEntity>(criteria);
            var productGroups = rawData.GroupBy(x => x.ProductId).ToList();
            var cachedProducts = GetProductFromCache();
            foreach (var productGroup in productGroups)
            {
                var productBettype = new ProductBettype();
                productBettype.ProductName = cachedProducts[productGroup.Key];
                foreach (var bettype in productGroup)
                {
                    productBettype.Bettypes.Add(new Bettype()
                    {
                        BettypeId = Converter.ToInt(bettype.TypeId),
                        BettypeName = Converter.ToString2(bettype.TypeName)
                    });
                }

                productBettypes.Add(productBettype);
            }

            return productBettypes;
        }

        /// <summary>
        /// Gets the currency list.
        /// </summary>
        /// <returns>The currency list.</returns>
        public List<EntityBase<int, string>> GetCurrencyList()
        {
            List<EntityBase<int, string>> result = new List<EntityBase<int, string>>();
            result.Add(new EntityBase<int, string>()
            {
                Id = (int)CoreConstants.CurrencyContants.All,
                Name = string.Format("{0} ({1})", Labels.All, Labels.RM)
            });

            var rawData = ReportService.ExchangeCurrencies;
            foreach (var row in rawData)
            {
                result.Add(new EntityBase<int, string>()
                {
                    Id = Converter.ToInt(row.Key),
                    Name = Converter.ToString2(row.Value)
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the league type list.
        /// </summary>
        /// <returns>The league type list.</returns>
        public List<BaseModel> GetLeagueTypeList()
        {
            List<BaseModel> result = new List<BaseModel>();
            MAMonitorCollection collection = MAMonitorSection.Instance.LeagueTypes;
            foreach (MAMonitorElement element in collection)
            {
                if (!element.IsActive)
                {
                    continue;
                }

                result.Add(new BaseModel()
                {
                    Id = Converter.ToInt(element.Id),
                    Name = Converter.ToString2(element.Name),
                    AliasName = Converter.ToString2(element.AliasName)
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the league list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The league list.</returns>
        public List<EntityBase<int, string>> GetLeagueList(GetLeagueListCriteria criteria)
        {
            List<EntityBase<int, string>> result = new List<EntityBase<int, string>>();
            var rawData = GetReportData<League>(criteria);
            foreach (var row in rawData)
            {
                var league = new EntityBase<int, string>()
                {
                    Id = row.LeagueId,
                    Name = row.LeagueName
                };
                result.Add(league);
            }

            return result;
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public WinLossInfo GetReport(GetReportCriteria criteria)
        {
            WinLossInfo fullReport = new WinLossInfo();
            WinLossInfo pagingReport = new WinLossInfo();

            if (!criteria.IsPagingCallBack)
            {
                fullReport = GetAndConvertFullReport(criteria);
                pagingReport = fullReport;
                ClearAllPagingSessionKey();

                if (criteria.ReportBy != 2)
                {
                    SessionManager.Set<WinLossInfo>(ReportSessionKey, fullReport);
                }
            }
            else
            {
                fullReport = SessionManager.Get<WinLossInfo>(ReportSessionKey);
                if (criteria.ReportBy == 2)
                {
                    fullReport = null;
                }
            }

            if (fullReport == null)
            {
                fullReport = GetAndConvertFullReport(criteria);
                pagingReport = fullReport;
            }

            if (fullReport.ChildsInfo != null && fullReport.ChildsInfo.Count > 0 && !criteria.IsExportExcel)
            {
                pagingReport = GetPagingReport(fullReport, pagingReport, criteria);
            }

            return pagingReport;
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public virtual void ExportReport(GetReportCriteria criteria)
        {
            throw new Exception(Labels.Error_MethodNotImplemented);
        }

        /// <summary>
        /// Gets the name of the report type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The name of the report type.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public string GetReportTypeName(GetReportCriteria criteria)
        {
            var reportTypeName = string.Empty;
            switch (criteria.ReportBy)
            {
                case ReportTypeConstants.ReportByDaily:
                    reportTypeName = Labels.Date;
                    break;

                case ReportTypeConstants.ReportByMonthly:
                    reportTypeName = Labels.Month;
                    break;

                case ReportTypeConstants.ReportByUsername:
                    reportTypeName = Labels.UserName;
                    break;

                case ReportTypeConstants.ReportByBetType:
                    reportTypeName = Labels.Bet_Type;
                    break;

                case ReportTypeConstants.ReportByLeague:
                    reportTypeName = Labels.League;
                    break;

                case ReportTypeConstants.ReportByGame:
                    reportTypeName = Labels.Game;
                    break;

                case ReportTypeConstants.ReportByCurrency:
                    reportTypeName = Labels.Currency;
                    break;

                case ReportTypeConstants.ReportBySport:
                    reportTypeName = Labels.Sport;
                    break;

                default:
                    reportTypeName = Labels.Bet_Type;
                    break;
            }

            return reportTypeName;
        }

        /// <summary>
        /// Get and convert data from database.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The full report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private WinLossInfo GetAndConvertFullReport(GetReportCriteria criteria)
        {
            DataSet dataSet = null;

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var param = criteria;
                dataSet = objectDb.ExecuteDataSet(param);
            }

            var report = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.Total,
                    ChildsInfo = new List<WinLossInfo>()
                }
            };

            report.GrandTotal.ChildsInfo.Add(new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Member.ToString()
            });

            report.GrandTotal.ChildsInfo.Add(new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Agent.ToString(),
            });

            report.GrandTotal.ChildsInfo.Add(new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Super.ToString()
            });

            report.GrandTotal.ChildsInfo.Add(new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Master.ToString(),
            });

            WinLossInfo info;
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                info = GetWinLossInfo(criteria, dataRow);
                if (!IsCorrectFilteredInfo(criteria, info.ChildsInfo[0]))
                {
                    continue;
                }

                report.GrandTotal.Sum(info);
                info = WinLossInfo.Format(info);
                report.ChildsInfo.Add(info);
            }

            report.GrandTotal = WinLossInfo.Format(report.GrandTotal);
            return report;
        }

        /// <summary>
        /// Clear all paging session key.
        /// </summary>
        private void ClearAllPagingSessionKey()
        {
            for (int i = 0; i < HttpContext.Current.Session.Count; i++)
            {
                if (HttpContext.Current.Session.Keys[i].StartsWith(_pagingReportSessionKey))
                {
                    HttpContext.Current.Session.Remove(HttpContext.Current.Session.Keys[i]);
                    i--;
                }
            }
        }

        /// <summary>
        /// Get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The WinLossInfo paging report.</returns>
        private WinLossInfo GetPagingReport(
            WinLossInfo fullReport,
            WinLossInfo pagingReport,
            GetReportCriteria criteria)
        {
            string pagingSessionKey = CreatePagingSessionKey(criteria);
            pagingReport = SessionManager.Get<WinLossInfo>(pagingSessionKey);

            if (pagingReport == null)
            {
                pagingReport = GetPagingReportDetail(fullReport, pagingReport, criteria);
                SessionManager.Set<WinLossInfo>(pagingSessionKey, pagingReport);
            }

            return pagingReport;
        }

        /// <summary>
        /// Create paging session key.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The paging session key.</returns>
        private string CreatePagingSessionKey(GetReportCriteria criteria)
        {
            string pagingSessionKey = _pagingReportSessionKey + "." + criteria.ReportBy
                    + "." + criteria.FromDate + "." + criteria.ToDate + "." + criteria.SportType
                    + "." + criteria.BetTypeId + "." + criteria.CurrencyId + "." + criteria.LiveStatus
                    + "." + criteria.Leagues + "." + criteria.Usernames + "." + criteria.MinTurnover
                    + "." + criteria.MaxTurnover + "." + criteria.MinWinLoss + "." + criteria.MaxWinLoss
                    + "." + criteria.IsPagingCallBack + "." + criteria.StartIndex + "." + criteria.UsersPerPage
                    + criteria.Products;

            return pagingSessionKey;
        }

        /// <summary>
        /// The detail of get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The WinLossInfo paging report.</returns>
        private WinLossInfo GetPagingReportDetail(
            WinLossInfo fullReport,
            WinLossInfo pagingReport,
            GetReportCriteria criteria)
        {
            pagingReport = new WinLossInfo
            {
                Id = fullReport.Id,
                Name = fullReport.Name,
                LeagueId = fullReport.LeagueId,
                LeagueName = fullReport.LeagueName,
                HomeId = fullReport.HomeId,
                HomeName = fullReport.HomeName,
                AwayId = fullReport.AwayId,
                AwayName = fullReport.AwayName,
                MatchId = fullReport.MatchId,
                MatchCode = fullReport.MatchCode,
                BetCount = fullReport.BetCount,
                WinLoss = fullReport.WinLoss,
                Turnover = fullReport.Turnover,
                GrandTotal = fullReport.GrandTotal,
                NumberUsers = fullReport.ChildsInfo.Count,
                ChildsInfo = fullReport.ChildsInfo.Skip(criteria.StartIndex).Take(criteria.UsersPerPage).ToList()
            };

            return pagingReport;
        }

        /// <summary>
        /// Determines whether [is correct filtered information] [the specified criteria].
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="info">The information.</param>
        /// <returns>Is Correct Filtered Info.</returns>
        private bool IsCorrectFilteredInfo(GetReportCriteria criteria, WinLossInfo info)
        {
            return (string.IsNullOrEmpty(criteria.MinTurnover) ||
                    Converter.ToDecimal(criteria.MinTurnover) <= info.Turnover) &&
                   (string.IsNullOrEmpty(criteria.MaxTurnover) ||
                    info.Turnover <= Converter.ToDecimal(criteria.MaxTurnover)) &&
                   (string.IsNullOrEmpty(criteria.MinWinLoss) ||
                   Converter.ToDecimal(criteria.MinWinLoss) <= info.WinLoss) &&
                   (string.IsNullOrEmpty(criteria.MaxWinLoss) ||
                   info.WinLoss <= Converter.ToDecimal(criteria.MaxWinLoss));
        }

        /// <summary>
        /// Gets the win loss information.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="dataRow">The row of data  table.</param>
        /// <returns>The win loss information.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private WinLossInfo GetWinLossInfo(GetReportCriteria criteria, DataRow dataRow)
        {
            var info = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>()
            };

            switch (criteria.ReportBy)
            {
                case ReportTypeConstants.ReportByDaily:
                    DateTime winLossDate = Converter.ToDateTime(dataRow["rptdate"]);
                    info.Id = winLossDate.ToShortDateString();
                    info.Name = winLossDate.ToShortFormatedDate();
                    break;

                case ReportTypeConstants.ReportByMonthly:
                    var dateTime = new DateTime(Converter.ToInt(dataRow["Year"]), Converter.ToInt(dataRow["Month"]), 1);
                    info.Id = dateTime.ToShortDateString();
                    info.Name = dateTime.ToMonthYearWithSlash();
                    break;

                case ReportTypeConstants.ReportByUsername:
                    info.Id = Converter.ToString2(dataRow["username"]);
                    info.Name = info.Id;
                    break;

                case ReportTypeConstants.ReportByBetType:
                    var betTypeId = Converter.ToInt(dataRow["bettype"]);
                    info.Id = Converter.ToString2(betTypeId);
                    info.Name = GetBetTypeName(betTypeId);
                    break;

                case ReportTypeConstants.ReportByLeague:
                    var leagueId = Converter.ToInt(dataRow["leagueid"]);
                    info.Id = Converter.ToString2(leagueId);
                    info.Name = CacheRef.GetLeagueName(leagueId);
                    break;

                case ReportTypeConstants.ReportByGame:
                    int matchId = Converter.ToInt(dataRow["matchid"]);
                    info.Id = Converter.ToString2(dataRow["matchid"]);
                    info.MatchId = matchId;
                    switch (matchId)
                    {
                        case CoreConstants.SportContants.HorseRacing:
                            info.HomeId = Converter.ToInt(dataRow["homeid"]);
                            info.AwayId = Converter.ToInt(dataRow["awayid"]);
                            info.LeagueName = CacheRef.GetLeagueName(info.HomeId);
                            info.Name = Labels.Racing +
                                " - " + info.LeagueName +
                                " - " + Labels.Race + " " + info.AwayId.ToString();
                            break;

                        case CoreConstants.SportContants.NumberGame:
                            info.AwayId = Converter.ToInt(dataRow["awayid"]);
                            info.Name = Labels.NumberGame + " - " + Labels.Game_No + " " + info.AwayId;
                            break;

                        case CoreConstants.SportContants.LiveCasino:
                            info.HomeId = Converter.ToInt(dataRow["homeid"]);
                            info.LeagueName = CacheRef.GetLeagueName(info.HomeId);
                            info.MatchCode = Converter.ToString2(dataRow["matchcode"]);
                            info.Name = info.MatchCode + " " + Labels.Game_table + " - " + info.LeagueName;
                            break;

                        case CoreConstants.BetTypeConstants.P2PGames:
                            info.MatchCode = Converter.ToString2(dataRow["matchcode"]);
                            info.Name = info.MatchCode;
                            break;

                        case CoreConstants.BetTypeConstants.Outright:
                            info.Name = Labels.Outright;
                            break;

                        case CoreConstants.BetTypeConstants.MixParlay:
                            info.Name = Labels.Mix_Parlay;
                            break;

                        case CoreConstants.BetTypeConstants.SystemParlay:
                            info.Name = Labels.System_Parlay;
                            break;

                        default:
                            info.HomeId = Converter.ToInt(dataRow["homeid"]);
                            info.AwayId = Converter.ToInt(dataRow["awayid"]);
                            info.HomeName = CacheRef.GetTeamName(info.HomeId);
                            info.AwayName = CacheRef.GetTeamName(info.AwayId);
                            info.MatchCode = Converter.ToString2(dataRow["matchcode"]);
                            if (string.IsNullOrEmpty(info.AwayName) && string.IsNullOrEmpty(info.HomeName))
                            {
                                info.Name = info.MatchCode;
                            }
                            else
                            {
                                info.Name = info.MatchCode + " " + info.HomeName + Labels.Vs + info.AwayName;
                            }

                            break;
                    }

                    break;

                case ReportTypeConstants.ReportByCurrency:
                    info.Id = Converter.ToString2(dataRow["currencyname"]);
                    info.Name = info.Id;
                    break;

                case ReportTypeConstants.ReportBySport:
                    info.Id = Converter.ToString2(dataRow["sport"]);
                    info.Name = info.Id;
                    break;

                default:
                    break;
            }

            var memberWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Member.ToString(),
                WinLoss = Converter.ToDecimal(dataRow["Mem_Winloss"]),
                Turnover = Converter.ToDecimal(dataRow["Mem_Turnover"]),
                BetCount = Converter.ToInt(dataRow["Betcount"])
            };

            var agentWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Agent.ToString(),
                WinLoss = Converter.ToDecimal(dataRow["Ag_Winloss"]),
                Turnover = Converter.ToDecimal(dataRow["Ag_Turnover"])
            };

            var masterWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Master.ToString(),
                WinLoss = Converter.ToDecimal(dataRow["MAg_Winloss"]),
                Turnover = Converter.ToDecimal(dataRow["MAg_Turnover"])
            };

            var superWinLoss = new WinLossInfo
            {
                Id = CoreConstants.RoleConstants.Master.ToString(),
                WinLoss = Converter.ToDecimal(dataRow["SAg_Winloss"]),
                Turnover = Converter.ToDecimal(dataRow["SAg_Turnover"])
            };

            info.ChildsInfo.Add(memberWinLoss);
            info.ChildsInfo.Add(agentWinLoss);
            info.ChildsInfo.Add(masterWinLoss);
            info.ChildsInfo.Add(superWinLoss);
            return info;
        }

        /// <summary>
        /// Gets the name of the bet type.
        /// </summary>
        /// <param name="betTypeId">The bet type identifier.</param>
        /// <returns>The name of the bet type.</returns>
        private string GetBetTypeName(int betTypeId)
        {
            switch (betTypeId)
            {
                case CoreConstants.BetTypeConstants.KickOff:
                    return Labels.Kick_Off;

                case CoreConstants.BetTypeConstants.P2PGames:
                    return Labels.P2P;

                default:
                    return CacheRef.GetBettype(betTypeId);
            }
        }

        private Dictionary<int, string> GetProductFromCache()
        {
            Dictionary<int, string> products;
            products = SessionManager.Get<Dictionary<int, string>>(_productSessionKey);

            if (products == null)
            {
                GetProductList();
                products = SessionManager.Get<Dictionary<int, string>>(_productSessionKey);
            }

            return products;
        }
    }
}