// --------------------------------------------------------------------------
// <copyright file="ExportTopPunter_ByUser_BetType_NonDaily_SummaryService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Implement.PunterAnalysis
{
    using System.Diagnostics.CodeAnalysis;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Special.Criteria.PunterAnalysis;
    using Fanex.AccountingReports.Special.Models.PunterAnalysis;
    using Fanex.AccountingReports.Special.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class PunterAnalysisService
    {
        /// <summary>
        /// Exports the top punter by user bet type non daily summary.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTopPunter_ByUser_BetType_NonDaily_Summary(
            PunterWinLossInfo report,
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetTopPunter_ByUser_BetTypeCriteria criteria)
        {
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var currentColumn = startColumn;
            int maxColumnIndex = 4;

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

            //// Header

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = string.Format(
                "{0} - {1}",
                criteria.BetTypeName,
                criteria.LiveTypeName);
            currentCellRange.Style = headerTextStyle;
            currentRow++;

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Year;
            currentCell.Style = headerTextStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            this.ExportTopPunter_BetType_Daily_PunterInfo(
                ref workSheet,
              currentRow,
              ref currentColumn,
              headerTextStyle,
              headerTextStyle,
              headerTextStyle,
              headerTextStyle,
              Labels.Win_Loss,
              Labels.Turnover,
              Labels.Margin,
              Labels.Bet_Count,
              true);
            currentRow++;

            //// Header

            if (report != null && report.GrandTotal.ChildsInfo.Count > 0)
            {
                var yearLength = report.GrandTotal.ChildsInfo.Count;
                for (var i = 0; i < yearLength; i++)
                {
                    var year = report.GrandTotal.ChildsInfo[i];
                    currentColumn = startColumn;
                    if (year != null)
                    {
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = year.Id;
                        currentCell.Style = textStyle;
                        currentCell.Style.NumberFormat = string.Empty;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        this.ExportTopPunter_BetType_Daily_PunterInfo(
                            ref workSheet,
                            currentRow,
                            ref currentColumn,
                            _2dNegativeNumberStyle,
                            _2dNumberStyle,
                            _2dNegativePercentNumberStyle,
                            intNumberStyle,
                            year.WinLoss,
                            year.Turnover,
                            year.Margin,
                            year.BetCount,
                            false);
                        currentRow++;
                    }
                }
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }
        }
    }
}
