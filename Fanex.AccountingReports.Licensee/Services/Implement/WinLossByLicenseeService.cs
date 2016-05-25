// --------------------------------------------------------------------------
// <copyright file="WinLossByLicenseeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee;
    using Fanex.AccountingReports.Licensee.Models.WinLossByLicensee;
    using Fanex.AccountingReports.Licensee.Repositories.Implement;
    using Fanex.AccountingReports.Licensee.Repositories.Interface;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Common;
using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByLicenseeService : ServiceBase, IWinLossByLicenseeService
    {
        private Dictionary<int, string> _exchangeCurrencies = ReportService.ExchangeCurrencies;
        private IWinLossByLicenseeRepository _repository = new WinLossByLicenseeRepository();

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        private enum ReportDrilldownLevel
        {
            /// <summary>
            /// The currency type.
            /// </summary>
            Currency = 1,

            /// <summary>
            /// The level type.
            /// </summary>
            Level = 2
        }

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        private enum ReportRowType
        {
            /// <summary>
            /// The normal row.
            /// </summary>
            NormalRow = 1,

            /// <summary>
            /// The total row.
            /// </summary>
            Total = 2,

            /// <summary>
            /// The grand total row.
            /// </summary>
            GrandTotal = 3
        }

        /// <summary>
        /// Get win loss by licensee report, currency level.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Win loss by licensee report, currency level.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossByLicensee GetCurrencyReport(GetCurrencyReportCriteria criteria)
        {
            try
            {
                var rawData = _repository.GetCurrencyReport(criteria);

                WinLossByLicensee currencyReport = new WinLossByLicensee
                {
                    ReportItemLength = rawData.Rows.Count,
                    TransactionList = new List<WinLossByLicensee>(),
                    Total = new List<WinLossByLicensee>(),
                    BaseCurrencyTotal = new List<WinLossByLicensee>()
                };

                Dictionary<int, WinLossByLicensee> totalByCurrency = new Dictionary<int, WinLossByLicensee>();
                Dictionary<string, WinLossByLicensee> baseCurrencyTotal = this.CreateBaseCurrencyList();
                var startIndex = criteria.StartIndex;
                var endIndex = criteria.EndIndex;
                var index = 0;

                rawData.AsEnumerable().GroupBy(data => data["ReportType"])
                       .ToList()
                       .ForEach(group =>
                       {
                           var winLossByDateGroup = new WinLossByLicensee
                           {
                               TransactionList = new List<WinLossByLicensee>()
                           };

                           group.ToList()
                                .ForEach(winLossByLicensee =>
                                {
                                    var wlData = GetWinLossDataByConvertCurrency(winLossByLicensee, criteria);

                                    // Insert row if get all data for exporting or in paging range
                                    if (criteria.IsGetAll || (startIndex <= index && index <= endIndex))
                                    {
                                        winLossByDateGroup.TransactionList.Add(wlData);
                                    }

                                    CalculateTotal(winLossByLicensee, wlData, ref totalByCurrency, ref baseCurrencyTotal);
                                    index++;
                                });
                           if (winLossByDateGroup.TransactionList.Count > 0)
                           {
                               winLossByDateGroup.BettingDate = winLossByDateGroup.TransactionList[0].BettingDate;
                               currencyReport.TransactionList.Add(winLossByDateGroup);
                           }
                       });

                this.AssignTotalData(ref currencyReport, totalByCurrency, baseCurrencyTotal);
                return currencyReport;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Get win loss by licensee report, super level.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Win loss by licensee report, super level.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossByLicensee GetLevelReport(GetLevelReportByCustIdCriteria criteria)
        {
            try
            {
                var resultCriteria = GetLevelCriteria(criteria);

                var rawData = _repository.GetLevelReport(resultCriteria).AsEnumerable().OrderBy(s => s["username"]);

                WinLossByLicensee levelReport = new WinLossByLicensee
                {
                    ReportItemLength = rawData.Count(),
                    TransactionList = new List<WinLossByLicensee>(),
                    Total = new List<WinLossByLicensee>(),
                    BaseCurrencyTotal = new List<WinLossByLicensee>()
                };

                Dictionary<int, WinLossByLicensee> totalByCurrency = new Dictionary<int, WinLossByLicensee>();
                Dictionary<string, WinLossByLicensee> baseCurrencyTotal = this.CreateBaseCurrencyList();
                var startIndex = criteria.StartIndex;
                var endIndex = criteria.EndIndex;
                var index = 0;

                rawData.ToList().ForEach(winLossByLicensee =>
                {
                    var wlData = GetWinLossDataByConvertCurrency(winLossByLicensee, criteria);

                    // Insert row if get all data for exporting or in paging range
                    if (criteria.IsGetAll || (startIndex <= index && index <= endIndex))
                    {
                        levelReport.TransactionList.Add(wlData);
                    }

                    CalculateTotal(winLossByLicensee, wlData, ref totalByCurrency, ref baseCurrencyTotal);
                    index++;
                });

                this.AssignTotalData(ref levelReport, totalByCurrency, baseCurrencyTotal);
                return levelReport;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public CriteriaBase GetLevelCriteria(GetLevelReportByCustIdCriteria criteria)
        {
            switch (criteria.LevelReport)
            {
                case 1:
                    return new WinLossBySuperCriteria
                    {
                        LicenseeID = criteria.LicenseeID,
                        Currency = criteria.Currency,
                        FromDate = criteria.FromDate,
                        ToDate = criteria.ToDate,
                        ShowSB = criteria.ShowSB,
                        ShowRB = criteria.ShowRB,
                        ShowNG = criteria.ShowNG,
                        ShowBA = criteria.ShowBA,
                        ShowLC = criteria.ShowLC,
                        ShowVS = criteria.ShowVS,
                        ShowCS = criteria.ShowCS,
                        ShowKN = criteria.ShowKN,
                        ShowMGC = criteria.ShowMGC,
                        ShowAGC = criteria.ShowAGC,
                        ShowGD = criteria.ShowGD,
                        Drawfilter = criteria.Drawfilter
                    };
                case 2:
                    return new WinLossByMasterCriteria
                    {
                        CustID = criteria.CustID,
                        LicenseeID = criteria.LicenseeID,
                        Currency = criteria.Currency,
                        FromDate = criteria.FromDate,
                        ToDate = criteria.ToDate,
                        ShowSB = criteria.ShowSB,
                        ShowRB = criteria.ShowRB,
                        ShowNG = criteria.ShowNG,
                        ShowBA = criteria.ShowBA,
                        ShowLC = criteria.ShowLC,
                        ShowVS = criteria.ShowVS,
                        ShowCS = criteria.ShowCS,
                        ShowKN = criteria.ShowKN,
                        ShowMGC = criteria.ShowMGC,
                        ShowAGC = criteria.ShowAGC,
                        ShowGD = criteria.ShowGD,
                        Drawfilter = criteria.Drawfilter
                    };
                case 3:
                    return new WinLossByAgentCriteria
                    {
                        CustID = criteria.CustID,
                        LicenseeID = criteria.LicenseeID,
                        Currency = criteria.Currency,
                        FromDate = criteria.FromDate,
                        ToDate = criteria.ToDate,
                        ShowSB = criteria.ShowSB,
                        ShowRB = criteria.ShowRB,
                        ShowNG = criteria.ShowNG,
                        ShowBA = criteria.ShowBA,
                        ShowLC = criteria.ShowLC,
                        ShowVS = criteria.ShowVS,
                        ShowCS = criteria.ShowCS,
                        ShowKN = criteria.ShowKN,
                        ShowMGC = criteria.ShowMGC,
                        ShowAGC = criteria.ShowAGC,
                        ShowGD = criteria.ShowGD,
                        Drawfilter = criteria.Drawfilter
                    };

                case 4:
                    return new WinLossByMemberCriteria
                    {
                        CustID = criteria.CustID,
                        LicenseeID = criteria.LicenseeID,
                        Currency = criteria.Currency,
                        FromDate = criteria.FromDate,
                        ToDate = criteria.ToDate,
                        ShowSB = criteria.ShowSB,
                        ShowRB = criteria.ShowRB,
                        ShowNG = criteria.ShowNG,
                        ShowBA = criteria.ShowBA,
                        ShowLC = criteria.ShowLC,
                        ShowVS = criteria.ShowVS,
                        ShowCS = criteria.ShowCS,
                        ShowKN = criteria.ShowKN,
                        ShowMGC = criteria.ShowMGC,
                        ShowAGC = criteria.ShowAGC,
                        ShowGD = criteria.ShowGD,
                        Drawfilter = criteria.Drawfilter
                    };
            }

            return criteria;
        }

        public void ExportCurrencyReport(GetCurrencyReportCriteria criteria)
        {
            try
            {
                criteria.IsGetAll = true;
                var currencyReport = this.GetCurrencyReport(criteria);

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = LicenseeResources.WinLossByLicensee_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                criteria.LicenseeName = criteria.LicenseeName.Replace("&amp;", "&");
                this.ExportReportHeader(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    criteria,
                    ReportDrilldownLevel.Currency);

                this.ExportCurrencyReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    currencyReport);

                //// MakeExcelFile

                excelFile.ReponseStream(string.Format(
                    LicenseeResources.WinLossByLicensee_ReportName,
                    criteria.LicenseeName.Replace(" ", "_").Replace("&", string.Empty),
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_')));
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the level report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportLevelReport(GetLevelReportByCustIdCriteria criteria)
        {
            try
            {
                criteria.IsGetAll = true;
                var levelReport = this.GetLevelReport(criteria);

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = LicenseeResources.WinLossByLicensee_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                criteria.LicenseeName = criteria.LicenseeName.Replace("&amp;", "&");
                this.ExportReportHeader(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    criteria,
                    ReportDrilldownLevel.Level);

                this.ExportLevelReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    levelReport);

                //// MakeExcelFile

                excelFile.ReponseStream(string.Format(
                    LicenseeResources.WinLossByLicensee_ReportName,
                    criteria.LicenseeName.Replace(" ", "_").Replace("&", string.Empty),
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_')));
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Get win loss data.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>
        /// Win loss data.
        /// </returns>
        private WinLossByLicensee GetWinLossData(DataRow rawData)
        {
            return new WinLossByLicensee
            {
                Turnover = Converter.ToDecimal(rawData["Turnover"]),

                CustomerWinLoss = Converter.ToDecimal(rawData["Winlost"]),
                CustomerComm = Converter.ToDecimal(rawData["PlayerComm"]),

                LicenseeWinLoss = Converter.ToDecimal(rawData["LicenseeWL"]),
                BAComm = Converter.ToDecimal(rawData["CommBA"]),

                RacingWinLoss = Converter.ToDecimal(rawData["WinlossRB"]),
                RacingComm = Converter.ToDecimal(rawData["CommRB"]),

                AmtDuetoBA = Converter.ToDecimal(rawData["AmtDuetoBA"]),
                AmtDuetoRacing = Converter.ToDecimal(rawData["AmtDuetoRB"]),
                AmtDuetoCompany = Converter.ToDecimal(rawData["Company"])
            };
        }

        /// <summary>
        /// Get winLoss data by MYR currency.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>
        /// WinLoss data by MYR currency.
        /// </returns>
        private WinLossByLicensee GetWinLossDataByMYRCurrency(DataRow rawData)
        {
            return new WinLossByLicensee
            {
                Turnover = Converter.ToDecimal(rawData["TurnoverMYR"]),

                CustomerWinLoss = Converter.ToDecimal(rawData["WinlostMYR"]),
                CustomerComm = Converter.ToDecimal(rawData["PlayerCommMYR"]),

                LicenseeWinLoss = Converter.ToDecimal(rawData["LicenseeWLMYR"]),
                BAComm = Converter.ToDecimal(rawData["CommBAMYR"]),

                RacingWinLoss = Converter.ToDecimal(rawData["WinlossRBMYR"]),
                RacingComm = Converter.ToDecimal(rawData["CommRBMYR"]),

                AmtDuetoBA = Converter.ToDecimal(rawData["AmtDuetoBAMYR"]),
                AmtDuetoRacing = Converter.ToDecimal(rawData["AmtDuetoRBMYR"]),
                AmtDuetoCompany = Converter.ToDecimal(rawData["CompanyMYR"])
            };
        }

        /// <summary>
        /// Get winLoss data by EUR currency.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>
        /// WinLoss data by EUR currency.
        /// </returns>
        private WinLossByLicensee GetWinLossDataByEURCurrency(DataRow rawData)
        {
            return new WinLossByLicensee
            {
                Turnover = Converter.ToDecimal(rawData["TurnoverEUR"]),

                CustomerWinLoss = Converter.ToDecimal(rawData["WinlostEUR"]),
                CustomerComm = Converter.ToDecimal(rawData["PlayerCommEUR"]),

                LicenseeWinLoss = Converter.ToDecimal(rawData["LicenseeWLEUR"]),
                BAComm = Converter.ToDecimal(rawData["CommBAEUR"]),

                RacingWinLoss = Converter.ToDecimal(rawData["WinlossRBEUR"]),
                RacingComm = Converter.ToDecimal(rawData["CommRBEUR"]),

                AmtDuetoBA = Converter.ToDecimal(rawData["AmtDuetoBAEUR"]),
                AmtDuetoRacing = Converter.ToDecimal(rawData["AmtDuetoRBEUR"]),
                AmtDuetoCompany = Converter.ToDecimal(rawData["CompanyEUR"])
            };
        }

        /// <summary>
        /// Get winLoss data by USD currency.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>
        /// WinLoss data by USD currency.
        /// </returns>
        private WinLossByLicensee GetWinLossDataByUSDCurrency(DataRow rawData)
        {
            return new WinLossByLicensee
            {
                Turnover = Converter.ToDecimal(rawData["TurnoverUSD"]),

                CustomerWinLoss = Converter.ToDecimal(rawData["WinlostUSD"]),
                CustomerComm = Converter.ToDecimal(rawData["PlayerCommUSD"]),

                LicenseeWinLoss = Converter.ToDecimal(rawData["LicenseeWLUSD"]),
                BAComm = Converter.ToDecimal(rawData["CommBAUSD"]),

                RacingWinLoss = Converter.ToDecimal(rawData["WinlossRBUSD"]),
                RacingComm = Converter.ToDecimal(rawData["CommRBUSD"]),

                AmtDuetoBA = Converter.ToDecimal(rawData["AmtDuetoBAUSD"]),
                AmtDuetoRacing = Converter.ToDecimal(rawData["AmtDuetoRBUSD"]),
                AmtDuetoCompany = Converter.ToDecimal(rawData["CompanyUSD"])
            };
        }

        /// <summary>
        /// Get winLoss data by GBP currency.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>
        /// WinLoss data by GBP currency.
        /// </returns>
        private WinLossByLicensee GetWinLossDataByGBPCurrency(DataRow rawData)
        {
            return new WinLossByLicensee
            {
                Turnover = Converter.ToDecimal(rawData["TurnoverGBP"]),

                CustomerWinLoss = Converter.ToDecimal(rawData["WinlostGBP"]),
                CustomerComm = Converter.ToDecimal(rawData["PlayerCommGBP"]),

                LicenseeWinLoss = Converter.ToDecimal(rawData["LicenseeWLGBP"]),
                BAComm = Converter.ToDecimal(rawData["CommBAGBP"]),

                RacingWinLoss = Converter.ToDecimal(rawData["WinlossRBGBP"]),
                RacingComm = Converter.ToDecimal(rawData["CommRBGBP"]),

                AmtDuetoBA = Converter.ToDecimal(rawData["AmtDuetoBAGBP"]),
                AmtDuetoRacing = Converter.ToDecimal(rawData["AmtDuetoRBGBP"]),
                AmtDuetoCompany = Converter.ToDecimal(rawData["CompanyGBP"])
            };
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private WinLossByLicensee GetWinLossDataByConvertCurrency(DataRow rawData, GetCurrencyReportCriteria criteria)
        {
            WinLossByLicensee winLossData = null;

            CurrencyContants convertToCurrency = (CurrencyContants)criteria.ConvertToCurrency;
            switch (convertToCurrency)
            {
                case CurrencyContants.All:
                    winLossData = this.GetWinLossData(rawData);
                    break;

                case CurrencyContants.MYR:
                    winLossData = this.GetWinLossDataByMYRCurrency(rawData);
                    break;

                case CurrencyContants.EUR:
                    winLossData = this.GetWinLossDataByEURCurrency(rawData);
                    break;

                case CurrencyContants.USD:
                    winLossData = this.GetWinLossDataByUSDCurrency(rawData);
                    break;

                case CurrencyContants.GBP:
                    winLossData = this.GetWinLossDataByGBPCurrency(rawData);
                    break;
            }

            winLossData.BetCount = Converter.ToInt(rawData["Betcount"]);
            winLossData.CurrencyId = Converter.ToInt(rawData["Currency"]);
            winLossData.CurrencyName = Converter.ToString2(rawData["currencyName"]);

            DateTime bettingDate = this.GetBettingDate(rawData["ReportType"].ToString(), criteria.ReportType);
            switch (criteria.ReportType)
            {
                case (int)ReportTimeType.Daily:
                    winLossData.BettingDate = bettingDate.ToShortFormatedDateWithSlash();
                    break;

                case (int)ReportTimeType.Monthly:
                    winLossData.BettingDate = bettingDate.ToMonthYearWithSlash();
                    winLossData.FromDate = bettingDate.FirstDayOfMonth() < criteria.FromDate
                        ? criteria.FromDate.ToShortFormatedDate()
                        : bettingDate.FirstDayOfMonth().ToShortFormatedDate();
                    winLossData.ToDate = criteria.ToDate < bettingDate.LastDayOfMonth()
                        ? criteria.ToDate.ToShortFormatedDate()
                        : bettingDate.LastDayOfMonth().ToShortFormatedDate();
                    break;
            }

            return winLossData;
        }

        /// <summary>
        /// Gets the win loss data by convert currency.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The win loss data by convert currency.</returns>
        private WinLossByLicensee GetWinLossDataByConvertCurrency(DataRow rawData, GetLevelReportByCustIdCriteria criteria)
        {
            WinLossByLicensee winLossData = null;

            var currencies = _exchangeCurrencies;

            CurrencyContants convertToCurrency = (CurrencyContants)criteria.ConvertToCurrency;
            switch (convertToCurrency)
            {
                case CurrencyContants.All:
                    winLossData = this.GetWinLossData(rawData);
                    break;

                case CurrencyContants.MYR:
                    winLossData = this.GetWinLossDataByMYRCurrency(rawData);
                    break;

                case CurrencyContants.EUR:
                    winLossData = this.GetWinLossDataByEURCurrency(rawData);
                    break;

                case CurrencyContants.USD:
                    winLossData = this.GetWinLossDataByUSDCurrency(rawData);
                    break;

                case CurrencyContants.GBP:
                    winLossData = this.GetWinLossDataByGBPCurrency(rawData);
                    break;
            }

            winLossData.UserId = Converter.ToInt(rawData["custid"]);
            winLossData.UserName = rawData["username"].ToString();

            if (criteria.Currency == 0)
            {
                winLossData.CurrencyId = Converter.ToInt(rawData["Currency"]);
                winLossData.CurrencyName = currencies[winLossData.CurrencyId];
            }
            else
            {
                winLossData.CurrencyId = criteria.Currency;
                winLossData.CurrencyName = criteria.CurrencyName;
            }

            winLossData.BetCount = Converter.ToInt(rawData["Betcount"]);
            return winLossData;
        }

        /// <summary>
        /// CalculateTotal: sum win loss data to total and base currency total.
        /// </summary>
        /// <param name="rawData">Dynamic object contain all win loss data return from data.</param>
        /// <param name="winLossData">The win loss data.</param>
        /// <param name="totalByCurrency">The total by currency.</param>
        /// <param name="baseCurrencyTotal">The base currency total.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void CalculateTotal(
            DataRow rawData,
            WinLossByLicensee winLossData,
            ref Dictionary<int, WinLossByLicensee> totalByCurrency,
            ref Dictionary<string, WinLossByLicensee> baseCurrencyTotal)
        {
            var currencies = _exchangeCurrencies;
            //// CalculateTotal

            if (!totalByCurrency.ContainsKey(winLossData.CurrencyId))
            {
                totalByCurrency.Add(
                    winLossData.CurrencyId,
                    new WinLossByLicensee
                    {
                        CurrencyId = winLossData.CurrencyId,
                        CurrencyName = currencies[winLossData.CurrencyId]
                    });
            }

            var totalRow = totalByCurrency[winLossData.CurrencyId];
            totalRow.BetCount += winLossData.BetCount;
            totalRow.Turnover += winLossData.Turnover;

            totalRow.CustomerWinLoss += winLossData.CustomerWinLoss;
            totalRow.CustomerComm += winLossData.CustomerComm;

            totalRow.LicenseeWinLoss += winLossData.LicenseeWinLoss;
            totalRow.BAComm += winLossData.BAComm;

            totalRow.RacingWinLoss += winLossData.RacingWinLoss;
            totalRow.RacingComm += winLossData.RacingComm;

            totalRow.AmtDuetoBA += winLossData.AmtDuetoBA;
            totalRow.AmtDuetoRacing += winLossData.AmtDuetoRacing;
            totalRow.AmtDuetoCompany += winLossData.AmtDuetoCompany;

            totalByCurrency[winLossData.CurrencyId] = totalRow;

            //// CalculateTotal
            //// CalculateBaseCurrencyTotal

            var myrTotal = baseCurrencyTotal[Labels.MYR];
            myrTotal.BetCount += Converter.ToInt(rawData["Betcount"]);
            myrTotal.Turnover += Converter.ToDecimal(rawData["TurnoverMYR"]);
            myrTotal.CustomerWinLoss += Converter.ToDecimal(rawData["WinlostMYR"]);
            myrTotal.CustomerComm += Converter.ToDecimal(rawData["PlayerCommMYR"]);
            myrTotal.LicenseeWinLoss += Converter.ToDecimal(rawData["LicenseeWLMYR"]);
            myrTotal.BAComm += Converter.ToDecimal(rawData["CommBAMYR"]);
            myrTotal.RacingWinLoss += Converter.ToDecimal(rawData["WinlossRBMYR"]);
            myrTotal.RacingComm += Converter.ToDecimal(rawData["CommRBMYR"]);
            myrTotal.AmtDuetoBA += Converter.ToDecimal(rawData["AmtDuetoBAMYR"]);
            myrTotal.AmtDuetoRacing += Converter.ToDecimal(rawData["AmtDuetoRBMYR"]);
            myrTotal.AmtDuetoCompany += Converter.ToDecimal(rawData["CompanyMYR"]);
            baseCurrencyTotal[Labels.MYR] = myrTotal;

            var eurTotal = baseCurrencyTotal[Labels.EUR];
            eurTotal.BetCount += Converter.ToInt(rawData["Betcount"]);
            eurTotal.Turnover += Converter.ToDecimal(rawData["TurnoverEUR"]);
            eurTotal.CustomerWinLoss += Converter.ToDecimal(rawData["WinlostEUR"]);
            eurTotal.CustomerComm += Converter.ToDecimal(rawData["PlayerCommEUR"]);
            eurTotal.LicenseeWinLoss += Converter.ToDecimal(rawData["LicenseeWLEUR"]);
            eurTotal.BAComm += Converter.ToDecimal(rawData["CommBAEUR"]);
            eurTotal.RacingWinLoss += Converter.ToDecimal(rawData["WinlossRBEUR"]);
            eurTotal.RacingComm += Converter.ToDecimal(rawData["CommRBEUR"]);
            eurTotal.AmtDuetoBA += Converter.ToDecimal(rawData["AmtDuetoBAEUR"]);
            eurTotal.AmtDuetoRacing += Converter.ToDecimal(rawData["AmtDuetoRBEUR"]);
            eurTotal.AmtDuetoCompany += Converter.ToDecimal(rawData["CompanyEUR"]);
            baseCurrencyTotal[Labels.EUR] = eurTotal;

            var usdTotal = baseCurrencyTotal[Labels.USD];
            usdTotal.BetCount += Converter.ToInt(rawData["Betcount"]);
            usdTotal.Turnover += Converter.ToDecimal(rawData["TurnoverUSD"]);
            usdTotal.CustomerWinLoss += Converter.ToDecimal(rawData["WinlostUSD"]);
            usdTotal.CustomerComm += Converter.ToDecimal(rawData["PlayerCommUSD"]);
            usdTotal.LicenseeWinLoss += Converter.ToDecimal(rawData["LicenseeWLUSD"]);
            usdTotal.BAComm += Converter.ToDecimal(rawData["CommBAUSD"]);
            usdTotal.RacingWinLoss += Converter.ToDecimal(rawData["WinlossRBUSD"]);
            usdTotal.RacingComm += Converter.ToDecimal(rawData["CommRBUSD"]);
            usdTotal.AmtDuetoBA += Converter.ToDecimal(rawData["AmtDuetoBAUSD"]);
            usdTotal.AmtDuetoRacing += Converter.ToDecimal(rawData["AmtDuetoRBUSD"]);
            usdTotal.AmtDuetoCompany += Converter.ToDecimal(rawData["CompanyUSD"]);
            baseCurrencyTotal[Labels.USD] = usdTotal;

            var gbpTotal = baseCurrencyTotal[Labels.GBP];
            gbpTotal.BetCount += Converter.ToInt(rawData["Betcount"]);
            gbpTotal.Turnover += Converter.ToDecimal(rawData["TurnoverGBP"]);
            gbpTotal.CustomerWinLoss += Converter.ToDecimal(rawData["WinlostGBP"]);
            gbpTotal.CustomerComm += Converter.ToDecimal(rawData["PlayerCommGBP"]);
            gbpTotal.LicenseeWinLoss += Converter.ToDecimal(rawData["LicenseeWLGBP"]);
            gbpTotal.BAComm += Converter.ToDecimal(rawData["CommBAGBP"]);
            gbpTotal.RacingWinLoss += Converter.ToDecimal(rawData["WinlossRBGBP"]);
            gbpTotal.RacingComm += Converter.ToDecimal(rawData["CommRBGBP"]);
            gbpTotal.AmtDuetoBA += Converter.ToDecimal(rawData["AmtDuetoBAGBP"]);
            gbpTotal.AmtDuetoRacing += Converter.ToDecimal(rawData["AmtDuetoRBGBP"]);
            gbpTotal.AmtDuetoCompany += Converter.ToDecimal(rawData["CompanyGBP"]);
            baseCurrencyTotal[Labels.GBP] = gbpTotal;

            //// CalculateBaseCurrencyTotal
        }

        /// <summary>
        /// Assign total data to current report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="totalByCurrency">The total by currency.</param>
        /// <param name="baseCurrencyTotal">The base currency total.</param>
        private void AssignTotalData(
            ref WinLossByLicensee report,
          Dictionary<int, WinLossByLicensee> totalByCurrency,
          Dictionary<string, WinLossByLicensee> baseCurrencyTotal)
        {
            foreach (var currencyTotal in totalByCurrency)
            {
                report.Total.Add(currencyTotal.Value);
            }

            foreach (var baseTotal in baseCurrencyTotal)
            {
                report.BaseCurrencyTotal.Add(baseTotal.Value);
            }
        }

        /// <summary>
        /// Create base currency list (MYR, EUR, USD).
        /// </summary>
        /// <returns>Base currency list.</returns>
        private Dictionary<string, WinLossByLicensee> CreateBaseCurrencyList()
        {
            var baseCurrencyList = new Dictionary<string, WinLossByLicensee>();
            var currencies = _exchangeCurrencies;

            baseCurrencyList.Add(
                Labels.MYR,
                new WinLossByLicensee
                {
                    CurrencyId = 2,
                    CurrencyName = currencies[2]
                });

            baseCurrencyList.Add(
                Labels.EUR,
                new WinLossByLicensee
                {
                    CurrencyId = 6,
                    CurrencyName = currencies[6]
                });

            baseCurrencyList.Add(
                Labels.USD,
                new WinLossByLicensee
                {
                    CurrencyId = 3,
                    CurrencyName = currencies[3]
                });

            baseCurrencyList.Add(
               Labels.GBP,
               new WinLossByLicensee
               {
                   CurrencyId = 12,
                   CurrencyName = currencies[12]
               });

            return baseCurrencyList;
        }

        /// <summary>
        /// To do: store procedure must return exactly string by report type.
        /// </summary>
        /// <param name="bettingDate">The betting date.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns>Betting date.</returns>
        private DateTime GetBettingDate(string bettingDate, int reportType)
        {
            DateTime date = DateTime.Now;
            switch (reportType)
            {
                case (int)ReportTimeType.Daily:
                    date = new DateTime(
                        Converter.ToInt(bettingDate.Substring(0, 4)),
                        Converter.ToInt(bettingDate.Substring(4, 2)),
                        Converter.ToInt(bettingDate.Substring(6, 2)));
                    break;

                case (int)ReportTimeType.Monthly:
                    date = new DateTime(
                        Converter.ToInt(bettingDate.Substring(0, 4)),
                       Converter.ToInt(bettingDate.Substring(4, 2)),
                       1);
                    break;
            }

            return date;
        }

        //// ExportExcel

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="drilldownLevel">The drilldown level.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            WinLossByLicenseeCriteria criteria,
            ReportDrilldownLevel drilldownLevel)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            var currentColumn = startColumn;

            ExcelCell currentCell;
            CellRange currentCellRange;

            var title = string.Empty;
            var fistColumnText = string.Empty;
            switch (drilldownLevel)
            {
                case ReportDrilldownLevel.Currency:
                    var currencyName = criteria.ConvertToCurrency != 0 ?
                        string.Format(
                            LicenseeResources.WinLossByLicensee_ReportTitle_BaseCurrency,
                            criteria.ConvertToCurrencyName) :
                        string.Empty;
                    title = string.Format(
                        LicenseeResources.WinLossByLicensee_CurrencyReportTitle,
                        criteria.LicenseeName,
                        criteria.FromDate.ToShortFormatedDate(),
                        criteria.ToDate.ToShortFormatedDate(),
                        criteria.CurrencyName,
                        currencyName);
                    fistColumnText = Labels.Date;
                    break;

                case ReportDrilldownLevel.Level:

                    ////case ReportDrilldownLevel.Member:
                    currencyName = criteria.ConvertToCurrency != 0 ?
                        string.Format(
                            LicenseeResources.WinLossByLicensee_ReportTitle_BaseCurrency,
                            criteria.ConvertToCurrencyName) :
                        string.Empty;

                    title = string.Format(
                        LicenseeResources.WinLossByLicensee_AgentReportTitle,
                        criteria.LicenseeName,
                        criteria.FromDate.ToShortFormatedDate(),
                        criteria.ToDate.ToShortFormatedDate(),
                        criteria.CurrencyName,
                        currencyName);
                    fistColumnText = Labels.UserName;
                    break;
            }

            currentColumn = startColumn;
            worksheet.Columns[currentColumn].Width = 140 * ExcelExtensions.FontWidth;
            currentColumn++;

            worksheet.Columns[currentColumn].Width = 100 * ExcelExtensions.FontWidth;
            currentColumn++;

            worksheet.Columns[currentColumn].Width = 160 * ExcelExtensions.FontWidth;
            currentColumn++;

            worksheet.Columns[currentColumn].Width = 100 * ExcelExtensions.FontWidth;
            currentColumn++;

            worksheet.Columns[currentColumn].Width = 120 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 100 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 120 * ExcelExtensions.FontWidth;
            currentColumn++;

            worksheet.Columns[currentColumn].Width = 270 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 100 * ExcelExtensions.FontWidth;
            currentColumn++;

            worksheet.Columns[currentColumn].Width = 120 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 100 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 120 * ExcelExtensions.FontWidth;
            currentColumn++;

            worksheet.Columns[currentColumn].Width = 120 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 120 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 150 * ExcelExtensions.FontWidth;
            currentColumn++;

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 14);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow += 2;

            //// Filter

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 14);

            //// Title

            // First Row
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = fistColumnText;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Currency;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Turnover;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.TotalTrans;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.CustomerWinLoss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 3;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = string.Empty;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 2;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RacingRevenue;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 3;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.AmountDueTo;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 3;
            currentRow++;

            // Second Row
            currentColumn = startColumn + 4;
            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.WinLoss;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Comm;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Total;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = this.GetColLicenseeWinLossTitle(criteria.ShowSB, criteria.ShowNG, criteria.ShowLC, criteria.ShowVS, criteria.ShowCS, criteria.ShowKN, criteria.ShowAGC, criteria.ShowGD);
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.BAComm;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCellRange.Value = Labels.WinLoss;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Comm;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Total;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.BA;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Racing;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = this.GetColLicenseeWinLossTitleSuffix(criteria.ShowSB, criteria.ShowNG, criteria.ShowLC, criteria.ShowVS, criteria.ShowCS, criteria.ShowKN, criteria.ShowAGC, criteria.ShowGD);
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;

            //// Title
        }

        /// <summary>
        /// Exports the win loss row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="intStyle">The INT style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="drilldownLevel">The drilldown level.</param>
        /// <param name="rowType">Type of the row.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportWinLossRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            WinLossByLicensee winLoss,
            CellStyle textStyle,
            CellStyle intStyle,
            CellStyle twoDecimalNumberStyle,
            ReportDrilldownLevel drilldownLevel,
            ReportRowType rowType)
        {
            var currentColumn = startColumn;
            ExcelCell currentCell;

            if (rowType == ReportRowType.NormalRow)
            {
                switch (drilldownLevel)
                {
                    case ReportDrilldownLevel.Currency:
                        break;

                    case ReportDrilldownLevel.Level:
                        currentCell = worksheet.Cells[currentRow, currentColumn];
                        currentCell.Value = winLoss.UserName;
                        currentCell.Style = textStyle;
                        break;
                }
            }

            currentColumn++;
            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.CurrencyName;
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.Turnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            if (rowType != ReportRowType.GrandTotal)
            {
                currentCell = worksheet.Cells[currentRow, currentColumn];
                currentCell.Value = winLoss.BetCount;
                currentCell.Style = intStyle;
            }

            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.CustomerWinLoss;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.CustomerComm;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.CustomerWinLossTotal;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.LicenseeWinLoss;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.BAComm;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.RacingWinLoss;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.RacingComm;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.RacingWinLossTotal;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.AmtDuetoBA;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.AmtDuetoRacing;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.AmtDuetoCompany;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Export total of win loss report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="total">The total.</param>
        /// <param name="drilldownLevel">The drilldown level.</param>
        private void ExportReportTotal(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            List<WinLossByLicensee> total,
            ReportDrilldownLevel drilldownLevel)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;

            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.TotalRowColor, true);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.TotalRowColor, true, false, false);
            intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.TotalRowColor, true, false, false);
            twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var totalLength = total.Count();
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + totalLength - 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Total;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            foreach (var winLoss in total)
            {
                this.ExportWinLossRow(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    winLoss,
                    textStyle,
                    intStyle,
                    twoDecimalNumberStyle,
                    drilldownLevel,
                    ReportRowType.Total);
            }
        }

        /// <summary>
        /// Export base currency total of win loss report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="total">The total.</param>
        /// <param name="drilldownLevel">The drilldown level.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportBaseCurrencyTotal(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            List<WinLossByLicensee> total,
            ReportDrilldownLevel drilldownLevel)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;

            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.GrandTotalRowColor, true, false, false);
            intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            intStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);

            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);

            var totalLength = total.Count();
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + totalLength - 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Total;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            currentColumn = startColumn + 3;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + totalLength - 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = total[0].BetCount;
            currentCellRange.Style = intStyle;

            var isFirstRow = true;

            foreach (var winLoss in total)
            {
                this.ExportWinLossRow(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    winLoss,
                    textStyle,
                    intStyle,
                    twoDecimalNumberStyle,
                    drilldownLevel,
                    ReportRowType.GrandTotal);

                if (isFirstRow)
                {
                    textStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
                    intStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
                    twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
                    isFirstRow = false;
                }
            }
        }

        /// <summary>
        /// Export content of win loss report.
        /// </summary>
        /// <param name="worksheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportCurrencyReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            WinLossByLicensee report)
        {
            //// ExcelTypes

            var currentColumn = startColumn;
            CellRange currentCellRange;

            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            //// ExcelTypes

            if (report != null && report.TransactionList != null && report.TransactionList.Count > 0)
            {
                var groups = report.TransactionList.ToList();
                for (int i = 0; i < groups.Count; i++)
                {
                    textStyle.FillPattern.SetSolid(i % 2 == 0
                        ? ExcelExtensions.EvenRowColor
                        : ExcelExtensions.OddRowColor);
                    intStyle.FillPattern.SetSolid(i % 2 == 0
                       ? ExcelExtensions.EvenRowColor
                       : ExcelExtensions.OddRowColor);
                    twoDecimalNumberStyle.FillPattern.SetSolid(i % 2 == 0
                       ? ExcelExtensions.EvenRowColor
                       : ExcelExtensions.OddRowColor);

                    var group = groups[i];
                    var groupLength = group.TransactionList.Count;
                    currentColumn = startColumn;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + groupLength - 1, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = group.BettingDate;
                    currentCellRange.Style = textStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    foreach (var winLoss in group.TransactionList)
                    {
                        this.ExportWinLossRow(
                            ref worksheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            winLoss,
                            textStyle,
                            intStyle,
                            twoDecimalNumberStyle,
                            ReportDrilldownLevel.Currency,
                            ReportRowType.NormalRow);
                    }
                }

                this.ExportReportTotal(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    report.Total,
                    ReportDrilldownLevel.Currency);

                this.ExportReportBaseCurrencyTotal(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    report.BaseCurrencyTotal,
                    ReportDrilldownLevel.Currency);
            }
            else
            {
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, currentColumn + 14);
            }
        }

        /// <summary>
        /// Gets text of column Licensee Win Loss.
        /// </summary>
        /// <param name="showSB">If set to <c>true</c> [show SB].</param>
        /// <param name="showNG">If set to <c>true</c> [show NG].</param>
        /// <param name="showLC">If set to <c>true</c> [show LC].</param>
        /// <param name="showVS">If set to <c>true</c> [show VS].</param>
        /// <param name="showCS">If set to <c>true</c> [show CS].</param>
        /// <param name="showKN">If set to <c>true</c> [show KN].</param>
        /// <returns>Text of column Licensee Win Loss.</returns>
        private string GetColLicenseeWinLossTitle(bool showSB, bool showNG, bool showLC, bool showVS, bool showCS, bool showKN, bool showAGC, bool showGD)
        {
            var suffix = this.GetColLicenseeWinLossTitleSuffix(showSB, showNG, showLC, showVS, showCS, showKN, showAGC, showGD);
            if (!string.IsNullOrEmpty(suffix))
            {
                return Labels.LicenseeWinLoss + " (" + suffix + ")";
            }

            return Labels.LicenseeWinLoss;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private string GetColLicenseeWinLossTitleSuffix(bool showSB, bool showNG, bool showLC, bool showVS, bool showCS, bool showKN, bool showAGC, bool showGD)
        {
            var products = new List<string>();

            if (showSB)
            {
                products.Add(Labels.SB);
            }

            if (showNG)
            {
                products.Add(Labels.NG);
            }

            if (showLC)
            {
                products.Add(Labels.LC);
            }

            if (showVS)
            {
                products.Add(Labels._VS);
            }

            if (showCS)
            {
                products.Add(Labels.CS);
            }

            if (showKN)
            {
                products.Add(Labels.KN);
            }

            if (showAGC)
            {
                products.Add(Labels.AGC);
            }

            if (showGD)
            {
                products.Add(Labels.GD);
            }

            if (products.Count > 0)
            {
                return string.Join("+", products);
            }
            else
            {
                return Labels.SB + "+" + Labels.NG + "+" + Labels.LC;
            }
        }

        //// ExportExcel

        /// <summary>
        /// Exports the content of the level report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportLevelReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            WinLossByLicensee report)
        {
            //// ExcelTypes

            var currentColumn = startColumn;
            CellRange currentCellRange;

            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            //// ExcelTypes

            if (report != null && report.TransactionList != null && report.TransactionList.Count > 0)
            {
                for (int i = 0; i < report.TransactionList.Count; i++)
                {
                    var winLoss = report.TransactionList[i];

                    currentColumn = startColumn;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = winLoss.UserName;
                    currentCellRange.Style = textStyle;

                    this.ExportWinLossRow(
                        ref worksheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        winLoss,
                        textStyle,
                        intStyle,
                        twoDecimalNumberStyle,
                        ReportDrilldownLevel.Level,
                        ReportRowType.NormalRow);
                }

                this.ExportReportTotal(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    report.Total,
                    ReportDrilldownLevel.Level);

                this.ExportReportBaseCurrencyTotal(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    report.BaseCurrencyTotal,
                    ReportDrilldownLevel.Level);
            }
            else
            {
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, currentColumn + 14);
            }
        }
    }
}