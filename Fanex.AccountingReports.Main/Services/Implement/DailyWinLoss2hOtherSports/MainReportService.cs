// --------------------------------------------------------------------------
// <copyright file="MainReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hOtherSports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hOtherSports;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hOtherSportsService
    {
        /// <summary>
        /// Gets the main2h report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The main2h report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetMain2hReport(GetMain2hReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    EightRMTotal = new WinLossInfo
                    {
                        Name = Labels.EightRM
                    },
                    BARMTotal = new WinLossInfo
                    {
                        Name = Labels.BARM
                    },
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit
                    },
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Grand_Total
                    },
                    NormalTotal = new WinLossInfo
                    {
                        Name = Labels.Virtual_All_Total
                    },
                    ChildsInfo = new List<WinLossInfo>()
                };

                Hashtable outParams = new Hashtable();
                var rawData = this._reportRepository.GetMain2hReport(criteria, ref outParams);
                if (rawData != null)
                {
                    var sportIdList = rawData.Select(info => Converter.ToInt(info.SportType)).Distinct().ToList();
                    this.SaveSportTypeCacheRef(sportIdList, ref this._sportTypeNameCache);
                    var sportGroups = rawData.GroupBy(data => data.SportType);
                    var isIBC = false;
                    foreach (var sportGroup in sportGroups)
                    {
                        var sportWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(sportGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo(),
                            EightRMTotal = new WinLossInfo(),
                            BARMTotal = new WinLossInfo(),
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit,
                                EnableVirtual = true
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit,
                                EnableVirtual = true
                            },
                            NormalTotal = new WinLossInfo
                            {
                                EnableVirtual = true
                            }
                        };

                        sportWinLoss.Name = this._sportTypeNameCache[sportWinLoss.Id];
                        sportWinLoss.NormalTotal.Id = sportWinLoss.Id;
                        sportWinLoss.NormalTotal.Name = sportWinLoss.Name;
                        sportWinLoss.EightRMTotal.Name = sportWinLoss.Name + " " + Labels.EightRM;
                        sportWinLoss.BARMTotal.Name = sportWinLoss.Name + " " + Labels.BARM;
                        isIBC = this.IsSportBookSport(sportWinLoss.Id);
                        foreach (var data in sportGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = sportWinLoss.Id,
                                Name = sportWinLoss.Name,
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                                IBC = Converter.ToInt(data.ibc),
                                SiteType = Converter.ToInt(data.Site)
                            };

                            var rowType = this.GetRowType(info.IBC, info.SiteType);
                            if ((!this._barmConfig.IsActive && rowType == RowType.BARM) ||
                                (!this._8rmConfig.IsActive && rowType == RowType.EightRM))
                            {
                                continue;
                            }

                            //// CalculateReportTotal

                            switch (rowType)
                            {
                                case RowType.Normal:
                                    info.EnableVirtual = this.IsApplyVirtualRate(info.Id);
                                    report.NormalTotal.Add(info);
                                    sportWinLoss.GrandTotal.Add(info);
                                    sportWinLoss.ChildsInfo.Add(info);
                                    break;
                                case RowType.EightRM:
                                    info.AliasName = info.Name + " " + Labels.EightRM;
                                    report.EightRMTotal.Add(info);
                                    sportWinLoss.EightRMTotal.Add(info);
                                    break;
                                case RowType.BARM:
                                    info.AliasName = info.Name + " " + Labels.BARM;
                                    report.BARMTotal.Add(info);
                                    sportWinLoss.BARMTotal.Add(info);
                                    break;
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    sportWinLoss.CreditTotal.Add(info);
                                    break;
                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    sportWinLoss.DepositTotal.Add(info);
                                    break;
                            }

                            //// CalculateReportTotal
                        }

                        sportWinLoss.NormalTotal.Add(sportWinLoss.CreditTotal);
                        sportWinLoss.NormalTotal.Add(sportWinLoss.DepositTotal);
                        report.CreditTotal.Add(sportWinLoss.CreditTotal);
                        report.DepositTotal.Add(sportWinLoss.DepositTotal);
                        report.NormalTotal.Add(sportWinLoss.NormalTotal);

                        sportWinLoss = WinLossInfo.Format_Total(sportWinLoss);
                        report.ChildsInfo.Add(sportWinLoss);
                    }

                    report.GrandTotal.Add(report.CreditTotal);
                    report.GrandTotal.Add(report.DepositTotal);
                    report.GrandTotal.Add(report.EightRMTotal);
                    report.GrandTotal.Add(report.BARMTotal);
                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hOtherSportMain2hReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hOtherSportMain2hReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the report direct filter.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport_DirectFilter(GetMain2hReportCriteria criteria)
        {
            try
            {
                var report = this.GetMain2hReport(criteria);
                this.ExportMain2hReport(criteria, report);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the main2h report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportMain2hReport(GetMain2hReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hOtherSportMain2hReportSessionKey);
                if (report == null)
                {
                    report = this.GetMain2hReport(criteria);
                }

                this.ExportMain2hReport(criteria, report);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the main2h report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportMain2hReport(
            GetMain2hReportCriteria criteria,
            WinLossInfo report)
        {
            //// ExcelTypes

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = MainResources.DailyWinLoss2h_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 10;
            var currentRow = startRow;

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, true, false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);

            var subTotalRowColor = ExcelExtensions.BackGroundColor;
            var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
            var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, subTotalRowColor, false, false, false);
            var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                false,
                true,
                false);
            var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                subTotalRowColor,
                false,
                true,
                false);

            //// ExcelTypes
            //// MakeExcelFile

            //// Header

            workSheet.Columns[startColumn].Width = 190 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 130 * ExcelExtensions.FontWidth;
            }

            var title = MainResources.DailyWinLoss2h_OtherSportReport_MainReport_Title;
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            // FirstRow
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Sport_Type;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member_Win_Loss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.SMA_Win_Loss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Commission;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Income;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Variance;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Turnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Margin;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;
            currentRow++;

            // SecondRow
            currentColumn = startColumn + 4;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            currentColumn++; // VarianceColumn

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;
            currentRow++;

            //// Header

            if (report != null && report.ChildsInfo.Count > 0)
            {
                var sportLength = report.ChildsInfo.Count;
                WinLossInfo info;
                for (var i = 0; i < sportLength; i++)
                {
                    var sportWinLoss = report.ChildsInfo[i];
                    info = sportWinLoss.NormalTotal;
                    this.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                       ref twoDecimalNumberStyle,
                       ref twoDecimalPositiveNumberStyle,
                       ref twoDecimalPositivePercentNumberStyle,
                       true);

                    this.ExportNormalRow(
                        ref workSheet,
                        textStyle,
                        twoDecimalNumberStyle,
                        twoDecimalPositiveNumberStyle,
                        twoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType.Normal);

                    info = sportWinLoss.CreditTotal;
                    this.ExportNormalRow(
                        ref workSheet,
                        subTotalTextStyle,
                        subTotalTwoDecimalNumberStyle,
                        subTotalTwoDecimalPositiveNumberStyle,
                        subTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportCredit);

                    info = sportWinLoss.DepositTotal;
                    this.ExportNormalRow(
                        ref workSheet,
                        subTotalTextStyle,
                        subTotalTwoDecimalNumberStyle,
                        subTotalTwoDecimalPositiveNumberStyle,
                        subTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportDeposit);

                    info = sportWinLoss.EightRMTotal;
                    if (this._8rmConfig.IsActive && info.LineTotal != 0)
                    {
                        this.ExportRMRow(
                            ref workSheet,
                            subTotalTextStyle,
                            subTotalTwoDecimalNumberStyle,
                            subTotalTwoDecimalPositiveNumberStyle,
                            subTotalTwoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            maxColumnIndex,
                            ref currentRow);
                    }

                    info = sportWinLoss.BARMTotal;
                    if (this._8rmConfig.IsActive && info.LineTotal != 0)
                    {
                        this.ExportRMRow(
                            ref workSheet,
                            subTotalTextStyle,
                            subTotalTwoDecimalNumberStyle,
                            subTotalTwoDecimalPositiveNumberStyle,
                            subTotalTwoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            maxColumnIndex,
                            ref currentRow);
                    }

                    workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                this.SetOddEvenColor_ExportExcel(
                    ref textStyle,
                    ref twoDecimalNumberStyle,
                    ref twoDecimalPositiveNumberStyle,
                    ref twoDecimalPositivePercentNumberStyle,
                    false);

                //// GrandTotal

                // GrandTotalValue
                var grandTotalSeparatorColor = ExcelExtensions.GrandTotalSeparatorColor;
                info = report.GrandTotal;
                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Name;
                currentCell.Style = grandTotalRowTextStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Value = info.WinLoss;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Value = info.Position;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Value = info.Commission;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.LineTotal;
                currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VirtualLineTotal;
                currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Variance;
                currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Value = info.Turnover;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Value = info.VirtualTurnover;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = grandTotalTwoDecimalPositivePercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                this.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = grandTotalTwoDecimalPositivePercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                this.ExportColumnTurnover(info.VirtualMargin, info.VirtualTurnover, ref currentCell);
                currentColumn++;
                currentRow++;

                //// GrandTotal
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            //// MakeExcelFile

            var reportName = string.Format(
                MainResources.DailyWinLoss2h_ReportName,
                criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the normal row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="info">The information.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="rowType">Type of the row.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNormalRow(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int rowType)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            switch (rowType)
            {
                case RowType._2hReportCredit:
                case RowType._2hReportDeposit:
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualLineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.Variance;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Turnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualTurnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            this.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            this.ExportColumnTurnover(info.VirtualMargin, info.VirtualTurnover, ref currentCell);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the RM row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="info">The information.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="currentRow">The current row.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportRMRow(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            int maxColumnIndex,
            ref int currentRow)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualLineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Variance;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Turnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualTurnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            this.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            this.ExportColumnTurnover(info.VirtualMargin, info.VirtualTurnover, ref currentCell);
            currentColumn++;
            currentRow++;
        }
    }
}
