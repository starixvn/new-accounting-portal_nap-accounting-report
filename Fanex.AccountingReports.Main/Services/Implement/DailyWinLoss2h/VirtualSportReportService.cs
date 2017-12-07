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
        /// Gets the virtual sport report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The virtual sport report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetVirtualSportReport(GetVirtualSportReportCriteria criteria)
        {
            try
            {
                var report = new LiveAndNoneLiveReport
                {
                    NoneLiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.All_Total
                        },
                        EightRMTotal = new WinLossInfo(),
                        BARMTotal = new WinLossInfo(),
                        CreditTotal = new WinLossInfo
                        {
                            Name = Labels.Credit
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Name = Labels.Deposit
                        },
                        EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.VirtualSoccer)
                    }
                };

                var rawData = this._reportRepository.GetVirtualSportReport(criteria);
                if (rawData != null)
                {
                    var rawReport = rawData[0] as List<dynamic>;
                    var sportTypeList = rawReport.Select(data => Converter.ToInt(data.SportType))
                                                 .ToList();
                    CacheRef.SaveCacheRefResponse(sportTypeList, CacheRef.GetSportName, ref this._sportTypeNameCache);
                    var sportTypeGroups = rawReport.GroupBy(data => data.SportType);
                    foreach (var sportTypeGroup in sportTypeGroups)
                    {
                        var sportTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(sportTypeGroup.Key),
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
                                Hedge = Converter.ToString2(data.IsHedge),
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
                        report.NoneLiveReport.CreditTotal.Add(sportTypeWinLoss.CreditTotal);
                        report.NoneLiveReport.DepositTotal.Add(sportTypeWinLoss.DepositTotal);
                        sportTypeWinLoss = WinLossInfo.Format_Total(sportTypeWinLoss);
                        report.NoneLiveReport.ChildsInfo.Add(sportTypeWinLoss);
                    }

                    if (this._8rmConfig.IsActive)
                    {
                        var _8RMReport = rawData[1];
                        foreach (var data in _8RMReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLost8RM),
                                IsLive = Converter.ToBool(data.liveindicator)
                            };

                            report.NoneLiveReport.EightRMTotal.Add(info);
                        }

                        report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.EightRMTotal);
                    }

                    if (this._barmConfig.IsActive)
                    {
                        var barmReport = rawData[2];
                        foreach (var data in barmReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLostBARM),
                                IsLive = Converter.ToBool(data.liveindicator)
                            };

                            report.NoneLiveReport.BARMTotal.Add(info);
                        }

                        report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.BARMTotal);
                    }

                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.CreditTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.DepositTotal);
                    report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);
                }

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hVirtualSportReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hVirtualSportReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the virtual sport report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The virtual sport report by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetVirtualSportReportBySport(GetVirtualSportReportBySportCriteria criteria)
        {
            try
            {
                var report = new LiveAndNoneLiveReport
                {
                    NoneLiveReport = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.All_Total
                        },
                        EightRMTotal = new WinLossInfo(),
                        BARMTotal = new WinLossInfo(),
                        CreditTotal = new WinLossInfo
                        {
                            Name = Labels.Credit
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Name = Labels.Deposit
                        },
                        EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(criteria.Sporttype)
                    }
                };

                var rawData = this._reportRepository.GetVirtualSportReportBySport(criteria);
                if (rawData != null)
                {
                    var rawReport = rawData[0] as List<dynamic>;
                    var betTypeList = rawReport.Select(data => Converter.ToInt(data.Bettype))
                                                 .ToList();
                    CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                    var betTypeGroups = rawReport.GroupBy(data => data.Bettype);
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
                                Hedge = Converter.ToString2(data.IsHedge),
                                SiteType = Converter.ToInt(data.Site)
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
                        report.NoneLiveReport.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                        report.NoneLiveReport.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                        betTypeWinLoss = WinLossInfo.Format_Total(betTypeWinLoss);
                        report.NoneLiveReport.ChildsInfo.Add(betTypeWinLoss);
                    }

                    if (this._8rmConfig.IsActive)
                    {
                        var _8RMReport = rawData[1];
                        foreach (var data in _8RMReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLost8RM),
                                IsLive = Converter.ToBool(data.liveindicator)
                            };

                            report.NoneLiveReport.EightRMTotal.Add(info);
                        }

                        report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.EightRMTotal);
                    }

                    if (this._barmConfig.IsActive)
                    {
                        var barmReport = rawData[2];
                        foreach (var data in barmReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLostBARM),
                                IsLive = Converter.ToBool(data.liveindicator)
                            };

                            report.NoneLiveReport.BARMTotal.Add(info);
                        }

                        report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.BARMTotal);
                    }

                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.CreditTotal);
                    report.NoneLiveReport.GrandTotal.Add(report.NoneLiveReport.DepositTotal);
                    report.NoneLiveReport = WinLossInfo.Format_Total(report.NoneLiveReport);
                }

                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hVirtualSportReportBySportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hVirtualSportReportBySportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the virtual sport report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportVirtualSportReport(GetVirtualSportReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hVirtualSportReportSessionKey);
                if (report == null)
                {
                    report = this.GetVirtualSportReport(criteria);
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

                var title = MainResources.DailyWinLoss2h_VirtualSportReport_Title;
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

                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report.NoneLiveReport,
                    SportContants.VirtualSportGroup);

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
        /// Exports the virtual sport report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportVirtualSportReportBySport(GetVirtualSportReportBySportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(MainConstants.DailyWinLoss2hVirtualSportReportBySportSessionKey);
                if (report == null)
                {
                    report = this.GetVirtualSportReportBySport(criteria);
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

                var title = string.Format(MainResources.DailyWinLoss2h_VirtualSportReportBySport_Title, criteria.SportTypeName).ToUpper();
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

                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report.NoneLiveReport,
                    SportContants.VirtualSportGroup);

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
        /// Exports the type of the virtual sport report by bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportVirtualSportReportByBetType(GetReportByBetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hReportByBetTypeSessionKey);
                if (report == null)
                {
                    report = this.GetReportByBetType(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_CasinoReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 7;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                //// ExcelTypes
                //// MakeExcelFile

                var title = string.Format(
                    MainResources.DailyWinLoss2h_ReportByBetType_Title,
                    Labels.Virtual_Sports + "|" + criteria.SportTypeName,
                    criteria.BetTypeName,
                    string.Empty).ToUpper();
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                this.ExportHeaderColumns_VirtualSport_BetType(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn);

                this.ExportContent_VirtualSport_BetType(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report,
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

        /// <summary>
        /// Exports the header columns virtual sport bet type.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportHeaderColumns_VirtualSport_BetType(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn)
        {
            //// ExcelTypes

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var maxColumnIndex = 7;
            var currentColumn = startColumn;
            ExcelCell currentCell;

            //// ExcelTypes

            workSheet.Columns[startColumn].Width = 200 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 1].Width = 500 * ExcelExtensions.FontWidth;

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
            currentCell.Value = Labels.Turnover;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Company_Turnover;
            currentCell.Value = Labels.Margin;
            currentCell.Style = headerTextStyle;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the content virtual sport bet type.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportContent_VirtualSport_BetType(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            WinLossInfo report,
            GetReportByBetTypeCriteria criteria)
        {
            int maxColumnIndex = 7;
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

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

            var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            if (report != null && report.ChildsInfo.Count > 0)
            {
                var leagueLength = report.ChildsInfo.Count;
                WinLossInfo info;

                for (var i = 0; i < leagueLength; i++)
                {
                    //// LeagueHeadLine

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        true);

                    var league = report.ChildsInfo[i];
                    info = league;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = textStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 1);
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
                        //// NormalRow

                        info = league.ChildsInfo[j];

                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MatchCode;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.HomeTeamName +
                            (info.IsNeutral ? " " + Labels.Symbol_NeutralSuffix : string.Empty) +
                            Labels.Vs + info.AwayTeamName;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
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
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Turnover;
                        currentCell.Style = twoDecimalNumberStyle;
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
                        currentRow++;

                        //// NormalRow
                    }

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        false);

                    if (league.ChildsInfo.Count >= 2)
                    {
                        //// LeagueTotal

                        info = league.GrandTotal;
                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 6);
                        currentCellRange.Merged = true;
                        currentCellRange.Style = textStyle;
                        currentColumn = currentColumn + 1 + maxColumnIndex - 6;

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
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Turnover;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
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
                        currentRow++;

                        //// LeagueTotal
                    }
                }

                //// GrandTotal

                info = report.GrandTotal;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 6);
                currentCellRange.Merged = true;
                currentCellRange.Style = grandTotalRowTextStyle_FirstRow;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCellRange.Value = string.Format(
                    "{0}{1}{2} " + Labels.Total,
                    criteria.Liveindicator ? Labels.Live + "! " : string.Empty,
                    criteria.BetTypeName,
                    criteria.IsHedge ? "(" + Labels.Hedge + ")" : string.Empty);
                currentColumn = currentColumn + 1 + maxColumnIndex - 6;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.WinLoss;
                currentCell.Style = grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Position;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Commission;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.LineTotal;
                currentCell.Style = grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Turnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = gtrPercentNumberStyle_FirstRow;
                if (info.Turnover != 0)
                {
                    currentCell.Value = info.Margin;
                }
                else
                {
                    currentCell.Value = Labels.NotAvailable;
                }

                currentColumn++;
                currentRow++;

                //// GrandTotal
                //// IBCTotal

                decimal allIBCTotal = 0;
                if (report.EightRMTotal != null)
                {
                    allIBCTotal += report.EightRMTotal.WinLoss;
                }

                if (report.BARMTotal != null)
                {
                    allIBCTotal += report.BARMTotal.WinLoss;
                }

                if (allIBCTotal != 0)
                {
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 6);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = this.GetIBCTotalName();
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn = currentColumn + maxColumnIndex - 6 + 1;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = allIBCTotal;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
                    currentCellRange.Merged = true;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentRow++;
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
