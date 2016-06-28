// --------------------------------------------------------------------------
// <copyright file="LiveCasinoAgencyWinLossByAgentService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.LiveCasinoWinLossByAgency
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoWinLossByAgency;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoWinLossByAgency;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoAgencyWinLossByAgentService : LiveCasinoAgencyWinLossService
    {
        /// <summary>
        /// Gets the report by daily range.
        /// </summary>
        /// <param name="winLossInfoTable">The win loss information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report by daily range.</returns>
        public override WinLossInfo GetReport(
            List<WinLossInfoEntity> winLossInfoTable,
            GetReportCriteria criteria)
        {
            var report = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>()
                }
            };

            WinLossInfo info;
            foreach (var data in winLossInfoTable)
            {
                info = GetWinLossInfo(data);
                info.Name = data.Id;
                info = WinLossInfo.Format(info);
                report.ChildsInfo.Add(info);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name)
                                                 .ThenBy(winLoss => winLoss.CurrencyName)
                                                 .ToList();
            return report;
        }

        /// <summary>
        /// Sorts the report data.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>Report data after sort.</returns>
        public override WinLossInfo SortReportData(WinLossInfo report, GetReportCriteria getReportCriteria)
        {
            if (getReportCriteria.SortColumnName == "Name" || getReportCriteria.SortColumnName == "RollingTurnover")
            {
                ListExtensions.SortDataByProperty<WinLossInfo>(
                    report.ChildsInfo,
                    getReportCriteria.SortDirection,
                    getReportCriteria.SortColumnName,
                    getReportCriteria.SortDataType);
            }
            else
            {
                SortFirstSubLevelReport(report, getReportCriteria);
            }

            return report;
        }

        /// <summary>
        /// Sorts the first sub level report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="getReportCriteria">The get report criteria.</param>
        /// <returns>First sub level report data after sort.</returns>
        public override WinLossInfo SortFirstSubLevelReport(WinLossInfo report, GetReportCriteria getReportCriteria)
        {
            bool isAsc = string.Equals(getReportCriteria.SortDirection, CoreConstants.SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);
            switch (getReportCriteria.SortColumnName)
            {
                case "MemberTotal":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[0].RealWinLoss)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[0].RealWinLoss)
                                              .ToList();
                    break;
                case "AgentTotal":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[1].RealWinLoss)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[1].RealWinLoss)
                                              .ToList();
                    break;
                case "MasterTotal":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[2].RealWinLoss)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[2].RealWinLoss)
                                              .ToList();
                    break;
                case "SuperTotal":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[3].RealWinLoss)
                                              .ToList() :
                                        report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[3].RealWinLoss)
                                              .ToList();
                    break;
                case "CompanyWinLoss":
                    report.ChildsInfo = isAsc ? report.ChildsInfo
                                              .OrderBy(winLossInfo => winLossInfo.ChildsInfo[4].RealWinLoss)
                                              .ToList() :
                    report.ChildsInfo = report.ChildsInfo
                                              .OrderByDescending(winLossInfo => winLossInfo.ChildsInfo[4].RealWinLoss)
                                              .ToList();
                    break;
                default:
                    break;
            }

            return report;
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public override void ExportReport(
            WinLossInfo report,
            GetReportCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.LiveCasinoAgencyWinLoss_SheetName;
            ExcelWorksheet workSheet;

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 18;
            var currentRow = startRow;

            var currentColumn = startColumn;
            ExcelCell currentCell;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var intNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.IntNumberFormat, backgroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalNegativeNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                false,
                true);

            var reportLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (reportLength > 0)
            {
                var itemPerSheet = MaxItemPerSheet;
                var totalSheet = (reportLength / itemPerSheet) + (reportLength % itemPerSheet != 0 ? 1 : 0);
                WinLossInfo info;
                for (var sheetIndex = 0; sheetIndex < totalSheet; sheetIndex++)
                {
                    workSheet = worksheets.Add(sheetName + (totalSheet > 1 ? " - " + (sheetIndex + 1) : string.Empty));
                    currentRow = 0;
                    ExportReport_Header(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        PeriodTypeConstants.LiveCasinoAgencyWinLossAgent,
                        maxColumnIndex,
                        criteria);

                    var startIndex = itemPerSheet * sheetIndex;
                    var endIndex = (sheetIndex == totalSheet - 1) ?
                        reportLength :
                        (itemPerSheet * sheetIndex) + itemPerSheet;

                    for (var i = startIndex; i < endIndex; i++)
                    {
                        info = report.ChildsInfo[i];
                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Name;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        ExportReport_WinLossRow(
                            ref workSheet,
                            currentColumn,
                            ref currentRow,
                            intNumberStyle,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalNegativeNumberStyle,
                            criteria,
                            info);
                        currentRow++;
                    }

                    ExportReport_GrandTotal(
                        report.GrandTotal,
                        ref workSheet,
                        startColumn,
                        ref currentRow,
                        criteria);
                }
            }
            else
            {
                workSheet = worksheets.Add(sheetName);
                ExportReport_Header(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    PeriodTypeConstants.LiveCasinoAgencyWinLossAgent,
                    maxColumnIndex,
                    criteria);
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            var reportName = string.Format(
                GamingResources.LiveCasinoAgencyWinLoss_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }
    }
}