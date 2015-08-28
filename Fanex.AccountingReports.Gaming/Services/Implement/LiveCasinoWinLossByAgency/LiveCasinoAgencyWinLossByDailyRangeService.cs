// --------------------------------------------------------------------------
// <copyright file="LiveCasinoAgencyWinLossByDailyRangeService.cs" company="Nexcel Solutions Vietnam">
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
    public class LiveCasinoAgencyWinLossByDailyRangeService : LiveCasinoAgencyWinLossService
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
            var timeRangeGroups = winLossInfoTable.GroupBy(
                data => Converter.ToDateTime(data.Id));
            foreach (var timeRangeGroup in timeRangeGroups)
            {
                DateTime dateTime = timeRangeGroup.Key;
                var timeRangeWinLoss = new WinLossInfo
                {
                    Id = dateTime.ToShortFormatedDate(),
                    Name = dateTime.ToShortFormatedDate(),
                    ChildsInfo = new List<WinLossInfo>()
                };

                foreach (var data in timeRangeGroup)
                {
                    info = GetWinLossInfo(data);
                    info = WinLossInfo.Format(info);
                    timeRangeWinLoss.ChildsInfo.Add(info);
                }

                timeRangeWinLoss.ChildsInfo = timeRangeWinLoss.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName).ToList();
                report.ChildsInfo.Add(timeRangeWinLoss);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Name).ToList();
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
            if (getReportCriteria.SortColumnName == "Name")
            {
                ListExtensions.SortDataByProperty<WinLossInfo>(
                    report.ChildsInfo,
                    getReportCriteria.SortDirection,
                    getReportCriteria.SortColumnName,
                    getReportCriteria.SortDataType);
            }
            else
            {
                SortSecondSubLevelReport(report, getReportCriteria);
                SortFirstSubLevelReport(report, getReportCriteria);
            }

            return report;
        }

        public override WinLossInfo SortFirstSubLevelReport(WinLossInfo report, GetReportCriteria getReportCriteria)
        {
            return base.SortFirstSubLevelReport(report, getReportCriteria);
        }

        public override WinLossInfo SortSecondSubLevelReport(WinLossInfo report, GetReportCriteria getReportCriteria)
        {
            return base.SortSecondSubLevelReport(report, getReportCriteria);
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
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
            CellRange currentCellRange;

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

            var timeRangeLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (timeRangeLength > 0)
            {
                var itemPerSheet = MaxItemPerSheet;
                var totalSheet = (timeRangeLength / itemPerSheet) + (timeRangeLength % itemPerSheet != 0 ? 1 : 0);
                WinLossInfo info;
                var reportLength = 0;
                for (var sheetIndex = 0; sheetIndex < totalSheet; sheetIndex++)
                {
                    workSheet = worksheets.Add(sheetName + (totalSheet > 1 ? " - " + (sheetIndex + 1) : string.Empty));
                    currentRow = 0;
                    ExportReport_Header(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        PeriodTypeConstants.LiveCasinoAgencyWinLossDaily,
                        maxColumnIndex,
                        criteria);

                    var startIndex = itemPerSheet * sheetIndex;
                    var endIndex = (sheetIndex == totalSheet - 1) ?
                        timeRangeLength :
                        (itemPerSheet * sheetIndex) + itemPerSheet;

                    for (var i = startIndex; i < endIndex; i++)
                    {
                        var timeRangeWinLoss = report.ChildsInfo[i];
                        reportLength = timeRangeWinLoss.ChildsInfo.Count;
                        for (var j = 0; j < reportLength; j++)
                        {
                            info = timeRangeWinLoss.ChildsInfo[j];
                            currentColumn = startColumn;
                            if (j == 0)
                            {
                                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + reportLength - 1, currentColumn);
                                currentCellRange.Merged = true;
                                currentCellRange.Value = timeRangeWinLoss.Name;
                                currentCellRange.Style = textStyle;
                                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            }

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
                    PeriodTypeConstants.LiveCasinoAgencyWinLossDaily,
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