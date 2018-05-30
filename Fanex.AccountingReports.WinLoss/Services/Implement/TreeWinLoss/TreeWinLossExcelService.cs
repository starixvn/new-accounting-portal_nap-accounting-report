namespace Fanex.AccountingReports.WinLoss.Services.Implement.TreeWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.TreeWinLoss;
    using Fanex.AccountingReports.WinLoss.Resources;
    using Fanex.AccountingReports.WinLoss.Services.Interface.TreeWinLoss;
    using Fanex.Common;

    /// <summary>
    /// Export excel for tree win loss report.
    /// </summary>
    public class TreeWinLossExcelService : ServiceBase, ITreeWinLossExcelService
    {
        private const int _MaxUserPerSheet = 40000;
        private static CellStyle headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
        private static Color backgroundColor = ExcelExtensions.BackGroundColor;
        private static CellStyle textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
        private static CellStyle intStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, backgroundColor, false, false, false);
        private static CellStyle pecentStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.PercentFormat, backgroundColor, false, false, false);
        private static CellStyle redTextStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false, HorizontalAlignmentStyle.Right, false, Color.Red);
        private static CellStyle rightTextStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false, HorizontalAlignmentStyle.Right, false, Color.Black);

        private static CellStyle twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
            NumberFormatConstants.TwoDecimalDigitFormat,
            backgroundColor,
            false,
            false,
            false);

        private static CellStyle twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
            NumberFormatConstants.TwoDecimalDigitFormat,
            backgroundColor,
            false,
            true,
            false);

        private static CellStyle twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
            NumberFormatConstants.TwoDecimalDigitPercentFormat,
            backgroundColor,
            false,
            true,
            false);

        private static CellStyle grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);

        private static CellStyle grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
            NumberFormatConstants.TwoDecimalDigitFormat,
            ExcelExtensions.GrandTotalRowColor,
            true,
            false,
            false);

        private static CellStyle grandTotalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
            NumberFormatConstants.TwoDecimalDigitFormat,
            ExcelExtensions.GrandTotalRowColor,
            true,
            true,
            false);

        private readonly ITreeWinLossService _reportService;

        public TreeWinLossExcelService()
            : this(null)
        {
        }

        public TreeWinLossExcelService(ITreeWinLossService reportService)
        {
            _reportService = reportService ?? new TreeWinLossService();
        }

        /// <summary>
        /// Export Excel for Tree Win Loss by product.
        /// </summary>
        /// <param name="criteria">The Tree Win Loss Product Criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void ExportExcelTreeWinLossByProduct(GetTreeWinLossByProductCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<TreeWinLoss>(TreeWinLossUtil.TreeWinLossByProductSession);
                if (report == null)
                {
                    report = _reportService.GetTreeWinLossByProduct(criteria);
                }

                var excelFile = new ExcelFile();
                ExcelWorksheetCollection workSheets = excelFile.Worksheets;
                string sheetName = WinLossResources.TreeWinLoss_SheetName;
                ExcelWorksheet workSheet = workSheets.Add(sheetName);
                var isShowNA = report.IsShowNotAvailableTurnover;
                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 18;

                var currentColumn = startColumn;
                CellRange currentCellRange;

                workSheet.Columns[startColumn].Width = 80 * ExcelExtensions.FontWidth;
                for (var colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
                }

                var title = string.Format(
                    "{0} From {1} To {2}",
                    WinLossResources.TreeWinLoss_WLByCurrency,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                var currentRow = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 1);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow++;
                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex - 1);
                this.ExportTreeWinLossByProductHeader(ref workSheet, ref currentRow, startColumn);

                var childsInfoLength = report.ChildsInfo.Count;
                if (childsInfoLength > 0)
                {
                    this.ExportTreeWinLossByProductAssignBodyData(isShowNA, ref workSheet, report.ChildsInfo, ref currentRow, startColumn);
                    this.ExportTreeWinLossByProductAssignFooterData(isShowNA, ref workSheet, report.TotalInfo, ref currentRow, startColumn);
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 16);
                }

                var reportName = string.Format(
                    WinLossResources.TreeWinLoss_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Export excel for tree win loss all levels.
        /// </summary>
        /// <param name="criteria">The Tree Win Loss Level Criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void ExportExcelTreeWinLossByLevel(GetTreeWinLossByLevelCriteria criteria)
        {
            try
            {
                var report = new TreeWinLoss();
                switch (criteria.RoleId)
                {
                    case 5:
                        report = SessionManager.Get<TreeWinLoss>(TreeWinLossUtil.TreeWinLossByCommSession);
                        break;

                    case 4:
                        report = SessionManager.Get<TreeWinLoss>(TreeWinLossUtil.TreeWinLossBySuperSession);
                        break;

                    case 3:
                        report = SessionManager.Get<TreeWinLoss>(TreeWinLossUtil.TreeWinLossByMasterSession);
                        break;

                    case 2:
                        report = SessionManager.Get<TreeWinLoss>(TreeWinLossUtil.TreeWinLossByAgentSession);
                        break;
                }

                if (report == null)
                {
                    report = _reportService.GetTreeWinLossByLevel(criteria);
                }

                var allVirtualRate = _reportService.GetAllVirtualRate(report);

                var excelFile = new ExcelFile();
                ExcelWorksheetCollection workSheets = excelFile.Worksheets;

                this.ExportTreeWinLossInfo(report, allVirtualRate.EnabledVirtualRateUser, criteria, workSheets);
                this.ExportVirtualRateInfo(allVirtualRate, criteria, workSheets);

                var reportName = string.Format(
                    WinLossResources.TreeWinLoss_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTreeWinLossInfo(TreeWinLoss report, Dictionary<string, int> enabledVirtualRateUser, GetTreeWinLossByLevelCriteria criteria, ExcelWorksheetCollection workSheets)
        {
            string sheetName = WinLossResources.TreeWinLoss_SheetName;
            ExcelWorksheet workSheet = workSheets.Add(sheetName);

            var isShowNA = false;
            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 19;

            var titleStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var currentColumn = startColumn;
            CellRange currentCellRange;

            workSheet.Columns[startColumn].Width = 50 * ExcelExtensions.FontWidth;
            for (var colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            string title = GetReportTitleByLevel(criteria);
            var currentRow = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = titleStyle;
            currentRow++;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            this.ExportTreeWinLossByLevelHeader(ref workSheet, ref currentRow, startColumn);
            isShowNA = report.IsShowNotAvailableTurnover;
            var childsInfoLength = report.ChildsInfo.Count;
            if (childsInfoLength > 0)
            {
                this.ExportTreeWinLossByLevelAssignBodyData(isShowNA, ref workSheet, report, enabledVirtualRateUser, ref currentRow, startColumn);
                this.ExportTreeWinLossByLevelAssignFooterData(isShowNA, ref workSheet, report.TotalInfo, ref currentRow, startColumn);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 18);
            }
        }

        /// <summary>
        /// Exports the virtual rate information.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="workSheets">The work sheets.</param>
        private void ExportVirtualRateInfo(TreeWinLossVirtualRate report, GetTreeWinLossByLevelCriteria criteria, ExcelWorksheetCollection workSheets)
        {
            int maxUserPerSheet = _MaxUserPerSheet;
            int redundancyUsers = report.NumberUserEnabledVirtualRate % maxUserPerSheet;
            int numberSheetBeforeRounding = Converter.ToInt(report.NumberUserEnabledVirtualRate / maxUserPerSheet);
            int numberSheet = redundancyUsers == 0 ? numberSheetBeforeRounding : numberSheetBeforeRounding + 1;

            for (int sheetIndex = 0; sheetIndex < numberSheet; sheetIndex++)
            {
                var virtualRateByUserInSheet = report.SMAVirtualRate.ReportRows.Skip(sheetIndex * maxUserPerSheet).Take(maxUserPerSheet).ToList();
                string sheetName = string.Format(WinLossResources.TreeWinLoss_VirtualRate_SheetName, sheetIndex + 1);
                ExcelWorksheet worksheet = workSheets.Add(sheetName);
                for (var colIndex = 1; colIndex < 12; colIndex++)
                {
                    worksheet.Columns[colIndex].Width = 120 * ExcelExtensions.FontWidth;
                }

                worksheet.Columns[1].Width = 200 * ExcelExtensions.FontWidth;
                worksheet.Columns[2].Width = 200 * ExcelExtensions.FontWidth;
                worksheet.Columns[9].Width = 180 * ExcelExtensions.FontWidth;
                worksheet.Columns[10].Width = 180 * ExcelExtensions.FontWidth;
                worksheet.Columns[11].Width = 180 * ExcelExtensions.FontWidth;

                int currentRow = 0;
                string title = GetReportTitleByLevel(criteria);

                CellRange currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, 0, currentRow, 11);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow++;
                worksheet.ExportEmptyRow(currentRow, 0, ref currentRow, 11);

                this.ExportVirtualRateReportHeader(worksheet, ref currentRow);

                for (int userIndex = 0; userIndex < virtualRateByUserInSheet.Count; userIndex++)
                {
                    var virtualRateByUser = virtualRateByUserInSheet[userIndex];
                    this.ExportVirtualRateByUser(worksheet, userIndex, virtualRateByUser, ref currentRow);
                }
            }
        }

        /// <summary>
        /// Header for export excel tree win loss by product.
        /// </summary>
        /// <param name="workSheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTreeWinLossByProductHeader(ref ExcelWorksheet workSheet, ref int currentRow, int currentColumn)
        {
            CellRange currentCellRange;
            ExcelCell currentCell;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Currency;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealTurnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.ExtraPT;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealWinLoss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Comm;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealIncome;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 11);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.BaseRM;
            currentCellRange.Style = headerTextStyle;
            currentRow++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealTurnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.VirtualTurnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.TurnoverVariance;
            currentCellRange.Style = headerTextStyle;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn + 3, currentRow, currentColumn + 5);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.WinlossVariance;
            currentCellRange.Style = headerTextStyle;
            currentRow++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = WinLossResources.Plus;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = WinLossResources.Minus;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Total;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = WinLossResources.Plus;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = WinLossResources.Minus;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Total;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.VarianceMargin;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealIncome;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.VirtualWinLoss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Margin;
            currentCellRange.Style = headerTextStyle;
            currentRow++;
        }

        /// <summary>
        /// Assign data for body session of export excel tree win loss by product.
        /// </summary>
        /// <param name="isShowNA">Check show Not Available.</param>
        /// <param name="workSheet">The worksheet.</param>
        /// <param name="report">The list items.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startColumn">The current column.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTreeWinLossByProductAssignBodyData(
            bool isShowNA,
            ref ExcelWorksheet workSheet,
            List<TreeWinLossInfo> report,
            ref int currentRow,
            int startColumn)
        {
            ExcelCell currentCell;
            TreeWinLossInfo info;
            for (var i = 0; i < report.Count; i++)
            {
                if (i % 2 == 0)
                {
                    textStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                    twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                    twoDecimalPositiveNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                    twoDecimalPositivePercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                }
                else
                {
                    textStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                    twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                    twoDecimalPositiveNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                    twoDecimalPositivePercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                }

                info = report[i];
                var currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Name;
                currentCell.Style = textStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Turnover;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.ExtraAmount;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.WinLoss;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Comm;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.WinLossTotal;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                if (isShowNA)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BaseRMTurnover;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BaseRMVirtualTurnover;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;
                }

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVariancePlus;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVarianceMinus;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVarianceAbsoluteTotal;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWLVariancePlus;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWLVarianceMinus;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWinlostVarianceTotal;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                if (isShowNA || info.BaseRMVarianceMargin == Labels.NotAvailable)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.ToDecimal(info.BaseRMVarianceMargin);
                    currentCell.Style = twoDecimalPositiveNumberStyle;
                }

                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTotal;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMVirtualTotal;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                if (isShowNA || info.BaseRMMargin == Labels.NotAvailable)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.ToDecimal(info.BaseRMMargin);
                    currentCell.Style = twoDecimalPositiveNumberStyle;
                }

                currentRow++;
            }
        }

        /// <summary>
        /// Assign data for footer of export excel tree win loss by product.
        /// </summary>
        /// <param name="isShowNA">Check show Not Available.</param>
        /// <param name="workSheet">The worksheet.</param>
        /// <param name="totalInfo">The list total items.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startColumn">The current column.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTreeWinLossByProductAssignFooterData(
            bool isShowNA,
            ref ExcelWorksheet workSheet,
            List<TreeWinLossInfo> totalInfo,
            ref int currentRow,
            int startColumn)
        {
            TreeWinLossInfo info;
            CellRange currentCellRange;
            ExcelCell currentCell;
            for (int i = 0; i < totalInfo.Count; i++)
            {
                info = totalInfo[i];
                var currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                currentCellRange.Merged = true;
                currentCellRange.Value = info.Name;
                currentCellRange.Style = grandTotalRowTextStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentColumn += 6;
                if (isShowNA)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BaseRMTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BaseRMVirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;
                }

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVariancePlus;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVarianceMinus;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVarianceAbsoluteTotal;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWLVariancePlus;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWLVarianceMinus;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWinlostVarianceTotal;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                if (isShowNA || info.BaseRMVarianceMargin == Labels.NotAvailable)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.ToDecimal(info.BaseRMVarianceMargin);
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                }

                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTotal;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMVirtualTotal;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                if (isShowNA || info.BaseRMMargin == Labels.NotAvailable)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.ToDecimal(info.BaseRMMargin);
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                }

                currentRow++;
            }
        }

        /// <summary>
        /// Header for export excel tree win loss by level.
        /// </summary>
        /// <param name="workSheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startColumn">The start column.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTreeWinLossByLevelHeader(ref ExcelWorksheet workSheet, ref int currentRow, int startColumn)
        {
            CellRange currentCellRange;
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.No;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.User_Name;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealTurnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.ExtraPT;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealWinLoss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Comm;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 2, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealIncome;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 11);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.BaseRM;
            currentCellRange.Style = headerTextStyle;
            currentRow++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealTurnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.VirtualTurnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.TurnoverVariance;
            currentCellRange.Style = headerTextStyle;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn + 3, currentRow, currentColumn + 5);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.WinlossVariance;
            currentCellRange.Style = headerTextStyle;
            currentRow++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = WinLossResources.Plus;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = WinLossResources.Minus;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Total;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = WinLossResources.Plus;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = WinLossResources.Minus;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Total;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.VarianceMargin;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RealIncome;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.VirtualWinLoss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Margin;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 2, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = WinLossResources.VirtualRate;
            currentCellRange.Style = headerTextStyle;

            currentRow++;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTreeWinLossByLevelAssignBodyData(
            bool isShowNA,
            ref ExcelWorksheet workSheet,
            TreeWinLoss report,
            Dictionary<string, int> enabledVirtualRateUser,
            ref int currentRow,
            int startColumn)
        {
            ExcelCell currentCell;
            TreeWinLossInfo info;
            for (var i = 0; i < report.ChildsInfo.Count; i++)
            {
                if (i % 2 == 0)
                {
                    intStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                    textStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                    twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                    twoDecimalPositiveNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                    twoDecimalPositivePercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                }
                else
                {
                    intStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                    textStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                    twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                    twoDecimalPositiveNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                    twoDecimalPositivePercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                }

                info = report.ChildsInfo[i];
                var currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Converter.ToInt(i + 1);
                intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCell.Style = intStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Name;
                currentCell.Style = textStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Turnover;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.ExtraAmount;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.WinLoss;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Comm;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.WinLossTotal;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                if (isShowNA)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BaseRMTurnover;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BaseRMVirtualTurnover;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;
                }

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVariancePlus;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVarianceMinus;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVarianceAbsoluteTotal;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWLVariancePlus;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWLVarianceMinus;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWinlostVarianceTotal;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                if (isShowNA || info.BaseRMVarianceMargin == Labels.NotAvailable)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.ToDecimal(info.BaseRMVarianceMargin);
                    currentCell.Style = twoDecimalPositiveNumberStyle;
                }

                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTotal;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMVirtualTotal;
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                if (isShowNA || info.BaseRMMargin == Labels.NotAvailable)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.ToDecimal(info.BaseRMMargin);
                    currentCell.Style = twoDecimalPositiveNumberStyle;
                }

                currentColumn++;

                bool isDisabledVirtualRate = info.VirtualRate != 1;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = isDisabledVirtualRate ? "Disabled" :
                    GetVirtualRateColumnValue(enabledVirtualRateUser[info.Name]);
                currentCell.Style = isDisabledVirtualRate ? redTextStyle : rightTextStyle;

                currentRow++;
            }
        }

        /// <summary>
        /// Assign data for footer session of export excel tree win loss by level.
        /// </summary>
        /// <param name="isShowNA">Check show Not Available.</param>
        /// <param name="workSheet">The worksheet.</param>
        /// <param name="totalInfo">The list total items.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startColumn">The current column.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTreeWinLossByLevelAssignFooterData(
            bool isShowNA,
            ref ExcelWorksheet workSheet,
            List<TreeWinLossInfo> totalInfo,
            ref int currentRow,
            int startColumn)
        {
            CellRange currentCellRange;
            ExcelCell currentCell;
            TreeWinLossInfo info;
            for (int j = 0; j < totalInfo.Count; j++)
            {
                info = totalInfo[j];
                var currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
                currentCellRange.Merged = true;
                currentCellRange.Value = info.Name;
                currentCellRange.Style = grandTotalRowTextStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentColumn += 7;

                if (isShowNA)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BaseRMTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BaseRMVirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;
                }

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVariancePlus;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVarianceMinus;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTOVarianceAbsoluteTotal;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWLVariancePlus;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWLVarianceMinus;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMWinlostVarianceTotal;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                if (isShowNA || info.BaseRMVarianceMargin == Labels.NotAvailable)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.ToDecimal(info.BaseRMVarianceMargin);
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                }

                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMTotal;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BaseRMVirtualTotal;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                if (isShowNA || info.BaseRMMargin == Labels.NotAvailable)
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Labels.NotAvailable;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                }
                else
                {
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Converter.ToDecimal(info.BaseRMMargin);
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                }

                currentColumn++;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = string.Empty;
                currentCell.Style = grandTotalRowTextStyle;

                currentRow++;
            }
        }

        /// <summary>
        /// Exports the virtual rate report header.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportVirtualRateReportHeader(ExcelWorksheet workSheet, ref int currentRow)
        {
            workSheet.Rows[currentRow].Height = 450;
            int currentColumn = 0;
            ExcelCell currentCell;
            CellRange currentCellRange;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.No;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.User;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Sport;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 8);
            currentCellRange.Merged = true;
            currentCellRange.Value = string.Format("{0} %", Labels.VirtualRate);
            currentCellRange.Style = headerTextStyle;
            currentRow++;

            workSheet.Rows[currentRow].Height = 450;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.HDP;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.OU;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.OE;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels._1st_HDP;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels._1st_OU;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.FirstHalf_OE;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Moneyline;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels._1st_Moneyline;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.SetXGameYWinner;
            currentCell.Style = headerTextStyle;

            currentRow++;
        }

        /// <summary>
        /// Exports the virtual rate by user.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="userIndex">Index of the user.</param>
        /// <param name="virtualRateByUser">The virtual rate by user.</param>
        /// <param name="currentRow">The current row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportVirtualRateByUser(ExcelWorksheet workSheet, int userIndex, SMAVirtualRate virtualRateByUser, ref int currentRow)
        {
            int currentColumn = 0;
            ExcelCell currentCell;
            CellRange currentCellRange;
            int numberMergeRow = virtualRateByUser.ReportRows.Count - 1;

            if (userIndex % 2 == 0)
            {
                intStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                textStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                pecentStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
            }
            else
            {
                intStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                textStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                pecentStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
            }

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + numberMergeRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = userIndex + 1;
            intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCellRange.Style = intStyle;

            foreach (var virtualRateBySportAndLiveStatus in virtualRateByUser.ReportRows)
            {
                currentColumn = 1;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = virtualRateByUser.UserName;
                textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCell.Style = textStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                currentCell.Value = virtualRateBySportAndLiveStatus.Sport;
                currentCell.Style = textStyle;
                currentColumn++;

                VirtualRateByBettypeInfo virtualRateByBettype = null;
                for (int i = 0; i < 9; i++)
                {
                    virtualRateByBettype = virtualRateBySportAndLiveStatus.VirtualRateByBettypeInfo[i];
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    SetCellValue(virtualRateByBettype.VirtualRate, ref currentCell);
                    pecentStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentCell.Style = pecentStyle;
                    currentColumn++;
                }

                currentRow++;
            }
        }

        private string GetVirtualRateColumnValue(int userIndex)
        {
            int sheetIndexBeforRounding = Converter.ToInt((userIndex + 1) / _MaxUserPerSheet);
            int sheetIndex = (userIndex + 1) % _MaxUserPerSheet == 0 ?
                sheetIndexBeforRounding :
                sheetIndexBeforRounding + 1;
            return string.Format(WinLossResources.TreeWinLoss_VirtualRate_SheetName, sheetIndex);
        }

        private void SetCellValue(decimal? virtualRate, ref ExcelCell currentCell)
        {
            if (virtualRate == null)
            {
                currentCell.Value = string.Empty;
            }
            else
            {
                currentCell.Value = virtualRate;
            }
        }

        /// <summary>
        /// Gets the report title by level.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Report title.</returns>
        private string GetReportTitleByLevel(GetTreeWinLossByLevelCriteria criteria)
        {
            string title = string.Empty;
            switch (criteria.RoleId)
            {
                case 5:
                    title = string.Format(WinLossResources.SuperWinLoss, criteria.Name);
                    break;

                case 4:
                    title = string.Format(WinLossResources.MasterWinLoss, criteria.Name);
                    break;

                case 3:
                    title = string.Format(WinLossResources.AgentWinLoss, criteria.Name);
                    break;

                case 2:
                    title = string.Format(WinLossResources.MemberWinLoss, criteria.Name);
                    break;
            }

            title += string.Format(
                " - From {0} To {1}",
                criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));

            return title;
        }
    }
}