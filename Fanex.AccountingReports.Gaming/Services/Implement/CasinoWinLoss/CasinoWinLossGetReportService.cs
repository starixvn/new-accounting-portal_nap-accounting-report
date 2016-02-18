// -----------------------------------------------------------------------
// <copyright file="CasinoWinLossGetReportService.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoWinLoss
{
    using System;
    using System.Data;
    using System.Linq;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Models.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoWinLoss;
    using Fanex.Common;

    /// <summary>
    /// The get report service of Casino Win Lost report.
    /// </summary>
    public class CasinoWinLossGetReportService : ServiceBase, ICasinoWinLossGetReportService
    {
        private const string CasinoWinLossPrefixSessionKey = "Fanex.AccountingReports.Gaming.CasinoWinLoss.";

        private string _casinoWinLossSessionKey = string.Empty;

        private ICasinoWinLossRepository _casinoWinLossRepository = null;

        public CasinoWinLossGetReportService()
        {
            if (AppSession.CurrentUser != null)
            {
                _casinoWinLossSessionKey = CasinoWinLossPrefixSessionKey + AppSession.CurrentUser.Username;
            }
            else
            {
                _casinoWinLossSessionKey = CasinoWinLossPrefixSessionKey;
            }

            _casinoWinLossRepository = new CasinoWinLossRepository();
        }

        /// <summary>
        /// Get report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The CasinoWinLoss report.</returns>
        public CasinoWinLoss GetReport(GetCasinoWinLossCriteria criteria)
        {
            try
            {
                CasinoWinLoss report = new CasinoWinLoss();

                DataTable dataTable = _casinoWinLossRepository.GetData(criteria);
                report = ConvertData(report, dataTable);

                SessionManager.Set<CasinoWinLoss>(_casinoWinLossSessionKey, report);

                return report;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Convert data.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="dataTable">The data from database.</param>
        /// <returns>The CasinoWinLoss report.</returns>
        private CasinoWinLoss ConvertData(CasinoWinLoss report, DataTable dataTable)
        {
            report = GroupByBetType(report, dataTable); 
            report = GetTotalCurrency(report, dataTable); 

            return report;
        }

        /// <summary>
        /// Group by bet type.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="dataTable">The data from database.</param>
        /// <returns>The CasinoWinLoss report.</returns>
        private CasinoWinLoss GroupByBetType(CasinoWinLoss report, DataTable dataTable)
        {
            var betTypeList = dataTable.AsEnumerable().Select(row => row["bettype"]).Distinct().ToList();

            foreach (var betTypeItem in betTypeList)
            {
                var betTypeId = Converter.ToInt(betTypeItem);
                var betType = new CasinoWinLoss();
                betType.Name = GetBetTypeName(betTypeId);

                var currenciesOfBetType = dataTable.AsEnumerable().Where(row => Converter.ToInt(row["bettype"]) == Converter.ToInt(betTypeItem)).ToList();

                foreach (var currency in currenciesOfBetType)
                {
                    var winLost = new CasinoWinLoss
                    {
                        Name = Converter.ToString2(currency["currencyName"]),
                        Turnover = Converter.ToDecimal(currency["Turnover"]),
                        WinLost = Converter.ToDecimal(currency["winlost"]),
                        Position = Converter.ToDecimal(currency["pos"]),
                        Commission = Converter.ToDecimal(currency["comm"]),
                        LineTotal = Converter.ToDecimal(currency["LineTotal"])
                    };

                    betType.Rows.Add(winLost);
                    betType.SumTotal(winLost);
                }

                report.Rows.Add(betType);
                report.SumTotal(betType);
            }

            return report;
        }

        /// <summary>
        /// Get bet type name.
        /// </summary>
        /// <param name="betTypeId">The bet type id.</param>
        /// <returns>The bet type name.</returns>
        private string GetBetTypeName(int betTypeId)
        {
            string betTypeName = string.Empty;

            switch (betTypeId)
            {
                case 71:
                    betTypeName = Labels.CasinoGames;
                    break;
            }

            return betTypeName;
        }

        /// <summary>
        /// Get total of each currency.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="dataTable">The data from database.</param>
        /// <returns>The CasinoWinLoss report.</returns>
        private CasinoWinLoss GetTotalCurrency(CasinoWinLoss report, DataTable dataTable)
        {
            var currencies = dataTable.AsEnumerable().Select(row => row["currency"]).Distinct().ToList();
            var totalCurrency = new CasinoWinLoss();
            totalCurrency.Name = Labels.Total;

            foreach (var currency in currencies)
            {
                var rowsOfCurrency = dataTable.AsEnumerable().Where(row => Converter.ToInt(row["currency"]) == Converter.ToInt(currency)).ToList();
                var currencyItem = new CasinoWinLoss();

                foreach (var row in rowsOfCurrency)
                {
                    var winLost = new CasinoWinLoss
                    {
                        Name = Converter.ToString2(row["currencyName"]),
                        Turnover = Converter.ToDecimal(row["Turnover"]),
                        WinLost = Converter.ToDecimal(row["winlost"]),
                        Position = Converter.ToDecimal(row["pos"]),
                        Commission = Converter.ToDecimal(row["comm"]),
                        LineTotal = Converter.ToDecimal(row["LineTotal"])
                    };

                    currencyItem.SumTotal(winLost);
                    currencyItem.Name = winLost.Name;
                }

                totalCurrency.Rows.Add(currencyItem);
            }

            report.Rows.Add(totalCurrency);

            return report;
        }
    }
}
