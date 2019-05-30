namespace Fanex.AccountingReports.Main.Services.Implement.WinLoss3rdSummary
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Models.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    using spreadsheetLineStyle = ExcelUtil.Spreadsheet;

    /// <summary>
    /// Export win loss summary 3rd service.
    /// </summary>
    public partial class WinLossSummary3rdService
    {
        private const short GrandTotalColorIndex = 47;
        private const short GrandTotalBorderColorIndex = 49;

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport(GetReportCriteria criteria)
        {
            WinLoss3rdInfo report = SessionManager.Get<WinLoss3rdInfo>(MainConstants.WinLossSummary3rdReportSessionKey);
            if (report == null)
            {
                report = this.GetReport(criteria);
                if (report == null || report.SportTypeWinLosses.Count == 0)
                {
                    return;
                }
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = MainResources.WinLossSummary3rd_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int numberOfGroupColumnsTotal = 0;
            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var parentStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.ParentColor, true, HorizontalAlignmentStyle.Center);
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, true, HorizontalAlignmentStyle.Left);
            var text8RM_BARMStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);

            var soccerTotalStyle = Color.FromArgb(221, 221, 221);
            var subTotalTitleStyle = ExcelExtensions.GetCellTextStyle(soccerTotalStyle, true, HorizontalAlignmentStyle.Right);
            var titleStyle_Total = ExcelExtensions.GetCellTextStyle(ExcelExtensions.TotalRowColor, true, HorizontalAlignmentStyle.Right);

            // Grand total style
            var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true, HorizontalAlignmentStyle.Right);
            var grandTotalRowTextStyleWithBorder = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true, HorizontalAlignmentStyle.Right);
            grandTotalRowTextStyleWithBorder.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, spreadsheetLineStyle.LineStyle.Medium);

            // fomular
            var fomularStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Left);

            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            var maxColumIndex = 10;
            workSheet.Columns[0].Width = 200 * ExcelExtensions.FontWidth;
            workSheet.Columns[1].Width = 150 * ExcelExtensions.FontWidth;
            for (int colIndex = 2; colIndex <= maxColumIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            var title = string.Format(
                MainResources.WinLossSummary3rd_SummaryReportTitle,
                criteria.Fromdate.ToShortFormatedDate(),
                 criteria.Todate.ToShortFormatedDate());
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumIndex);

            // First Row
            currentColumn = startColumn;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Bettype;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Member_Win_Loss;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.SMA_Win_Loss;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Comm;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.House_Income;
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Variance;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.House_Turnover;
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Margin;
            currentColumn += 2;

            // Second Row
            currentRow++;
            currentColumn = startColumn + 4;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Real;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Virtual;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            // Variance
            currentColumn++; // ignore Variance column

            // Real Turnover
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Real;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Virtual;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            // Real Margin
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Real;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Virtual;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            int sportLen, betLen;
            SportTypeWinLossInfo sportWinLoss = null;
            BetTypeWinLossInfo betWinLoss = null;
            string betTypeName = string.Empty;
            bool liveIndicator = false;
            if (report != null && (sportLen = report.SportTypeWinLosses.Count) > 0)
            {
                for (int i = 0; i < sportLen; i++)
                {
                    currentRow++;
                    sportWinLoss = report.SportTypeWinLosses[i];

                    currentColumn = startColumn;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumIndex);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = sportWinLoss.Name;
                    currentCellRange.Style = parentStyle;

                    // Bet type
                    var isNewCricket = false;
                    var betWinLossNewCricket = sportWinLoss.BetTypeWinLosses[0];
                    betLen = sportWinLoss.BetTypeWinLosses.Count;
                    for (int j = 0; j < betLen; j++)
                    {
                        betWinLoss = sportWinLoss.BetTypeWinLosses[j];
                        if (betWinLoss.Id == 500)
                        {
                            isNewCricket = true;
                            betWinLossNewCricket = betWinLoss;
                            continue;
                        }

                        var backgroundColor = j % 2 == 0 ? ExcelExtensions.BackGroundColor : ExcelExtensions.EvenRowColor;
                        textStyle.FillPattern.SetSolid(backgroundColor);
                        text8RM_BARMStyle.FillPattern.SetSolid(backgroundColor);

                        {
                            var winloss = betWinLoss.Total;
                            if (betWinLoss.Id < 221 && betWinLoss.Id > 212)
                            {
                                // Total of each total sport type
                                currentRow++;
                                currentColumn = startColumn;
                                this.WriteWinLossSummaryTotal(
                                    workSheet,
                                    currentRow,
                                    currentColumn,
                                    winloss.Name + " " + Labels.Total + ":",
                                    subTotalTitleStyle,
                                    numberOfGroupColumnsTotal,
                                    betWinLoss.Total,
                                    soccerTotalStyle);
                                continue;
                            }

                            currentRow++;
                            currentColumn = startColumn;
                            betTypeName = winloss.Name;
                            liveIndicator = Convert.ToBoolean(winloss.LiveIndicator);

                            this.WriteWinLossSummary(
                                workSheet,
                                currentRow,
                                currentColumn,
                                winloss,
                                textStyle,
                                backgroundColor);
                        }

                        // 8RM winloss
                        if (betWinLoss.EightRM != null)
                        {
                            currentRow++;
                            currentColumn = startColumn;

                            this.WriteWinLossSummary(
                                workSheet,
                                currentRow,
                                currentColumn,
                                betWinLoss.EightRM,
                                text8RM_BARMStyle,
                                backgroundColor);
                        }

                        // BARM winloss
                        if (betWinLoss.BARM != null)
                        {
                            currentRow++;
                            currentColumn = startColumn;

                            this.WriteWinLossSummary(
                                workSheet,
                                currentRow,
                                currentColumn,
                                betWinLoss.BARM,
                                text8RM_BARMStyle,
                                backgroundColor);
                        }

                        // Hedge
                        if (betWinLoss.Hedge != null)
                        {
                            currentRow++;
                            currentColumn = startColumn;

                            this.WriteHedgeWinLossSummary(
                                workSheet,
                                currentRow,
                                currentColumn,
                                Labels.Hedge_title,
                                betWinLoss.Hedge,
                                text8RM_BARMStyle,
                                backgroundColor);
                        }
                        var hasOtherMatchType = betWinLoss.EightRM != null || betWinLoss.BARM != null || betWinLoss.Hedge != null;
                        var showNormal = betWinLoss.Normal != null && hasOtherMatchType;
                        if (showNormal)
                        {
                            currentRow++;
                            currentColumn = startColumn;

                            this.WriteHedgeWinLossSummary(
                                workSheet,
                                currentRow,
                                currentColumn,
                                Labels.Normal_title,
                                betWinLoss.Normal,
                                text8RM_BARMStyle,
                                backgroundColor);
                        }
                    }

                    // for NewCricket
                    if (isNewCricket == true)
                    {
                        // for Total
                        currentRow++;
                        currentColumn = startColumn;
                        this.WriteWinLossSummaryTotal(
                            workSheet,
                            currentRow,
                            currentColumn,
                            "Total:",
                            titleStyle_Total,
                            numberOfGroupColumnsTotal,
                            sportWinLoss.TotalForNewCricket,
                            ExcelExtensions.TotalRowColor);

                        // for Charge Commisson

                        {
                            var winloss = betWinLossNewCricket.Total;
                            currentRow++;
                            currentColumn = startColumn;
                            this.WriteWinLossSummaryTotal(
                                workSheet,
                                currentRow,
                                currentColumn,
                                winloss.Name,
                                titleStyle_Total,
                                numberOfGroupColumnsTotal,
                                winloss,
                                ExcelExtensions.TotalRowColor);
                        }
                    }

                    // Total of Sport
                    currentRow++;
                    currentColumn = startColumn;

                    this.WriteWinLossSummaryTotal(
                        workSheet,
                        currentRow,
                        currentColumn,
                        string.Format("{0} {1}:", sportWinLoss.Name, Labels.Total),
                        titleStyle_Total,
                        numberOfGroupColumnsTotal,
                        sportWinLoss.Total,
                        ExcelExtensions.TotalRowColor);

                    if (sportWinLoss.IBCTotal != null)
                    {
                        currentRow++;
                        currentColumn = startColumn;

                        this.WriteWinLossSummaryTotal(
                            workSheet,
                            currentRow,
                            currentColumn,
                            Labels._8RM_BARM + ":",
                            titleStyle_Total,
                            numberOfGroupColumnsTotal,
                            sportWinLoss.IBCTotal,
                            ExcelExtensions.TotalRowColor);
                    }

                    if (sportWinLoss.HedgeTotal != null)
                    {
                        currentRow++;
                        currentColumn = startColumn;

                        this.WriteWinLossSummaryTotal(
                            workSheet,
                            currentRow,
                            currentColumn,
                            Labels.Hedge_title + ":",
                            titleStyle_Total,
                            numberOfGroupColumnsTotal,
                            sportWinLoss.HedgeTotal,
                            ExcelExtensions.TotalRowColor);
                    }
                    var totalHasOtherMatchType = sportWinLoss.IBCTotal != null || sportWinLoss.HedgeTotal != null;
                    var showTotalNormal = sportWinLoss.NormalTotal != null && totalHasOtherMatchType;

                    if (showTotalNormal)
                    {
                        currentRow++;
                        currentColumn = startColumn;

                        this.WriteWinLossSummaryTotal(
                            workSheet,
                            currentRow,
                            currentColumn,
                            Labels.Normal_title + ":",
                            titleStyle_Total,
                            numberOfGroupColumnsTotal,
                            sportWinLoss.NormalTotal,
                            ExcelExtensions.TotalRowColor);
                    }
                }

                // All Total
                currentRow++;
                currentColumn = startColumn;

                this.WriteWinLossSummaryTotal(
                    workSheet,
                    currentRow,
                    currentColumn,
                    Labels.Grand_Total.ToUpper() + ":",
                    grandTotalRowTextStyleWithBorder,
                    numberOfGroupColumnsTotal,
                    report.AllTotal,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true);

                // IBC (8RM + BARM)
                currentRow++;
                currentColumn = startColumn;

                this.WriteWinLossSummaryTotal(
                    workSheet,
                    currentRow,
                    currentColumn,
                    "(8RM + BARM) Total:",
                    grandTotalRowTextStyle,
                    numberOfGroupColumnsTotal,
                    report.IBCTotal,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumIndex);
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow - 1, currentColumn, currentRow - 1, currentColumn + maxColumIndex);
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            }

            currentRow++;
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = MainResources.WinLossSummary3rd_Note;
            currentCellRange.Style = fomularStyle;

            var reportName = string.Format(
                MainResources.WinLossSummary3rd_ReportName,
                criteria.Fromdate.ToShortFormatedDateWithSlash(),
                criteria.Todate.ToShortFormatedDateWithSlash()).Replace("/", "_");
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the details.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportDetails(GetMatchGameDetailsCriteria criteria)
        {
            MatchGame3rdInfo report = SessionManager.Get<MatchGame3rdInfo>(MainConstants.WinLossSummary3rdReportDetailSessionKEy);
            if (report == null)
            {
                report = this.GetDetails(criteria);
                if (report == null || report.MatchGameBetTypes.Count == 0)
                {
                    return;
                }
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = MainResources.WinLossSummary3rdDetail_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            string title = string.Format(
                MainResources.WinLossSummary3rd_SummaryReportDetailTitle,
                criteria.ParentName,
                criteria.FromDate.ToShortFormatedDate(),
                criteria.ToDate.ToShortFormatedDate());

            SportTypeWinLoss type = report.Type;
            if (type == SportTypeWinLoss.SportBook || type == SportTypeWinLoss.VirtualSports)
            {
                this.ExportHomeAwayTemplateDetails(report, title, ref workSheet);
            }
            else
            {
                this.ExportTemplateDetails(report, title, ref workSheet);
            }

            var reportName = string.Format(
                MainResources.WinLossSummary3rdDetail_ReportName,
                criteria.FromDate.ToShortFormatedDateWithSlash(),
                criteria.ToDate.ToShortFormatedDateWithSlash()).Replace("/", "_");
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Writes the win loss summary.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="titleStyle">The title style.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        private void WriteWinLossSummary(
            ExcelWorksheet workSheet,
            int currentRow,
            int currentColumn,
            WinLossInfo winLoss,
            CellStyle titleStyle,
            Color? backgroundColor = null)
        {
            ExcelCell currentCell;

            // Bet type
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = titleStyle;

            string betTypeName = winLoss.Name;
            if (winLoss.IBC == RowType.Normal && winLoss.LiveIndicator == 1)
            {
                betTypeName = string.Format("{0}! {1}", Labels.Live, betTypeName);
            }

            currentCell.Value = betTypeName;
            currentColumn++;

            // WinLoss detail
            Color backgroundColorValue = backgroundColor.HasValue ? backgroundColor.Value : ExcelExtensions.BackGroundColor;
            this.WriteWinLossDetail(
                workSheet,
                currentRow,
                currentColumn,
                winLoss,
                backgroundColorValue,
                false,
                false);
        }

        /// <summary>
        /// Writes the hedge win loss summary.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="betTypeName">Name of the bet type.</param>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="titleStyle">The title style.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        private void WriteHedgeWinLossSummary(
            ExcelWorksheet workSheet,
            int currentRow,
            int currentColumn,
            string betTypeName,
            WinLossInfo winLoss,
            CellStyle titleStyle,
            Color? backgroundColor = null)
        {
            ExcelCell currentCell;

            // Bet type
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = titleStyle;
            currentCell.Value = betTypeName;
            currentColumn++;

            // WinLoss detail
            Color backgroundColorValue = backgroundColor.HasValue ? backgroundColor.Value : ExcelExtensions.BackGroundColor;
            this.WriteWinLossDetail(
                workSheet,
                currentRow,
                currentColumn,
                winLoss,
                backgroundColorValue,
                false,
                false);
        }

        /// <summary>
        /// Writes the win loss summary total.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="title">The title.</param>
        /// <param name="titleStyle">The title style.</param>
        /// <param name="numberOfGroupColumn">The number of group column.</param>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="bold">If set to <c>true</c> [bold].</param>
        /// <param name="setGrandBorder">If set to <c>true</c> [set grand border].</param>
        private void WriteWinLossSummaryTotal(
            ExcelWorksheet workSheet,
            int currentRow,
            int currentColumn,
            string title,
            CellStyle titleStyle,
            int numberOfGroupColumn,
            WinLossInfo winLoss,
            Color backgroundColor,
            bool bold = false,
            bool setGrandBorder = false)
        {
            CellRange currentCellRange;

            // Total
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + numberOfGroupColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = titleStyle;

            currentColumn++;
            currentColumn += numberOfGroupColumn;

            // WinLoss detail
            this.WriteWinLossDetail(
                workSheet,
                currentRow,
                currentColumn,
                winLoss,
                backgroundColor,
                bold,
                setGrandBorder);
        }

        /// <summary>
        /// Writes the win loss detail.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="isBold">If set to <c>true</c> [is bold].</param>
        /// <param name="topBorder">If set to <c>true</c> [top border].</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void WriteWinLossDetail(
            ExcelWorksheet workSheet,
            int currentRow,
            int currentColumn,
            WinLossInfo winLoss,
            Color backgroundColor,
            bool isBold,
            bool topBorder)
        {
            ExcelCell currentCell;

            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, isBold, false, false);
            var twoDecimalNumberStyleWithColor = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, isBold, true, false);
            var twoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitPercentFormat, backgroundColor, isBold, true, false);
            if (topBorder)
            {
                twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, spreadsheetLineStyle.LineStyle.Medium);
                twoDecimalNumberStyleWithColor.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, spreadsheetLineStyle.LineStyle.Medium);
                twoDecimalPercentNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, spreadsheetLineStyle.LineStyle.Medium);
            }

            // WinLoss
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.WinLoss;
            currentCell.Style = twoDecimalNumberStyleWithColor;
            currentColumn++;

            // Position
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            // Commission
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = winLoss.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            // Real Line Total
            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (winLoss.RealLineTotal.HasValue)
            {
                currentCell.Value = winLoss.RealLineTotal.Value;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = twoDecimalNumberStyleWithColor;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (winLoss.VirtualLineTotal.HasValue)
            {
                currentCell.Value = winLoss.VirtualLineTotal.Value;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = twoDecimalNumberStyleWithColor;
            currentColumn++;

            // Variance
            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (winLoss.Variance.HasValue)
            {
                currentCell.Value = winLoss.Variance.Value;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = twoDecimalNumberStyleWithColor;
            currentColumn++;

            // Real Turnover
            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (winLoss.RealTurnover.HasValue)
            {
                currentCell.Value = winLoss.RealTurnover.Value;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (winLoss.VirtualTurnover.HasValue)
            {
                currentCell.Value = winLoss.VirtualTurnover.Value;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            // Real Margin
            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (winLoss.RealMargin.HasValue)
            {
                currentCell.Value = winLoss.RealMargin.Value;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = twoDecimalPercentNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (winLoss.VirtualMargin.HasValue)
            {
                currentCell.Value = winLoss.VirtualMargin.Value;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = twoDecimalPercentNumberStyle;
        }

        /// <summary>
        /// Exports the home away template details.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="title">The title.</param>
        /// <param name="workSheet">The work sheet.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportHomeAwayTemplateDetails(MatchGame3rdInfo details, string title, ref ExcelWorksheet workSheet)
        {
            int numberOfGroupColumnsTotal = 3;
            int startRow = 0;
            int startColumn = 0;
            int currentRow = startRow;

            // header style
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var parentStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.ParentColor, true, HorizontalAlignmentStyle.Center);
            var leagueStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, true, HorizontalAlignmentStyle.Left);

            // text style
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);

            // Total style
            var titleStyle_TotalByLeague = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, true, HorizontalAlignmentStyle.Right);
            var titleStyle_TotalByBetType = ExcelExtensions.GetCellTextStyle(ExcelExtensions.TotalRowColor, true, HorizontalAlignmentStyle.Right);

            var grandTotalRowTextStyleWithBorder = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true, HorizontalAlignmentStyle.Right);
            grandTotalRowTextStyleWithBorder.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, spreadsheetLineStyle.LineStyle.Medium);

            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            var maxColumIndex = 13;

            workSheet.Columns[0].Width = 120 * ExcelExtensions.FontWidth;
            workSheet.Columns[1].Width = 200 * ExcelExtensions.FontWidth;
            workSheet.Columns[2].Width = 40 * ExcelExtensions.FontWidth;
            workSheet.Columns[3].Width = 200 * ExcelExtensions.FontWidth;
            for (int colIndex = 4; colIndex <= maxColumIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumIndex);

            // First Row
            currentColumn = startColumn;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Code;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Event;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = "Member Win Loss";
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = "SMA Win Loss";
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Comm;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = "House Income";
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Variance;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = "House Turnover";
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Margin;

            // Second Row
            // Ignore the first 7 columns
            currentRow++;
            currentColumn = startColumn;
            currentColumn += 7;

            // Line Total
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            // Ignore Variance column
            currentColumn++;

            // Turnover
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            // Margin
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            Color oddEvenColor;
            int betLen, matchGameLeagueLen, matchGameLen;
            MatchGameBetTypeInfo betWinLoss;
            MatchGameLeagueInfo matchGameLeagueWinLoss;
            MatchGameWinLossInfo matchGameWinLoss;

            betLen = details.MatchGameBetTypes.Count;
            for (int i = 0; i < betLen; i++)
            {
                betWinLoss = details.MatchGameBetTypes[i];

                currentRow++;
                currentColumn = startColumn;

                string betName = string.Format(
                    "{0} {1}",
                    betWinLoss.LiveIndicator == 1 ? Labels.Live + "!" : string.Empty,
                    betWinLoss.Key.Name);
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = betName;
                currentCellRange.Style = parentStyle;

                // League
                matchGameLeagueLen = betWinLoss.MatchGameLeagues.Count;
                for (int j = 0; j < matchGameLeagueLen; j++)
                {
                    matchGameLeagueWinLoss = betWinLoss.MatchGameLeagues[j];

                    currentRow++;
                    currentColumn = startColumn;

                    // League name
                    // Ignore the first column
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumIndex - 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = matchGameLeagueWinLoss.Key.Name;
                    currentCellRange.Style = leagueStyle;

                    // Match winloss
                    matchGameLen = matchGameLeagueWinLoss.MatchGameWinLosses.Count;
                    for (int k = 0; k < matchGameLen; k++)
                    {
                        oddEvenColor = k % 2 == 0
                            ? ExcelExtensions.EvenRowColor
                            : ExcelExtensions.OddRowColor;

                        matchGameWinLoss = matchGameLeagueWinLoss.MatchGameWinLosses[k];

                        currentRow++;
                        currentColumn = startColumn;

                        // winloss
                        textStyle.FillPattern.SetSolid(oddEvenColor);
                        this.WriteWinLossSummaryHomeAwayDetails(
                            workSheet,
                            currentRow,
                            currentColumn,
                            matchGameWinLoss,
                            textStyle,
                            betWinLoss.Key.Id == BetTypeConstants.Outright);
                    }

                    // Total by League
                    currentRow++;
                    currentColumn = startColumn;
                    this.WriteWinLossSummaryTotal(
                        workSheet,
                        currentRow,
                        currentColumn,
                        Labels.Total + ":",
                        titleStyle_TotalByLeague,
                        numberOfGroupColumnsTotal,
                        matchGameLeagueWinLoss.Total,
                        ExcelExtensions.BackGroundColor);
                }

                // Total by Bet type
                currentRow++;
                currentColumn = startColumn;
                this.WriteWinLossSummaryTotal(
                    workSheet,
                    currentRow,
                    currentColumn,
                    string.Format("{0} {1}:", betName, Labels.Total),
                    titleStyle_TotalByBetType,
                    numberOfGroupColumnsTotal,
                    betWinLoss.Total,
                    ExcelExtensions.TotalRowColor);
            }

            // Grand total
            currentRow++;
            currentColumn = startColumn;
            this.WriteWinLossSummaryTotal(
                workSheet,
                currentRow,
                currentColumn,
                Labels.Grand_Total + ":",
                grandTotalRowTextStyleWithBorder,
                numberOfGroupColumnsTotal,
                details.Total,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true);
        }

        /// <summary>
        /// Exports the template details.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="title">The title.</param>
        /// <param name="workSheet">The work sheet.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTemplateDetails(MatchGame3rdInfo details, string title, ref ExcelWorksheet workSheet)
        {
            int numberOfGroupColumnsTotal = 0;
            int startRow = 0;
            int startColumn = 0;
            int currentRow = startRow;

            // header style
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var parentStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.ParentColor, true, HorizontalAlignmentStyle.Center);
            var leagueStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, true, HorizontalAlignmentStyle.Left);

            // text style
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);

            // Total style
            var titleStyle_TotalByLeague = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, true, HorizontalAlignmentStyle.Right);
            var titleStyle_TotalByBetType = ExcelExtensions.GetCellTextStyle(ExcelExtensions.TotalRowColor, true, HorizontalAlignmentStyle.Right);

            var grandTotalRowTextStyleWithBorder = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true, HorizontalAlignmentStyle.Right);
            grandTotalRowTextStyleWithBorder.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, spreadsheetLineStyle.LineStyle.Medium);

            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            var maxColumIndex = 10;

            workSheet.Columns[0].Width = 150 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumIndex);

            // First Row
            currentColumn = startColumn;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Code;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = "Member Win Loss";
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = "SMA Win Loss";
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Comm;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = "House Income";
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Variance;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = "House Turnover";
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Margin;

            // Second Row
            // Ignore the first 4 columns
            currentRow++;
            currentColumn = startColumn;
            currentColumn += 4;

            // Line Total
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            // Ignore Variance column
            currentColumn++;

            // Turnover
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            // Margin
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            Color oddEvenColor;
            int betLen, matchGameLeagueLen, matchGameLen;
            MatchGameBetTypeInfo betWinLoss;
            MatchGameLeagueInfo matchGameLeagueWinLoss;
            MatchGameWinLossInfo matchGameWinLoss;

            betLen = details.MatchGameBetTypes.Count;
            for (int i = 0; i < betLen; i++)
            {
                betWinLoss = details.MatchGameBetTypes[i];

                currentRow++;
                currentColumn = startColumn;
                string betName = string.Format(
                    "{0} {1}",
                    betWinLoss.LiveIndicator == 1 ? Labels.Live + "!" : string.Empty,
                    betWinLoss.Key.Name);
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = betName;
                currentCellRange.Style = parentStyle;

                matchGameLeagueLen = betWinLoss.MatchGameLeagues.Count;
                for (int j = 0; j < matchGameLeagueLen; j++)
                {
                    matchGameLeagueWinLoss = betWinLoss.MatchGameLeagues[j];

                    currentRow++;
                    currentColumn = startColumn;

                    // League name
                    // Ignore the first column
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumIndex - 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = matchGameLeagueWinLoss.Key.Name;
                    currentCellRange.Style = leagueStyle;

                    // Match winloss
                    matchGameLen = matchGameLeagueWinLoss.MatchGameWinLosses.Count;
                    for (int k = 0; k < matchGameLen; k++)
                    {
                        oddEvenColor = k % 2 == 0
                            ? ExcelExtensions.EvenRowColor
                            : ExcelExtensions.OddRowColor;

                        matchGameWinLoss = matchGameLeagueWinLoss.MatchGameWinLosses[k];

                        currentRow++;
                        currentColumn = startColumn;

                        // winloss
                        textStyle.FillPattern.SetSolid(oddEvenColor);
                        this.WriteWinLossSummaryDetails(
                            workSheet,
                            currentRow,
                            currentColumn,
                            matchGameWinLoss,
                            textStyle);
                    }

                    // Total by League
                    currentRow++;
                    currentColumn = startColumn;
                    this.WriteWinLossSummaryTotal(
                        workSheet,
                        currentRow,
                        currentColumn,
                        Labels.Total + ":",
                        titleStyle_TotalByLeague,
                        numberOfGroupColumnsTotal,
                        matchGameLeagueWinLoss.Total,
                        ExcelExtensions.BackGroundColor);
                }

                // Total by Bet type
                currentRow++;
                currentColumn = startColumn;
                this.WriteWinLossSummaryTotal(
                    workSheet,
                    currentRow,
                    currentColumn,
                    string.Format("{0} {1}:", betName, Labels.Total),
                    titleStyle_TotalByBetType,
                    numberOfGroupColumnsTotal,
                    betWinLoss.Total,
                    ExcelExtensions.TotalRowColor);
            }

            // Grand total
            currentRow++;
            currentColumn = startColumn;
            this.WriteWinLossSummaryTotal(
                workSheet,
                currentRow,
                currentColumn,
                Labels.Grand_Total + ":",
                grandTotalRowTextStyleWithBorder,
                numberOfGroupColumnsTotal,
                details.Total,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true);
        }

        /// <summary>
        /// Writes the win loss summary details.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="matchGameWinLoss">The match game win loss.</param>
        /// <param name="titleStyle">The title style.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        private void WriteWinLossSummaryDetails(
            ExcelWorksheet workSheet,
            int currentRow,
            int currentColumn,
            MatchGameWinLossInfo matchGameWinLoss,
            CellStyle titleStyle,
            Color? backgroundColor = null)
        {
            ExcelCell currentCell;

            // Code
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = titleStyle;
            currentCell.Value = matchGameWinLoss.MatchCode;
            currentColumn++;

            // WinLoss detail
            Color backgroundColorValue = backgroundColor.HasValue ? backgroundColor.Value : ExcelExtensions.BackGroundColor;
            this.WriteWinLossDetail(
                workSheet,
                currentRow,
                currentColumn,
                matchGameWinLoss.WinLossDetail,
                backgroundColorValue,
                false,
                false);
        }

        /// <summary>
        /// Writes the win loss summary home away details.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="matchGameWinLoss">The match game win loss.</param>
        /// <param name="titleStyle">The title style.</param>
        /// <param name="isOutright">If set to <c>true</c> [is outright].</param>
        /// <param name="backgroundColor">Color of the background.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void WriteWinLossSummaryHomeAwayDetails(
            ExcelWorksheet workSheet,
            int currentRow,
            int currentColumn,
            MatchGameWinLossInfo matchGameWinLoss,
            CellStyle titleStyle,
            bool isOutright,
            Color? backgroundColor = null)
        {
            ExcelCell currentCell;

            // Code
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = titleStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCell.Value = matchGameWinLoss.MatchCode;
            currentColumn++;

            if (isOutright)
            {
                CellRange currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
                currentCellRange.Merged = true;
                currentCellRange.Style = titleStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                currentCellRange.Value = matchGameWinLoss.HomeTeam.Name;
                currentColumn += 3;
            }
            else
            {
                // Home
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = titleStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCell.Value = matchGameWinLoss.HomeTeam.Name;
                currentColumn++;

                // -vs-
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = titleStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCell.Value = Labels.Vs;
                currentColumn++;

                // Away
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = titleStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                currentCell.Value = matchGameWinLoss.AwayTeam.Name;
                currentColumn++;
            }

            // WinLoss detail
            Color backgroundColorValue = backgroundColor.HasValue ? backgroundColor.Value : ExcelExtensions.BackGroundColor;
            this.WriteWinLossDetail(
                workSheet,
                currentRow,
                currentColumn,
                matchGameWinLoss.WinLossDetail,
                backgroundColorValue,
                false,
                false);
        }

        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        private void InitColumnWith(ref ISheet worksheet)
        {
            for (int i = 0; i < 14; i++)
            {
                worksheet.SetColumnWidth(i, 150 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(1, 220 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(2, 60 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(3, 220 * ExcelExtensions.FontWidth);
        }
    }
}