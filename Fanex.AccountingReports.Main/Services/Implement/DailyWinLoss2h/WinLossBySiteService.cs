// --------------------------------------------------------------------------
// <copyright file="WinLossBySiteService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using Fanex.Data.Repository;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;
    using MainSportConstants = Fanex.AccountingReports.Main.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        /// <summary>
        /// Gets the win loss by site bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The win loss by site bet type.</returns>
        public WinLossInfo GetWinLossBySite_BetType(GetWinLossBySite_BetTypeCriteria criteria)
        {
            try
            {
                var enableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(criteria.SportGroupId) ||
                     criteria.ProductId != MainSportConstants.SportContants._2hCasinoSport;
                var isShowVirtualTurnover = enableVirtual;

                var report = WinlossBySiteInfo(criteria);

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hWinLossBySiteBetTypeReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hWinLossBySiteBetTypeReportSessionKey, null);
                throw;
            }
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private CriteriaBase WinlossBySiteCriteria(GetWinLossBySite_BetTypeCriteria criteria)
        {
            var winLossDateSuffix = ReportService.GetStoreMappingSuffix(criteria.WinLostDate);
            if (winLossDateSuffix == ReportService.WarehouseSPSuffix)
            {
                return new GetWinLossBySite_BetTypeWHCriteria
                {
                    BetType = criteria.BetType,
                    IsHedge = criteria.IsHedge,
                    LiveIndicator = criteria.LiveIndicator,
                    ProductId = criteria.ProductId,
                    Site = criteria.Site,
                    SportType = criteria.SportType,
                    WinLostDate = criteria.WinLostDate
                };
            }

            switch (criteria.SportGroupId)
            {
                case CoreConstants.SportContants.HorseRacing:
                    return new GetWinLossBySite_RacingCriteria
                    {
                        BetType = criteria.BetType,
                        Site = criteria.Site,
                        SportType = criteria.SportType,
                        WinLostDate = criteria.WinLostDate
                    };
                case CoreConstants.SportContants.LiveCasino:
                case MainSportConstants.SportContants._2hGoldDeluxeSport:
                case MainSportConstants.SportContants._2hLiveCasinoSport:
                case MainSportConstants.SportContants._2hAGCasinoSport:
                case MainSportConstants.SportContants._2hMicroGamingLiveCasinoSport:
                    return new GetWinLossBySite_LiveCasinoCriteria
                    {
                        BetType = criteria.BetType,
                        Site = criteria.Site,
                        WinLostDate = criteria.WinLostDate
                    };
                case CoreConstants.SportContants.Keno:
                    return new GetWinLossBySite_KenoCriteria
                    {
                        BetType = criteria.BetType,
                        Site = criteria.Site,
                        WinLostDate = criteria.WinLostDate
                    };
                case CoreConstants.SportContants.Bingo:
                    return new GetWinLossBySite_BingoCriteria
                    {
                        Site = criteria.Site,
                        WinLostDate = criteria.WinLostDate
                    };
                case MainSportConstants.SportContants._2hReportCasino:
                    return new GetWinLossBySite_CasinoCriteria
                    {
                        Site = criteria.Site,
                        WinLostDate = criteria.WinLostDate,
                        ProductId = criteria.ProductId,
                        BetType = criteria.BetType
                    };
                case CoreConstants.SportContants.VirtualSoccer:
                    return new GetWinLossBySite_VirtualSportsCriteria
                    {
                        Site = criteria.Site,
                        WinLostDate = criteria.WinLostDate,
                        SportType = criteria.SportType,
                        BetType = criteria.BetType
                    };
                case CoreConstants.SportContants.MetricGamingSuperLive:
                    return new GetWinLossBySite_MetricGamingCriteria
                    {
                        Site = criteria.Site,
                        WinLostDate = criteria.WinLostDate,
                        BetType = criteria.BetType
                    };
                case CoreConstants.SportContants.Bettrade:
                    return new GetWinLossBySite_BettradeCriteria
                    {
                        WinLostDate = criteria.WinLostDate,
                        Site = criteria.Site
                    };
                case CoreConstants.SportContants.ColossusBet:
                    return new GetWinLossBySite_ColossusBetCriteria
                    {
                        WinLostDate = criteria.WinLostDate,
                        Site = criteria.Site
                    };
                case CoreConstants.SportContants.CockFighting:
                    return new GetWinLossBySite_CockFightingCriteria
                    {
                        WinLostDate = criteria.WinLostDate,
                        Site = criteria.Site
                    };
            }

            return criteria;
        }

        private WinLossInfo WinlossBySiteInfo(GetWinLossBySite_BetTypeCriteria criteria)
        {
            int sportGroupId = criteria.SportGroupId;
            var convertCriteria = WinlossBySiteCriteria(criteria);
            bool isHideCasinoTurnover = criteria.ProductId == MainSportConstants.SportContants._2hCasinoSport && DateTime.Compare(criteria.WinLostDate, AppSettings.CasinoSingleWalletLaunchDate) < 0;
            var enableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(criteria.SportGroupId) ||
                                !isHideCasinoTurnover ||
                                sportGroupId == CoreConstants.SportContants.LiveCasino ||
                                sportGroupId == CoreConstants.SportContants.Keno ||
                                sportGroupId == CoreConstants.SportContants.VirtualSportGroup;

            WinLossInfo report = new WinLossInfo
            {
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.Total,
                    EnableVirtual = enableVirtual
                },
                ChildsInfo = new List<WinLossInfo>(),
                Name = ReportService.GetSiteTypeName(criteria.Site),
                EnableVirtual = enableVirtual,
                IsHideRealTurnover = _dailyWinLoss2hUtilityService.IsHideRealTurnover(criteria.SportGroupId) || isHideCasinoTurnover
            };

            report.GrandTotal.IsHideRealTurnover = report.IsHideRealTurnover;
            var rawData = this.GetReportData<dynamic>(convertCriteria);
            if (rawData != null)
            {
                foreach (var data in rawData)
                {
                    WinLossInfo info = new WinLossInfo
                    {
                        Name = Converter.ToString2(data.site),
                        WinLoss = Converter.ToDecimal(data.WinLost),
                        Position = Converter.ToDecimal(data.Position),
                        Commission = Converter.ToDecimal(data.Commission),
                        LineTotal = Converter.ToDecimal(data.RealLineTotal),
                        VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                        Turnover = Converter.ToDecimal(data.RealTurnover),
                        VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                        EnableVirtual = enableVirtual,
                        IsHideRealTurnover = report.IsHideRealTurnover
                    };

                    if (info.IsHideRealTurnover)
                    {
                        info.Turnover = 0;
                    }

                    if (!enableVirtual)
                    {
                        info.VirtualTurnover = 0;
                    }

                    report.GrandTotal.Add(info);
                    info = WinLossInfo.Format(info);
                    report.ChildsInfo.Add(info);
                }

                report = WinLossInfo.Format_Total(report);
            }

            return report;
        }

        /// <summary>
        /// Exports the win loss by site bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportWinLossBySite_BetType(GetWinLossBySite_BetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hWinLossBySiteBetTypeReportSessionKey);
                if (report == null)
                {
                    report = this.GetWinLossBySite_BetType(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 10;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                var backgroundColor = ExcelExtensions.SubTotalRowColor;
                var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
                var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, true, false);
                var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    backgroundColor,
                    false,
                    true,
                    false);

                var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
                var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);
                grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);
                gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

                //// ExcelTypes
                //// MakeExcelFile

                //// Header
                for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 130 * ExcelExtensions.FontWidth;
                }

                var title = criteria.PreviousBreadScrum + " | " +
                    string.Format(MainResources.DailyWinLoss2h_WinLossBySite_BetType_ReportTitle, ReportService.GetSiteTypeName(criteria.Site));
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                // FirstRow
                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Site;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Member_Win_Loss;
                currentCellRange.Style = headerTextStyle;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.SMA_Win_Loss;
                currentCellRange.Style = headerTextStyle;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Commission;
                currentCellRange.Style = headerTextStyle;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Company_Income;
                currentCellRange.Style = headerTextStyle;
                currentColumn += 2;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Variance;
                currentCellRange.Style = headerTextStyle;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Company_Turnover;
                currentCellRange.Style = headerTextStyle;
                currentColumn += 2;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Margin;
                currentCellRange.Style = headerTextStyle;
                currentColumn += 2;
                currentRow++;

                // SecondRow
                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = report.Name;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentColumn += 3;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Real;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Virtual;
                currentColumn++;

                currentColumn++; // VarianceColumn

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Real;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Virtual;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Real;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Virtual;
                currentColumn++;
                currentRow++;

                //// Header

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    var isShowVirtualTurnover = report.EnableVirtual ||
                        criteria.SportGroupId == SportContants.LiveCasino ||
                        criteria.SportGroupId == SportContants.Keno ||
                        criteria.SportGroupId == SportContants.VirtualSportGroup;
                    var reportLength = report.ChildsInfo.Count;
                    WinLossInfo info;
                    for (var i = 0; i < reportLength; i++)
                    {
                        info = report.ChildsInfo[i];
                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            i % 2 == 0);

                        this.ExportNormalRow_WinLossBySite_BetType(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType.Normal,
                            isShowVirtualTurnover);
                    }

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        false);

                    info = report.GrandTotal;
                    this.ExportNormalRow_WinLossBySite_BetType(
                        ref workSheet,
                        grandTotalRowTextStyle_FirstRow,
                        grandTotalRowTwoDecimalNumberStyle_FirstRow,
                        grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                        gtrPercentNumberStyle_FirstRow,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportGrandTotal,
                        isShowVirtualTurnover);
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.WinLostDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the normal row win loss by site bet type.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="info">The information.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="rowType">Type of the row.</param>
        /// <param name="isShowVirtualTurnover">If set to <c>true</c> [is show virtual turnover].</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNormalRow_WinLossBySite_BetType(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int rowType,
            bool isShowVirtualTurnover = false)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            switch (rowType)
            {
                case RowType._2hReportGrandTotal:
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
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
            currentCell.Style = twoDecimalPositiveNumberStyle;
            switch (rowType)
            {
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.VirtualLineTotal;

            switch (rowType)
            {
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.Variance;
            currentCell.Value = info.Variance;

            switch (rowType)
            {
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Turnover;
            if (info.IsHideRealTurnover)
            {
                currentCell.Value = Labels.NotAvailable;
            }
            else
            {
                currentCell.Value = info.Turnover;
            }

            switch (rowType)
            {
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.VirtualTurnover;
            if (isShowVirtualTurnover)
            {
                currentCell.Value = info.VirtualTurnover;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            switch (rowType)
            {
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            if (info.Turnover != 0)
            {
                currentCell.Value = info.Margin;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            if (info.VirtualTurnover != 0)
            {
                currentCell.Value = info.VirtualMargin;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }

            currentColumn++;
            currentRow++;
        }
    }
}
