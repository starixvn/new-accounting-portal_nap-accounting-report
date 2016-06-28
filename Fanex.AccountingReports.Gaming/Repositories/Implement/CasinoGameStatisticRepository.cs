// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameStatisticRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoGameStatisticRepository : ICasinoGameStatisticRepository
    {
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<List<WinLossInfoEntity>> GetReport(GetReportByDailyCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<List<WinLossInfoEntity>> report = new List<List<WinLossInfoEntity>>();
            if (result == null)
            {
                return report;
            }

            if (result.Tables.Count >= 1)
            {
                var winLossInfoTable = result.Tables[0];
                var winLossInfo = new List<WinLossInfoEntity>();
                string custId = string.Empty;
                int numberOfPlayers = 0;

                if (winLossInfoTable.Rows.Count == 1 && string.IsNullOrEmpty(winLossInfoTable.Rows[0]["BetCount"].ToString()))
                {
                    winLossInfoTable = new DataTable();
                }

                foreach (DataRow row in winLossInfoTable.Rows)
                {
                    if (criteria.ReportType == PeriodTypeConstants.CasinoGameStatisticCompany && !criteria.IsGrandRM)
                    {
                        custId = Converter.ToString2(row["Company"]);
                    }

                    if (criteria.ReportType == PeriodTypeConstants.CasinoGameStatisticUsername && !criteria.IsGrandRM)
                    {
                        custId = Converter.ToString2(row["custid"]);
                    }
                    else
                    {
                        numberOfPlayers = Converter.ToInt(row["NoOfPlayers"]);
                    }

                    var gamePayout = Converter.ToDecimal(row["TotalBets"]) == 0 ? 0 : Converter.ToDecimal(row["TotalWins"]) / Converter.ToDecimal(row["TotalBets"]);
                    var info = new WinLossInfoEntity
                    {
                        Id = criteria.IsGrandRM ? string.Empty : GetWinLossInfoId(criteria, row),
                        CustId = custId,
                        NumberOfPlayers = numberOfPlayers,

                        NumberOfGames = GetBetCountByReportTye(criteria.ReportType, row),

                        CurrencyId = criteria.IsGrandRM ? 0 : Converter.To<int>(row["CurrencyId"]),
                        CurrencyName = Converter.ToString2(row["Currency"]),

                        TotalBets = Converter.ToDecimal(row["TotalBets"]),
                        TotalWins = Converter.ToDecimal(row["TotalWins"]),
                        GameIncome = Converter.ToDecimal(row["GameIncome"]),
                        GamePayout = Math.Round(gamePayout, 4),
                        TotalRefund = Converter.ToDecimal(row["TotalRefund"]),
                        CompanyNetIncome = Converter.ToDecimal(row["CompanyNetIncome"]),
                        CompanyTurnover = Converter.ToDecimal(row["CompanyTurnOver"]),
                        CompanyCommission = Converter.ToDecimal(row["CompanyComm"]),
                    };

                    winLossInfo.Add(info);
                }

                report.Add(winLossInfo);
            }

            if (result.Tables.Count >= 2)
            {
                var grandTotalInfoTable = result.Tables[1];
                var grandTotalInfo = new List<WinLossInfoEntity>();
                foreach (DataRow row in grandTotalInfoTable.Rows)
                {
                    var gamePayout = Converter.ToDecimal(row["TotalBets"]) == 0 ? 0 : Converter.ToDecimal(row["TotalWins"]) / Converter.ToDecimal(row["TotalBets"]);
                    grandTotalInfo.Add(new WinLossInfoEntity
                    {
                        NumberOfPlayers = criteria.ReportType == PeriodTypeConstants.CasinoGameStatisticUsername && !criteria.IsGrandRM ? 0 : Converter.ToInt(row["NoOfPlayers"]),
                        NumberOfGames = GetBetCountByReportTye(criteria.ReportType, row),
                        CurrencyName = Converter.ToString2(row["Currency"]),
                        TotalBets = Converter.ToDecimal(row["TotalBets"]),
                        TotalWins = Converter.ToDecimal(row["TotalWins"]),
                        GameIncome = Converter.ToDecimal(row["GameIncome"]),
                        GamePayout = Math.Round(gamePayout, 4),
                        TotalRefund = Converter.ToDecimal(row["TotalRefund"]),
                        CompanyNetIncome = Converter.ToDecimal(row["CompanyNetIncome"]),
                        CompanyTurnover = Converter.ToDecimal(row["CompanyTurnOver"]),
                        CompanyCommission = Converter.ToDecimal(row["CompanyComm"])
                    });
                }

                report.Add(grandTotalInfo);
            }

            return report;
        }

        private int GetBetCountByReportTye(int reportType, DataRow row)
        {
            int betcount = 0;
            switch (reportType)
            {
                case PeriodTypeConstants.CasinoGameStatisticDaily:
                case PeriodTypeConstants.CasinoGameStatisticMonthly:
                case PeriodTypeConstants.CasinoGameStatisticUsername:
                case PeriodTypeConstants.CasinoGameStatisticGameType:
                case PeriodTypeConstants.CasinoGameStatisticGameTable:
                case PeriodTypeConstants.CasinoGameStatisticCompany:
                case PeriodTypeConstants.CasinoGameStatisticCurrency:
                    betcount = Converter.ToInt(row["BetCount"]);
                    break;
            }

            return betcount;
        }

        private string GetWinLossInfoId(GetReportByDailyCriteria criteria, DataRow dataRow)
        {
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.LiveCasinoGameStatisticDaily:
                    return Converter.ToDateTime(dataRow["WinLostDate"]).ToShortFormatedDate();
                case PeriodTypeConstants.LiveCasinoGameStatisticMonthly:
                    return string.Format("{0}-{1}", dataRow["Year"], dataRow["Month"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticUsername:
                    return Converter.ToString2(dataRow["Username"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticGameType:
                    return Converter.ToString2(dataRow["GameTypeId"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticGameTable:
                    return Converter.ToString2(dataRow["LeagueId"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                    return Converter.ToString2(dataRow["LicenseeName"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticCurrency:
                    return Converter.ToString2(dataRow["Currency"]);
                default:
                    return string.Empty;
            }
        }

        public ProductList GetProductList(GetProductListCriteria criteria)
        {
            ProductList products = new ProductList();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = db.ExecuteReader();
                while (reader.Read())
                {
                    products.ProductGroup.Add(Converter.To<string>(reader["ProductGroup"]));
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        products.Products.Add(new ProductItem()
                        {
                            ProductId = Converter.To<int>(reader["ProductID"]),
                            ProductName = Converter.To<string>(reader["ProductName"]),
                            ProductGroup = Converter.To<string>(reader["ProductGroup"])
                        });
                    }
                }
            }

            return products;
        }

        public List<GameTypeItem> GetGameType(GetGameTypeListCriteria criteria)
        {
            List<GameTypeItem> gameTypes = new List<GameTypeItem>();
            if (criteria.ProductId == null)
            {
                return gameTypes;
            }

            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var reader = db.ExecuteReader();
                while (reader.Read())
                {
                    gameTypes.Add(new GameTypeItem()
                    {
                        ProductId = Converter.To<int>(reader["ProductId"]),
                        TypeId = Converter.To<int>(reader["TypeId"]),
                        Name = Converter.To<string>(reader["TypeNamee"])
                    });
                }
            }

            return gameTypes;
        }

        public List<EntityBase<int, string>> GetGameTableList(GetGameTableListCriteria criteria)
        {
            List<EntityBase<int, string>> gameTable = new List<EntityBase<int, string>>();

            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var reader = db.ExecuteReader();
                while (reader.Read())
                {
                    gameTable.Add(new EntityBase<int, string>()
                    {
                        Id = Converter.To<int>(reader["GameTableId"]),
                        Name = Converter.To<string>(reader["GameTable"])
                    });
                }
            }

            return gameTable;
        }

        public List<WinLossInfo> GetReportByGrandTotal(GetReportByDailyCriteria criteria)
        {
            if (Converter.To<DateTime>(criteria.EffectiveDate) > Converter.To<DateTime>(criteria.FromDate))
            {
                criteria.FromDate = criteria.EffectiveDate;
            }

            List<WinLossInfo> totaList = new List<WinLossInfo>();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var row = db.ExecuteReader();
                while (row.Read())
                {
                    var gamePayout = Converter.ToDecimal(row["TotalBets"]) == 0 ? 0 : Converter.ToDecimal(row["TotalWins"]) / Converter.ToDecimal(row["TotalBets"]);
                    totaList.Add(new WinLossInfo
                    {
                        Name = Labels.Total + " " + Converter.ToString2(row["Currency"]),
                        CurrencyName = Converter.ToString2(row["Currency"]),
                        CurrencyId = criteria.ReportType == 6 ? Converter.ToInt(row["CurrencyId"]) : 0,
                        NumberOfPlayers = Converter.ToInt(row["NoOfPlayers"]),
                        NumberOfGames = Converter.ToInt(row["BetCount"]),

                        TotalBets = Converter.ToDecimal(row["TotalBets"]),
                        TotalWins = Converter.ToDecimal(row["TotalWins"]),
                        GameIncome = Converter.ToDecimal(row["GameIncome"]),
                        GamePayout = Math.Round(gamePayout, 4),
                        TotalRefund = Converter.ToDecimal(row["TotalRefund"]),
                        CompanyNetIncome = Converter.ToDecimal(row["CompanyNetIncome"]),
                        CompanyTurnover = Converter.ToDecimal(row["CompanyTurnOver"]),
                        CompanyCommission = Converter.ToDecimal(row["CompanyComm"])
                    });
                }
            }

            if (criteria.ReportType == 6)
            {
                totaList = totaList.OrderBy(s => s.CurrencyId).ToList();
            }

            return totaList;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public Hourly GetReportByHourly(GetReportByHourlyCriteria criteria)
        {
            Hourly hourly = new Hourly();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var reader = db.ExecuteReader();
                WinLossInfo total = new WinLossInfo();
                bool isEmptyData = false;
                while (reader.Read())
                {
                    var gamePayout = Converter.ToDecimal(reader["TotalBets"]) == 0 ? 0 : Converter.ToDecimal(reader["TotalWins"]) / Converter.ToDecimal(reader["TotalBets"]);
                    WinLossInfo row = new WinLossInfo();
                    row.Name = Converter.To<string>(reader["Description"]);
                    row.NumberOfPlayers = Converter.To<int>(reader["NoOfPlayers"]);
                    row.NumberOfGames = Converter.To<int>(reader["BetCount"]);
                    row.CurrencyName = Converter.To<string>(reader["Currency"]);
                    row.TotalBets = Math.Round(Converter.To<decimal>(reader["TotalBets"]), 2);
                    row.TotalWins = Math.Round(Converter.To<decimal>(reader["TotalWins"]), 2);
                    row.GameIncome = Math.Round(Converter.To<decimal>(reader["GameIncome"]), 2);
                    row.GamePayout = Math.Round(gamePayout, 4);
                    row.TotalRefund = Math.Round(Converter.To<decimal>(reader["TotalRefund"]), 2);
                    row.CompanyNetIncome = Math.Round(Converter.To<decimal>(reader["CompanyNetIncome"]), 2);
                    row.CompanyTurnover = Math.Round(Converter.To<decimal>(reader["CompanyTurnover"]), 2);
                    row.CompanyCommission = Math.Round(Converter.To<decimal>(reader["CompanyComm"]), 2);

                    total.Sum(row);

                    isEmptyData = string.IsNullOrEmpty(Converter.To<string>(reader["BetCount"]));
                    if (isEmptyData)
                    {
                        hourly.DataOnEachHourList.Add(new Hourly()
                        {
                            Name = row.Name,
                            OnlinePlayers = string.Empty
                        });

                        hourly.DataAccumulatedList.Add(new Hourly()
                        {
                            Name = row.Name,
                            OnlinePlayers = string.Empty
                        });
                    }
                    else
                    {
                        hourly.DataOnEachHourList.Add(new Hourly()
                        {
                            Name = row.Name,
                            OnlinePlayers = isEmptyData ? string.Empty : row.NumberOfPlayers.ToString("#,##0"),
                            NumberOfGames = Converter.To<string>(row.NumberOfGames),
                            Currency = row.CurrencyName,
                            TotalBets = Converter.To<string>(row.TotalBets),
                            TotalWins = Converter.To<string>(row.TotalWins),
                            GameIncome = Converter.To<string>(row.GameIncome),
                            GamePayout = Converter.To<string>(row.GamePayout),
                            TotalRefund = Converter.To<string>(row.TotalRefund),
                            CompanyTO = Converter.To<string>(row.CompanyTurnover),
                            CompanyComm = Converter.To<string>(row.CompanyCommission),
                            CompanyNetInCome = Converter.To<string>(row.CompanyNetIncome),
                        });

                        gamePayout = total.TotalBets == 0 ? 0 : total.TotalWins / total.TotalBets;
                        hourly.DataAccumulatedList.Add(new Hourly()
                        {
                            Name = Converter.To<string>(reader["Description"]),
                            OnlinePlayers = isEmptyData ? string.Empty : row.NumberOfPlayers.ToString("#,##0"),
                            NumberOfGames = Converter.To<string>(total.NumberOfGames),
                            Currency = row.CurrencyName,
                            TotalBets = Converter.To<string>(total.TotalBets),
                            TotalWins = Converter.To<string>(total.TotalWins),
                            GameIncome = Converter.To<string>(total.GameIncome),
                            GamePayout = Converter.To<string>(Math.Round(gamePayout, 4)),
                            TotalRefund = Converter.To<string>(total.TotalRefund),
                            CompanyTO = Converter.To<string>(total.CompanyTurnover),
                            CompanyComm = Converter.To<string>(total.CompanyCommission),
                            CompanyNetInCome = Converter.To<string>(total.CompanyNetIncome),
                        });
                    }
                }

                var totalOnlinePlayers = hourly.DataOnEachHourList.Where(s => !string.IsNullOrEmpty(s.NumberOfGames)).Sum(s => Converter.To<decimal>(s.OnlinePlayers));
                var countOnlinePlayers = hourly.DataOnEachHourList.Where(s => !string.IsNullOrEmpty(s.NumberOfGames)).Count();
                var onlinePlayersAverage = countOnlinePlayers == 0 ? 0 : totalOnlinePlayers / countOnlinePlayers;
                hourly.OnlinePlayers = onlinePlayersAverage.ToString("#,##0.##");
            }

            return hourly;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public CompanyInfo GetLicenseeList(GetLicenseeListCriteria criteria)
        {
            CompanyInfo result = new CompanyInfo();
            ObjectDb db = new ObjectDb(criteria.GetSettingKey());
            db.AssignParamValues(criteria.GetSpParams());
            var effectiveDate = new DateTime(1900, 01, 01);
            result = new CompanyInfo
            {
                ChildInfo = new List<CompanyInfo>()
            };

            CompanyInfo info = new CompanyInfo
            {
                Id = CompanyConstants.IBC,
                Name = Labels.IBC_Group,
                EffectiveDate = effectiveDate.ToString("MMM dd, yyyy"),
                ChildInfo = new List<CompanyInfo>()
            };

            result.ChildInfo.Add(info);

            info = new CompanyInfo
            {
                Id = CompanyConstants.Licensee,
                Name = Labels.Licensee,
                EffectiveDate = effectiveDate.ToString("MMM dd, yyyy"),
                ChildInfo = new List<CompanyInfo>()
            };

            using (var reader = db.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = Converter.ToString2(reader["Licenseename"]);
                    effectiveDate = Converter.To<DateTime>(reader["EffectiveDate"]);
                    if (effectiveDate > Converter.To<DateTime>(criteria.ToDate))
                    {
                        continue;
                    }
                    
                    info.ChildInfo.Add(new CompanyInfo
                    {
                        Id = Converter.To<int>(reader["LicenseeID"]),
                        Name = name,
                        EffectiveDate = effectiveDate.ToString("MMM dd, yyyy"),
                    });
                }
            }

            result.ChildInfo.Add(info);

            return result;
        }
    }
}