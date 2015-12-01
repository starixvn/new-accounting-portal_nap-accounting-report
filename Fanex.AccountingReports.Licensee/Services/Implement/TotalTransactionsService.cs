// --------------------------------------------------------------------------
// <copyright file="TotalTransactionsService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.5">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.TotalTransactions;
    using Fanex.AccountingReports.Licensee.Models.TotalTransactions;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Common.Extensions;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TotalTransactionsService : ServiceBase, ITotalTransactionsService
    {
        public TotalTransactionsService(IDynamicRepository dynamicRepository = null) :
            base(dynamicRepository)
        {
        }

        /// <summary>
        /// Gets all resellers.
        /// </summary>
        /// <returns>All resellers.</returns>
        public IEnumerable<Reseller> GetAllResellers()
        {
            var resellerList = new List<Reseller>();

            try
            {
                var rawData = GetReportData<dynamic>(new GetLicenseeByResellerCriteria
                {
                    ExcludeTest = true
                });
                Reseller currentReseller = null;

                // order reseller by name and licensee to reseller
                rawData
                    .OrderBy(reseller => reseller.ResellerName)
                    .Select(s => s)
                    .ForEach(data =>
                    {
                        // assign data for reseller
                        if (currentReseller == null || currentReseller.Id != data.ResellerID)
                        {
                            currentReseller = new Reseller
                            {
                                Id = data.ResellerID,
                                Name = data.ResellerName,
                                Licensees = new List<Licensee>()
                            };

                            resellerList.Add(currentReseller);
                        }

                        // add licensee for each reseller
                        currentReseller.Licensees.Add(
                            new Licensee
                            {
                                Id = data.LicenseeID,
                                Name = data.LicenseeName
                            });
                    });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return resellerList;
        }

        /// <summary>
        /// Gets the reseller rates.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <returns>The reseller rates.</returns>
        public IEnumerable<ResellerRate> GetResellerRates(DateTime date)
        {
            var resellerRate = GetReportData<ResellerRate>(new GetResellerRatesCriteria { InputDate = date });

            return resellerRate;
        }

        /// <summary>
        /// Gets the total transactions.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The total transactions.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public TransactionItem GetTotalTransactions(GetTotalTransactionsCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var toDate = Fanex.AccountingReports.Core.Extensions.DateTimeExtensions.LastDayOfMonth(criteria.FromDate);
            criteria.ToDate = DateTime.Now < toDate ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) : toDate;
            TransactionItem totalTransaction = null;
            try
            {
                if (criteria.IsValid())
                {
                    IEnumerable<dynamic> rawTransData = GetReportData<dynamic>(criteria);

                    var rates = this.GetResellerRates(criteria.FromDate);

                    var resellerTrans = rawTransData
                        .GroupBy(r => r.ResellerID)
                        .Select(group => new TransactionItem
                        {
                            Id = group.Key,
                            Name = group.FirstOrDefault().ResellerName,
                            BetCount = group.Sum(r => r.BetCount),
                            Childs = new List<TransactionItem>(
                                    group
                                    .Select(l => new TransactionItem
                                    {
                                        Name = l.LicenseeName,
                                        BetCount = l.BetCount,
                                        EURCost = this.GetLicenseeTransCost(rates, (int)group.Key, l.BetCount),
                                        Parent = group.Select(p => new TransactionItem
                                        {
                                            Id = p.ResellerID,
                                            Name = p.ResellerName
                                        }).FirstOrDefault()
                                    })),

                            EURCost = group.Sum(r => (decimal)this.GetLicenseeTransCost(rates, (int)group.Key, r.BetCount))
                        })
                        .ToList<TransactionItem>();

                    totalTransaction = new TransactionItem
                    {
                        Name = Labels.Total,
                        BetCount = resellerTrans.Sum(s => s.BetCount),
                        EURCost = resellerTrans.Sum(s => s.EURCost),
                        Childs = resellerTrans
                    };

                    SessionManager.Set<TransactionItem>(LicenseeConstants.TotalTransationSessionKey, totalTransaction);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return totalTransaction;
        }

        /// <summary>
        /// Gets all reseller rate.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>All reseller rate.</returns>
        public IEnumerable<ResellerRate> GetAllResellerRate(GetResellerRatesCriteria criteria)
        {
            try
            {
                var resellerRate = DynamicRepository.Fetch<ResellerRate>(criteria);
                return resellerRate;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Exports the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void Export(GetTotalTransactionsCriteria criteria)
        {
            try
            {
                //// Init Criteria

                var toDate = Fanex.AccountingReports.Core.Extensions.DateTimeExtensions.LastDayOfMonth(DateTime.Now);
                criteria.ToDate = DateTime.Now < toDate ? DateTime.Now : toDate;
                criteria.NormalLicensee = criteria.NormalLicensee ?? string.Empty;
                criteria.BALicensee = criteria.BALicensee ?? string.Empty;
                criteria.NormalLicenseeName = criteria.NormalLicenseeName ?? string.Empty;
                criteria.BALicenseeName = criteria.BALicenseeName ?? string.Empty;

                //// Init Criteria

                //// Get Data

                var transactionReport = SessionManager.Get<TransactionItem>(LicenseeConstants.TotalTransationSessionKey) ?? this.GetTotalTransactions(criteria);

                //// Get Data

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = LicenseeResources.TotalTransactionByLicensee_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startColumn = 0;
                int startRow = 0;
                var currentRow = startRow;

                //// ExcelTypes

                //// MakeExcelFile

                this.ExportHeader(ref workSheet, startRow, startColumn, ref currentRow, criteria);
                this.ExportContent(ref workSheet, startRow, startColumn, ref currentRow, criteria, transactionReport);

                //// MakeExcelFile

                excelFile.ReponseStream(string.Format(
                    "{0}_{1}",
                    LicenseeResources.TotalTransactionByLicensee_ReportName,
                    criteria.FromDate.ToMonthYear().Replace(' ', '_')));
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Gets the licensee trans cost.
        /// </summary>
        /// <param name="rates">The rates.</param>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <param name="betCount">The bet count.</param>
        /// <returns>The licensee trans cost.</returns>
        private decimal GetLicenseeTransCost(
            IEnumerable<ResellerRate> rates,
            int resellerId,
            double betCount)
        {
            var resellerRate = rates.FirstOrDefault(rate =>
                rate.MinTrans <= betCount &&
                rate.MaxTrans >= betCount &&
                rate.ResellerID == resellerId);

            var transCost = resellerRate == null ? 0 : resellerRate.Rate * (decimal)betCount;

            return transCost;
        }

        /// <summary>
        /// Gets the transaction cost string.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="resellerName">Name of the reseller.</param>
        /// <returns>The transaction cost string.</returns>
        private string GetTransactionCostString(TransactionItem transaction, string resellerName)
        {
            string transCost = string.Empty;
            if (this.IsExcludeEURCost(resellerName))
            {
                transCost = Labels.NotAvailable;
            }
            else
            {
                transCost = transaction.EURCost.Format(2);
            }

            return transCost;
        }

        /// <summary>
        /// Determines whether [is exclude EUR cost] [the specified reseller name].
        /// </summary>
        /// <param name="resellerName">Name of the reseller.</param>
        /// <returns>Is Exclude EUR Cost.</returns>
        private bool IsExcludeEURCost(string resellerName = "vcasia")
        {
            return string.IsNullOrWhiteSpace(resellerName) ||
                string.Equals("vcasia", resellerName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Exports the header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetTotalTransactionsCriteria criteria)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;

            currentColumn = startColumn;
            worksheet.Columns[currentColumn].Width = 200 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 200 * ExcelExtensions.FontWidth;
            currentColumn++;
            worksheet.Columns[currentColumn].Width = 200 * ExcelExtensions.FontWidth;
            currentColumn++;

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = string.Format(LicenseeResources.TotalTransactionByLicensee_Header, criteria.FromDate.ToMonthYear());
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow += 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 2);

            //// Title

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Licensee;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = LicenseeResources.TotalTransactions;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = LicenseeResources.TotalTransactionByLicensee_EURCost;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;

            //// Title
        }

        /// <summary>
        /// Exports the content.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="transactionReport">The transaction report.</param>
        private void ExportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetTotalTransactionsCriteria criteria,
            TransactionItem transactionReport)
        {
            //// ExcelTypes

            var currentColumn = startColumn;
            var endRow = currentRow;
            var endColumn = currentColumn;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(Color.FromArgb(89, 161, 191));

            //// ExcelTypes

            if (transactionReport != null && transactionReport.Childs.Count > 0)
            {
                var resellerCount = transactionReport.Childs.Count;
                for (var i = 0; i < resellerCount; i++)
                {
                    var reseller = transactionReport.Childs[i];
                    this.ExportResellerRow(ref worksheet, startRow, startColumn, ref currentRow, criteria, reseller);
                    var licenseeCount = reseller.Childs.Count;
                    for (var j = 0; j < licenseeCount; j++)
                    {
                        var licensee = reseller.Childs[j];
                        this.ExportLicenseeRow(ref worksheet, startRow, startColumn, ref currentRow, criteria, licensee);
                    }
                }

                this.ExportTotalRow(ref worksheet, startRow, startColumn, ref currentRow, criteria, transactionReport);
            }
            else
            {
                endColumn = currentColumn + 2;
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, endColumn);
            }
        }

        /// <summary>
        /// Exports the row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="betCountStyle">The bet count style.</param>
        /// <param name="eurCostStyle">The EUR cost style.</param>
        private void ExportRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            TransactionItem transaction,
            CellStyle textStyle,
            CellStyle betCountStyle,
            CellStyle eurCostStyle)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.Name;
            currentCellRange.Style = textStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.BetCount;
            currentCellRange.Style = betCountStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = eurCostStyle;
            if (transaction.EURCostString != Labels.NotAvailable)
            {
                currentCellRange.Value = transaction.EURCost;
            }
            else
            {
                currentCellRange.Value = transaction.EURCostString;
            }

            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the licensee row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="transaction">The transaction.</param>
        private void ExportLicenseeRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetTotalTransactionsCriteria criteria,
            TransactionItem transaction)
        {
            var backgroundColor = ExcelExtensions.BackGroundColor;
            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            string paddingText = new string(' ', 3);
            currentCellRange.Merged = true;
            currentCellRange.Value = paddingText + transaction.Name;
            currentCellRange.Style.FormatCellText(backgroundColor, false);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.BetCount;
            currentCellRange.Style.FormatCellNumberStyle(string.Empty, backgroundColor, false, false, false);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style.FormatCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            if (transaction.EURCostString != Labels.NotAvailable)
            {
                currentCellRange.Value = transaction.EURCost;
            }
            else
            {
                currentCellRange.Value = transaction.EURCostString;
            }

            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the reseller row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="transaction">The transaction.</param>
        private void ExportResellerRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetTotalTransactionsCriteria criteria,
            TransactionItem transaction)
        {
            var backgroundColor = ExcelExtensions.SubTotalRowColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, true);
            var betCountStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, backgroundColor, true, false, false);
            var eurCostStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, true, false, false);
            this.ExportRow(
                ref worksheet,
                startRow,
                startColumn,
                ref currentRow,
                transaction,
                textStyle,
                betCountStyle,
                eurCostStyle);
        }

        /// <summary>
        /// Exports the total row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="transaction">The transaction.</param>
        private void ExportTotalRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetTotalTransactionsCriteria criteria,
            TransactionItem transaction)
        {
            var backgroundColor = ExcelExtensions.GrandTotalRowColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, true);
            textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            var betCountStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, backgroundColor, true, false, false);
            var eurCostStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, true, false, false);

            textStyle.Font.Color = Color.Black;
            betCountStyle.Font.Color = Color.Black;
            eurCostStyle.Font.Color = Color.Black;
            this.ExportRow(
                ref worksheet,
                startRow,
                startColumn,
                ref currentRow,
                transaction,
                textStyle,
                betCountStyle,
                eurCostStyle);
        }

        /// <summary>
        /// Gets the name of the products.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The name of the products.</returns>
        private string GetProductsName(GetTotalTransactionsCriteria criteria)
        {
            var products = new List<string>();
            if (criteria.ShowSB)
            {
                products.Add(Labels.Sportsbook);
            }

            if (criteria.ShowRB)
            {
                products.Add(Labels.Racing);
            }

            if (criteria.ShowNG)
            {
                products.Add(Labels.NumberGame);
            }

            if (criteria.ShowLC)
            {
                products.Add(Labels.LiveCasino);
            }

            if (criteria.ShowVS)
            {
                products.Add(Labels.Virtual_Sports);
            }

            if (criteria.ShowKN)
            {
                products.Add(Labels.KN);
            }

            return string.Join(Labels.SeperateChar, products);
        }
    }
}