namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Repositories.Implement;
    using Fanex.AccountingReports.Main.Repositories.Interface;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService : ServiceBase, IDailyWinLoss2hService
    {
        private IDailyWinLoss2hRepository _reportRepository;
        private RMElement _8rmConfig;
        private RMElement _barmConfig;
        private Dictionary<object, string> _sportTypeNameCache;
        private Dictionary<object, string> _leagueNameCache;
        private Dictionary<object, string> _teamNameCache;
        private Dictionary<object, string> _betTypeNameCache;
        private DailyWinLoss2hUtilityService _dailyWinLoss2hUtilityService;

        public DailyWinLoss2hService()
            : this(null)
        {
        }

        public DailyWinLoss2hService(IDailyWinLoss2hRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new DailyWinLoss2hRepository();
            this._8rmConfig = RMSection.Instance.Report[CoreConstants.ReportName.DailyWinLoss2h.ToString()]
                                                .ReportConfigs[RMSection.RMType.EightRM.ToString()];
            this._barmConfig = RMSection.Instance.Report[CoreConstants.ReportName.DailyWinLoss2h.ToString()]
                                                 .ReportConfigs[RMSection.RMType.BARM.ToString()];
            _dailyWinLoss2hUtilityService = new DailyWinLoss2hUtilityService();
        }

        /// <summary>
        /// Sorts the win loss information by league.
        /// </summary>
        /// <param name="infoA">The information a.</param>
        /// <param name="infoB">The information b.</param>
        /// <returns>Compare value between info A and info B.</returns>
        private int SortWinLossInfoByLeague(WinLossInfo infoA, WinLossInfo infoB)
        {
            if (infoA.Name.StartsWith(MainConstants.DailyWinLoss2hPopularLeagueNamePrefix) &&
                !infoB.Name.StartsWith(MainConstants.DailyWinLoss2hPopularLeagueNamePrefix))
            {
                return -1;
            }

            if (!infoA.Name.StartsWith(MainConstants.DailyWinLoss2hPopularLeagueNamePrefix) &&
                infoB.Name.StartsWith(MainConstants.DailyWinLoss2hPopularLeagueNamePrefix))
            {
                return 1;
            }

            return string.Compare(infoA.Name, infoB.Name);
        }

        /// <summary>
        /// Gets the name of the bet type.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The name of the bet type.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public string GetBetTypeNameFromWinLossDetail(WinLossInfo winLoss)
        {
            string result = winLoss.Name;
            switch (winLoss.Id)
            {
                case CoreConstants.BetTypeConstants.Handicap:
                    result = Labels.HDP;
                    break;

                case CoreConstants.BetTypeConstants.OverUnder2:
                    result = Labels.OU;
                    break;

                case CoreConstants.BetTypeConstants._1stHandicap:
                    result = Labels._1st_HDP;
                    break;

                case CoreConstants.BetTypeConstants._1stOverUnder:
                    result = Labels._1st_OU;
                    break;

                case CoreConstants.BetTypeConstants.CorrectScore:
                    result = Labels.CS;
                    break;

                case CoreConstants.BetTypeConstants._1stCorrectScore:
                    result = Labels._1st_CS;
                    break;

                case CoreConstants.BetTypeConstants.TotalGoal:
                    result = Labels.TG;
                    break;

                case CoreConstants.BetTypeConstants.FirstGoalLastGoal:
                    result = Labels.FG_LG;
                    break;

                case MainConstants.DailyWinLoss2hKickOffId:
                case CoreConstants.BetTypeConstants.KickOff:
                    result = Labels.Kick_Off;
                    break;

                case CoreConstants.BetTypeConstants.CasinoGames:
                    result = Labels.Casino;
                    break;

                case CoreConstants.BetTypeConstants.P2P:
                    result = Labels.P2P;
                    break;

                case BetTypeConstants._2hReportBingo:
                    result = Labels.IBCbet_Bingo;
                    break;

                case CoreConstants.BetTypeConstants.Bingo:
                    result = Labels.IBCbet_Bingo;
                    break;
                default:
                    result = this._betTypeNameCache[winLoss.Id];
                    break;
            }

            if (CoreConstants.BetTypeConstants.MinMicroGamingLiveCasinoBetType < winLoss.Id &&
                winLoss.Id < CoreConstants.BetTypeConstants.MaxMicroGamingLiveCasinoBetType)
            {
                result = Labels.MicroGaming_LiveCasino;
            }

            if (CoreConstants.BetTypeConstants.MinMicroGamingRNGCasinoBetType < winLoss.Id &&
                winLoss.Id < CoreConstants.BetTypeConstants.MaxMicroGamingRNGCasinoBetType)
            {
                result = Labels.Microgaming_RNGCasino;
            }

            if (!string.IsNullOrEmpty(winLoss.Hedge) &&
                winLoss.Hedge.Contains("(" + Labels.Hedge + ")"))
            {
                result += winLoss.Hedge;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = this._betTypeNameCache[winLoss.Id];
            }

            return result;
        }

        /// <summary>
        /// Formats the special cell.
        /// </summary>
        /// <param name="currentCell">The current cell.</param>
        /// <param name="value_Str">The value string.</param>
        /// <param name="value">The value.</param>
        /// <param name="textType">Type of the text.</param>
        /// <param name="numberType">Type of the number.</param>
        private void FormatSpecialCell(
            ref ExcelCell currentCell,
            string value_Str,
            decimal value,
            CellStyle textType,
            CellStyle numberType)
        {
            currentCell.Style = numberType;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            if (value_Str != Labels.NotAvailable)
            {
                currentCell.Value = value;
            }
            else
            {
                currentCell.Value = value_Str;
            }
        }

        /// <summary>
        /// Exports the sport report.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="isSportSummary">If set to <c>true</c> [is sport summary].</param>
        /// <param name="report">The report.</param>
        /// <param name="title">The title.</param>
        /// <param name="isPartOfReport">If set to <c>true</c> [is part of report].</param>
        /// <param name="sportId">The sport identifier.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportSportReport(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            bool isSportSummary,
            WinLossInfo report,
            string title,
            bool isPartOfReport,
            int sportId)
        {
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var currentColumn = startColumn;
            int maxColumnIndex = 10;
            CellRange currentCellRange;

            currentColumn = startColumn;
            if (isPartOfReport)
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex);
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            }

            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            if (!isPartOfReport)
            {
                currentRow += 2;
                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }
            else
            {
                currentRow++;
            }

            _dailyWinLoss2hUtilityService.ExportHeaderColumns_SportReport(
                ref workSheet,
                ref currentRow,
                startRow,
                startColumn,
                isSportSummary);

            if (sportId == SoccerBetTypeConstants.SoccerMajorBetType && !report.IsLive)
            {
                this.ExportContent_MajorSoccerReport(
                ref workSheet,
                ref currentRow,
                startRow,
                startColumn,
                report,
                sportId);
            }
            else
            {
                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report,
                    sportId);
            }
        }

        /// <summary>
        /// Exports the IBC row sport report.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="info">The information.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportIBCRow_SportReport(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex)
        {
            ExcelCell currentCell;

            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualLineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Variance;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Turnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualTurnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Margin;
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualMargin;
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentRow++;
        }

        /// <summary>
        /// Gets the total name of the IBC.
        /// </summary>
        /// <returns>The total name of the IBC.</returns>
        private string GetIBCTotalName()
        {
            if (this._8rmConfig.IsActive &&
                this._barmConfig.IsActive)
            {
                return string.Format(
                    Labels.IBC + "({0}+{1}) " + Labels.Total,
                    Labels.EightRM,
                    Labels.BARM);
            }

            if (!this._8rmConfig.IsActive &&
                this._barmConfig.IsActive)
            {
                return string.Format(
                    Labels.IBC + "({0})" + Labels.Total,
                    Labels.BARM);
            }

            if (this._8rmConfig.IsActive &&
                !this._barmConfig.IsActive)
            {
                return string.Format(
                    Labels.IBC + "({0})" + Labels.Total,
                    Labels.EightRM);
            }

            return string.Empty;
        }

        /// <summary>
        /// Exports the header columns sport report not apply virtual rate.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="isSportSummary">If set to <c>true</c> [is sport summary].</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportHeaderColumns_SportReport_NotApplyVirtualRate(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            bool isSportSummary)
        {
            //// ExcelTypes

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            int maxColumnIndex = 6;
            var currentColumn = startColumn;
            ExcelCell currentCell;

            //// ExcelTypes

            workSheet.Columns[startColumn].Width = 190 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 130 * ExcelExtensions.FontWidth;
            }

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (isSportSummary)
            {
                currentCell.Value = Labels.Sport_Type;
            }
            else
            {
                currentCell.Value = Labels.Bet_Type;
            }

            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Member_Win_Loss;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.SMA_Win_Loss;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Commission;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Company_Income;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Company_Turnover;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Margin;
            currentCell.Style = headerTextStyle;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the content_ major soccer report.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="report">The report.</param>
        /// <param name="sportId">The sport identifier.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportContent_MajorSoccerReport(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            WinLossInfo report,
            int sportId)
        {
            int maxColumnIndex = 6;
            if (report.EnableVirtual)
            {
                maxColumnIndex = 10;
            }

            var currentColumn = startColumn;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                true,
                false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);

            var subTotalRowColor = ExcelExtensions.BackGroundColor;
            var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
            var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                false,
                false,
                false);
            var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                subTotalRowColor,
                false,
                true,
                false);
            var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                subTotalRowColor,
                false,
                true,
                false);

            var majorBetTypeGroupTotalColor = ExcelExtensions.TotalRowColor;
            var majorBetTypeGroupTextStyle = ExcelExtensions.GetCellTextStyle(majorBetTypeGroupTotalColor, false);
            majorBetTypeGroupTextStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            var majorBetTypeGroupTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                majorBetTypeGroupTotalColor,
                false,
                false,
                false);
            var majorBetTypeGroupTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                majorBetTypeGroupTotalColor,
                false,
                true,
                false);
            var majorBetTypeGroupTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                majorBetTypeGroupTotalColor,
                false,
                true,
                false);

            var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            if (report != null && report.ChildsInfo.Count > 0)
            {
                var isShowVirtualTurnover = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(sportId) ||
                    sportId == CoreConstants.SportContants.LiveCasino ||
                    sportId == CoreConstants.SportContants.VirtualSportGroup;
                _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                    ref textStyle,
                    ref twoDecimalNumberStyle,
                    ref twoDecimalPositiveNumberStyle,
                    ref twoDecimalPositivePercentNumberStyle,
                    true);

                WinLossInfo info;
                var betTypeGroupLength = report.ChildsInfo.Count;
                for (var i = 0; i < betTypeGroupLength; i++)
                {
                    var betTypeGroup = report.ChildsInfo[i];
                    for (int j = 0; j < betTypeGroup.ChildsInfo.Count; j++)
                    {
                        var betType = betTypeGroup.ChildsInfo[j];
                        info = betType.GrandTotal;
                        _dailyWinLoss2hUtilityService.ExportNormalRow(
                            ref workSheet,
                             textStyle,
                             twoDecimalNumberStyle,
                             twoDecimalPositiveNumberStyle,
                             twoDecimalPositivePercentNumberStyle,
                             info,
                             startColumn,
                             ref currentRow,
                             RowType.Normal,
                             report.EnableVirtual,
                             isShowVirtualTurnover);

                        info = betType.CreditTotal;
                        _dailyWinLoss2hUtilityService.ExportNormalRow(
                            ref workSheet,
                            subTotalTextStyle,
                            subTotalTwoDecimalNumberStyle,
                            subTotalTwoDecimalPositiveNumberStyle,
                            subTotalTwoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportCredit,
                            report.EnableVirtual,
                            isShowVirtualTurnover);

                        info = betType.DepositTotal;
                        _dailyWinLoss2hUtilityService.ExportNormalRow(
                            ref workSheet,
                            subTotalTextStyle,
                            subTotalTwoDecimalNumberStyle,
                            subTotalTwoDecimalPositiveNumberStyle,
                            subTotalTwoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportDeposit,
                            report.EnableVirtual,
                            isShowVirtualTurnover);
                    }

                    info = betTypeGroup.GrandTotal;
                    _dailyWinLoss2hUtilityService.ExportNormalRow(
                           ref workSheet,
                           majorBetTypeGroupTextStyle,
                           majorBetTypeGroupTwoDecimalNumberStyle,
                           majorBetTypeGroupTwoDecimalPositiveNumberStyle,
                           majorBetTypeGroupTwoDecimalPositivePercentNumberStyle,
                           info,
                           startColumn,
                           ref currentRow,
                           RowType.Normal,
                           report.EnableVirtual,
                           isShowVirtualTurnover);
                }

                for (int i = 0; i < report.GrandTotal.ChildsInfo.Count; i++)
                {
                    info = report.GrandTotal.ChildsInfo[i];
                    _dailyWinLoss2hUtilityService.ExportNormalRow(
                        ref workSheet,
                        i == 0 ? grandTotalRowTextStyle_FirstRow : grandTotalRowTextStyle,
                        i == 0 ? grandTotalRowTwoDecimalNumberStyle_FirstRow : grandTotalRowTwoDecimalNumberStyle,
                        i == 0 ? grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow : grandTotalTwoDecimalPositiveNumberStyle,
                        i == 0 ? gtrPercentNumberStyle_FirstRow : grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportGrandTotal,
                        report.EnableVirtual,
                        isShowVirtualTurnover);
                }

                info = report.GrandTotal;
                _dailyWinLoss2hUtilityService.ExportNormalRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportGrandTotal,
                    report.EnableVirtual,
                    isShowVirtualTurnover);

                info = report.CreditTotal;
                _dailyWinLoss2hUtilityService.ExportNormalRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportCredit,
                    report.EnableVirtual,
                    isShowVirtualTurnover);

                info = report.DepositTotal;
                _dailyWinLoss2hUtilityService.ExportNormalRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportDeposit,
                    report.EnableVirtual,
                    isShowVirtualTurnover);

                //// IBCTotal

                info = report.EightRMTotal;
                if (this._8rmConfig.IsActive &&
                    info != null &&
                    info.WinLoss != 0)
                {
                    this.ExportIBCRow_SportReport(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        maxColumnIndex);
                }

                info = report.BARMTotal;
                if (this._8rmConfig.IsActive &&
                    info != null &&
                    info.WinLoss != 0)
                {
                    this.ExportIBCRow_SportReport(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        maxColumnIndex);
                }

                //// IBCTotal
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }
        }
    }
}
