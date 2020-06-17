using ExcelUtil.Spreadsheet;
using Fanex.AccountingReports.Core.Constants;
using Fanex.AccountingReports.Core.Extensions;
using Fanex.AccountingReports.Core.Web;
using Fanex.AccountingReports.WinLoss.Constants;
using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
using Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss;
using Fanex.AccountingReports.WinLoss.Resources;
using Fanex.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Fanex.AccountingReports.WinLoss.Services.Implement.LeagueWinLoss
{
    public class LeagueWinLossExportExcel
    {
        private readonly LeagueWinLossService _reportService;

        public LeagueWinLossExportExcel() : this(null)
        {
        }

        public LeagueWinLossExportExcel(LeagueWinLossService reportService)
        {
            _reportService = reportService ?? new LeagueWinLossService();
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
            var report = SessionManager.Get<LeagueWinLossModel>(WinLossContants.LeagueWinLossSessionKey);
            if (report == null)
            {
                report = _reportService.GetLeagueWinLoss(criteria);
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
            for (int colIndex = 0; colIndex < 12; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn].Width = 350 * ExcelExtensions.FontWidth;
            var title = criteria.IsNormalMode ?
                string.Format(
                    WinLossResources.LeagueWinLoss_ReportTitle,
                    criteria.StartDate.ToShortFormatedDate()) :
                string.Format("League Win Loss Report From {0} To {1}",
                    criteria.StartDate.ToShortFormatedDate(),
                    criteria.EndDate.ToShortFormatedDate());

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 11);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentCellRange.Style.Font.Size = 200;
            currentCellRange.Style.Font.Weight = 700;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 11);
          
            ExportHeader(ref workSheet, startColumn, ref currentRow, Labels.League);

            if (report.SportWinLoss.Count > 0)
            {
                var len = report.SportWinLoss.Count;
                LeagueWinLossItem info;
                for (var i = 0; i < len; i++)
                {
                    var sport = report.SportWinLoss[i];

                    currentColumn = startColumn;
                    info = sport.Total;
                    ExportLeagueWinLossNormalSportsTotal(startColumn, currentRow, info, workSheet, subRowTextStyle, subRowPositiveTwoDecimalNumberStyle, subRowTwoDecimalNumberStyle, subRowPositiveTwoDecimalPercentNumberStyle);
                    currentRow++;

                    var childsLength = sport.Leagues.Count;
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

                        info = sport.Leagues[j];
                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LeagueName;
                        currentCell.Style = textStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MemberWinLoss;
                        currentCell.Style = positiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.SMAWinLoss;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.ExtraPT;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.VarianceIncome;
                        currentCell.Style = positiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.VirtualWinLoss;
                        currentCell.Style = positiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.RealIncome;
                        currentCell.Style = positiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.VirtualTurnover;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.RealTurnover;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];

                        if (info.VirtualTurnover != 0)
                        {
                            currentCell.Value = info.VirtualMargin;
                        }
                        else
                        {
                            currentCell.Value = Labels.NotAvailable;
                        }

                        currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];

                        if (info.RealTurnover != 0)
                        {
                            currentCell.Value = info.RealMargin;
                        }
                        else
                        {
                            currentCell.Value = Labels.NotAvailable;
                        }

                        currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                        currentColumn++;
                        currentRow++;
                    }
                }

                info = report.Total;
                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.LeagueName;
                currentCell.Style = grandTotalRowTextStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.MemberWinLoss;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.SMAWinLoss;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.ExtraPT;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Commission;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VarianceIncome;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VirtualWinLoss;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealIncome;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VirtualTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                if (info.VirtualTurnover != 0)
                {
                    currentCell.Value = info.VirtualMargin;
                }
                else
                {
                    currentCell.Value = Labels.NotAvailable;
                }

                currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                if (info.RealTurnover != 0)
                {
                    currentCell.Value = info.RealMargin;
                }
                else
                {
                    currentCell.Value = Labels.NotAvailable;
                }

                currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;
                currentRow++;

                if (IsShowTotal(report.SoccerRM))
                {
                    info = report.SoccerRM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.LeagueName;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 6;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (IsShowTotal(report.OtherSportRM))
                {
                    info = report.OtherSportRM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.LeagueName;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 6;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (IsShowTotal(report.SoccerBARM))
                {
                    info = report.SoccerBARM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.LeagueName;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 6;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (IsShowTotal(report.OtherSportBARM))
                {
                    info = report.OtherSportBARM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.LeagueName;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 6;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (IsShowTotal(report.SoccerRM)
                    || IsShowTotal(report.OtherSportRM)
                    || IsShowTotal(report.SoccerBARM)
                    || IsShowTotal(report.SoccerBARM))
                {
                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.LeagueName;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 6;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = report.Total.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = report.Total.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = report.Total.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 11);
            }

            var reportName = criteria.IsNormalMode ? string.Format(
                    WinLossResources.LeagueWinLoss_ReportName,
                    criteria.StartDate.ToShortFormatedDateWithSlash().Replace('/', '_')) :
                string.Format(
                    "League_Win_Loss_Report_From_{0}_To_{1}",
                    criteria.StartDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.EndDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);

        }

        private int ExportLeagueWinLossNormalSportsTotal(
            int startColumn,
            int currentRow,
            LeagueWinLossItem info,
            ExcelWorksheet workSheet,
            CellStyle subRowTextStyle,
            CellStyle subRowPositiveTwoDecimalNumberStyle,
            CellStyle subRowTwoDecimalNumberStyle,
            CellStyle subRowPositiveTwoDecimalPercentNumberStyle)
        {
            var currentColumn = startColumn;
            var currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LeagueName;
            currentCell.Style = subRowTextStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.MemberWinLoss;
            currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.SMAWinLoss;
            currentCell.Style = subRowTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.ExtraPT;
            currentCell.Style = subRowTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = subRowTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VarianceIncome;
            currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualWinLoss;
            currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.RealIncome;
            currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualTurnover;
            currentCell.Style = subRowTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.RealTurnover;
            currentCell.Style = subRowTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];

            if (info.VirtualTurnover != 0)
            {
                currentCell.Value = Converter.ToDecimal(info.VirtualMargin);
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = subRowPositiveTwoDecimalPercentNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];

            if (info.RealTurnover != 0)
            {
                currentCell.Value = Converter.ToDecimal(info.RealMargin);
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = subRowPositiveTwoDecimalPercentNumberStyle;

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

            var report = SessionManager.Get<List<LiveNonLiveWinLossModel>>(WinLossContants.LeagueWinLossBySportSessionKey);
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

            for (int colIndex = 0; colIndex < 12; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn].Width = 140 * ExcelExtensions.FontWidth;
            var title = criteria.IsNormalMode ?
                string.Format(
                    WinLossResources.LeagueWinLoss_ReportTitle,
                    criteria.StartDate.ToShortFormatedDate()) :
                string.Format("League Win Loss Report From {0} To {1}",
                    criteria.StartDate.ToShortFormatedDate(),
                    criteria.EndDate.ToShortFormatedDate());

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 11);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentCellRange.Style.Font.Size = 200;
            currentCellRange.Style.Font.Weight = 700;
            currentRow += 2;

            if (report != null && report.Count > 0)
            {
                var reportLength = report.Count;
                for (var i = 0; i < reportLength; i++)
                {
                    workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 11);
                    this.ExportLeagueWinLossBySport(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        report[i],
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

            var reportName = criteria.IsNormalMode ? string.Format(
                     WinLossResources.LeagueWinLoss_ReportName,
                     criteria.StartDate.ToShortFormatedDateWithSlash().Replace('/', '_')) :
                 string.Format(
                     "League_Win_Loss_Report_From_{0}_To_{1}",
                     criteria.StartDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                     criteria.EndDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the type of the league win loss by bet.
        /// </summary>
        /// <param name = "criteria" > The criteria.</param>
        //[SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        //    "SP2101:MethodMustNotContainMoreLinesThan",
        //    Justification = "Reviewed. Suppression is OK here.")]
        public void ExportLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria)
        {
            var report = SessionManager.Get<MatchWinLossModel>(WinLossContants.LeagueWinLossByBetTypeSessionKey);
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
            var headerColor = ExcelExtensions.HeaderTitleColor;

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

            for (int colIndex = 0; colIndex < 13; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 1].Width = 100 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 1].Width = 360 * ExcelExtensions.FontWidth;

            var title = criteria.IsNormalMode ?
              string.Format(
                  WinLossResources.LeagueWinLoss_ReportTitle,
                  criteria.StartDate.ToShortFormatedDate()) :
              string.Format("League Win Loss Report From {0} To {1}",
                  criteria.StartDate.ToShortFormatedDate(),
                  criteria.EndDate.ToShortFormatedDate());

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 12);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentCellRange.Style.Font.Size = 200;
            currentCellRange.Style.Font.Weight = 700;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 12);

            var matchTitle = string.Format(
              WinLossResources.LeagueWinLossByBetType_ReportTitle,
              criteria.SportName,
              criteria.LeagueName,
              criteria.BetTypeName,
              criteria.LiveIndicator ? "Live" : "Non-Live");

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 12);
            currentCellRange.Merged = true;
            currentCellRange.Value = matchTitle;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow += 2;

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Code;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            ExportHeader(ref workSheet, currentColumn, ref currentRow, Labels.Event);

            if (report.Match.Count > 0)
            {
                var len = report.Match.Count;
                MatchWinLossItem info;
                for (var i = 0; i < len; i++)
                {
                    info = report.Match[i];
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
                    currentCell.Value = info.MatchEvent;
                    currentCell.Style = textStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.MemberWinLoss;
                    currentCell.Style = positiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.SMAWinLoss;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.ExtraPT;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Commission;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VarianceIncome;
                    currentCell.Style = positiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = positiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = positiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = twoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    if (info.VirtualTurnover != 0)
                    {
                        currentCell.Value = info.VirtualMargin;
                    }
                    else
                    {
                        currentCell.Value = Labels.NotAvailable;
                    }

                    currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    if (info.RealTurnover != 0)
                    {
                        currentCell.Value = info.RealMargin;
                    }
                    else
                    {
                        currentCell.Value = Labels.NotAvailable;
                    }

                    currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                    currentRow++;
                }

                info = report.Total;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                currentCellRange.Merged = true;
                currentCellRange.Value = info.MatchEvent;
                currentCellRange.Style = grandTotalRowTextStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCellRange.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn += 2;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.MemberWinLoss;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.SMAWinLoss;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.ExtraPT;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Commission;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VarianceIncome;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VirtualWinLoss;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealIncome;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VirtualTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];

                if (info.VirtualTurnover != 0)
                {
                    currentCell.Value = info.VirtualMargin;
                }
                else
                {
                    currentCell.Value = Labels.NotAvailable;
                }

                currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                if (info.RealTurnover != 0)
                {
                    currentCell.Value = info.RealMargin;
                }
                else
                {
                    currentCell.Value = Labels.NotAvailable;
                }

                currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                currentColumn++;
                currentRow++;

                if (IsShowTotal(report.RM))
                {
                    info = report.RM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.MatchEvent;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 7;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (IsShowTotal(report.BARM))
                {
                    info = report.BARM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.MatchEvent;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 7;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (IsShowTotal(report.RM)
                    || IsShowTotal(report.BARM))
                {
                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.MatchEvent;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 7;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = report.Total.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = report.Total.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = report.Total.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 12);
            }

            var reportName = criteria.IsNormalMode ? string.Format(
                    WinLossResources.LeagueWinLoss_ReportName,
                    criteria.StartDate.ToShortFormatedDateWithSlash().Replace('/', '_')) :
                string.Format(
                    "League_Win_Loss_Report_From_{0}_To_{1}",
                    criteria.StartDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.EndDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);

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
            LiveNonLiveWinLossModel report,
            GetLeagueWinLossBySportCriteria criteria)
        {
            var bettypeWinLoss = report.BetTypeWinLoss;
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
            for (int colIndex = 0; colIndex < 11; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn].Width = 240 * ExcelExtensions.FontWidth;
            var title = string.Format(
                WinLossResources.LeagueWinLossBySport_ReportTitle,
                criteria.SportName,
                criteria.LeagueName,
                (bettypeWinLoss.BettypeWinLoss.Count > 0) ? " - " + report.Name : string.Empty);

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 11);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(ExcelExtensions.HeaderTitleColor);
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow++;

            ExportHeader(ref workSheet, startColumn, ref currentRow, Labels.Bet_Type);
            if (bettypeWinLoss.BettypeWinLoss.Count > 0)
            {
                var len = bettypeWinLoss.BettypeWinLoss.Count;
                BettypeWinLossItem info;
                for (var i = 0; i < len; i++)
                {
                    info = bettypeWinLoss.BettypeWinLoss[i];
                    ExportLeagueWinLossByBetTypeNormalBetType(i, currentRow, startColumn, info, workSheet, textStyle, twoDecimalNumberStyle, positiveTwoDecimalNumberStyle, positiveTwoDecimalPercentNumberStyle);
                    currentRow++;
                }

                info = bettypeWinLoss.Total;
                ExportLeagueWinLossByBetTypeTotal(currentRow, startColumn, info, workSheet, grandTotalRowTextStyle, grandTotalRowPositiveTwoDecimalNumberStyle, grandTotalRowTwoDecimalNumberStyle, grandTotalRowPositiveTwoDecimalPercentNumberStyle);
                currentRow++;

                if (IsShowTotal(bettypeWinLoss.RM))
                {
                    info = bettypeWinLoss.RM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.BettypeName;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 6;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (IsShowTotal(bettypeWinLoss.BARM))
                {
                    info = bettypeWinLoss.BARM;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.BettypeName;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 6;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }

                if (IsShowTotal(bettypeWinLoss.RM)
                    || IsShowTotal(bettypeWinLoss.BARM))
                {
                    info = bettypeWinLoss.GrandTotal;
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 5);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.BettypeName;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn += 6;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = bettypeWinLoss.Total.VirtualWinLoss;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = bettypeWinLoss.Total.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = bettypeWinLoss.Total.RealTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = string.Empty;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentColumn += 2;
                    currentRow++;
                }
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 11);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private int ExportLeagueWinLossByBetTypeNormalBetType(
            int i,
            int currentRow,
            int startColumn,
            BettypeWinLossItem info,
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
            currentCell.Value = info.BettypeName;
            currentCell.Style = textStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.MemberWinLoss;
            currentCell.Style = positiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.SMAWinLoss;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.ExtraPT;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VarianceIncome;
            currentCell.Style = positiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualWinLoss;
            currentCell.Style = positiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.RealIncome;
            currentCell.Style = positiveTwoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualTurnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.RealTurnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (info.VirtualTurnover != 0)
            {
                currentCell.Value = info.VirtualMargin;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = positiveTwoDecimalPercentNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (info.RealTurnover != 0)
            {
                currentCell.Value = info.RealMargin;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = positiveTwoDecimalPercentNumberStyle;
            return 1;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportLeagueWinLossByBetTypeTotal(
            int currentRow,
            int startColumn,
            BettypeWinLossItem info,
            ExcelWorksheet workSheet,
            CellStyle grandTotalRowTextStyle,
            CellStyle grandTotalRowPositiveTwoDecimalNumberStyle,
            CellStyle grandTotalRowTwoDecimalNumberStyle,
            CellStyle grandTotalRowPositiveTwoDecimalPercentNumberStyle)
        {
            var currentColumn = startColumn;
            var currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.BettypeName;
            currentCell.Style = grandTotalRowTextStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.MemberWinLoss;
            currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.SMAWinLoss;
            currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.ExtraPT;
            currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VarianceIncome;
            currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualWinLoss;
            currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.RealIncome;
            currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualTurnover;
            currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.RealTurnover;
            currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (info.VirtualTurnover != 0)
            {
                currentCell.Value = info.VirtualMargin;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (info.RealTurnover != 0)
            {
                currentCell.Value = info.RealMargin;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
            currentCell.Style.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
        }

        private bool IsShowTotal(BaseWinLoss winloss)
        {
            return winloss.VirtualWinLoss != 0 || winloss.RealIncome != 0 || winloss.VirtualTurnover != 0 || winloss.RealTurnover != 0;
        }

        private void ExportHeader(
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            string firstHeader)
        {
            var currentColumn = 0;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerColor = ExcelExtensions.HeaderTitleColor;

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = firstHeader;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member_Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.SMA_Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Extra_PT;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++; ;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Commission;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.House;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.House_Turnover;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Margin;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentRow++;

            currentColumn = startColumn + 5;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Variance;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Virtual_Win_Loss;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Real_Income;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Virtual;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Real;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Virtual;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Real;
            currentCell.Style.FormatTitleCell(headerColor);
            currentRow++;
        }
    }
}