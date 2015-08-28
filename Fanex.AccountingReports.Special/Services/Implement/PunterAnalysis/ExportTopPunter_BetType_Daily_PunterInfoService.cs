// --------------------------------------------------------------------------
// <copyright file="ExportTopPunter_BetType_Daily_PunterInfoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Implement.PunterAnalysis
{
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class PunterAnalysisService
    {
        /// <summary>
        /// Exports the top punter bet type daily punter information.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="colWinLossStyle">The col win loss style.</param>
        /// <param name="colTurnoverStyle">The col turnover style.</param>
        /// <param name="colMarginStyle">The col margin style.</param>
        /// <param name="colBetCountStyle">The col bet count style.</param>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="turnover">The turnover.</param>
        /// <param name="margin">The margin.</param>
        /// <param name="betCount">The bet count.</param>
        /// <param name="isHeader">If set to <c>true</c> [is header].</param>
        private void ExportTopPunter_BetType_Daily_PunterInfo(
            ref ExcelWorksheet workSheet,
            int currentRow,
            ref int currentColumn,
            CellStyle colWinLossStyle,
            CellStyle colTurnoverStyle,
            CellStyle colMarginStyle,
            CellStyle colBetCountStyle,
            object winLoss,
            object turnover,
            object margin,
            object betCount,
            bool isHeader)
        {
            if (isHeader)
            {
                workSheet.Columns[currentColumn].Width = 120 * ExcelExtensions.FontWidth;
                workSheet.Columns[currentColumn + 1].Width = 120 * ExcelExtensions.FontWidth;
                workSheet.Columns[currentColumn + 2].Width = 120 * ExcelExtensions.FontWidth;
                workSheet.Columns[currentColumn + 3].Width = 120 * ExcelExtensions.FontWidth;
            }

            ExcelCell currentCell;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss;
            currentCell.Style = colWinLossStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = turnover;
            currentCell.Style = colTurnoverStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = margin;
            currentCell.Style = colMarginStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = betCount;
            currentCell.Style = colBetCountStyle;
            currentColumn++;
        }
    }
}
