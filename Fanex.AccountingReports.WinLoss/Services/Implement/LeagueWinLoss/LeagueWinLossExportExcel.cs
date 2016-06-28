using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using ExcelUtil.Spreadsheet;
using Fanex.AccountingReports.Core.Configurations;
using Fanex.AccountingReports.Core.Constants;
using Fanex.AccountingReports.Core.Extensions;
using Fanex.AccountingReports.Core.Services.Impl;
using Fanex.AccountingReports.Core.Web;
using Fanex.AccountingReports.WinLoss.Constants;
using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
using Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss;
using Fanex.AccountingReports.WinLoss.Resources;
using Fanex.AccountingReports.WinLoss.Services.Interface;
using Fanex.AccountingReports.WinLoss.Services.Interface.LeagueWinLoss;
using Fanex.Common;

namespace Fanex.AccountingReports.WinLoss.Services.Implement.LeagueWinLoss
{
    public class LeagueWinLossExportExcel : ILeagueWinLossExportExcel
    {
        private readonly ILeagueWinLossService _reportService;
        private RMElement _8rmConfig;
        private RMElement _barmConfig;

        public LeagueWinLossExportExcel() : this(null)
        {
        }

        public LeagueWinLossExportExcel(ILeagueWinLossService reportService)
        {
            _reportService = reportService ?? new LeagueWinLossService();
            this._8rmConfig = RMSection.Instance.Report[ReportName.LeagueWinLoss.ToString()]
                .ReportConfigs[RMSection.RMType.EightRM.ToString()];
            this._barmConfig = RMSection.Instance.Report[ReportName.LeagueWinLoss.ToString()]
                .ReportConfigs[RMSection.RMType.BARM.ToString()];
        }

        /// <summary>
        /// Exports the league win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        public void ExportLeagueWinLoss(GetLeagueWinLossCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LeagueWinLossInfo>(WinLossContants.LeagueWinLossSessionKey);
                if (report == null)
                {
                    ILeagueWinLossService service = new LeagueWinLossService();
                    report = service.GetLeagueWinLoss(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = WinLossResources.LeagueWinLoss_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                var titleColor = ExcelExtensions.HeaderTitleColor;
                var headerColor = ExcelExtensions.HeaderTitleColor;
                var groupHeadLineColor = ExcelExtensions.SubTotalRowColor;

                var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    false,
                    false);
                var positiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    true,
                    false);
                var positiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    true,
                    false);

