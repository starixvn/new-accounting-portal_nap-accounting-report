// --------------------------------------------------------------------------
// <copyright file="NumberGameWinLossByCurrencyService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2013.02.19">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.NumberGameWinLossByCurrency
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByCurrency;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByCurrency;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinLossByCurrencyService : ServiceBase, INumberGameWinLossByCurrencyService
    {
        public NumberGameWinLossByCurrencyService(IDynamicRepository dynamicRepository = null) :
            base(dynamicRepository)
        {
        }

        /// <summary>
        /// Gets the number game win loss by currency main.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The number game win loss by currency main.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<NumberGameWinLossByCurrencyMain> GetNumberGameWinLossByCurrencyMain(NumberGameWinLossByCurrencyMainCriteria criteria)
        {
            var numberGameMainList = new List<NumberGameWinLossByCurrencyMain>();
            try
            {
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    NumberGameWinLossByCurrencyMain grandTotal = new NumberGameWinLossByCurrencyMain
                    {
                        Currency = Labels.Grand_Total,
                        Turnover = 0,
                        MemberWinLoss = 0,
                        GamePayout = 0,
                        NetTurnover = 0,
                        PTWinLoss = 0,
                        CommExpense = 0,
                        NetIncome = 0,
                        Margin = 0
                    };

                    decimal turnoverTmp = 0;
                    decimal memberWinLossTmp = 0;
                    decimal netTurnoverTmp = 0;
                    decimal winLossPTTmp = 0;
                    decimal comExpenseTmp = 0;
                    decimal netIncomeTmp = 0;

                    foreach (var row in rawData)
                    {
                        turnoverTmp = Converter.ToDecimal(row.TurnOver);
                        memberWinLossTmp = Converter.ToDecimal(row.MemberWinloss);
                        netTurnoverTmp = Converter.ToDecimal(row.NetTurnOver);
                        winLossPTTmp = Converter.ToDecimal(row.PTWinloss);
                        comExpenseTmp = Converter.ToDecimal(row.CommExpense);
                        netIncomeTmp = Converter.ToDecimal(row.NetIncome);

                        grandTotal.Turnover += turnoverTmp;
                        grandTotal.MemberWinLoss += memberWinLossTmp;
                        grandTotal.NetTurnover += netTurnoverTmp;
                        grandTotal.PTWinLoss += winLossPTTmp;
                        grandTotal.CommExpense += comExpenseTmp;
                        grandTotal.NetIncome += netIncomeTmp;

                        numberGameMainList.Add(new NumberGameWinLossByCurrencyMain
                        {
                            Currency = row.CurrencyName.ToString(),
                            Turnover = turnoverTmp,
                            MemberWinLoss = memberWinLossTmp,
                            GamePayout = Converter.ToDecimal(row.GamePayOut),
                            NetTurnover = netTurnoverTmp,
                            PTWinLoss = winLossPTTmp,
                            CommExpense = comExpenseTmp,
                            NetIncome = netIncomeTmp,
                            Margin = Converter.ToDecimal(row.Margin)
                        });
                    }

                    if (numberGameMainList.Count > 0)
                    {
                        grandTotal.GamePayout = grandTotal.MemberWinLoss / grandTotal.Turnover;
                        grandTotal.Margin = grandTotal.NetIncome / grandTotal.NetTurnover;
                        numberGameMainList.Add(grandTotal);
                    }
                }

                SessionManager.Set<List<NumberGameWinLossByCurrencyMain>>(GamingConstants.NumberGameWinLossByCurrencyMainSessionKey, numberGameMainList);
                return numberGameMainList;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the number game win loss by currency detail.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The number game win loss by currency detail.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<NumberGameWinLossByCurrencyDetail> GetNumberGameWinLossByCurrencyDetail(NumberGameWinLossByCurrencyDetailCriteria criteria)
        {
            var numberGameDetailList = new List<NumberGameWinLossByCurrencyDetail>();
            try
            {
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    NumberGameWinLossByCurrencyDetail grandTotal = new NumberGameWinLossByCurrencyDetail
                    {
                        Username = "Total",
                        Turnover = 0,
                        GrossComm = 0,
                        MemberWinLoss = 0,
                        MemberComm = 0,
                        MemberTotal = 0,
                        AgentWinLoss = 0,
                        AgentComm = 0,
                        AgentTotal = 0,
                        MasterWinLoss = 0,
                        MasterComm = 0,
                        MasterTotal = 0,
                        SuperWinLoss = 0,
                        SuperComm = 0,
                        SuperTotal = 0,
                        Company = 0,
                        RoleID = -1,
                        CustID = 0
                    };

                    string userNameTmp = string.Empty;
                    decimal turnoverTmp = 0;
                    decimal grossCommTmp = 0;
                    decimal memberWinLossTmp = 0;
                    decimal memberCommTmp = 0;
                    decimal memberTotalTmp = 0;
                    decimal agentWinLossTmp = 0;
                    decimal agentCommTmp = 0;
                    decimal agentTotalTmp = 0;
                    decimal masterWinLossTmp = 0;
                    decimal masterCommTmp = 0;
                    decimal masterTotalTmp = 0;
                    decimal superWinLossTmp = 0;
                    decimal superCommTmp = 0;
                    decimal superTotalTmp = 0;
                    decimal companyTmp = 0;

                    foreach (var row in rawData)
                    {
                        turnoverTmp = Converter.ToDecimal(row.TurnOver);
                        grossCommTmp = Converter.ToDecimal(row.GrossComm);
                        memberWinLossTmp = Converter.ToDecimal(row.memberWL);
                        memberCommTmp = Converter.ToDecimal(row.memberComm);
                        memberTotalTmp = Converter.ToDecimal(row.memberTotal);
                        agentWinLossTmp = Converter.ToDecimal(row.agentWL);
                        agentCommTmp = Converter.ToDecimal(row.agentComm);
                        agentTotalTmp = Converter.ToDecimal(row.agentTotal);
                        masterWinLossTmp = Converter.ToDecimal(row.masterWL);
                        masterCommTmp = Converter.ToDecimal(row.masterComm);
                        masterTotalTmp = Converter.ToDecimal(row.masterTotal);
                        superWinLossTmp = Converter.ToDecimal(row.superWL);
                        superCommTmp = Converter.ToDecimal(row.superComm);
                        superTotalTmp = Converter.ToDecimal(row.superTotal);
                        companyTmp = Converter.ToDecimal(row.Company);

                        grandTotal.Turnover += turnoverTmp;
                        grandTotal.GrossComm += grossCommTmp;
                        grandTotal.MemberWinLoss += memberWinLossTmp;
                        grandTotal.MemberComm += memberCommTmp;
                        grandTotal.MemberTotal += memberTotalTmp;
                        grandTotal.AgentWinLoss += agentWinLossTmp;
                        grandTotal.AgentComm += agentCommTmp;
                        grandTotal.AgentTotal += agentTotalTmp;
                        grandTotal.MasterWinLoss += masterWinLossTmp;
                        grandTotal.MasterComm += masterCommTmp;
                        grandTotal.MasterTotal += masterTotalTmp;
                        grandTotal.SuperWinLoss += superWinLossTmp;
                        grandTotal.SuperComm += superCommTmp;
                        grandTotal.SuperTotal += superTotalTmp;
                        grandTotal.Company += companyTmp;

                        numberGameDetailList.Add(new NumberGameWinLossByCurrencyDetail
                        {
                            Username = row.username.ToString(),
                            Turnover = turnoverTmp,
                            GrossComm = grossCommTmp,
                            MemberWinLoss = memberWinLossTmp,
                            MemberComm = memberCommTmp,
                            MemberTotal = memberTotalTmp,
                            AgentWinLoss = agentWinLossTmp,
                            AgentComm = agentCommTmp,
                            AgentTotal = agentTotalTmp,
                            MasterWinLoss = masterWinLossTmp,
                            MasterComm = masterCommTmp,
                            MasterTotal = masterTotalTmp,
                            SuperWinLoss = superWinLossTmp,
                            SuperComm = superCommTmp,
                            SuperTotal = superTotalTmp,
                            Company = companyTmp,
                            RoleID = Converter.To<int>(row.RoleID),
                            CustID = Converter.To<int>(row.CustID),
                            Currency = criteria.Currency
                        });
                    }

                    if (numberGameDetailList.Count > 0)
                    {
                        numberGameDetailList.Add(grandTotal);
                    }
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            switch (criteria.RoleID)
            {
                case 0:
                    SessionManager.Set<List<NumberGameWinLossByCurrencyDetail>>(GamingConstants.NumberGameWinLossByCurrencySuperSessionKey, numberGameDetailList);
                    break;

                case 3:
                    SessionManager.Set<List<NumberGameWinLossByCurrencyDetail>>(GamingConstants.NumberGameWinLossByCurrencyMasterSessionKey, numberGameDetailList);
                    break;

                case 2:
                    SessionManager.Set<List<NumberGameWinLossByCurrencyDetail>>(GamingConstants.NumberGameWinLossByCurrencyAgentSessionKey, numberGameDetailList);
                    break;

                case 1:
                    SessionManager.Set<List<NumberGameWinLossByCurrencyDetail>>(GamingConstants.NumberGameWinLossByCUrrencyMemberSessionKey, numberGameDetailList);
                    break;
            }

            return numberGameDetailList;
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
                Dictionary<int, string> currList = ReportService.GetExchangeCurrencies(
                    new GetExchangeCurrenciesCriteria
                    {
                        ExcludeTest = true
                    });

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
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        #region Export excel for Main

        /// <summary>
        /// Gets the export excel number game win loss by current main.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void GetExportExcelNumberGameWinLossByCurMain(NumberGameWinLossByCurrencyMainCriteria criteria)
        {
            var getCache = SessionManager.Get<List<NumberGameWinLossByCurrencyMain>>(GamingConstants.NumberGameWinLossByCurrencyMainSessionKey);
            getCache = getCache ?? this.GetNumberGameWinLossByCurrencyMain(criteria).ToList();
            var starCols = 0;
            var starRows = 3;

            var excelFile = new ExcelFile();

            if (getCache.Count > 0)
            {
                this.ExportNumberGameWinLossByCurMain(starRows, starCols, ref excelFile, getCache, criteria);
            }

            excelFile.ReponseStream(GamingResources.NumberGameWinLossByCurrency_Main_ReportName);
        }

        #endregion Export excel for Main

        #region Export for Detail

        /// <summary>
        /// Gets the export excel number game win loss by current detail.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void GetExportExcelNumberGameWinLossByCurDetail(NumberGameWinLossByCurrencyDetailCriteria criteria)
        {
            var getCache = new List<NumberGameWinLossByCurrencyDetail>();
            switch (criteria.RoleID)
            {
                case 0:
                    getCache = SessionManager.Get<List<NumberGameWinLossByCurrencyDetail>>(GamingConstants.NumberGameWinLossByCurrencySuperSessionKey);
                    break;

                case 3:
                    getCache = SessionManager.Get<List<NumberGameWinLossByCurrencyDetail>>(GamingConstants.NumberGameWinLossByCurrencyMasterSessionKey);
                    break;

                case 2:
                    getCache = SessionManager.Get<List<NumberGameWinLossByCurrencyDetail>>(GamingConstants.NumberGameWinLossByCurrencyAgentSessionKey);
                    break;

                case 1:
                    getCache = SessionManager.Get<List<NumberGameWinLossByCurrencyDetail>>(GamingConstants.NumberGameWinLossByCUrrencyMemberSessionKey);
                    break;
            }

            getCache = getCache ?? this.GetNumberGameWinLossByCurrencyDetail(criteria).ToList();
            var starCols = 0;
            var starRows = 3;
            var userNameTemp = string.IsNullOrEmpty(criteria.Username) ? criteria.Username : ("_USERNAME_" + criteria.Username);
            var reportName = GamingResources.NumberGameWinLossByCurrency_Detail_ReportName + criteria.Currency + userNameTemp;
            var excelFile = new ExcelFile();
            int maxSheetItems = 3000;
            if (getCache.Count > 0)
            {
                this.ExportNumberGameWinLossByCurDetail(starRows, starCols, maxSheetItems, ref excelFile, getCache, criteria);
            }

            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the number game win loss by current main assign header.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="workSheetReport">The work sheet report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNumberGameWinLossByCurMainAssignHeader(
            int startRow,
            int startCol,
            ref ExcelWorksheet workSheetReport,
            NumberGameWinLossByCurrencyMainCriteria criteria)
        {
            var backGroundHeaderFormat = ExcelExtensions.GetCellTitleStyle(Color.FromArgb(60, 94, 165));
            var nextCol = 0;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Value = Labels.Currency;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol += 1), startRow, startCol + nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Value = Labels.Turnover;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol += 1), startRow, startCol + nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Value = Labels.Member_WL;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol += 1), startRow, startCol + nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Value = Labels.GamePayout;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 5).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 5).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 5).Value = Labels.Company;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Net_Turnover;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.PT_WL;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Comm_Expense;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Net_Income;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Margin;
            workSheetReport.Columns[startCol + nextCol].Width = 255 * 15;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 3, startCol, startRow - 3, nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 3, startCol, startRow - 3, nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 3, startCol, startRow - 3, nextCol).Value = string.Format(
                "{0} From {1:MMM, dd, yyyy} To {2:MMM, dd, yyyy}",
                GamingResources.NumberGameWinLossByCurrency_Main_Title,
                Convert.ToDateTime(criteria.FromDate),
                Convert.ToDateTime(criteria.ToDate));
        }

        /// <summary>
        /// Exports the number game win loss by current main.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="dataList">The data list.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNumberGameWinLossByCurMain(
            int startRow,
            int startCol,
            ref ExcelFile excelFile,
            List<NumberGameWinLossByCurrencyMain> dataList,
            NumberGameWinLossByCurrencyMainCriteria criteria)
        {
            var totalSheet = dataList.Count;
            var cellStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.OddRowColor, false);
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var positiveNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
            var numberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, true, false);
            var numberMarginStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.BackGroundColor,
                false,
                true,
                false);

            var nextRow = 0;

            var workSheetReport = excelFile.Worksheets.Add(string.Format("NumberGameWinLossByCurrencyMain"));
            this.ExportNumberGameWinLossByCurMainAssignHeader(startRow, startCol, ref workSheetReport, criteria);

            startRow++;
            for (int i = 0; i < totalSheet - 1; i++)
            {
                var item = dataList[i];
                var nextCol = 0;

                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Style = cellStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.Currency;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.Turnover;

                workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.MemberWinLoss;

                workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberMarginStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.GamePayout;

                workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.NetTurnover;

                workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.PTWinLoss;

                workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.CommExpense;

                workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.NetIncome;

                workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberMarginStyle;
                workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.Margin;

                nextRow++;
            }

            var grandTotalStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            var grandTotalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);
            var grandTotalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            var grandTotalNumberMarginStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);

            var grandTotal = dataList[dataList.Count - 1];
            var nextColGrandTotal = 0;

            CellRange currentCellRange;
            ExcelCell currentCell;

            currentCellRange = workSheetReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol);
            currentCellRange.Merged = true;
            currentCellRange.Style = grandTotalStyle;
            currentCellRange.Value = grandTotal.Currency;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            nextColGrandTotal++;

            currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
            currentCell.Style = grandTotalPositiveNumberStyle;
            currentCell.Value = Math.Round(grandTotal.Turnover, 2);
            nextColGrandTotal++;

            currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
            currentCell.Style = grandTotalNumberStyle;
            currentCell.Value = Math.Round(grandTotal.MemberWinLoss, 2);
            nextColGrandTotal++;

            currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
            currentCell.Style = grandTotalNumberMarginStyle;
            currentCell.Value = grandTotal.GamePayout;
            nextColGrandTotal++;

            currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
            currentCell.Style = grandTotalPositiveNumberStyle;
            currentCell.Value = Math.Round(grandTotal.NetTurnover, 2);
            nextColGrandTotal++;

            currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
            currentCell.Style = grandTotalNumberStyle;
            currentCell.Value = Math.Round(grandTotal.PTWinLoss, 2);
            nextColGrandTotal++;

            currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
            currentCell.Style = grandTotalPositiveNumberStyle;
            currentCell.Value = Math.Round(grandTotal.CommExpense, 2);
            nextColGrandTotal++;

            currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
            currentCell.Style = grandTotalNumberStyle;
            currentCell.Value = Math.Round(grandTotal.NetIncome, 2);
            nextColGrandTotal++;

            currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
            currentCell.Style = grandTotalNumberMarginStyle;
            currentCell.Value = grandTotal.Margin;
            nextColGrandTotal++;
        }

        /// <summary>
        /// Exports the excel number game win loss by current detail assign header.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="workSheetReport">The work sheet report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportExcelNumberGameWinLossByCurDetailAssignHeader(
            int startRow,
            int startCol,
            ref ExcelWorksheet workSheetReport,
            NumberGameWinLossByCurrencyDetailCriteria criteria)
        {
            var backGroundHeaderFormat = ExcelExtensions.GetCellTitleStyle(Color.FromArgb(60, 94, 165));
            var nextCol = 0;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Value = Labels.UserName;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol += 1), startRow, startCol + nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Value = Labels.Turnover;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol += 1), startRow, startCol + nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Value = Labels.GrossComm;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Value = Labels.Member;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.WinLoss;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Comm;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Total;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Value = Labels.Agent;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.WinLoss;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Comm;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Total;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Value = Labels.Master;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.WinLoss;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Comm;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Total;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol + 1), startRow - 1, startCol + nextCol + 3).Value = Labels.Super;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.WinLoss;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Comm;

            workSheetReport.Cells[startRow, startCol + (nextCol += 1)].Style = backGroundHeaderFormat;
            workSheetReport.Cells[startRow, startCol + nextCol].Value = Labels.Total;

            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + (nextCol += 1), startRow, startCol + nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 1, startCol + nextCol, startRow, startCol + nextCol).Value = Labels.Company;

            var userNameTemp = string.IsNullOrEmpty(criteria.Username) ? criteria.Username : (" - Username " + criteria.Username);
            var title = GamingResources.NumberGameWinLossByCurrency_Detail_Title + criteria.Currency + userNameTemp;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 3, startCol, startRow - 3, nextCol).Merged = true;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 3, startCol, startRow - 3, nextCol).Style = backGroundHeaderFormat;
            workSheetReport.Cells.GetSubrangeAbsolute(startRow - 3, startCol, startRow - 3, nextCol).Value = string.Format(
                "{0} - From {1:MMM, dd, yyyy} To {2:MMM, dd, yyyy}",
                title,
                Convert.ToDateTime(criteria.FromDate),
                Convert.ToDateTime(criteria.ToDate));
        }

        /// <summary>
        /// Exports the number game win loss by current detail.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="maxItemsInSheet">The maximum items in sheet.</param>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="dataList">The data list.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNumberGameWinLossByCurDetail(
            int startRow,
            int startCol,
            int maxItemsInSheet,
            ref ExcelFile excelFile,
            List<NumberGameWinLossByCurrencyDetail> dataList,
            NumberGameWinLossByCurrencyDetailCriteria criteria)
        {
            var totalSheet = (dataList.Count % maxItemsInSheet > 0 ? 1 : 0) + (dataList.Count / maxItemsInSheet);
            var cellStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.OddRowColor, false);
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var positiveNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
            var numberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, true, false);

            var nextRow = 0;
            var itemIndex = 0;

            for (int i = 0; i < totalSheet; i++)
            {
                var workSheetReport = excelFile.Worksheets.Add(string.Format("NumberGameWinLossByCurrencyDetail_{0}", i + 1));

                this.ExportExcelNumberGameWinLossByCurDetailAssignHeader(startRow, startCol, ref workSheetReport, criteria);

                var totalItemsInCurrentSheet = i < totalSheet - 1 ? maxItemsInSheet : (dataList.Count - (i * maxItemsInSheet));
                startRow++;
                for (int j = 0; j < totalItemsInCurrentSheet; j++)
                {
                    if ((i == totalSheet - 1) && (j == totalItemsInCurrentSheet - 1))
                    {
                        continue;
                    }

                    var item = dataList[itemIndex];
                    var nextCol = 0;

                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Style = cellStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.Username;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.Turnover;

                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.GrossComm;

                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.MemberWinLoss;
                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.MemberComm;
                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.MemberTotal;

                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.AgentWinLoss;
                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.AgentComm;
                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.AgentTotal;

                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.MasterWinLoss;
                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.MasterComm;
                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.MasterTotal;

                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.SuperWinLoss;
                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = positiveNumberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.SuperComm;
                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.SuperTotal;

                    workSheetReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberStyle;
                    workSheetReport.Cells[startRow + nextRow, startCol + nextCol].Value = item.Company;

                    nextRow++;
                    itemIndex++;
                }

                if (i == totalSheet - 1 && dataList.Count > 0)
                {
                    var grandTotalStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                    var grandTotalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                        NumberFormatConstants.TwoDecimalDigitFormat,
                        ExcelExtensions.GrandTotalRowColor,
                        true,
                        true,
                        false);
                    var grandTotalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                        NumberFormatConstants.TwoDecimalDigitFormat,
                        ExcelExtensions.GrandTotalRowColor,
                        true,
                        false,
                        false);

                    var total = dataList[dataList.Count - 1];
                    var nextColGrandTotal = 0;
                    CellRange currentCellRange;
                    ExcelCell currentCell;

                    currentCellRange = workSheetReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol);
                    currentCellRange.Merged = true;
                    currentCellRange.Style = grandTotalStyle;
                    currentCellRange.Value = total.Username;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    nextColGrandTotal++;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalPositiveNumberStyle;
                    currentCell.Value = total.Turnover;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalPositiveNumberStyle;
                    currentCell.Value = total.GrossComm;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.MemberWinLoss;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalPositiveNumberStyle;
                    currentCell.Value = total.MemberComm;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.MemberTotal;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.AgentWinLoss;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalPositiveNumberStyle;
                    currentCell.Value = total.AgentComm;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.AgentTotal;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.MasterWinLoss;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalPositiveNumberStyle;
                    currentCell.Value = total.MasterComm;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.MasterTotal;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.SuperWinLoss;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalPositiveNumberStyle;
                    currentCell.Value = total.SuperComm;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.SuperTotal;
                    nextColGrandTotal += 1;

                    currentCell = workSheetReport.Cells[startRow + nextRow, startCol + nextColGrandTotal];
                    currentCell.Style = grandTotalNumberStyle;
                    currentCell.Value = total.Company;
                    nextColGrandTotal += 1;

                    nextRow = 0;
                }
            }
        }

        #endregion Export for Detail
    }
}