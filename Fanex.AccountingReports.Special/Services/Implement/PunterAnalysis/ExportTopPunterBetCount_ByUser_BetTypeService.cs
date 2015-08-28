// --------------------------------------------------------------------------
// <copyright file="ExportTopPunterBetCount_ByUser_BetTypeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Implement.PunterAnalysis
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Special.Constants;
    using Fanex.AccountingReports.Special.Criteria.PunterAnalysis;
    using Fanex.AccountingReports.Special.Models.PunterAnalysis;
    using Fanex.AccountingReports.Special.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class PunterAnalysisService
    {
        /// <summary>
        /// Exports the type of the top punter bet count by user bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportTopPunterBetCount_ByUser_BetType(GetTopPunterBetCount_ByUser_BetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterBetCountByUserBetTypeSessionKey);
                if (report == null)
                {
                    report = this.GetTopPunterBetCount_ByUser_BetType(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = SpecialResources.PunterAnalysis_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 4; // 5 columns 

                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                // gtr: grandTotalRow
                // 2d: TwoDecimal
                var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                var intNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, ExcelExtensions.BackGroundColor, false, false, false);
                var _2dNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
                var _2dNegativeNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    false,
                    true);
                _2dNegativeNumberStyle.NumberFormat = string.Format(
                    NegativeNumberFormat,
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    NumberFormatConstants.TwoDecimalDigitFormat);
                var gtrTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                gtrTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var gtr2dNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                gtr2dNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var gtr2dNegativeNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    true);
                gtr2dNegativeNumberStyle_FirstRow.NumberFormat = string.Format(
                    NegativeNumberFormat,
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    NumberFormatConstants.TwoDecimalDigitFormat);
                gtr2dNegativeNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var gtr2dIntNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.IntNumberFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                gtr2dIntNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

                //// Header

                for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                var title = string.Format(
                    SpecialResources.PunterAnalysis_TopPunterBetCount_ByUser_ReportTitle,
                    criteria.UserName,
                    criteria.Fromdate.ToShortFormatedDate(),
                    criteria.Todate.ToShortFormatedDate(),
                    ReportService.GetPeriodTypeName(criteria.Reporttype));
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                var filters = string.Empty;
                if (criteria.ByLeague)
                {
                    filters = string.Format(
                        SpecialResources.PunterAnalysis_TopPunterBetCount_ByLeague_BetType_Filters,
                        criteria.LeagueName,
                        criteria.SportName,
                        criteria.BetTypeName,
                        criteria.LiveTypeName);
                }
                else
                {
                    filters = string.Format(
                        SpecialResources.PunterAnalysis_TopPunterBetCount_ByUser_BetType_Filters,
                        criteria.SportName,
                        criteria.BetTypeName,
                        criteria.LiveTypeName);
                }

                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = filters;
                currentCellRange.Style = headerTextStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                workSheet.Rows[currentRow + 1].Height = ExcelExtensions.FormatLongText(filters, 1, 2, 140 * ExcelExtensions.FontWidth * (maxColumnIndex + 1), true);
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Date;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Bet_Count;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Win_Count;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Draw_Count;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Win_Loss;
                currentCell.Style = headerTextStyle;
                currentColumn++;
                currentRow++;

                //// Header

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    var len = report.ChildsInfo.Count;
                    PunterWinLossInfo info;
                    for (var i = 0; i < len; i++)
                    {
                        //// NormalRow

                        info = report.ChildsInfo[i];
                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Name;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.BetCount;
                        currentCell.Style = intNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.WinCount;
                        currentCell.Style = intNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.DrawCount;
                        currentCell.Style = intNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.WinLoss;
                        currentCell.Style = _2dNegativeNumberStyle;
                        currentColumn++;
                        currentRow++;

                        //// NormalRow
                    }

                    //// GrandTotal

                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = gtrTextStyle_FirstRow;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BetCount;
                    currentCell.Style = gtr2dIntNumberStyle_FirstRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.WinCount;
                    currentCell.Style = gtr2dIntNumberStyle_FirstRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.DrawCount;
                    currentCell.Style = gtr2dIntNumberStyle_FirstRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.WinLoss;
                    currentCell.Style = gtr2dNegativeNumberStyle_FirstRow;
                    currentColumn++;
                    currentRow++;

                    //// GrandTotal
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                //// MakeExcelFile

                var reportName = SpecialResources.PunterAnalysis_TopPunterBetCount_ByUser_ReportName;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
