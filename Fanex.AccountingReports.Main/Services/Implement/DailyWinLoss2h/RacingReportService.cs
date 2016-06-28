// --------------------------------------------------------------------------
// <copyright file="RacingReportService.cs" company="Nexcel Solutions Vietnam">
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
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
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
        /// Gets the summary racing report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The summary racing report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetSummaryRacingReport(GetSummaryRacingReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.All_Total
                    },
                    ChildsInfo = new List<WinLossInfo>(),
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit
                    }
                };

                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var sportTypeList = rawData.Select(data => Converter.ToInt(data.sporttype))
                                               .ToList();
                    CacheRef.SaveCacheRefResponse(sportTypeList, CacheRef.GetSportName, ref this._sportTypeNameCache);
                    var sportTypeGroups = rawData.GroupBy(data => new
                    {
                        sporttype = data.sporttype
                    });

                    foreach (var sportTypeGroup in sportTypeGroups)
                    {
                        var sportTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(sportTypeGroup.Key.sporttype),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo(),
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit
                            }
                        };

                        sportTypeWinLoss.Name = this._sportTypeNameCache[sportTypeWinLoss.Id];
                        sportTypeWinLoss.GrandTotal.Id = sportTypeWinLoss.Id;
                        sportTypeWinLoss.GrandTotal.Name = sportTypeWinLoss.Name;
                        foreach (var data in sportTypeGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = sportTypeWinLoss.Id,
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover),
                                SiteType = Converter.ToInt(data.Site)
                            };

                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);
                            switch (rowType)
                            {
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    sportTypeWinLoss.CreditTotal.Add(info);
                                    break;
                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    sportTypeWinLoss.DepositTotal.Add(info);
                                    break;
                            }
                        }

                        sportTypeWinLoss.GrandTotal.Add(sportTypeWinLoss.CreditTotal);
                        sportTypeWinLoss.GrandTotal.Add(sportTypeWinLoss.DepositTotal);
                        report.CreditTotal.Add(sportTypeWinLoss.CreditTotal);
                        report.DepositTotal.Add(sportTypeWinLoss.DepositTotal);
                        sportTypeWinLoss = WinLossInfo.Format_Total(sportTypeWinLoss);
                        report.ChildsInfo.Add(sportTypeWinLoss);
                    }

                    report.GrandTotal.Add(report.CreditTotal);
                    report.GrandTotal.Add(report.DepositTotal);
                    report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hSummaryRacingReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hSummaryRacingReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the racing report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The racing report by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetRacingReportBySport(GetRacingReportBySportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.All_Total
                    },
                    ChildsInfo = new List<WinLossInfo>(),
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit
                    }
                };

                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var betTypeList = rawData.Select(data => Converter.ToInt(data.bettype))
                                             .ToList();
                    CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                    var betTypeGroups = rawData.GroupBy(data => data.bettype);
                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        var betTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(betTypeGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo(),
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit
                            }
                        };

                        betTypeWinLoss.Name = this.GetBetTypeNameFromWinLossDetail(betTypeWinLoss);
                        betTypeWinLoss.GrandTotal.Id = betTypeWinLoss.Id;
                        betTypeWinLoss.GrandTotal.Name = betTypeWinLoss.Name;
                        foreach (var data in betTypeGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = betTypeWinLoss.Id,
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover),
                                SiteType = Converter.ToInt(data.site)
                            };

                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);
                            switch (rowType)
                            {
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    betTypeWinLoss.CreditTotal.Add(info);
                                    break;
                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    betTypeWinLoss.DepositTotal.Add(info);
                                    break;
                            }
                        }

                        betTypeWinLoss.GrandTotal.Add(betTypeWinLoss.CreditTotal);
                        betTypeWinLoss.GrandTotal.Add(betTypeWinLoss.DepositTotal);
                        report.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                        report.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                        betTypeWinLoss = WinLossInfo.Format_Total(betTypeWinLoss);
                        report.ChildsInfo.Add(betTypeWinLoss);
                    }

                    report.GrandTotal.Add(report.CreditTotal);
                    report.GrandTotal.Add(report.DepositTotal);
                    report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hRacingReportBySportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hRacingReportBySportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the racing report by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The racing report by bet type.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetRacingReportByBetType(GetRacingReportByBetTypeCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Grand_Total
                    },
                    ChildsInfo = new List<WinLossInfo>()
                };

                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var leagueGroups = rawData.GroupBy(data => data.leagueid);
                    var leagueIdList = rawData.Select(data => Converter.ToInt(data.leagueid))
                                              .ToList();
                    CacheRef.SaveCacheRefResponse(leagueIdList, CacheRef.GetLeagueName, ref this._leagueNameCache);
                    foreach (var leagueGroup in leagueGroups)
                    {
                        var leagueWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(leagueGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo()
                        };

                        leagueWinLoss.Name = this._leagueNameCache[leagueWinLoss.Id];
                        leagueWinLoss.GrandTotal.Name = Labels.Total;
                        foreach (var data in leagueGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Name = Converter.ToString2(data.Race),
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover)
                            };

                            leagueWinLoss.GrandTotal.Add(info);
                            info = WinLossInfo.Format_Total(info);
                            leagueWinLoss.ChildsInfo.Add(info);
                        }

                        leagueWinLoss.ChildsInfo = leagueWinLoss.ChildsInfo.OrderBy(info => info.Name).ToList();
                        report.GrandTotal.Add(leagueWinLoss.GrandTotal);
                        leagueWinLoss = WinLossInfo.Format_Total(leagueWinLoss);
                        report.ChildsInfo.Add(leagueWinLoss);
                    }

                    report.ChildsInfo = report.ChildsInfo.OrderBy(leagueGroup => leagueGroup.Id).ToList();
                    report.ChildsInfo.Sort(delegate(WinLossInfo infoA, WinLossInfo infoB)
                    {
                        return this.SortWinLossInfoByLeague(infoA, infoB);
                    });

                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hRacingReportByBeTypeSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hRacingReportByBeTypeSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the summary racing report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportSummaryRacingReport(GetSummaryRacingReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hSummaryRacingReportSessionKey);
                if (report == null)
                {
                    report = this.GetSummaryRacingReport(criteria);
                }

                //// ExcelTypes

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;
                var currentColumn = startColumn;
                CellRange currentCellRange;

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SummaryRacingReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var backgroundColor = ExcelExtensions.BackGroundColor;
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
                var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
                var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    false,
                    false);
                var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);
                var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);

                var subTotalRowColor = ExcelExtensions.TotalRowColor;
                var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
                var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, subTotalRowColor, false, false, false);
                var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);
                var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);

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

                workSheet.Columns[startColumn].Width = 200 * ExcelExtensions.FontWidth;
                for (int colIndex = 1; colIndex < 7; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                //// Header

                var title = MainResources.DailyWinLoss2h_SummaryRacingReport_Title;
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
                    true);

                //// Header

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    var reportLength = report.ChildsInfo.Count;
                    WinLossInfo info;

                    for (var i = 0; i < reportLength; i++)
                    {
                        var sportTypeWinLoss = report.ChildsInfo[i];
                        info = sportTypeWinLoss.GrandTotal;

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            true);
                        this.ExportNormalRow_Racing(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType.Normal);

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            false);
                        info = sportTypeWinLoss.CreditTotal;
                        this.ExportNormalRow_Racing(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportCredit);

                        info = sportTypeWinLoss.DepositTotal;
                        this.ExportNormalRow_Racing(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportDeposit);
                    }

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        false);

                    info = report.GrandTotal;
                    this.ExportNormalRow_Racing(
                        ref workSheet,
                        grandTotalRowTextStyle_FirstRow,
                        grandTotalRowTwoDecimalNumberStyle_FirstRow,
                        grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                        gtrPercentNumberStyle_FirstRow,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportGrandTotal);

                    info = report.CreditTotal;
                    this.ExportNormalRow_Racing(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportCredit);

                    info = report.DepositTotal;
                    this.ExportNormalRow_Racing(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportDeposit);
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

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

        /// <summary>
        /// Exports the racing report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportRacingReportBySport(GetRacingReportBySportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hRacingReportBySportSessionKey);
                if (report == null)
                {
                    report = this.GetRacingReportBySport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SummaryRacingReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;
                var currentColumn = startColumn;
                int maxColumnIndex = 6;
                CellRange currentCellRange;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var backgroundColor = ExcelExtensions.BackGroundColor;
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
                var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
                var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    false,
                    false);
                var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);
                var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);

                var subTotalRowColor = ExcelExtensions.TotalRowColor;
                var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
                var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, subTotalRowColor, false, false, false);
                var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);
                var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);

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

                workSheet.Columns[startColumn].Width = 200 * ExcelExtensions.FontWidth;
                for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                //// Header

                var title = string.Format(MainResources.DailyWinLoss2h_RacingReportBySport_Title, criteria.SportName);
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

                //// Header

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    var reportLength = report.ChildsInfo.Count;
                    WinLossInfo info;

                    for (var i = 0; i < reportLength; i++)
                    {
                        var betTypeWinLoss = report.ChildsInfo[i];

                        info = betTypeWinLoss.GrandTotal;
                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            true);
                        this.ExportNormalRow_Racing(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType.Normal);

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            false);
                        info = betTypeWinLoss.CreditTotal;
                        this.ExportNormalRow_Racing(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportCredit);

                        info = betTypeWinLoss.DepositTotal;
                        this.ExportNormalRow_Racing(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportDeposit);
                    }

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        false);

                    info = report.GrandTotal;
                    this.ExportNormalRow_Racing(
                        ref workSheet,
                        grandTotalRowTextStyle_FirstRow,
                        grandTotalRowTwoDecimalNumberStyle_FirstRow,
                        grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                        gtrPercentNumberStyle_FirstRow,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportGrandTotal);

                    info = report.CreditTotal;
                    this.ExportNormalRow_Racing(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportCredit);

                    info = report.DepositTotal;
                    this.ExportNormalRow_Racing(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportDeposit);
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

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

        /// <summary>
        /// Exports the type of the racing report by bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportRacingReportByBetType(GetRacingReportByBetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hRacingReportByBeTypeSessionKey);
                if (report == null)
                {
                    report = this.GetRacingReportByBetType(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SummaryRacingReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;
                var currentColumn = startColumn;
                int maxColumnIndex = 6;
                CellRange currentCellRange;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var backgroundColor = ExcelExtensions.BackGroundColor;
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
                var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
                var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    false,
                    false);
                var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);
                var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);

                var subTotalRowColor = ExcelExtensions.TotalRowColor;
                var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
                var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, subTotalRowColor, false, false, false);
                var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);
                var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);

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
                    workSheet.Columns[startColumn + colIndex].Width = 100 * ExcelExtensions.FontWidth;
                }

                var title = string.Format(
                    MainResources.DailyWinLoss2h_RacingReportByBetType_Title,
                    criteria.SportName,
                    criteria.BetTypeName);
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

                //// Header

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    var leagueLength = report.ChildsInfo.Count;
                    WinLossInfo info;

                    for (var i = 0; i < leagueLength; i++)
                    {
                        var league = report.ChildsInfo[i];

                        //// LeagueHeadLine

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            true);

                        info = league;
                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = info.Name;
                        currentCellRange.Style = textStyle;
                        currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                        currentRow++;

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                               ref twoDecimalNumberStyle,
                               ref twoDecimalPositiveNumberStyle,
                               ref twoDecimalPositivePercentNumberStyle,
                               false);

                        //// LeagueHeadLine

                        for (var j = 0; j < league.ChildsInfo.Count; j++)
                        {
                            info = league.ChildsInfo[j];
                            this.ExportNormalRow_Racing_ByBetType(
                                ref workSheet,
                                textStyle,
                                twoDecimalNumberStyle,
                                twoDecimalPositiveNumberStyle,
                                twoDecimalPositivePercentNumberStyle,
                                info,
                                startColumn,
                                ref currentRow,
                                RowType.Normal);
                        }

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            false);

                        info = league.GrandTotal;
                        this.ExportNormalRow_Racing_ByBetType(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportNormalTotal);
                    }

                    info = report.GrandTotal;
                    this.ExportNormalRow_Racing_ByBetType(
                        ref workSheet,
                        grandTotalRowTextStyle_FirstRow,
                        grandTotalRowTwoDecimalNumberStyle_FirstRow,
                        grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                        gtrPercentNumberStyle_FirstRow,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportGrandTotal);
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

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

        /// <summary>
        /// Exports the normal row racing.
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
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNormalRow_Racing(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int rowType)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            switch (rowType)
            {
                case RowType._2hReportCredit:
                case RowType._2hReportDeposit:
                case RowType._2hReportGrandTotal:
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
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
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            if (rowType == RowType._2hReportGrandTotal)
            {
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Turnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the normal row_ racing_ by bet type.
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
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNormalRow_Racing_ByBetType(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int rowType)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            if (rowType == RowType._2hReportNormalTotal ||
                rowType == RowType._2hReportGrandTotal)
            {
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.WinLoss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Position;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Commission;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            if (rowType == RowType._2hReportNormalTotal ||
                rowType == RowType._2hReportGrandTotal)
            {
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Turnover;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            if (rowType == RowType._2hReportNormalTotal ||
                rowType == RowType._2hReportGrandTotal)
            {
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            currentColumn++;
            currentRow++;
        }
    }
}
