// --------------------------------------------------------------------------
// <copyright file="CasinoReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

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
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        /// <summary>
        /// Gets the casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The casino report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetCasinoReport(GetCasinoReportCriteria criteria)
        {
            try
            {
                var isHideRealTurnover = false;
                var report = new LiveAndNoneLiveReport
                {
                    NoneLiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Id = SportContants._2hReportCasino,
                            Name = Labels.All_Total,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        CreditTotal = new WinLossInfo
                        {
                            Id = SportContants._2hReportCasino,
                            Name = Labels.Credit,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Id = SportContants._2hReportCasino,
                            Name = Labels.Deposit,
                            IsHideRealTurnover = isHideRealTurnover
                        },
                        EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants._2hReportCasino),
                        IsHideRealTurnover = isHideRealTurnover,
                        IsLive = false
                    }
                };

                Hashtable outParams = new Hashtable();
                var rawData = this._reportRepository.GetCasinoReport(criteria, ref outParams);
                if (rawData != null)
                {
                    var productGroups = rawData.GroupBy(data => data.ProductId);

                    foreach (var productGroup in productGroups)
                    {
                        int productId = Converter.ToInt(productGroup.Key);
                        isHideRealTurnover = productId == SportContants._2hCasinoSport && DateTime.Compare(criteria.Winlostdate, AppSettings.CasinoSingleWalletLaunchDate) < 0 ? true : false;
                        var productWinLoss = new WinLossInfo
                        {
                            Id = productId,
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo
                            {
                                IsHideRealTurnover = isHideRealTurnover
                            },
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit,
                                IsHideRealTurnover = isHideRealTurnover
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit,
                                IsHideRealTurnover = isHideRealTurnover
                            }
                        };

                        productWinLoss.Name = _dailyWinLoss2hUtilityService.GetSportName(productGroup.Key);
                        productWinLoss.GrandTotal.Id = productWinLoss.Id;
                        productWinLoss.GrandTotal.Name = productWinLoss.Name;
                        foreach (var data in productGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = Converter.ToInt(data.ProductId),
                                BettypeId = Converter.ToInt(data.BetType),
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover),
                                SiteType = Converter.ToInt(data.Site)
                            };

                            productWinLoss.CreditTotal.BettypeId = info.BettypeId;
                            productWinLoss.DepositTotal.BettypeId = info.BettypeId;
                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);
                            switch (rowType)
                            {
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    productWinLoss.CreditTotal.Add(info);
                                    if (isHideRealTurnover)
                                    {
                                        productWinLoss.CreditTotal.Turnover -= info.Turnover;
                                    }

                                    break;

                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    productWinLoss.DepositTotal.Add(info);
                                    if (isHideRealTurnover)
                                    {
                                        productWinLoss.DepositTotal.Turnover -= info.Turnover;
                                    }

                                    break;
                            }
                        }

                        productWinLoss.GrandTotal.Add(productWinLoss.CreditTotal);
                        productWinLoss.GrandTotal.Add(productWinLoss.DepositTotal);
                        report.NoneLiveReport.CreditTotal.Add(productWinLoss.CreditTotal);
                        report.NoneLiveReport.DepositTotal.Add(productWinLoss.DepositTotal);
                        productWinLoss = WinLossInfo.Format_Total(productWinLoss);
                        report.NoneLiveReport.ChildsInfo.Add(productWinLoss);
                    }

                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.CreditTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.DepositTotal);
                    report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);
                }

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hCasinoReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hCasinoReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportCasinoReport(GetCasinoReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hCasinoReportSessionKey);
                if (report == null)
                {
                    report = this.GetCasinoReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_CasinoReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                //// ExcelTypes
                //// MakeExcelFile

                var title = MainResources.DailyWinLoss2h_CasinoReport_Title;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                this.ExportHeaderColumns_SportReport_NotApplyVirtualRate(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false);

                this.ExportCasinoProductReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report.NoneLiveReport,
                    criteria);

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <param name="sportId">The sport identifier.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportCasinoProductReport(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            WinLossInfo report,
            GetCasinoReportCriteria criteria)
        {
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
                var isShowVirtualTurnover = false;
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
                    info = betTypeGroup.GrandTotal;
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

                    info = betTypeGroup.CreditTotal;
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

                    info = betTypeGroup.DepositTotal;
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

                info = report.GrandTotal;
                this.ExportTotalRow(
                    ref workSheet,
                    grandTotalRowTextStyle_FirstRow,
                    grandTotalRowTwoDecimalNumberStyle_FirstRow,
                    grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                    gtrPercentNumberStyle_FirstRow,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportGrandTotal,
                    criteria);

                info = report.CreditTotal;
                this.ExportTotalRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportCredit,
                    criteria);

                info = report.DepositTotal;
                this.ExportTotalRow(
                    ref workSheet,
                    grandTotalRowTextStyle,
                    grandTotalRowTwoDecimalNumberStyle,
                    grandTotalTwoDecimalPositiveNumberStyle,
                    grandTotalTwoDecimalPositivePercentNumberStyle,
                    info,
                    startColumn,
                    ref currentRow,
                    RowType._2hReportDeposit,
                    criteria);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 6);
            }
        }

        public void ExportTotalRow(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int rowType,
            GetCasinoReportCriteria criteria)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            switch (rowType)
            {
                case RowType._2hReportCredit:
                case RowType._2hReportDeposit:
                case RowType._2hReportVirtualAllTotal:
                case RowType._2hReportGrandTotal:
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
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
                case RowType._2hReportVirtualAllTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;

                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
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
                case RowType._2hReportVirtualAllTotal:
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    break;
            }

            currentColumn++;
            currentCell = workSheet.Cells[currentRow, currentColumn];

            if (DateTime.Compare(criteria.Winlostdate, AppSettings.CasinoSingleWalletLaunchDate) < 0)
            {
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCell.Value = Labels.NotAvailable;
            }
            else
            {
                currentCell.Style = twoDecimalPositivePercentNumberStyle;
                _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            }

            currentColumn++;
            currentRow++;
        }
    }
}