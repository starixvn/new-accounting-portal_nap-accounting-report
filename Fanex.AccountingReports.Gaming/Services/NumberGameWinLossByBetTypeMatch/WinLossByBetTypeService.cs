namespace Fanex.AccountingReports.Gaming.Services.Implement.NumberGameWinLossByBetTypeMatch
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Repositories.Implement.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByBetTypeService : ServiceBase
    {
        private readonly WinLossByBetTypeRepository _winLossByBetTypeRepository;

        private readonly string _winLossByBetTypeMainReportSessionKey =
            "Fanex.AccountingReports.Gaming.NumberGameWinLoss.WinLossByBetType.MainReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private readonly string _winLossByBetTypeDetailReportSessionKey =
            "Fanex.AccountingReports.Gaming.NumberGameWinLoss.WinLossByBetType.DetailReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        public WinLossByBetTypeService()
        {
            _winLossByBetTypeRepository = new WinLossByBetTypeRepository();
        }

        /// <summary>
        /// Gets the win loss by bet type main report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The win loss by bet type main report.</returns>
        public WinLossInfo GetMainReport(GetWinLossByBetTypeMainReportCriteria criteria)
        {
            var rawData = _winLossByBetTypeRepository.GetMainReport(criteria);
            WinLossInfo report = new WinLossInfo
            {
                DetailInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.Total
                }
            };

            if (rawData == null || rawData.Count == 0)
            {
                return report;
            }

            rawData.GroupBy(g => g.SportType).ToList()
                .ForEach(sport =>
                {
                    WinLossInfo sportData = new WinLossInfo { DetailInfo = new List<WinLossInfo>(), Name = CacheRef.GetSportName(sport.Key), GrandTotal = new WinLossInfo { Name = CacheRef.GetSportName(sport.Key) } };
                    sport.Select(s => s).ToList()
                    .ForEach(bettype =>
                    {
                        WinLossInfo bettypeRow = bettype;
                        bettypeRow.Name = CacheRef.GetBettype(bettype.Id);
                        bettypeRow = WinLossInfo.Format(bettypeRow);
                        sportData.GrandTotal.Sum(bettypeRow);
                        sportData.DetailInfo.Add(bettypeRow);

                        report.GrandTotal.Sum(bettypeRow);
                    });

                    sportData.GrandTotal = WinLossInfo.Format(sportData.GrandTotal);
                    report.DetailInfo.Add(sportData);
                });

            report.GrandTotal = WinLossInfo.Format(report.GrandTotal);

            SessionManager.Set<WinLossInfo>(_winLossByBetTypeMainReportSessionKey, report);
            return report;
        }

        /// <summary>
        /// Gets the win loss by bet type detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The win loss by bet type detail report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria)
        {
            var rawData = _winLossByBetTypeRepository.GetDetailReport(criteria);
            WinLossInfo report = new WinLossInfo
            {
                Id = criteria.BetTypeId,
                DetailInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.Total
                }
            };

            report.Name = CacheRef.GetBettype(report.Id);
            if (rawData == null || rawData.Count == 0)
            {
                return report;
            }

            var leagueGroups = rawData.GroupBy(data => data.LeagueId);
            foreach (var leagueGroup in leagueGroups)
            {
                WinLossInfo leagueWinLoss = new WinLossInfo
                {
                    Id = leagueGroup.Key,
                    DetailInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo()
                };

                if (criteria.BetTypeId < 3601 || criteria.BetTypeId > 3610)
                {
                    leagueWinLoss.Name = CacheRef.GetLeagueName(leagueWinLoss.Id);
                }
                else
                {
                    leagueWinLoss.Name = "Happy 5 Linked Games";
                }

                foreach (var data in leagueGroup)
                {
                    WinLossInfo info = new WinLossInfo
                    {
                        Name = data.MatchCode,
                        CustomerWinLoss = data.CustomerWinLoss,
                        AgencyWinLoss = data.AgencyWinLoss,
                        Commission = data.Commission,
                        LineTotal = data.LineTotal,
                        VirtualLineTotal = data.VirtualLineTotal,
                        Turnover = data.Turnover
                    };

                    leagueWinLoss.GrandTotal.Sum(info);
                    info = WinLossInfo.Format(info);
                    leagueWinLoss.DetailInfo.Add(info);
                }

                report.GrandTotal.Sum(leagueWinLoss.GrandTotal);
                leagueWinLoss.GrandTotal = WinLossInfo.Format(leagueWinLoss.GrandTotal);
                report.DetailInfo.Add(leagueWinLoss);
            }

            report.GrandTotal = WinLossInfo.Format(report.GrandTotal);

            SessionManager.Set<WinLossInfo>(_winLossByBetTypeDetailReportSessionKey, report);
            return report;
        }

        /// <summary>
        /// Exports the win loss by bet type main report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportMainReport(GetWinLossByBetTypeMainReportCriteria criteria)
        {
            WinLossInfo report = SessionManager.Get<WinLossInfo>(_winLossByBetTypeMainReportSessionKey);
            if (report == null)
            {
                report = GetMainReport(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.NumberGameWinLossByBetTypeMatch_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 8;
            var currentRow = startRow;

            var reportTitle = string.Format(
                GamingResources.NumberGameWinLossByBetTypeMatch_ByBetTypeMainReportTitle,
                criteria.ReportDate.ToShortFormatedDate());
            this.ExportReport_WinLossByBetTypeHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                reportTitle,
                ReportLevelType.WinLossByBetTypeMainReport);

            this.ExportReport_WinLossByBetTypeMainReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                criteria,
                report);

            var reportName = string.Format(
                GamingResources.NumberGameWinLossByBetTypeMatch_ReportName,
                Converter.ToDateTime(criteria.ReportDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the win loss by bet type detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria)
        {
            WinLossInfo report = SessionManager.Get<WinLossInfo>(_winLossByBetTypeDetailReportSessionKey);
            if (report == null)
            {
                report = GetDetailReport(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.NumberGameWinLossByBetTypeMatch_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 8;
            var currentRow = startRow;

            var betTypeName = report != null ? " - " + report.Name : string.Empty;
            var reportTitle = string.Format(
                GamingResources.NumberGameWinLossByBetTypeMatch_ByBetTypeDetailReportTitle,
                betTypeName,
                criteria.ReportDate.ToShortFormatedDate());
            this.ExportReport_WinLossByBetTypeHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                reportTitle,
                ReportLevelType.WinLossByBetTypeDetailReport);

            this.ExportReport_WinLossByBetTypeDetailReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                criteria,
                report);

            var reportName = string.Format(
                GamingResources.NumberGameWinLossByBetTypeMatch_ReportName,
                Converter.ToDateTime(criteria.ReportDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the report_ win loss by bet type header.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="reportTitle">The report title.</param>
        /// <param name="reportLevelType">Type of the report level.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_WinLossByBetTypeHeader(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            string reportTitle,
            ReportLevelType reportLevelType)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);

            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 3].Width = 120 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 7].Width = 120 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 8].Width = 120 * ExcelExtensions.FontWidth;

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = reportTitle;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            switch (reportLevelType)
            {
                case ReportLevelType.WinLossByBetTypeMainReport:
                    currentCellRange.Value = Labels.Bet_Type;
                    break;

                default:
                    currentCellRange.Value = string.Empty;
                    break;
            }

            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "Member Win Loss";
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "SMA Win Loss";
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Commission;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
            currentCellRange.Merged = true;
            currentCellRange.Value = "House";
            currentCellRange.Style = headerTextStyle;
            currentColumn += 5;
            currentRow++;

            currentColumn = startColumn + 4;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = "Real Income";
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = "Virtual Win Loss";
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Variance;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Margin;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the content of the report win loss by bet type main report.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_WinLossByBetTypeMainReportContent(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            GetWinLossByBetTypeMainReportCriteria criteria,
            WinLossInfo report)
        {
            if (report == null ||
                report.DetailInfo == null ||
                report.DetailInfo.Count == 0)
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                return;
            }

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textBoldStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, true);
            var twoDecimalNumberBoldStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, true, false, false);
            var twoDecimalPositiveNumberBoldStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                true,
                true,
                false);
            var twoDecimalPositivePercentNumberBoldStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                true,
                true,
                false);

            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                true,
                false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotal1stRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            grandTotal1stRowTextStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            grandTotal1stRowTwoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotal1stRowTwoDecimalPositiveNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotal1stRowTwoDecimalPositivePercentNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var reportLength = report.DetailInfo.Count;
            WinLossInfo info;
            for (var i = 0; i < reportLength; i++)
            {
                var sportTotal = report.DetailInfo[i].GrandTotal;

                NumberGameWinLossByBetTypeMatchService.SetOddEvenColor_ExportExcel(
                    ref textBoldStyle,
                    ref twoDecimalNumberBoldStyle,
                    ref twoDecimalPositiveNumberBoldStyle,
                    ref twoDecimalPositivePercentNumberBoldStyle,
                    true);
                ExportReport_WinLossByBetType_WinLossRow(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    maxColumnIndex,
                    textBoldStyle,
                    twoDecimalNumberBoldStyle,
                    twoDecimalPositiveNumberBoldStyle,
                    twoDecimalPositivePercentNumberBoldStyle,
                    sportTotal);

                var sportData = report.DetailInfo[i].DetailInfo;
                for (var j = 0; j < sportData.Count; j++)
                {
                    var bettypeData = sportData[j];
                    NumberGameWinLossByBetTypeMatchService.SetOddEvenColor_ExportExcel(
                    ref textStyle,
                    ref twoDecimalNumberStyle,
                    ref twoDecimalPositiveNumberStyle,
                    ref twoDecimalPositivePercentNumberStyle,
                    false);
                    ExportReport_WinLossByBetType_WinLossRow(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        maxColumnIndex,
                        textStyle,
                        twoDecimalNumberStyle,
                        twoDecimalPositiveNumberStyle,
                        twoDecimalPositivePercentNumberStyle,
                        bettypeData);
                }
            }

            NumberGameWinLossByBetTypeMatchService.SetOddEvenColor_ExportExcel(
                ref textBoldStyle,
                ref twoDecimalNumberBoldStyle,
                ref twoDecimalPositiveNumberBoldStyle,
                ref twoDecimalPositivePercentNumberBoldStyle,
                false);
            info = report.GrandTotal;
            ExportReport_WinLossByBetType_GrandTotalRow(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                grandTotal1stRowTextStyle,
                grandTotal1stRowTwoDecimalNumberStyle,
                grandTotal1stRowTwoDecimalPositiveNumberStyle,
                grandTotal1stRowTwoDecimalPositivePercentNumberStyle,
                info);
        }

        /// <summary>
        /// Exports the report win loss by bet type win loss row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="info">The information.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_WinLossByBetType_WinLossRow(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info)
        {
            var currentColumn = startColumn;
            ExcelCell currentCell;

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentCell.Value = info.Name;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.CustomerWinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.AgencyWinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Commission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.LineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.VirtualLineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.Variance;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            NumberGameWinLossByBetTypeMatchService.ExportColumnMargin(
                info.Turnover,
                info.Margin,
                ref currentCell);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the report_ win loss by bet type_ grand total row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="info">The information.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
          "SP2101:MethodMustNotContainMoreLinesThan",
          Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_WinLossByBetType_GrandTotalRow(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info)
        {
            var currentColumn = startColumn;
            ExcelCell currentCell;

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCell.Value = info.Name;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.CustomerWinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.AgencyWinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Commission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.LineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.VirtualLineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.Variance;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            NumberGameWinLossByBetTypeMatchService.ExportColumnMargin(
                info.Turnover,
                info.Margin,
                ref currentCell);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the content of the report win loss by bet type detail report.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_WinLossByBetTypeDetailReportContent(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            GetWinLossByBetTypeDetailReportCriteria criteria,
            WinLossInfo report)
        {
            if (report == null ||
                report.DetailInfo == null ||
                report.DetailInfo.Count == 0)
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                return;
            }

            var currentColumn = startColumn;
            CellRange currentCellRange;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                true,
                false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var subTotalRowColor = ExcelExtensions.TotalRowColor;
            var subTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, true);
            var subTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                true,
                false,
                false);
            var subTotalRowTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                true,
                true,
                false);
            var subTotalRowTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                subTotalRowColor,
                true,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotal1stRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            grandTotal1stRowTextStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            grandTotal1stRowTwoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotal1stRowTwoDecimalPositiveNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotal1stRowTwoDecimalPositivePercentNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var leagueLength = report.DetailInfo.Count;
            WinLossInfo info;
            for (var i = 0; i < leagueLength; i++)
            {
                var leagueWinLoss = report.DetailInfo[i];
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Value = leagueWinLoss.Name;
                currentCellRange.Style = textStyle;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                currentColumn += 4;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
                currentCellRange.Merged = true;
                currentCellRange.Style = textStyle;
                currentRow++;

                var matchLength = leagueWinLoss.DetailInfo.Count;
                for (var j = 0; j < matchLength; j++)
                {
                    info = leagueWinLoss.DetailInfo[j];
                    NumberGameWinLossByBetTypeMatchService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        (j + 1) % 2 == 0);
                    ExportReport_WinLossByBetType_WinLossRow(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        maxColumnIndex,
                        textStyle,
                        twoDecimalNumberStyle,
                        twoDecimalPositiveNumberStyle,
                        twoDecimalPositivePercentNumberStyle,
                        info);
                }
            }

            info = report.GrandTotal;
            ExportReport_WinLossByBetType_GrandTotalRow(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                grandTotal1stRowTextStyle,
                grandTotal1stRowTwoDecimalNumberStyle,
                grandTotal1stRowTwoDecimalPositiveNumberStyle,
                grandTotal1stRowTwoDecimalPositivePercentNumberStyle,
                info);
        }
    }
}