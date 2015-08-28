// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameFinancialReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoGameFinancialReportRepository : ILiveCasinoGameFinancialReportRepository
    {
        /// <summary>
        /// Gets the currencies.
        /// </summary>
        /// <returns>The currencies.</returns>
        public List<EntityBase<int, string>> GetCurrencies()
        {
            var result = new List<EntityBase<int, string>>();
            Dictionary<int, string> currList = ReportService.ExchangeCurrencies;

            foreach (var row in currList)
            {
                if (ReportService.IsExcludeCurrency(row.Value))
                {
                    continue;
                }

                result.Add(new EntityBase<int, string>()
                {
                    Id = row.Key,
                    Name = row.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the game types.
        /// </summary>
        /// <returns>The game types.</returns>
        public List<dynamic> GetGameTypes()
        {
            var dataTable = new DataTable();
            using (ObjectDb objDb = new ObjectDb("Gaming_LiveCasinoGameFinancial_GetGameType"))
            {
                var ds = objDb.ExecuteDataSet();
                if (ds != null && ds.Tables.Count >= 1)
                {
                    dataTable = ds.Tables[0];
                }
            }

            var result = new List<dynamic>();
            if (dataTable != null && dataTable.Rows.Count >= 1)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new
                    {
                        typeID = Convert.ToInt32(row["typeID"]),
                        typenamee = row["typenamee"].ToString1()
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the game tables.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The game tables.</returns>
        public List<dynamic> GetGameTables(GetGameTableCriteria criteria)
        {
            var result = new List<dynamic>();

            var dataTable = new DataTable();
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                var ds = objDb.ExecuteDataSet();
                if (ds != null && ds.Tables.Count >= 1)
                {
                    dataTable = ds.Tables[0];
                }
            }

            if (dataTable != null && dataTable.Rows.Count >= 1)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new
                    {
                        GameTableID = Convert.ToInt32(row["GameTableID"]),
                        GameTable = row["GameTable"].ToString1()
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the reseller.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The reseller.</returns>
        public List<dynamic> GetReseller(GetResellerCriteria criteria)
        {
            DataTable dataTable = null;
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                DataSet ds = objDb.ExecuteDataSet();
                if (ds != null)
                {
                    dataTable = ds.Tables[0];
                }
            }

            var result = new List<dynamic>();
            if (dataTable != null && dataTable.Rows.Count >= 1)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new
                    {
                        ResellerID = Convert.ToInt32(row["ResellerID"]),
                        ResellerName = row["ResellerName"].ToString1(),
                        ResellerType = row["ResellerType"].ToString1(),
                        LicenseeID = row["LicenseeID"].ToString1(),
                        LicenseeName = row["LicenseeName"].ToString1()
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the financial report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The financial report.</returns>
        public List<dynamic> GetFinancialReport(GetFinancialReportCriteria criteria, ref Hashtable outputParam)
        {
            DataTable dataTable = null;
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AddInParameter("datestart", DbType.DateTime, criteria.DateStart);
                objDb.AddInParameter("dateend", DbType.DateTime, criteria.DateEnd);
                objDb.AddInParameter("licensee", DbType.String, criteria.Licensee);
                objDb.AddInParameter("currency", DbType.String, criteria.Currency);
                objDb.AddInParameter("gametype", DbType.String, criteria.GameType);
                objDb.AddInParameter("gametable", DbType.String, criteria.GameTable);
                objDb.AddOutParameter("TotalPlayers", DbType.Int32, 8);
                objDb.AddOutParameter("TotalGames", DbType.Int32, 8);
                objDb.AddOutParameter("GamingLevy", DbType.Double, 8);
                objDb.AddOutParameter("ProRated", DbType.Double, 8);
                DataSet ds = objDb.ExecuteDataSet();
                outputParam = objDb.GetOuputParams();
                if (ds != null)
                {
                    dataTable = ds.Tables[0];
                }
            }

            var result = new List<dynamic>();
            if (dataTable != null && dataTable.Rows.Count >= 1)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new
                    {
                        TableName = row["tablename"].ToString1(),
                        players = Convert.ToInt32(row["players"]),
                        games = Convert.ToInt32(row["games"]),
                        totalbets = Convert.ToDecimal(row["totalbets"]),
                        totalwins = Convert.ToDecimal(row["totalwins"]),
                        GrossIncome = Convert.ToDecimal(row["GrossIncome"]),
                        Comm = Convert.ToDecimal(row["Comm"]),
                        NetIncome = Convert.ToDecimal(row["NetIncome"])
                    });
                }
            }

            return result;
        }
    }
}