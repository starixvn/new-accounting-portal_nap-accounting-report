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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
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
                foreach (DataRow row in winLossInfoTable.Rows)
                {
                    if (criteria.ReportType == PeriodTypeConstants.CasinoGameStatisticUsername)
                    {
                        numberOfPlayers = 0;
                        custId = Converter.ToString2(row["custid"]);
                    }
                    else
                    {
                        numberOfPlayers = Converter.ToInt(row["NoOfPlayers"]);
                        custId = string.Empty;
                    }
                    
                    var info = new WinLossInfoEntity
                    {
                        Id = GetWinLossInfoId(criteria, row),
                        CustId = custId,
                        NumberOfPlayers = numberOfPlayers,

                        NumberOfGames = GetBetCountByReportTye(criteria.ReportType, row),

                        CurrencyName = Converter.ToString2(row["Currency"]),

                        TotalBets = Converter.ToDecimal(row["TotalBets"]),
                        TotalWins = Converter.ToDecimal(row["TotalWins"]),
                        GameIncome = Converter.ToDecimal(row["GameIncome"]),
                        GamePayout = Converter.ToDecimal(row["GamePayout"]),
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
                    grandTotalInfo.Add(new WinLossInfoEntity
                    {
                        NumberOfPlayers = criteria.ReportType == PeriodTypeConstants.CasinoGameStatisticUsername ?
                            0 :
                            Converter.ToInt(row["NoOfPlayers"]),

                        NumberOfGames = GetBetCountByReportTye(criteria.ReportType, row),

                        CurrencyName = Converter.ToString2(row["Currency"]),

                        TotalBets = Converter.ToDecimal(row["TotalBets"]),
                        TotalWins = Converter.ToDecimal(row["TotalWins"]),
                        GameIncome = Converter.ToDecimal(row["GameIncome"]),
                        GamePayout = Converter.ToDecimal(row["GamePayout"]),
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
                    return Converter.ToString2(dataRow["GameTableID"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                    return Converter.ToString2(dataRow["Company"]);
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

        public List<WinLossInfo> GetReportByGrandTotal(GetReportByGrandRMCriteria criteria)
        {
            List<WinLossInfo> totaList = new List<WinLossInfo>();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var row = db.ExecuteReader();
                while (row.Read())
                {
                    totaList.Add(new WinLossInfo
                    {
                        Name = Labels.Total + " " + Converter.ToString2(row["Currency"]),
                        CurrencyName = Converter.ToString2(row["Currency"]),
                        NumberOfPlayers = Converter.ToInt(row["NoOfPlayers"]),
                        NumberOfGames = Converter.ToInt(row["BetCount"]),

                        TotalBets = Converter.ToDecimal(row["TotalBets"]),
                        TotalWins = Converter.ToDecimal(row["TotalWins"]),
                        GameIncome = Converter.ToDecimal(row["GameIncome"]),
                        TotalRefund = Converter.ToDecimal(row["TotalRefund"]),
                        CompanyNetIncome = Converter.ToDecimal(row["CompanyNetIncome"]),
                        CompanyTurnover = Converter.ToDecimal(row["CompanyTurnOver"]),
                        CompanyCommission = Converter.ToDecimal(row["CompanyComm"])
                    });
                }
            }

            return totaList;
        }

        public Hourly GetReportByHourly(GetReportByHourlyCriteria criteria)
        {
            Hourly hourly = new Hourly();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var reader = db.ExecuteReader();
                WinLossInfo total = new WinLossInfo();
                while (reader.Read())
                {
                    WinLossInfo row = new WinLossInfo();
                    row.Name = Converter.To<string>(reader["Description"]);
                    row.NumberOfPlayers = Converter.To<int>(reader["NoOfPlayers"]);
                    row.NumberOfGames = Converter.To<int>(reader["BetCount"]);
                    row.CurrencyName = Converter.To<string>(reader["Currency"]);
                    row.TotalBets = Converter.To<decimal>(reader["TotalBets"]);
                    row.TotalWins = Converter.To<decimal>(reader["TotalWins"]);
                    row.GameIncome = Converter.To<decimal>(reader["GameIncome"]);
                    row.TotalRefund = Converter.To<decimal>(reader["TotalRefund"]);
                    row.CompanyNetIncome = Converter.To<decimal>(reader["CompanyNetIncome"]);
                    row.CompanyTurnover = Converter.To<decimal>(reader["CompanyTurnover"]);
                    row.CompanyCommission = Converter.To<decimal>(reader["CompanyComm"]);

                    total.Sum(row);

                    if (string.IsNullOrEmpty(row.CurrencyName))
                    {
                        hourly.DataOnEachHourList.Add(new Hourly()
                        {
                            Name = row.Name
                        });

                        hourly.DataAccumulatedList.Add(new Hourly()
                        {
                            Name = row.Name
                        });
                    }
                    else
                    {
                        hourly.DataOnEachHourList.Add(new Hourly()
                        {
                            Name = row.Name,
                            OnlinePlayers = Converter.To<string>(row.NumberOfPlayers),
                            NumberOfGames = Converter.To<string>(row.NumberOfGames),
                            Currency = row.CurrencyName,
                            TotalBets = Converter.To<string>(row.TotalBets),
                            TotalWins = Converter.To<string>(row.TotalWins),
                            GameIncome = Converter.To<string>(row.GameIncome),
                            GamePayout = Converter.To<string>(row.GamePayout),
                            CompanyTO = Converter.To<string>(row.CompanyTurnover),
                            CompanyComm = Converter.To<string>(row.CompanyCommission),
                            CompanyNetInCome = Converter.To<string>(row.CompanyNetIncome),
                        });

                        hourly.DataAccumulatedList.Add(new Hourly()
                        {
                            Name = Converter.To<string>(reader["Description"]),
                            OnlinePlayers = Converter.To<string>(row.NumberOfPlayers),
                            NumberOfGames = Converter.To<string>(total.NumberOfGames),
                            Currency = row.CurrencyName,
                            TotalBets = Converter.To<string>(total.TotalBets),
                            TotalWins = Converter.To<string>(total.TotalWins),
                            GameIncome = Converter.To<string>(total.GameIncome),
                            GamePayout = Converter.To<string>(total.GamePayout),
                            CompanyTO = Converter.To<string>(total.CompanyTurnover),
                            CompanyComm = Converter.To<string>(total.CompanyCommission),
                            CompanyNetInCome = Converter.To<string>(total.CompanyNetIncome),
                        });
                    }
                }

                var onlinePlayersAverage = hourly.DataOnEachHourList.Average(s => Converter.To<decimal>(s.OnlinePlayers));
                hourly.OnlinePlayers = onlinePlayersAverage.ToString();
            }

            return hourly;
        }
    }
}