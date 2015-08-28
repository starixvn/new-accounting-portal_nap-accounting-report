// --------------------------------------------------------------------------
// <copyright file="DailyWinLoss2hRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyWinLoss2hRepository : IDailyWinLoss2hRepository
    {
        /// <summary>
        /// Gets the main2h report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The main2h report.</returns>
        public List<dynamic> GetMain2hReport(
            GetMain2hReportCriteria criteria,
            ref Hashtable outputParam)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
                outputParam = objectDb.GetOuputParams();
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        SportType = row["SportType"],
                        SportName = row["SportName"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        RealLineTotal = row["RealLineTotal"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        RealTurnover = row["RealTurnover"],
                        VirtualTurnover = row["VirtualTurnover"],
                        ibc = row["ibc"],
                        EnableVirtual = Converter.ToBool(Converter.ToString2(row["EnableVirtual"]).ToLower()) ?
                            false :
                            true,
                        Site = row["Site"],
                        DisplayOrder = Converter.ToInt(row["DisplayOrder"])
                    });
                }
            }

            return report;
        }

        /// <summary>
        /// Gets the casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The casino report.</returns>
        public List<dynamic> GetCasinoReport(
            GetCasinoReportCriteria criteria,
            ref Hashtable outputParam)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
                outputParam = objectDb.GetOuputParams();
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                bool isHavingHedgeColumn = false;
                if (dataTable.Columns.Contains("IsHedge"))
                {
                    isHavingHedgeColumn = true;
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        IsHedge = isHavingHedgeColumn ? Converter.ToString2(row["IsHedge"]) : string.Empty,
                        ProductId = row["ProductId"],
                        BetType = row["bettype"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        LineTotal = row["LineTotal"],
                        Turnover = row["Turnover"],
                        Site = row["Site"]
                    });
                }
            }

            return report;
        }

        /// <summary>
        /// Gets the system parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The system parlay report.</returns>
        public List<dynamic> GetSystemParlayReport(GetMixParlayReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        Status = row["Status"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        LineTotal = row["LineTotal"],
                        Turnover = row["Turnover"],
                        SportType = row["SportType"],
                        BetType = row["BetType"]
                    });
                }
            }

            return report;
        }

        /// <summary>
        /// Gets the summary number game report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The summary number game report.</returns>
        public List<dynamic> GetSummaryNumberGameReport(GetSummaryNumberGameReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                bool isHavingHedgeColumn = false;
                if (dataTable.Columns.Contains("IsHedge"))
                {
                    isHavingHedgeColumn = true;
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        IsHedge = isHavingHedgeColumn ? Converter.ToString2(row["IsHedge"]) : string.Empty,
                        bettype = row["bettype"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        RealLineTotal = row["RealLineTotal"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        RealTurnover = row["RealTurnover"],
                        VirtualTurnover = row["VirtualTurnover"],
                        liveindicator = row["liveindicator"],
                        Site = row["Site"]
                    });
                }
            }

            return report;
        }

        /// <summary>
        /// Gets the summary other sport report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The summary other sport report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetSummaryOtherSportReport(GetSummaryOtherSportReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> dataset = new List<dynamic>();
            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                bool isHavingHedgeColumn = dataTable.Columns.Contains("IsHedge");
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        IsHedge = isHavingHedgeColumn ? Converter.ToString2(row["IsHedge"]) : string.Empty,
                        SportType = row["SportType"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        RealLineTotal = row["RealLineTotal"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        RealTurnover = row["RealTurnover"],
                        VirtualTurnover = row["VirtualTurnover"],
                        liveindicator = row["liveindicator"],
                        Site = row["Site"]
                    });
                }
            }

            dataset.Add(report);
            List<dynamic> _8RMReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 2)
            {
                _8RMReport = this.Get8RMInfo(result.Tables[1], true);
            }

            dataset.Add(_8RMReport);
            List<dynamic> barmReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 3)
            {
                barmReport = this.GetBARMInfo(result.Tables[2], true);
            }

            dataset.Add(barmReport);
            return dataset;
        }

        /// <summary>
        /// Gets the other sport report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The other sport report by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetOtherSportReportBySport(GetOtherSportReportBySportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> dataset = new List<dynamic>();
            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                bool isHavingHedgeColumn = dataTable.Columns.Contains("IsHedge");
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        IsHedge = isHavingHedgeColumn ? Converter.ToString2(row["IsHedge"]) : string.Empty,
                        Bettype = row["Bettype"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        RealLineTotal = row["RealLineTotal"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        RealTurnover = row["RealTurnover"],
                        VirtualTurnover = row["VirtualTurnover"],
                        liveindicator = row["liveindicator"],
                        Site = row["Site"]
                    });
                }
            }

            dataset.Add(report);
            List<dynamic> _8RMReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 2)
            {
                _8RMReport = this.Get8RMInfo(result.Tables[1], true);
            }

            dataset.Add(_8RMReport);
            List<dynamic> barmReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 3)
            {
                barmReport = this.GetBARMInfo(result.Tables[2], true);
            }

            dataset.Add(barmReport);
            return dataset;
        }

        /// <summary>
        /// Gets the soccer report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The soccer report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetSoccerReport(GetSoccerReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> dataset = new List<dynamic>();
            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                bool isHavingHedgeColumn = dataTable.Columns.Contains("IsHedge");
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        IsHedge = isHavingHedgeColumn ? Converter.ToString2(row["IsHedge"]) : string.Empty,
                        Bettype = row["Bettype"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        RealLineTotal = row["RealLineTotal"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        RealTurnover = row["RealTurnover"],
                        VirtualTurnover = row["VirtualTurnover"],
                        liveindicator = row["liveindicator"],
                        Site = row["Site"]
                    });
                }
            }

            dataset.Add(report);
            List<dynamic> _8RMReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 2)
            {
                _8RMReport = this.Get8RMInfo(result.Tables[1], true);
            }

            dataset.Add(_8RMReport);
            List<dynamic> barmReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 3)
            {
                barmReport = this.GetBARMInfo(result.Tables[2], true);
            }

            dataset.Add(barmReport);
            return dataset;
        }

        /// <summary>
        /// Gets the bingo report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The bingo report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetBingoReport(GetBingoReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> dataset = new List<dynamic>();
            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                bool isHavingHedgeColumn = dataTable.Columns.Contains("IsHedge");
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        IsHedge = isHavingHedgeColumn ? Converter.ToString2(row["IsHedge"]) : string.Empty,
                        Bettype = row["Bettype"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        LineTotal = row["LineTotal"],
                        Turnover = row["Turnover"],
                        liveindicator = false,
                        Site = row["Site"]
                    });
                }
            }

            dataset.Add(report);
            List<dynamic> _8RMReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 2)
            {
                _8RMReport = this.Get8RMInfo(result.Tables[1], true);
            }

            dataset.Add(_8RMReport);
            List<dynamic> barmReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 3)
            {
                barmReport = this.GetBARMInfo(result.Tables[2], true);
            }

            dataset.Add(barmReport);
            return dataset;
        }

        /// <summary>
        /// Gets the virtual sport report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The virtual sport report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetVirtualSportReport(GetVirtualSportReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> dataset = new List<dynamic>();
            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                bool isHavingHedgeColumn = dataTable.Columns.Contains("IsHedge");
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        IsHedge = isHavingHedgeColumn ? Converter.ToString2(row["IsHedge"]) : string.Empty,
                        SportType = row["SportType"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        LineTotal = row["LineTotal"],
                        Turnover = row["Turnover"],
                        Site = row["Site"]
                    });
                }
            }

            dataset.Add(report);
            List<dynamic> _8RMReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 2)
            {
                _8RMReport = this.Get8RMInfo(result.Tables[1], false);
            }

            dataset.Add(_8RMReport);
            List<dynamic> barmReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 3)
            {
                barmReport = this.GetBARMInfo(result.Tables[2], false);
            }

            dataset.Add(barmReport);
            return dataset;
        }

        /// <summary>
        /// Gets the virtual sport report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The virtual sport report by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetVirtualSportReportBySport(GetVirtualSportReportBySportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> dataset = new List<dynamic>();
            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                bool isHavingHedgeColumn = dataTable.Columns.Contains("IsHedge");
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        IsHedge = isHavingHedgeColumn ? Converter.ToString2(row["IsHedge"]) : string.Empty,
                        Bettype = row["Bettype"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        LineTotal = row["LineTotal"],
                        Turnover = row["Turnover"],
                        Site = row["Site"]
                    });
                }
            }

            dataset.Add(report);
            List<dynamic> _8RMReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 2)
            {
                _8RMReport = this.Get8RMInfo(result.Tables[1], false);
            }

            dataset.Add(_8RMReport);
            List<dynamic> barmReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 3)
            {
                barmReport = this.GetBARMInfo(result.Tables[2], false);
            }

            dataset.Add(barmReport);
            return dataset;
        }

        /// <summary>
        /// Gets the report by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report by bet type.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetReportByBetType(
            GetReportByBetTypeCriteria criteria)
        {
            List<dynamic> dataSet = new List<dynamic>();
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        leagueid = row["leagueid"],
                        matchid = row["matchid"],
                        matchcode = row["matchcode"],
                        HomeId = row["HomeId"],
                        AwayId = row["AwayId"],
                        liveindicator = row["liveindicator"],
                        bType = row["bType"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        RealLineTotal = row["RealLineTotal"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        RealTurnover = row["RealTurnover"],
                        VirtualTurnover = row["VirtualTurnover"],
                        IsNeutral = row["IsNeutral"]
                    });
                }
            }

            dataSet.Add(report);

            List<dynamic> _8RMReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 2)
            {
                _8RMReport = this.Get8RMInfo(result.Tables[1], false);
            }

            dataSet.Add(_8RMReport);
            List<dynamic> barmReport = new List<dynamic>();
            if (result != null && result.Tables.Count >= 3)
            {
                barmReport = this.GetBARMInfo(result.Tables[2], false);
            }

            dataSet.Add(barmReport);
            return dataSet;
        }

        /// <summary>
        /// Gets the parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The parlay report data.</returns>
        public List<dynamic> GetParlayReport(GetParlayReportCriteria criteria)
        {
            List<dynamic> parlayReport = new List<dynamic>();

            string settingKey = criteria.GetSettingKey();

            using (var objectDb = new ObjectDb(settingKey))
            {
                var _params = criteria.GetSpParams();
                IDataReader reader = objectDb.ExecuteReader(_params);

                while (reader.Read())
                {
                    parlayReport.Add(new
                    {
                        DisplayOrder = Converter.ToInt(reader["DisplayOrder"]),
                        SportName = Converter.ToString2(reader["SportName"]),
                        Site = Converter.ToInt(reader["site"]),
                        Bettype = Converter.ToInt(reader["bettype"]),
                        Winlost = Converter.ToDecimal(reader["Winlost"]),
                        Position = Converter.ToDecimal(reader["Position"]),
                        Commission = Converter.ToDecimal(reader["Commission"]),
                        RealLineTotal = Converter.ToDecimal(reader["RealLineTotal"]),
                        VirtualLineTotal = Converter.ToDecimal(reader["VirtualLineTotal"]),
                        RealTurnover = Converter.ToDecimal(reader["RealTurnover"]),
                        VirtualTurnover = Converter.ToDecimal(reader["VirtualTurnover"]),
                        EnableVirtual = Converter.ToInt(reader["EnableVirtual"])
                    });
                }

                reader.Close();
            }

            return parlayReport;
        }

        public ProductWinLoss GetMainLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            var winLosses = new ProductWinLoss();

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var _params = criteria.GetSpParams();
                IDataReader reader = objectDb.ExecuteReader(_params);

                while (reader.Read())
                {
                    winLosses.ReportRows.Add(new ProductWinLoss
                    {
                        ProductId = Converter.ToInt(reader["ProductID"]),
                        Bettype = Converter.ToInt(reader["bettype"]),
                        Site = Converter.ToInt(reader["site"]),
                        WinLost = Converter.ToDecimal(reader["WinLost"]),
                        Position = Converter.ToDecimal(reader["Position"]),
                        Commission = Converter.ToDecimal(reader["Commission"]),
                        LineTotal = Converter.ToDecimal(reader["LineTotal"]),
                        Turnover = Converter.ToDecimal(reader["Turnover"])
                    });
                }

                reader.Close();
            }

            return winLosses;
        }

        /// <summary>
        /// Gets the 8RM information.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="getLiveIndicator">If set to <c>true</c> [get live indicator].</param>
        /// <returns>The 8RM information.</returns>
        private List<dynamic> Get8RMInfo(DataTable dataTable, bool getLiveIndicator)
        {
            List<dynamic> _8RMReport = new List<dynamic>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    _8RMReport.Add(new
                    {
                        liveindicator = getLiveIndicator ? row["liveindicator"] : false,
                        WinLost8RM = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        VirtualTurnover = row["VirtualTurnover"],
                        RealTurnover = row["RealTurnover"],
                        RealLineTotal = row["RealLineTotal"]
                    });
                }
            }

            return _8RMReport;
        }

        /// <summary>
        /// Gets the BARM information.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="getLiveIndicator">If set to <c>true</c> [get live indicator].</param>
        /// <returns>The BARM information.</returns>
        private List<dynamic> GetBARMInfo(DataTable dataTable, bool getLiveIndicator)
        {
            List<dynamic> barmReport = new List<dynamic>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    barmReport.Add(new
                    {
                        liveindicator = getLiveIndicator ? row["liveindicator"] : false,
                        WinLostBARM = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        VirtualTurnover = row["VirtualTurnover"],
                        RealTurnover = row["RealTurnover"],
                        RealLineTotal = row["RealLineTotal"]
                    });
                }
            }

            return barmReport;
        }
    }
}