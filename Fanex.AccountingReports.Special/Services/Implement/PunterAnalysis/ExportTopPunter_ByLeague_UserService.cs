// --------------------------------------------------------------------------
// <copyright file="ExportTopPunter_ByLeague_UserService.cs" company="Nexcel Solutions Vietnam">
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
        /// Exports the top punter by league user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportTopPunter_ByLeague_User(GetTopPunter_ByLeague_UserCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByLeagueUserSessionKey);
                if (report == null)
                {
                    report = this.GetTopPunter_ByLeague_User(criteria);
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
                var _2dNegativePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    false,
                    true);
                _2dNegativePercentNumberStyle.NumberFormat = string.Format(
                    NegativeNumberFormat,
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    NumberFormatConstants.TwoDecimalDigitPercentFormat);

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
                var gtr2dNegativePercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    true);
                gtr2dNegativePercentNumberStyle_FirstRow.NumberFormat = string.Format(
                    NegativeNumberFormat,
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    NumberFormatConstants.TwoDecimalDigitPercentFormat);
                gtr2dNegativePercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
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
                    SpecialResources.PunterAnalysis_TopPunterByUser_ReportTitle,
                    criteria.FromDate.ToShortFormatedDate(),
                    criteria.ToDate.ToShortFormatedDate(),
                    ReportService.GetPeriodTypeName(criteria.Reporttype));
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                var filters = string.Format(
                    SpecialResources.PunterAnalysis_TopPunterByLeague_User_Filters,
                    criteria.UserName,
                    criteria.LeagueName,
                    criteria.OrderByName,
                    criteria.OrderTypeName,
                    criteria.SportName);
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = filters;
                currentCellRange.Style = headerTextStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                workSheet.Rows[currentRow + 2].Height = ExcelExtensions.FormatLongText(filters, 2, 3, 140 * ExcelExtensions.FontWidth * (maxColumnIndex + 1), true);
                currentRow += 3;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Sport_Type;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Win_Loss;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Turnover;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Margin;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Bet_Count;
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
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.WinLoss;
                        currentCell.Style = _2dNegativeNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Turnover;
                        currentCell.Style = _2dNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Margin;
                        currentCell.Style = _2dNegativePercentNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.BetCount;
                        currentCell.Style = intNumberStyle;
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
                    currentCell.Value = info.WinLoss;
                    currentCell.Style = gtr2dNegativeNumberStyle_FirstRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Turnover;
                    currentCell.Style = gtr2dNumberStyle_FirstRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Margin;
                    currentCell.Style = gtr2dNegativePercentNumberStyle_FirstRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BetCount;
                    currentCell.Style = gtr2dIntNumberStyle_FirstRow;
                    currentColumn++;
                    currentRow++;

                    //// GrandTotal
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                //// MakeExcelFile

                var reportName = SpecialResources.PunterAnalysis_TopPunterByLeague_User_ReportName;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