                var subRowTextStyle = ExcelExtensions.GetCellTextStyle(groupHeadLineColor, true);
                var subRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    groupHeadLineColor,
                    true,
                    false,
                    false);
                var subRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    groupHeadLineColor,
                    true,
                    true,
                    false);
                var subRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    groupHeadLineColor,
                    true,
                    true,
                    false);

                var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                var grandTotalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);
                var grandTotalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);

                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                currentColumn = startColumn;
                for (int colIndex = 0; colIndex < 7; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                workSheet.Columns[startColumn].Width = 350 * ExcelExtensions.FontWidth;
                var title = string.Format(
                    WinLossResources.LeagueWinLoss_ReportTitle,
                    criteria.Winlostdate.ToShortFormatedDate());

                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 6);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style.FormatTitleCell(titleColor);
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 6);

                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.League;
                currentCell.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Win_Loss;
                currentCell.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Position;
                currentCell.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Comm;
                currentCell.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.LineTotal;
                currentCell.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Turnover;
                currentCell.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Margin;
                currentCell.Style.FormatTitleCell(headerColor);
                currentColumn++;
                currentRow++;

                if (report.ChildsInfo.Count > 0)
                {
                    var len = report.ChildsInfo.Count;
                    LeagueWinLossInfo info;
                    for (var i = 0; i < len; i++)
                    {
                        var sport = report.ChildsInfo[i];

                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = sport.Name;
                        currentCellRange.Style.FormatCellText(groupHeadLineColor, true);
                        currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                        currentRow++;

                        var childsLength = sport.ChildsInfo.Count;
                        for (var j = 0; j < childsLength; j++)
                        {
                            if (j % 2 == 0)
                            {
                                textStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                                twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                                positiveTwoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                                positiveTwoDecimalPercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                            }
                            else
                            {
                                textStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                                twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                                positiveTwoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                                positiveTwoDecimalPercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                            }

                            info = sport.ChildsInfo[j];
                            currentColumn = startColumn;
                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Name;
                            currentCell.Style = textStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.WinLoss;
                            currentCell.Style = positiveTwoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Position;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Commission;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.LineTotal;
                            currentCell.Style = positiveTwoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.TurnOver;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            if (info.TurnOver > 0)
                            {
                                currentCell.Value = Converter.ToDecimal(info.Margin);
                            }
                            else
                            {
                                currentCell.Value = info.Margin.ToString();
                            }

                            currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                            currentColumn++;
                            currentRow++;
                        }

                        info = sport.GrandTotal;
                        const int NewCricketSportType = 50;
                        var sportTotalRowCount = sport.Id != NewCricketSportType
                            ? ExportLeagueWinLossNormalSportsTotal(startColumn, currentRow, info, workSheet, textStyle, positiveTwoDecimalNumberStyle, twoDecimalNumberStyle, positiveTwoDecimalPercentNumberStyle)
                            : ExportLeagueWinLossNewCricketTotal(startColumn, currentRow, info, workSheet, textStyle, positiveTwoDecimalNumberStyle, twoDecimalNumberStyle, positiveTwoDecimalPercentNumberStyle);

                        currentRow += sportTotalRowCount;
                    }

                    info = report.Total;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.WinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Position;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Commission;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.TurnOver;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    if (info.TurnOver > 0)
                    {
                        currentCell.Value = Converter.ToDecimal(info.Margin);
                    }
                    else
                    {
                        currentCell.Value = info.Margin.ToString();
                    }

                    currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;
                    currentRow++;

                    if (this._8rmConfig.IsActive)
                    {
                        info = report.EightRM;
                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = info.Name;
                        currentCellRange.Style = grandTotalRowTextStyle;
                        currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        currentColumn += 4;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = string.Empty;
                        currentCellRange.Style = grandTotalRowTextStyle;
                        currentColumn += 2;
                        currentRow++;
                    }

                    if (this._barmConfig.IsActive)
                    {
                        info = report.BARM;
                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = info.Name;
                        currentCellRange.Style = grandTotalRowTextStyle;
                        currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        currentColumn += 4;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = string.Empty;
                        currentCellRange.Style = grandTotalRowTextStyle;
                        currentColumn += 2;
                        currentRow++;
                    }

                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.Name;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 4;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 6);
                }

                var reportName = string.Format(
                    WinLossResources.LeagueWinLoss_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private int ExportLeagueWinLossNewCricketTotal(
            int startColumn,
            int currentRow,
            LeagueWinLossInfo cricketGrandTotalInfo,
            ExcelWorksheet workSheet,
            CellStyle subRowTextStyle,
            CellStyle subRowPositiveTwoDecimalNumberStyle,
            CellStyle subRowTwoDecimalNumberStyle,
            CellStyle subRowPositiveTwoDecimalPercentNumberStyle)
        {
            for (var i = 0; i < cricketGrandTotalInfo.ChildsInfo.Count; i++)
            {
                var info = cricketGrandTotalInfo.ChildsInfo[i];
                var currentColumn = startColumn;
                var currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Name;
                currentCell.Style = subRowTextStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.WinLoss;
                currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Position;
                currentCell.Style = subRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Commission;
                currentCell.Style = subRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.LineTotal;
                currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.TurnOver;
                currentCell.Style = subRowTwoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                if (info.TurnOver > 0)
                {
                    currentCell.Value = Converter.ToDecimal(info.Margin);
                }
                else
                {
                    currentCell.Value = info.Margin.ToString();
                }

                currentCell.Style = subRowPositiveTwoDecimalPercentNumberStyle;
                currentRow++;
            }

            return cricketGrandTotalInfo.ChildsInfo.Count;
        }

        private int ExportLeagueWinLossNormalSportsTotal(
            int startColumn,
            int currentRow,
            LeagueWinLossInfo info,
            ExcelWorksheet workSheet,
            CellStyle subRowTextStyle,
            CellStyle subRowPositiveTwoDecimalNumberStyle,
            CellStyle subRowTwoDecimalNumberStyle,
            CellStyle subRowPositiveTwoDecimalPercentNumberStyle)
        {
            var currentColumn = startColumn;
            var currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = subRowTextStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = subRowTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = subRowTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.TurnOver;
            currentCell.Style = subRowTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (info.TurnOver > 0)
            {
                currentCell.Value = Converter.ToDecimal(info.Margin);
            }
            else
            {
                currentCell.Value = info.Margin.ToString();
            }

            currentCell.Style = subRowPositiveTwoDecimalPercentNumberStyle;
            currentColumn++;

            return 1;
        }

        /// <summary>
        /// Exports the league win loss by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LeagueWinLossInfo>(WinLossContants.LeagueWinLossBySportSessionKey);
                if (report == null)
                {
                    report = _reportService.GetLeagueWinLossBySport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = WinLossResources.LeagueWinLoss_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                var textStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                for (int colIndex = 0; colIndex < 7; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                workSheet.Columns[startColumn].Width = 140 * ExcelExtensions.FontWidth;
                var title = string.Format(
                    WinLossResources.LeagueWinLoss_ReportTitle,
                    criteria.Winlostdate.ToShortFormatedDate());

                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 6);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
                currentRow += 2;

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    var reportLength = report.ChildsInfo.Count;
                    for (var i = 0; i < reportLength; i++)
                    {
                        workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 6);
                        this.ExportLeagueWinLossBySport(
                            ref workSheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            report.ChildsInfo[i],
                            criteria);
                    }
                }
                else
                {
                    this.ExportLeagueWinLossBySport(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        null,
                        criteria);
                }

                var reportName = string.Format(
                    WinLossResources.LeagueWinLoss_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
        }

        /// <summary>
        /// Exports the type of the league win loss by bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LeagueWinLossInfo>(WinLossContants.LeagueWinLossByBetTypeSessionKey);
                if (report == null)
                {
                    report = _reportService.GetLeagueWinLossByBetType(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = WinLossResources.LeagueWinLoss_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    false,
                    false);
                var positiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    true,
                    false);
                var positiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    true,
                    false);

                var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                var grandTotalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);
                var grandTotalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);

                for (int colIndex = 0; colIndex < 7; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                workSheet.Columns[startColumn + 1].Width = 100 * ExcelExtensions.FontWidth;
                workSheet.Columns[startColumn + 1].Width = 360 * ExcelExtensions.FontWidth;
                var title = string.Format(
                    WinLossResources.LeagueWinLossByBetType_ReportTitle,
                    criteria.SportName,
                    criteria.LeagueName,
                    criteria.BetTypeName);

                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 7);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 7);

                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Code;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Event;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Win_Loss;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Position;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Comm;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.LineTotal;
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
                currentRow++;

                if (report.ChildsInfo.Count > 0)
                {
                    var len = report.ChildsInfo.Count;
                    LeagueWinLossInfo info;
                    for (var i = 0; i < len; i++)
                    {
                        info = report.ChildsInfo[i];
                        if (i % 2 == 0)
                        {
                            textStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                            twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                            positiveTwoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                            positiveTwoDecimalPercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                        }
                        else
                        {
                            textStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                            twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                            positiveTwoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                            positiveTwoDecimalPercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                        }

                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MatchCode;
                        currentCell.Style = textStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Event;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.WinLoss;
                        currentCell.Style = positiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Position;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = positiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.TurnOver;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        AssignMargin(currentCell, info);
                        currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                        currentColumn++;
                        currentRow++;
                    }

                    info = report.Total;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.Name;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentCellRange.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn += 2;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.WinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Position;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Commission;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.TurnOver;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    AssignMargin(currentCell, info);

                    currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    currentColumn++;
                    currentRow++;

                    if (this._8rmConfig.IsActive)
                    {
                        info = report.EightRM;
                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = info.Name;
                        currentCellRange.Style = grandTotalRowTextStyle;
                        currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        currentColumn += 5;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = string.Empty;
                        currentCellRange.Style = grandTotalRowTextStyle;
                        currentColumn += 2;
                        currentRow++;
                    }

                    if (this._barmConfig.IsActive)
                    {
                        info = report.BARM;
                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = info.Name;
                        currentCellRange.Style = grandTotalRowTextStyle;
                        currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        currentColumn += 5;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = string.Empty;
                        currentCellRange.Style = grandTotalRowTextStyle;
                        currentColumn += 2;
                        currentRow++;
                    }

                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.Name;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 5;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 7);
                }

                var reportName = string.Format(
                    WinLossResources.LeagueWinLoss_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
        }

        /// <summary>
        /// Exports the league win loss by sport.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private void ExportLeagueWinLossBySport(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            LeagueWinLossInfo report,
            GetLeagueWinLossBySportCriteria criteria)
        {
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            var positiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                true,
                false);
            var positiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.BackGroundColor,
                false,
                true,
                false);

            var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            var grandTotalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);
            var grandTotalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);

            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            currentColumn = startColumn;
            for (int colIndex = 0; colIndex < 7; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn].Width = 240 * ExcelExtensions.FontWidth;
            var title = string.Format(
                WinLossResources.LeagueWinLossBySport_ReportTitle,
                criteria.SportName,
                criteria.LeagueName,
                (report.ChildsInfo.Count > 0) ? " - " + report.Name : string.Empty);

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow++;

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Bet_Type;
            currentCell.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Win_Loss;
            currentCell.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Position;
            currentCell.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Comm;
            currentCell.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.LineTotal;
            currentCell.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Turnover;
            currentCell.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Margin;
            currentCell.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentColumn++;
            currentRow++;
            LeagueWinLossInfo chargedCommissionTotal = null;
            if (report.ChildsInfo.Count > 0)
            {
                var len = report.ChildsInfo.Count;
                LeagueWinLossInfo info;
                for (var i = 0; i < len; i++)
                {
                    info = report.ChildsInfo[i];
                    const int ChargedCommissionBetType = 500;
                    if (info.Id != ChargedCommissionBetType)
                    {
                        ExportLeagueWinLossByBetTypeNormalBetType(i, currentRow, startColumn, info, workSheet, textStyle, twoDecimalNumberStyle, positiveTwoDecimalNumberStyle, positiveTwoDecimalPercentNumberStyle);
                        currentRow++;
                    }
                    else
                    {
                        chargedCommissionTotal = info;
                    }
                }

                info = report.Total;
                ExportLeaguWinLossByBetTypeTotal(currentRow, startColumn, info, workSheet, grandTotalRowTextStyle, grandTotalRowPositiveTwoDecimalNumberStyle, grandTotalRowTwoDecimalNumberStyle, grandTotalRowPositiveTwoDecimalPercentNumberStyle);
                currentRow++;
                if (chargedCommissionTotal != null)
                {
                    ExportLeaguWinLossByBetTypeTotal(currentRow, startColumn, chargedCommissionTotal, workSheet, grandTotalRowTextStyle, grandTotalRowPositiveTwoDecimalNumberStyle, grandTotalRowTwoDecimalNumberStyle, grandTotalRowPositiveTwoDecimalPercentNumberStyle);
                    currentRow++;
                }

                if (this._8rmConfig.IsActive)
                {
                    info = report.EightRM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.Name;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 4;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (this._barmConfig.IsActive)
                {
                    info = report.BARM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.Name;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 4;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                info = report.GrandTotal;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Value = info.Name;
                currentCellRange.Style = grandTotalRowTextStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentColumn += 4;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.LineTotal;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                currentCellRange.Merged = true;
                currentCellRange.Value = string.Empty;
                currentCellRange.Style = grandTotalRowTextStyle;
                currentColumn += 2;
                currentRow++;
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 6);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private int ExportLeagueWinLossByBetTypeNormalBetType(
            int i,
            int currentRow,
            int startColumn,
            LeagueWinLossInfo info,
            ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle positiveTwoDecimalNumberStyle,
            CellStyle positiveTwoDecimalPercentNumberStyle)
        {
            if (i % 2 == 0)
            {
                textStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                positiveTwoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                positiveTwoDecimalPercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
            }
            else
            {
                textStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                positiveTwoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                positiveTwoDecimalPercentNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
            }

            var currentColumn = startColumn;
            var currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = positiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = positiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.TurnOver;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (info.TurnOver > 0)
            {
                currentCell.Value = Converter.ToDecimal(info.Margin);
            }
            else
            {
                currentCell.Value = info.Margin.ToString();
            }

            currentCell.Style = positiveTwoDecimalPercentNumberStyle;
            currentColumn++;
            return 1;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportLeaguWinLossByBetTypeTotal(
            int currentRow,
            int startColumn,
            LeagueWinLossInfo info,
            ExcelWorksheet workSheet,
            CellStyle grandTotalRowTextStyle,
            CellStyle grandTotalRowPositiveTwoDecimalNumberStyle,
            CellStyle grandTotalRowTwoDecimalNumberStyle,
            CellStyle grandTotalRowPositiveTwoDecimalPercentNumberStyle)
        {
            var currentColumn = startColumn;
            var currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = grandTotalRowTextStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.TurnOver;
            currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (info.TurnOver > 0)
            {
                currentCell.Value = Converter.ToDecimal(info.Margin);
            }
            else
            {
                currentCell.Value = info.Margin.ToString();
            }

            currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
        }

        private void AssignMargin(ExcelCell currentCell, LeagueWinLossInfo info)
        {
            decimal tempDecimal;
            if (decimal.TryParse(info.Margin, out tempDecimal))
            {
                currentCell.Value = Converter.ToDecimal(info.Margin);
            }
            else
            {
                currentCell.Value = info.Margin;
            }
        }
    }
}