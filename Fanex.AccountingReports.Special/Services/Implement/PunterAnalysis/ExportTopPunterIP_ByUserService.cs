// --------------------------------------------------------------------------
// <copyright file="ExportTopPunterIP_ByUserService.cs" company="Nexcel Solutions Vietnam">
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
        /// Exports the top punter IP by user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportTopPunterIP_ByUser(GetTopPunterIPByUserCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterIPByUserSessionKey);
                if (report == null)
                {
                    report = this.GetTopPunterIP_ByUser(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = SpecialResources.PunterAnalysis_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 3; // 4 columns 

                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;
                var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                var intNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, ExcelExtensions.BackGroundColor, false, false, false);

                //// Header

                for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                var title = string.Format(
                    SpecialResources.PunterAnalysis_TopPunterIP_ByUser_ReportTitle,
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

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Month;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Year;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.IP;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Country;
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
                        currentCell.Value = info.Month;
                        currentCell.Style = intNumberStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Year;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.IP;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Country;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;
                        currentRow++;

                        //// NormalRow
                    }
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                //// MakeExcelFile

                var reportName = SpecialResources.PunterAnalysis_TopPunterIP_ByUser_ReportName;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
