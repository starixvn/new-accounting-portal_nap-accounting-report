// --------------------------------------------------------------------------
// <copyright file="ExportTopPunter_ByUser_SportTypeService.cs" company="Nexcel Solutions Vietnam">
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
        /// Exports the type of the top punter by user sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportTopPunter_ByUser_SportType(GetTopPunter_ByUser_SportTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByUserSportTypeSessionKey);
                if (report == null)
                {
                    report = this.GetTopPunter_ByUser_SportType(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = SpecialResources.PunterAnalysis_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 1 + (4 * 3) - 1; // 13 columns 

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
                var gtrIntNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.IntNumberFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                gtrIntNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

                //// Header

                for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
                }

                var title = string.Format(
                    SpecialResources.PunterAnalysis_TopPunterByUser_SportType_ReportTitle,
                    criteria.FromDate.ToShortFormatedDate(),
                    criteria.ToDate.ToShortFormatedDate(),
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
                        SpecialResources.PunterAnalysis_TopPunter_ByLeague_SportType_Filters,
                        criteria.UserName,
                        criteria.LeagueName,
                        criteria.OrderByName,
                        criteria.OrderTypeName,
                        criteria.SportName);
                }
                else
                {
                    filters = string.Format(
                        SpecialResources.PunterAnalysis_TopPunterByUser_SportType_Filters,
                        criteria.UserName,
                        criteria.OrderByName,
                        criteria.OrderTypeName,
                        criteria.SportName);
                }

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
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Style = headerTextStyle;
                currentCellRange.Value = Labels.Bet_Type;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Style = headerTextStyle;
                currentCellRange.Value = Labels.Non_Live;
                currentColumn += 4;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Style = headerTextStyle;
                currentCellRange.Value = Labels.Live;
                currentColumn += 4;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Style = headerTextStyle;
                currentCellRange.Value = Labels.None_live + "&" + Labels.Live;
                currentColumn += 4;
                currentRow++;

                currentColumn = startColumn + 1;
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
                    PunterWinLossInfo nonLiveInfo;
                    PunterWinLossInfo liveInfo;
                    PunterWinLossInfo mixLiveTypeInfo;
                    for (var i = 0; i < len; i++)
                    {
                        info = report.ChildsInfo[i];
                        nonLiveInfo = info.ChildsInfo[0];
                        liveInfo = info.ChildsInfo[1];
                        mixLiveTypeInfo = info.ChildsInfo[2];

                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = textStyle;
                        currentCell.Value = info.Name;

                        //// NonLiveInfo

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNegativeNumberStyle;
                        if (nonLiveInfo.HasData && nonLiveInfo.WinLoss_Str != string.Empty)
                        {
                            currentCell.Value = nonLiveInfo.WinLoss;
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNumberStyle;
                        if (nonLiveInfo.HasData && nonLiveInfo.Turnover_Str != string.Empty)
                        {
                            currentCell.Value = nonLiveInfo.Turnover;
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNegativePercentNumberStyle;
                        if (nonLiveInfo.HasData)
                        {
                            if (nonLiveInfo.Margin_Str != string.Empty &&
                                nonLiveInfo.Margin_Str != Labels.NotAvailable)
                            {
                                currentCell.Value = nonLiveInfo.Margin;
                            }
                            else
                            {
                                currentCell.Value = nonLiveInfo.Margin_Str;
                            }
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = intNumberStyle;
                        if (nonLiveInfo.HasData && nonLiveInfo.BetCount_Str != string.Empty)
                        {
                            currentCell.Value = nonLiveInfo.BetCount;
                        }

                        //// NonLiveInfo
                        //// LiveInfo

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNegativeNumberStyle;
                        if (liveInfo.HasData && liveInfo.WinLoss_Str != string.Empty)
                        {
                            currentCell.Value = liveInfo.WinLoss;
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNumberStyle;
                        if (liveInfo.HasData && liveInfo.Turnover_Str != string.Empty)
                        {
                            currentCell.Value = liveInfo.Turnover;
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNegativePercentNumberStyle;
                        if (liveInfo.HasData)
                        {
                            if (liveInfo.Margin_Str != string.Empty &&
                                liveInfo.Margin_Str != Labels.NotAvailable)
                            {
                                currentCell.Value = liveInfo.Margin;
                            }
                            else
                            {
                                currentCell.Value = liveInfo.Margin_Str;
                            }
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = intNumberStyle;
                        if (liveInfo.HasData && liveInfo.BetCount_Str != string.Empty)
                        {
                            currentCell.Value = liveInfo.BetCount;
                        }

                        //// LiveInfo
                        //// MixLiveTypesInfo

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNegativeNumberStyle;
                        if (mixLiveTypeInfo.WinLoss_Str != string.Empty)
                        {
                            currentCell.Value = mixLiveTypeInfo.WinLoss;
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNumberStyle;
                        if (mixLiveTypeInfo.Turnover_Str != string.Empty)
                        {
                            currentCell.Value = mixLiveTypeInfo.Turnover;
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = _2dNegativePercentNumberStyle;
                        
                        if (mixLiveTypeInfo.Margin_Str != string.Empty &&
                            mixLiveTypeInfo.Margin_Str != Labels.NotAvailable)
                        {
                            currentCell.Value = mixLiveTypeInfo.Margin;
                        }
                        else
                        {
                            currentCell.Value = mixLiveTypeInfo.Margin_Str;
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn++];
                        currentCell.Style = intNumberStyle;
                        if (mixLiveTypeInfo.BetCount_Str != string.Empty)
                        {
                            currentCell.Value = mixLiveTypeInfo.BetCount;
                        }

                        //// MixLiveTypesInfo

                        currentRow++;
                    }

                    //// GrandTotal

                    info = report.GrandTotal;
                    nonLiveInfo = info.ChildsInfo[0];
                    liveInfo = info.ChildsInfo[1];
                    mixLiveTypeInfo = info.ChildsInfo[2];

                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtrTextStyle_FirstRow;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentCell.Value = info.Name;

                    //// NonLiveInfo

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNegativeNumberStyle_FirstRow;
                    if (nonLiveInfo.WinLoss_Str != string.Empty)
                    {
                        currentCell.Value = nonLiveInfo.WinLoss;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNumberStyle_FirstRow;
                    if (nonLiveInfo.Turnover_Str != string.Empty)
                    {
                        currentCell.Value = nonLiveInfo.Turnover;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNegativePercentNumberStyle_FirstRow;
                    if (nonLiveInfo.Margin_Str != string.Empty &&
                        nonLiveInfo.Margin_Str != Labels.NotAvailable)
                    {
                        currentCell.Value = nonLiveInfo.Margin;
                    }
                    else
                    {
                        currentCell.Value = nonLiveInfo.Margin_Str;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtrIntNumberStyle_FirstRow;
                    if (nonLiveInfo.BetCount_Str != string.Empty)
                    {
                        currentCell.Value = nonLiveInfo.BetCount;
                    }

                    //// NonLiveInfo
                    //// LiveInfo

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNegativeNumberStyle_FirstRow;
                    if (liveInfo.WinLoss_Str != string.Empty)
                    {
                        currentCell.Value = liveInfo.WinLoss;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNumberStyle_FirstRow;
                    if (liveInfo.Turnover_Str != string.Empty)
                    {
                        currentCell.Value = liveInfo.Turnover;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNegativePercentNumberStyle_FirstRow;
                    if (liveInfo.Margin_Str != string.Empty &&
                        liveInfo.Margin_Str != Labels.NotAvailable)
                    {
                        currentCell.Value = liveInfo.Margin;
                    }
                    else
                    {
                        currentCell.Value = liveInfo.Margin_Str;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtrIntNumberStyle_FirstRow;
                    if (liveInfo.BetCount_Str != string.Empty)
                    {
                        currentCell.Value = liveInfo.BetCount;
                    }

                    //// LiveInfo
                    //// MixLiveTypesInfo

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNegativeNumberStyle_FirstRow;
                    if (mixLiveTypeInfo.WinLoss_Str != string.Empty)
                    {
                        currentCell.Value = mixLiveTypeInfo.WinLoss;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNumberStyle_FirstRow;
                    if (mixLiveTypeInfo.Turnover_Str != string.Empty)
                    {
                        currentCell.Value = mixLiveTypeInfo.Turnover;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtr2dNegativePercentNumberStyle_FirstRow;
                    if (mixLiveTypeInfo.Margin_Str != string.Empty &&
                        mixLiveTypeInfo.Margin_Str != Labels.NotAvailable)
                    {
                        currentCell.Value = mixLiveTypeInfo.Margin;
                    }
                    else
                    {
                        currentCell.Value = mixLiveTypeInfo.Margin_Str;
                    }

                    currentCell = workSheet.Cells[currentRow, currentColumn++];
                    currentCell.Style = gtrIntNumberStyle_FirstRow;
                    if (mixLiveTypeInfo.BetCount_Str != string.Empty)
                    {
                        currentCell.Value = mixLiveTypeInfo.BetCount;
                    }

                    //// MixLiveTypesInfo

                    currentRow++;

                    //// GrandTotal
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                //// MakeExcelFile

                var reportName = SpecialResources.PunterAnalysis_TopPunterByUser_SportType_ReportName;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
