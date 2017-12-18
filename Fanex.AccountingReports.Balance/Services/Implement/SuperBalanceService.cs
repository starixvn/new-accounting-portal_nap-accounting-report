namespace Fanex.AccountingReports.Balance.Services.Implement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Balance.Criteria.SuperBalance;
    using Fanex.AccountingReports.Balance.Models.SuperBalance;
    using Fanex.AccountingReports.Balance.Repositories.Implement;
    using Fanex.AccountingReports.Balance.Repositories.Interface;
    using Fanex.AccountingReports.Balance.Resources;
    using Fanex.AccountingReports.Balance.Services.Interface;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SuperBalanceService : ServiceBase, ISuperBalanceService
    {
        private string _superBalanceSessionKey = "Fanex.AccountingReports.Balance.SuperBalance." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _superDownLineSessionKey = "Fanex.AccountingReports.Balance.SuperBalanceSuperDownline." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _masterDownLineSessionKey = "Fanex.AccountingReports.Balance.SuperBalanceMasterDownline." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _agentDownLineSessionKey = "Fanex.AccountingReports.Balance.SuperBalanceAgentDownline." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private ISuperBalanceRepository _superBalanceRepository;

        public SuperBalanceService()
            : this(null)
        {
        }

        public SuperBalanceService(ISuperBalanceRepository superBalanceRepository)
        {
            this._superBalanceRepository = superBalanceRepository ?? new SuperBalanceRepository();
        }

        private enum ReportRowType
        {
            /// <summary>
            /// The total.
            /// </summary>
            Total = 1,

            /// <summary>
            /// The grand total.
            /// </summary>
            GrandTotal = 2,

            /// <summary>
            /// The normal even row.
            /// </summary>
            NormalEvenRow = 3,

            /// <summary>
            /// The normal odd row.
            /// </summary>
            NormalOddRow = 4
        }

        private enum BalanceType
        {
            /// <summary>
            /// The today.
            /// </summary>
            Today = 1,

            /// <summary>
            /// The yesterday.
            /// </summary>
            Yesterday = 2
        }

        /// <summary>
        /// Gets the super balance.
        /// </summary>
        /// <param name="superBalanceCriteria">The super balance criteria.</param>
        /// <returns>The super balance.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperBalance GetSuperBalance(GetSuperBalanceCriteria superBalanceCriteria)
        {
            var result = new SuperBalance
            {
                SuperBalanceList = new List<SuperBalance>(),
                BaseBalanceTotal = new SuperBalance(),
            };

            var outputParam = new Hashtable();

            try
            {
                if (superBalanceCriteria.Currency == 0)
                {
                    superBalanceCriteria.Currency = null;
                }

                if (superBalanceCriteria.Username == null)
                {
                    superBalanceCriteria.Username = string.Empty;
                }

                if (superBalanceCriteria.FirstLetter == null)
                {
                    superBalanceCriteria.FirstLetter = string.Empty;
                }

                var rawData = _superBalanceRepository.GetSuperBalance(superBalanceCriteria, ref outputParam);

                if (rawData == null)
                {
                    return result;
                }

                if (superBalanceCriteria.IsViewTotalOnly)
                {
                    var superBalanceInfo = new SuperBalance()
                    {
                        CustId = null,
                        Username = null,
                        Currency = null,
                        Balance = 0,
                        BaseBalance = Converter.ToDecimal(rawData[0])
                    };

                    result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                    result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
                    result.HasData = !Convert.IsDBNull(rawData[0]);
                }
                else
                {
                    foreach (var row in rawData)
                    {
                        var balance = Converter.ToDecimal(row.Balance);
                        var baseBalance = Converter.ToDecimal(row.BaseBalance);

                        if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                            Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                        {
                            continue;
                        }

                        var superBalanceInfo = new SuperBalance()
                        {
                            CustId = row.CustId,
                            Username = row.Username,
                            Currency = row.Currency,
                            Balance = balance,
                            BaseBalance = baseBalance
                        };

                        result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                        superBalanceInfo = SuperBalance.FormatInfo(superBalanceInfo);
                        result.SuperBalanceList.Add(superBalanceInfo);
                    }

                    result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
                }

                SessionManager.Set(_superBalanceSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<SuperBalance>(_superBalanceSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the super balance super down line.
        /// </summary>
        /// <param name="superBalanceSuperDownlineCriteria">The super balance super down line criteria.</param>
        /// <returns>The super balance super down line.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperBalance GetSuperBalanceSuperDownLine(GetSuperBalanceSuperDownlineCriteria superBalanceSuperDownlineCriteria)
        {
            var result = new SuperBalance
            {
                SuperBalanceList = new List<SuperBalance>(),
                BaseBalanceTotal = new SuperBalance()
            };

            var outputParam = new Hashtable();

            try
            {
                var rawData = _superBalanceRepository.GetSuperBalanceDownline(superBalanceSuperDownlineCriteria, ref outputParam);

                if (rawData == null)
                {
                    return result;
                }

                foreach (var row in rawData)
                {
                    var balance = Converter.ToDecimal(row.Balance);
                    var baseBalance = Converter.ToDecimal(row.BaseBalance);

                    if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                        Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                    {
                        continue;
                    }

                    var superBalanceInfo = new SuperBalance()
                    {
                        CustId = row.CustId,
                        Username = row.Username,
                        Currency = row.Currency,
                        Balance = balance,
                        BaseBalance = baseBalance
                    };

                    result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                    superBalanceInfo = SuperBalance.FormatInfo(superBalanceInfo);
                    result.SuperBalanceList.Add(superBalanceInfo);
                }

                result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
                SessionManager.Set<SuperBalance>(_superDownLineSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<SuperBalance>(_superDownLineSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the super balance master down line.
        /// </summary>
        /// <param name="superBalanceMasterDownlineCriteria">The super balance master down line criteria.</param>
        /// <returns>The super balance master down line.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperBalance GetSuperBalanceMasterDownLine(GetSuperBalanceMasterDownlineCriteria superBalanceMasterDownlineCriteria)
        {
            var result = new SuperBalance
            {
                SuperBalanceList = new List<SuperBalance>(),
                BaseBalanceTotal = new SuperBalance()
            };

            var outputParam = new Hashtable();

            try
            {
                var rawData = _superBalanceRepository.GetSuperBalanceMasterDownline(superBalanceMasterDownlineCriteria, ref outputParam);

                if (rawData == null)
                {
                    return result;
                }

                foreach (var row in rawData)
                {
                    var balance = Converter.ToDecimal(row.Balance);
                    var baseBalance = Converter.ToDecimal(row.BaseBalance);

                    if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                        Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                    {
                        continue;
                    }

                    var superBalanceInfo = new SuperBalance()
                    {
                        CustId = row.CustId,
                        Username = row.Username,
                        Currency = row.Currency,
                        Balance = balance,
                        BaseBalance = baseBalance
                    };

                    result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                    superBalanceInfo = SuperBalance.FormatInfo(superBalanceInfo);
                    result.SuperBalanceList.Add(superBalanceInfo);
                }

                result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
                SessionManager.Set<SuperBalance>(_masterDownLineSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<SuperBalance>(_masterDownLineSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the super balance agent down line.
        /// </summary>
        /// <param name="superBalanceAgentDownlineCriteria">The super balance agent down line criteria.</param>
        /// <returns>The super balance agent down line.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperBalance GetSuperBalanceAgentDownLine(GetSuperBalanceAgentDownlineCriteria superBalanceAgentDownlineCriteria)
        {
            var result = new SuperBalance
            {
                SuperBalanceList = new List<SuperBalance>(),
                BaseBalanceTotal = new SuperBalance()
            };

            var outputParam = new Hashtable();

            try
            {
                var rawData = _superBalanceRepository.GetSuperBalanceDownline(superBalanceAgentDownlineCriteria, ref outputParam);

                if (rawData == null)
                {
                    return result;
                }

                foreach (var row in rawData)
                {
                    var balance = Converter.ToDecimal(row.Balance);
                    var baseBalance = Converter.ToDecimal(row.BaseBalance);

                    if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                        Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                    {
                        continue;
                    }

                    var superBalanceInfo = new SuperBalance()
                    {
                        CustId = row.CustId,
                        Username = row.Username,
                        Currency = row.Currency,
                        Balance = balance,
                        BaseBalance = row.BaseBalance
                    };

                    result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                    superBalanceInfo = SuperBalance.FormatInfo(superBalanceInfo);
                    result.SuperBalanceList.Add(superBalanceInfo);
                }

                result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
                SessionManager.Set<SuperBalance>(_agentDownLineSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<SuperBalance>(_agentDownLineSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the super balance report.
        /// </summary>
        /// <param name="superBalanceCriteria">The super balance criteria.</param>
        /// <returns> The super balance report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperBalance ExportSuperBalanceReport(GetSuperBalanceCriteria superBalanceCriteria)
        {
            var result = new SuperBalance();
            string balanceType = string.Empty;

            try
            {
                var fullReport = SessionManager.Get<SuperBalance>(_superBalanceSessionKey);

                if (fullReport == null)
                {
                    fullReport = this.GetSuperBalance(superBalanceCriteria);
                }

                balanceType = GetBalanceType(superBalanceCriteria.BalanceType);

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = BalanceResources.SuperBalance_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportHeader(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    RoleConstants.Super,
                    superBalanceCriteria);

                this.ExportReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport);

                var customerType = ReportService.GetRoleName(RoleConstants.Super);
                var reportName = customerType + balanceType + Labels.Balance;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        /// <summary>
        /// Exports the balance super down line report.
        /// </summary>
        /// <param name="getSuperBalanceSuperCriteria">The get super balance super criteria.</param>
        /// <returns>The balance super down line report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperBalance ExportBalanceSuperDownLineReport(GetSuperBalanceSuperDownlineCriteria getSuperBalanceSuperCriteria)
        {
            var result = new SuperBalance();
            string balanceType = string.Empty;
            try
            {
                var fullReport = SessionManager.Get<SuperBalance>(_superDownLineSessionKey);

                if (fullReport == null)
                {
                    fullReport = this.GetSuperBalanceSuperDownLine(getSuperBalanceSuperCriteria);
                }

                balanceType = GetBalanceType(getSuperBalanceSuperCriteria.BalanceType);

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = BalanceResources.SuperBalance_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportHeaderDownline(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    RoleConstants.Master,
                    balanceType);

                this.ExportReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport);

                var customerType = ReportService.GetRoleName(RoleConstants.Master);
                var reportName = customerType + balanceType + Labels.Balance;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        /// <summary>
        /// Exports the balance master down line report.
        /// </summary>
        /// <param name="getSuperBalanceMasterCriteria">The get super balance master criteria.</param>
        /// <returns>The balance master down line report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperBalance ExportBalanceMasterDownLineReport(GetSuperBalanceMasterDownlineCriteria getSuperBalanceMasterCriteria)
        {
            var result = new SuperBalance();
            string balanceType = string.Empty;

            try
            {
                var fullReport = SessionManager.Get<SuperBalance>(_masterDownLineSessionKey);

                if (fullReport == null)
                {
                    fullReport = this.GetSuperBalanceMasterDownLine(getSuperBalanceMasterCriteria);
                }

                balanceType = GetBalanceType(getSuperBalanceMasterCriteria.BalanceType);

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = BalanceResources.SuperBalance_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportHeaderDownline(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    RoleConstants.Agent,
                    balanceType);

                this.ExportReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport);

                var customerType = ReportService.GetRoleName(RoleConstants.Agent);
                var reportName = customerType + balanceType + Labels.Balance;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        /// <summary>
        /// Exports the balance agent down line report.
        /// </summary>
        /// <param name="getSuperBalanceAgentCriteria">The get super balance agent criteria.</param>
        /// <returns>The balance agent down line report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperBalance ExportBalanceAgentDownLineReport(GetSuperBalanceAgentDownlineCriteria getSuperBalanceAgentCriteria)
        {
            var result = new SuperBalance();
            string balanceType = string.Empty;

            try
            {
                var fullReport = SessionManager.Get<SuperBalance>(_agentDownLineSessionKey);

                if (fullReport == null)
                {
                    fullReport = this.GetSuperBalanceAgentDownLine(getSuperBalanceAgentCriteria);
                }

                balanceType = GetBalanceType(getSuperBalanceAgentCriteria.BalanceType);

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = BalanceResources.SuperBalance_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportHeaderDownline(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    RoleConstants.Member,
                    balanceType);

                this.ExportReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport);

                var customerType = ReportService.GetRoleName(RoleConstants.Member);
                var reportName = customerType + balanceType + Labels.Balance;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        /// <summary>
        /// Gets the currencies.
        /// </summary>
        /// <returns>The currencies.</returns>
        public List<EntityBase<int, string>> GetCurrencies()
        {
            var result = new List<EntityBase<int, string>>();
            try
            {
                Dictionary<int, string> currList = ReportService.ExchangeCurrencies;

                foreach (var row in currList)
                {
                    result.Add(new EntityBase<int, string>()
                    {
                        Id = row.Key,
                        Name = row.Value
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        /// <summary>
        /// Gets the list alphabet.
        /// </summary>
        /// <returns>The list alphabet.</returns>
        public List<string> GetListAlphabet()
        {
            List<string> arrLetter = new List<string>();
            arrLetter.Add("All");
            arrLetter.Add("0-9");
            for (var i = 'A'; i <= 'Z'; i++)
            {
                arrLetter.Add(((char)i).ToString());
            }

            return arrLetter;
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="superBalanceCriteria">The super balance criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int roleId,
            GetSuperBalanceCriteria superBalanceCriteria)
        {
            CellRange currentCellRange;
            ExcelCell currentCell;
            string balanceType = string.Empty;

            balanceType = GetBalanceType(superBalanceCriteria.BalanceType);

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
            currentCellRange.Value = string.Format("{0} {1} {2}", title, balanceType, Labels.Balance);
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;

            if (superBalanceCriteria.IsViewTotalOnly)
            {
                return;
            }

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
        /// Exports the report header down line.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="balanceType">Type of the balance.</param>
        public void ExportReportHeaderDownline(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int roleId,
            string balanceType)
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
            currentCellRange.Value = string.Format("{0} {1} {2}", title, balanceType, Labels.Balance);
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
            SuperBalance report)
        {
            if (report != null &&
                report.SuperBalanceList != null &&
                report.SuperBalanceList.Count > 0)
            {
                var reportLength = report.SuperBalanceList.Count;
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

                    this.ExportSuperBalanceDetail(
                        ref worksheet,
                        i + 1,
                        startRow,
                        startColumn,
                        ref currentRow,
                        report.SuperBalanceList[i],
                        rowType);
                }

                this.ExportSuperBalanceTotal(ref worksheet, startRow, startColumn, ref currentRow, report.BaseBalanceTotal.BaseBalance);
            }
            else if (report != null &&
                report.BaseBalanceTotal != null &&
                report.SuperBalanceList.Count == 0 &&
                report.HasData)
            {
                this.ExportSuperBalanceTotal(ref worksheet, startRow, startColumn, ref currentRow, report.BaseBalanceTotal.BaseBalance);
            }
            else
            {
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 4);
            }
        }

        /// <summary>
        /// Exports the super balance detail.
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
        private void ExportSuperBalanceDetail(
           ref ExcelWorksheet worksheet,
           int index,
           int startRow,
           int startColumn,
           ref int currentRow,
           SuperBalance detail,
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
            currentCell.Value = detail.Balance;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.BaseBalance;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the super balance total.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="total">The total.</param>
        private void ExportSuperBalanceTotal(
           ref ExcelWorksheet worksheet,
           int startRow,
           int startColumn,
           ref int currentRow,
           decimal total)
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
            currentCell.Value = total;
            currentCell.Style = twoDecimalNumberStyle;
        }

        /// <summary>
        /// Gets the type of the balance.
        /// </summary>
        /// <param name="balanceTypeVal">The balance type value.</param>
        /// <returns>The type of the balance.</returns>
        private string GetBalanceType(int balanceTypeVal)
        {
            string balanceType = string.Empty;

            if (balanceTypeVal == (int)BalanceType.Today)
            {
                balanceType = Labels.Today;
            }
            else
            {
                balanceType = Labels.Yesterday;
            }

            return balanceType;
        }
    }
}