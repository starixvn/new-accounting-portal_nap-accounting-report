// --------------------------------------------------------------------------
// <copyright file="ExportTopPunter_ByUser_BetTypeService.cs" company="Nexcel Solutions Vietnam">
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
        /// Exports the type of the top punter by user bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportTopPunter_ByUser_BetType(GetTopPunter_ByUser_BetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<PunterWinLossInfo>(SpecialConstants.PunterAnalysisTopPunterByUserBetTypeSessionKey);
                if (report == null)
                {
                    report = this.GetTopPunter_ByUser_BetType(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = SpecialResources.PunterAnalysis_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);
                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 4;
                var currentRow = startRow;

                //// ExcelTypes

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                var title = string.Format(
                    SpecialResources.PunterAnalysis_TopPunterByUser_BetType_ReportTitle,
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
                        SpecialResources.PunterAnalysis_TopPunterByLeague_BetType_Filters,
                         criteria.UserName,
                        criteria.LeagueName,
                        criteria.OrderByName,
                        criteria.OrderTypeName,
                        criteria.SportName,
                        criteria.BetTypeName,
                        criteria.LiveTypeName);
                }
                else
                {
                    filters = string.Format(
                        SpecialResources.PunterAnalysis_TopPunterByUser_BetType_Filters,
                        criteria.UserName,
                        criteria.OrderByName,
                        criteria.OrderTypeName,
                        criteria.SportName,
                        criteria.BetTypeName,
                        criteria.LiveTypeName);
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

                switch (criteria.Reporttype)
                {
                    case PeriodTypeConstants.Daily:
                        this.ExportTopPunter_ByUser_BetType_Daily(
                            report,
                            ref workSheet,
                            startRow,
                            startColumn,
                            ref currentRow);
                        break;
                    case PeriodTypeConstants.MonthRange:
                        this.ExportTopPunter_ByUser_BetType_NonDaily_Summary(
                            report,
                            ref workSheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            criteria);
                        workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                        this.ExportTopPunter_ByUser_BetType_NonDaily(
                            report,
                            ref workSheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            criteria);
                        break;
                    default:
                        this.ExportTopPunter_ByUser_BetType_NonDaily(
                            report,
                            ref workSheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            criteria);
                        break;
                }

                var reportName = SpecialResources.PunterAnalysis_TopPunterByUser_BetType_ReportName;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
