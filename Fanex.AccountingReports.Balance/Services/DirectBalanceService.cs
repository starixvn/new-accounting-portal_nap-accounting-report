namespace Fanex.AccountingReports.Balance.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Balance.Constants;
    using Fanex.AccountingReports.Balance.Criteria.DirectBalance;
    using Fanex.AccountingReports.Balance.Models.DirectBalance;
    using Fanex.AccountingReports.Balance.Resources;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DirectBalanceService : ServiceBase
    {
        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        private enum ReportRowType
        {
            /// <summary>
            /// The grand total.
            /// </summary>
            GrandTotal = 1,

            /// <summary>
            /// The normal even row.
            /// </summary>
            NormalEvenRow = 2,

            /// <summary>
            /// The normal odd row.
            /// </summary>
            NormalOddRow = 3
        }

        /// <summary>
        /// Gets the direct balance.
        /// </summary>
        /// <param name="directBalanceCriteria">The direct balance criteria.</param>
        /// <returns>The direct balance.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public DirectBalance GetDirectBalance(GetDirectBalanceCriteria directBalanceCriteria)
        {
            var result = new DirectBalance();
            var directBalanceList = new List<DirectBalanceDetail>();
            if (directBalanceCriteria.Currency == 0)
            {
                directBalanceCriteria.Currency = null;
            }

            if (string.IsNullOrEmpty(directBalanceCriteria.Username))
            {
                directBalanceCriteria.Username = string.Empty;
            }

            var currencies = ReportService.ExchangeCurrencies;

            var rawData = GetReportData<dynamic>(directBalanceCriteria);
            if (rawData != null)
            {
                decimal baseBalanceTotal = 0;
                foreach (var row in rawData)
                {
                    var currencyId = Converter.ToInt(row.currency);
                    decimal balance = Converter.ToDecimal(row.Balance);
                    decimal baseBalance = Converter.ToDecimal(row.BaseBalance);
                    directBalanceList.Add(new DirectBalanceDetail()
                    {
                        CustId = Converter.ToString2(row.custid),
                        Username = Converter.ToString2(row.username),
                        Currency = currencies[currencyId],
                        Balance = balance.Format(2),
                        BaseBalance = baseBalance.Format(2)
                    });

                    baseBalanceTotal += baseBalance;
                }

                result.DirectBalanceList = directBalanceList;
                result.BaseBalanceTotal = baseBalanceTotal.Format(2);
            }

            SessionManager.Set<DirectBalance>(BalanceConstants.DirectBalanceSessionKey, result);
            return result;
        }

        /// <summary>
        /// Gets the direct balance master down line.
        /// </summary>
        /// <param name="directBalanceMasterDownLineCriteria">The direct balance master down line criteria.</param>
        /// <returns>The direct balance master down line.</returns>
        public DirectBalance GetDirectBalanceMasterDownLine(GetDirectBalanceMasterDownlineCriteria directBalanceMasterDownLineCriteria)
        {
            var result = new DirectBalance();
            var directBalanceDownlineList = new List<DirectBalanceDetail>();
            var currencies = ReportService.ExchangeCurrencies;

            var rawData = GetReportData<dynamic>(directBalanceMasterDownLineCriteria);
            if (rawData != null)
            {
                decimal baseBalanceDownlineTotal = 0;
                foreach (var row in rawData)
                {
                    var currencyId = Converter.ToInt(row.currency);
                    decimal balance = Converter.ToDecimal(row.bal);
                    decimal baseBalance = Converter.ToDecimal(row.baseBalance);
                    if (balance != 0)
                    {
                        directBalanceDownlineList.Add(new DirectBalanceDetail()
                        {
                            CustId = Converter.ToString2(row.custid),
                            Username = Converter.ToString2(row.username),
                            Currency = currencies[currencyId],
                            Balance = balance.Format(2),
                            BaseBalance = baseBalance.Format(2)
                        });
                    }

                    baseBalanceDownlineTotal += baseBalance;
                }

                result.DirectBalanceList = directBalanceDownlineList;
                result.BaseBalanceTotal = baseBalanceDownlineTotal.Format(2);
            }

            SessionManager.Set<DirectBalance>(BalanceConstants.DirectBalanceMasterDownLineSessionKey, result);
            return result;
        }

        /// <summary>
        /// Gets the direct balance agent down line.
        /// </summary>
        /// <param name="directBalanceAgentDownLineCriteria">The direct balance agent down line criteria.</param>
        /// <returns>The direct balance agent down line.</returns>
        public DirectBalance GetDirectBalanceAgentDownLine(GetDirectBalanceAgentDownlineCriteria directBalanceAgentDownLineCriteria)
        {
            var result = new DirectBalance();
            var directBalanceDownlineList = new List<DirectBalanceDetail>();
            var currencies = ReportService.ExchangeCurrencies;

            var rawData = GetReportData<dynamic>(directBalanceAgentDownLineCriteria);
            if (rawData != null)
            {
                decimal baseBalanceDownlineTotal = 0;
                foreach (var row in rawData)
                {
                    var currencyId = Converter.ToInt(row.currency);
                    decimal balance = Converter.ToDecimal(row.Balance);
                    decimal baseBalance = Converter.ToDecimal(row.BaseBalance);
                    if (balance != 0)
                    {
                        directBalanceDownlineList.Add(new DirectBalanceDetail()
                        {
                            CustId = Converter.ToString2(row.custid),
                            Username = Converter.ToString2(row.username),
                            Currency = currencies[currencyId],
                            Balance = balance.Format(2),
                            BaseBalance = baseBalance.Format(2)
                        });
                    }

                    baseBalanceDownlineTotal += baseBalance;
                }

                result.DirectBalanceList = directBalanceDownlineList;
                result.BaseBalanceTotal = baseBalanceDownlineTotal.Format(2);
            }

            SessionManager.Set<DirectBalance>(BalanceConstants.DirectBalanceAgentdownLineSessionKey, result);
            return result;
        }

        /// <summary>
        /// Exports the direct balance report.
        /// </summary>
        /// <param name="directBalanceCriteria">The direct balance criteria.</param>
        /// <returns>The direct balance report.</returns>
        public DirectBalance ExportDirectBalanceReport(GetDirectBalanceCriteria directBalanceCriteria)
        {
            var result = new DirectBalance();
            var fullReport = SessionManager.Get<DirectBalance>(BalanceConstants.DirectBalanceSessionKey);
            if (fullReport == null)
            {
                fullReport = this.GetDirectBalance(directBalanceCriteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = BalanceResources.DirectBalance_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                directBalanceCriteria);

            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            var customerType = ReportService.GetRoleName(directBalanceCriteria.RoleID);
            var reportName = Labels.DirectBalance + Labels.Symbol_ReportNameSeparator + customerType;
            excelFile.ReponseStream(reportName);

            return result;
        }

        /// <summary>
        /// Exports the balance master down line report.
        /// </summary>
        /// <param name="getDirectBalanceMasterCriteria">The get direct balance master criteria.</param>
        /// <returns>The balance master down line report.</returns>
        public DirectBalance ExportBalanceMasterDownLineReport(GetDirectBalanceMasterDownlineCriteria getDirectBalanceMasterCriteria)
        {
            var result = new DirectBalance();
            var fullReport = SessionManager.Get<DirectBalance>(BalanceConstants.DirectBalanceMasterDownLineSessionKey);
            if (fullReport == null)
            {
                fullReport = this.GetDirectBalanceMasterDownLine(getDirectBalanceMasterCriteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = BalanceResources.DirectBalance_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeaderDownLine(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                3);
            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            excelFile.ReponseStream(BalanceResources.DirectBalance_MasterDownline_ReportName);

            return result;
        }

        /// <summary>
        /// Exports the balance agent down line report.
        /// </summary>
        /// <param name="getDirectBalanceAgentCriteria">The get direct balance agent criteria.</param>
        /// <returns>The balance agent down line report.</returns>
        public DirectBalance ExportBalanceAgentDownLineReport(GetDirectBalanceAgentDownlineCriteria getDirectBalanceAgentCriteria)
        {
            var result = new DirectBalance();
            var fullReport = SessionManager.Get<DirectBalance>(BalanceConstants.DirectBalanceAgentdownLineSessionKey);
            if (fullReport == null)
            {
                fullReport = this.GetDirectBalanceAgentDownLine(getDirectBalanceAgentCriteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = BalanceResources.DirectBalance_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeaderDownLine(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                4);

            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            excelFile.ReponseStream(BalanceResources.DirectBalance_AgentDownline_ReportName);

            return result;
        }

        /// <summary>
        /// Gets the currencies.
        /// </summary>
        /// <returns>The currencies.</returns>
        public List<EntityBase<int, string>> GetCurrencies()
        {
            var result = new List<EntityBase<int, string>>();
            Dictionary<int, string> currList = ReportService.GetExchangeCurrencies(
                     new GetExchangeCurrenciesCriteria
                     {
                         ExcludeTest = false
                     });

            foreach (var row in currList)
            {
                result.Add(new EntityBase<int, string>()
                {
                    Id = row.Key,
                    Name = row.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Exports the report header down line.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="roleId">The role identifier.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeaderDownLine(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int roleId)
        {
            CellRange currentCellRange;
            ExcelCell currentCell;

            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            worksheet.Columns[startColumn].Width = 50 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 1].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 2].Width = 80 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 3].Width = 150 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 4].Width = 150 * ExcelExtensions.FontWidth;
            var currentColumn = startColumn;

            currentColumn = startColumn;
            var title = ReportService.GetRoleName(roleId);
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
            currentCellRange.Merged = true;
            currentCellRange.Value = title + " " + Labels.Balance;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;

            // First Row
            currentColumn = startColumn;
            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.No;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = title;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Currency;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Balance;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.BaseRM;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetDirectBalanceCriteria criteria)
        {
            CellRange currentCellRange;
            ExcelCell currentCell;

            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            worksheet.Columns[startColumn].Width = 50 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 1].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 2].Width = 80 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 3].Width = 150 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 4].Width = 150 * ExcelExtensions.FontWidth;
            var currentColumn = startColumn;
            currentColumn = startColumn;

            var title = ReportService.GetRoleName(criteria.RoleID) + " " + Labels.Balance;
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;

            // First Row
            currentColumn = startColumn;
            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.No;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = title;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Currency;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Balance;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.BaseRM;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            DirectBalance report)
        {
            if (report != null &&
                report.DirectBalanceList != null &&
                report.DirectBalanceList.Count > 0)
            {
                var reportLength = report.DirectBalanceList.Count;
                var rowType = ReportRowType.NormalOddRow;
                for (int i = 0; i < reportLength; i++)
                {
                    if (i % 2 == 0)
                    {
                        rowType = ReportRowType.NormalEvenRow;
                    }
                    else
                    {
                        rowType = ReportRowType.NormalOddRow;
                    }

                    this.ExportDirectBalanceDetail(
                        ref worksheet,
                        i + 1,
                        startRow,
                        startColumn,
                        ref currentRow,
                        report.DirectBalanceList[i],
                        rowType);
                }

                this.ExportDirectBalanceTotal(ref worksheet, startRow, startColumn, ref currentRow, report.BaseBalanceTotal);
            }
            else
            {
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 4);
            }
        }

        /// <summary>
        /// Exports the direct balance detail.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="index">The index.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="detail">The detail.</param>
        /// <param name="rowType">Type of the row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportDirectBalanceDetail(
            ref ExcelWorksheet worksheet,
            int index,
            int startRow,
            int startColumn,
            ref int currentRow,
            DirectBalanceDetail detail,
            ReportRowType rowType)
        {
            ExcelCell currentCell;
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);

            switch (rowType)
            {
                case ReportRowType.NormalOddRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
                    twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.NormalEvenRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.EvenRowColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.EvenRowColor, false, false, false);
                    twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;
            }

            var currentColumn = startColumn;
            var isNormalRow = rowType == ReportRowType.NormalEvenRow ||
                rowType == ReportRowType.NormalOddRow;
            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = index;
            currentCell.Style = intStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Username;
            currentCell.Style = textStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Currency;
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Converter.ToDecimal(detail.Balance, 2);
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Converter.ToDecimal(detail.BaseBalance, 2);
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the direct balance total.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="total">The total.</param>
        private void ExportDirectBalanceTotal(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            string total)
        {
            ExcelCell currentCell;
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            CellRange currentCellRange;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, startColumn, currentRow, startColumn + 3);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Total;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            currentCell = worksheet.Cells[currentRow, startColumn + 4];
            currentCell.Value = Converter.ToDecimal(total, 2);
            currentCell.Style = twoDecimalNumberStyle;
        }
    }
}