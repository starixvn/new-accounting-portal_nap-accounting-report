// --------------------------------------------------------------------------
// <copyright file="MainReportService.cs" company="Nexcel Solutions Vietnam">
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
    using Core.Configurations;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        /// <summary>
        /// Gets the main2h report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The main2h report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetMain2hReport(GetMain2hReportCriteria criteria)
        {
            try
            {
                string sportName = string.Empty;
                WinLossInfo report = new WinLossInfo
                {
                    EightRMTotal = new WinLossInfo
                    {
                        Name = Labels.EightRM
                    },
                    BARMTotal = new WinLossInfo
                    {
                        Name = Labels.BARM
                    },
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Grand_Total.ToUpper()
                    },
                    NormalTotal = new WinLossInfo
                    {
                        Name = Labels.Virtual_All_Total
                    },
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit,
                        EnableVirtual = true
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit,
                        EnableVirtual = true
                    },
                    ChildsInfo = new List<WinLossInfo>(),
                    EnableVirtual = true
                };

                Hashtable outParams = new Hashtable();
                var rawData = this._reportRepository.GetMain2hReport(criteria, ref outParams);
                decimal excludeFromGrandTotal = 0;
                WinLossInfo ibcReport = new WinLossInfo
                {
                    Id = CoreConstants.ProductContants.SportBook,
                    Name = Labels.Sportsbook,
                    EightRMTotal = new WinLossInfo
                    {
                        Name = Labels.EightRM
                    },
                    BARMTotal = new WinLossInfo
                    {
                        Name = Labels.BARM
                    },
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Grand_Total
                    },
                    NormalTotal = new WinLossInfo
                    {
                        Name = Labels.Sportsbook_Total,
                        EnableVirtual = true
                    },
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit,
                        EnableVirtual = true
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit,
                        EnableVirtual = true
                    },
                    ChildsInfo = new List<WinLossInfo>()
                };

                WinLossInfo thirdPartyReport = new WinLossInfo
                {
                    Id = CoreConstants.ProductContants.ThirdParty,
                    Name = Labels.Third_Party,
                    EightRMTotal = new WinLossInfo
                    {
                        Name = Labels.EightRM
                    },
                    BARMTotal = new WinLossInfo
                    {
                        Name = Labels.BARM
                    },
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Grand_Total
                    },
                    NormalTotal = new WinLossInfo
                    {
                        Name = Labels.Non_Sports_Total
                    },
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit,
                        EnableVirtual = true
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit,
                        EnableVirtual = true
                    },
                    ChildsInfo = new List<WinLossInfo>()
                };

                if (rawData != null)
                {
                    this._sportTypeNameCache = new Dictionary<object, string>();
                    var sportIdList = rawData.Select(info => Converter.ToInt(info.SportType)).Distinct().ToList();
                    _dailyWinLoss2hUtilityService.SaveSportTypeCacheRef(sportIdList, ref this._sportTypeNameCache);
                    var sportGroups = rawData.GroupBy(data => data.SportType);
                    var isIBC = false;
                    foreach (var sportGroup in sportGroups)
                    {
                        int groupId = Converter.ToInt(sportGroup.Key);

                        var sportWinLoss = new WinLossInfo
                        {
                            Id = groupId,
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo(),
                            EightRMTotal = new WinLossInfo
                            {
                                Id = RowType.EightRM
                            },
                            BARMTotal = new WinLossInfo
                            {
                                Id = RowType.BARM
                            },
                            CreditTotal = new WinLossInfo
                            {
                                Id = groupId,
                                Name = Labels.Credit
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Id = groupId,
                                Name = Labels.Deposit
                            },
                            NormalTotal = new WinLossInfo(),
                            DisplayOrder = sportGroup.FirstOrDefault().DisplayOrder
                        };

                        sportWinLoss.Name = this._sportTypeNameCache[sportWinLoss.Id];
                        sportWinLoss.NormalTotal.Id = sportWinLoss.Id;
                        sportWinLoss.EightRMTotal.Name = sportWinLoss.Name + " " + Labels.EightRM;
                        sportWinLoss.BARMTotal.Name = sportWinLoss.Name + " " + Labels.BARM;
                        sportWinLoss.EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(sportWinLoss.Id);
                        sportWinLoss.CreditTotal.EnableVirtual = sportWinLoss.EnableVirtual;
                        sportWinLoss.DepositTotal.EnableVirtual = sportWinLoss.EnableVirtual;
                        sportWinLoss.NormalTotal.EnableVirtual = sportWinLoss.EnableVirtual;
                        isIBC = _dailyWinLoss2hUtilityService.IsSportBookSport(sportWinLoss.Id);
                        foreach (var data in sportGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = sportWinLoss.Id,
                                Name = sportWinLoss.Name,
                                EnableVirtual = sportWinLoss.EnableVirtual,
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                                IBC = Converter.ToInt(data.ibc),
                                SiteType = Converter.ToInt(data.Site),
                                DisplayOrder = Convert.ToInt32(data.DisplayOrder),
                                ExtraPositionTaking = Converter.ToDecimal(data.ExtraPositionTaking)
                            };

                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(info.IBC, info.SiteType);
                            if (!info.EnableVirtual &&
                                info.Id != CoreConstants.SportContants.LiveCasino &&
                                info.Id != CoreConstants.SportContants.Keno &&
                                info.Id != CoreConstants.SportContants.VirtualSportGroup &&
                                info.Id != SportContants._2hReportCasino &&
                                info.Id != CoreConstants.SportContants.ColossusBet &&
                                info.Id != CoreConstants.SportContants.CockFighting)
                            {
                                info.VirtualTurnover = 0;
                            }

                            if ((!this._barmConfig.IsActive && rowType == RowType.BARM) ||
                                (!this._8rmConfig.IsActive && rowType == RowType.EightRM))
                            {
                                continue;
                            }

                            //// CalculateReportTotal

                            switch (rowType)
                            {
                                case RowType.EightRM:
                                    info.AliasName = info.Name + " " + Labels.EightRM;
                                    if (isIBC)
                                    {
                                        ibcReport.EightRMTotal.Add(info);
                                    }
                                    else
                                    {
                                        thirdPartyReport.EightRMTotal.Add(info);
                                    }

                                    sportWinLoss.EightRMTotal.Add(info);
                                    break;

                                case RowType.BARM:
                                    info.AliasName = info.Name + " " + Labels.BARM;
                                    if (isIBC)
                                    {
                                        ibcReport.BARMTotal.Add(info);
                                    }
                                    else
                                    {
                                        thirdPartyReport.BARMTotal.Add(info);
                                    }

                                    sportWinLoss.BARMTotal.Add(info);
                                    break;

                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    sportWinLoss.CreditTotal.Id = info.Id;
                                    sportWinLoss.CreditTotal.Add(info);
                                    break;

                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    sportWinLoss.DepositTotal.Id = info.Id;
                                    sportWinLoss.DepositTotal.Add(info);
                                    break;
                            }

                            if ((info.Id == SoccerBetTypeConstants.SoccerMajorBetType ||
                                info.Id == SoccerBetTypeConstants.SoccerMinorBetType ||
                                info.Id == SportContants._2hparlaySport) &&
                                info.SiteType != 0)
                            {
                                sportName = Converter.ToString2(data.SportName);
                            }
                            //// CalculateReportTotal
                        }

                        AssignWinLossInfoSportName(sportWinLoss, sportName);
                        sportWinLoss.NormalTotal.Name = sportWinLoss.Name;
                        sportWinLoss.NormalTotal.Add(sportWinLoss.CreditTotal);
                        sportWinLoss.NormalTotal.Add(sportWinLoss.DepositTotal);
                        if (isIBC)
                        {
                            ibcReport.CreditTotal.Add(sportWinLoss.CreditTotal);
                            ibcReport.DepositTotal.Add(sportWinLoss.DepositTotal);
                        }
                        else
                        {
                            thirdPartyReport.CreditTotal.Add(sportWinLoss.CreditTotal);
                            thirdPartyReport.DepositTotal.Add(sportWinLoss.DepositTotal);
                        }

                        if (!sportWinLoss.EnableVirtual)
                        {
                            excludeFromGrandTotal += sportWinLoss.NormalTotal.LineTotal;
                        }

                        sportWinLoss = WinLossInfo.Format_Total(sportWinLoss);
                        if (isIBC)
                        {
                            ibcReport.ChildsInfo.Add(sportWinLoss);
                        }
                        else
                        {
                            if (_dailyWinLoss2hUtilityService.IsHideRealTurnover(sportWinLoss.Id))
                            {
                                sportWinLoss.CreditTotal.IsHideRealTurnover = true;
                                sportWinLoss.DepositTotal.IsHideRealTurnover = true;
                                sportWinLoss.NormalTotal.IsHideRealTurnover = true;
                            }

                            thirdPartyReport.ChildsInfo.Add(sportWinLoss);
                        }
                    }

                    ibcReport.NormalTotal.Add(ibcReport.CreditTotal);
                    ibcReport.NormalTotal.Add(ibcReport.DepositTotal);
                    ibcReport.ChildsInfo = ibcReport.ChildsInfo.OrderBy(info => info.DisplayOrder).ToList();
                    thirdPartyReport.NormalTotal.Add(thirdPartyReport.CreditTotal);
                    thirdPartyReport.NormalTotal.Add(thirdPartyReport.DepositTotal);
                    thirdPartyReport.ChildsInfo = thirdPartyReport.ChildsInfo.OrderBy(info => info.DisplayOrder).ToList();

                    report.EightRMTotal.Add(ibcReport.EightRMTotal);
                    report.EightRMTotal.Add(thirdPartyReport.EightRMTotal);
                    report.BARMTotal.Add(ibcReport.BARMTotal);
                    report.BARMTotal.Add(thirdPartyReport.BARMTotal);

                    report.NormalTotal.Add(ibcReport.NormalTotal);
                    report.NormalTotal.Add(thirdPartyReport.NormalTotal);
                    report.CreditTotal.Add(ibcReport.CreditTotal);
                    report.CreditTotal.Add(thirdPartyReport.CreditTotal);
                    report.DepositTotal.Add(ibcReport.DepositTotal);
                    report.DepositTotal.Add(thirdPartyReport.DepositTotal);

                    report.CreditTotal.Add(report.EightRMTotal);
                    report.DepositTotal.Add(report.BARMTotal);

                    ibcReport = WinLossInfo.Format_Total(ibcReport);
                    thirdPartyReport = WinLossInfo.Format_Total(thirdPartyReport);
                    report.ChildsInfo.Add(ibcReport);
                    report.ChildsInfo.Add(thirdPartyReport);

                    report.GrandTotal.Add(report.CreditTotal);
                    report.GrandTotal.Add(report.DepositTotal);
                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hMain2hReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hMain2hReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the main2h report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMain2hReport(GetMain2hReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hMain2hReportSessionKey);
                var infoRMList = new List<WinLossInfo>();
                if (report == null)
                {
                    report = this.GetMain2hReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 12;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

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

                var subTotalRowColor = ExcelExtensions.TotalRowColor;
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

                //// ExcelTypes
                //// MakeExcelFile

                //// Header

                workSheet.Columns[startColumn].Width = 190 * ExcelExtensions.FontWidth;
                for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 130 * ExcelExtensions.FontWidth;
                }

                var title = MainResources.DailyWinLoss2h_Main2hReport_ReportTitle;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                // FirstRow
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Sport_Type;
                currentCellRange.Style = headerTextStyle;
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

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Company_Income;
                currentCellRange.Style = headerTextStyle;
                currentColumn += 5;

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
                currentColumn = startColumn + 4;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Variance;
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
                currentCell.Value = Labels.Extra_PT;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.After_Extra_PT;
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

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Real;
                currentColumn++;

                currentRow++;

                //// Header

                if (report != null && report.ChildsInfo.Count >= 2 &&
                    (report.ChildsInfo[0].ChildsInfo.Count > 0 ||
                    report.ChildsInfo[1].ChildsInfo.Count > 0))
                {
                    var ibcGroupLength = report.ChildsInfo.Count;
                    WinLossInfo info;
                    for (var i = 0; i < ibcGroupLength; i++)
                    {
                        infoRMList = new List<WinLossInfo>();
                        var ibcWinLoss = report.ChildsInfo[i];
                        var sportLength = ibcWinLoss.ChildsInfo.Count;
                        for (var j = 0; j < sportLength; j++)
                        {
                            var sportWinLoss = ibcWinLoss.ChildsInfo[j];
                            var isShowVirtualTurnover = sportWinLoss.EnableVirtual ||
                                sportWinLoss.Id == CoreConstants.SportContants.LiveCasino ||
                                sportWinLoss.Id == CoreConstants.SportContants.Keno ||
                                sportWinLoss.Id == CoreConstants.SportContants.VirtualSportGroup ||
                                sportWinLoss.Id == SportContants._2hReportCasino ||
                                sportWinLoss.Id == CoreConstants.SportContants.ColossusBet;

                            bool isCasinoSport = sportWinLoss.Id == SportContants._2hReportCasino;

                            var len = sportWinLoss.ChildsInfo.Count;

                            info = sportWinLoss.NormalTotal;
                            _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                                ref textStyle,
                                ref twoDecimalNumberStyle,
                                ref twoDecimalPositiveNumberStyle,
                                ref twoDecimalPositivePercentNumberStyle,
                                true);

                            if (isCasinoSport)
                            {
                                this.ExportMainReportCasinoRow(
                                    ref workSheet,
                                    textStyle,
                                    twoDecimalNumberStyle,
                                    twoDecimalPositiveNumberStyle,
                                    twoDecimalPositivePercentNumberStyle,
                                    info,
                                    startColumn,
                                    ref currentRow,
                                    criteria.Winlostdate,
                                    RowType.Normal);
                            }
                            else
                            {
                                this.ExportMainDataRow(
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
                            }

                            _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                                ref textStyle,
                                ref twoDecimalNumberStyle,
                                ref twoDecimalPositiveNumberStyle,
                                ref twoDecimalPositivePercentNumberStyle,
                                false);

                            info = sportWinLoss.CreditTotal;

                            if (isCasinoSport)
                            {
                                this.ExportMainReportCasinoRow(
                                    ref workSheet,
                                    textStyle,
                                    twoDecimalNumberStyle,
                                    twoDecimalPositiveNumberStyle,
                                    twoDecimalPositivePercentNumberStyle,
                                    info,
                                    startColumn,
                                    ref currentRow,
                                    criteria.Winlostdate,
                                    RowType._2hReportCredit);
                            }
                            else
                            {
                                this.ExportMainDataRow(
                                    ref workSheet,
                                    textStyle,
                                    twoDecimalNumberStyle,
                                    twoDecimalPositiveNumberStyle,
                                    twoDecimalPositivePercentNumberStyle,
                                    info,
                                    startColumn,
                                    ref currentRow,
                                    RowType._2hReportCredit,
                                    report.EnableVirtual,
                                    isShowVirtualTurnover);
                            }

                            info = sportWinLoss.DepositTotal;

                            if (isCasinoSport)
                            {
                                this.ExportMainReportCasinoRow(
                                    ref workSheet,
                                    textStyle,
                                    twoDecimalNumberStyle,
                                    twoDecimalPositiveNumberStyle,
                                    twoDecimalPositivePercentNumberStyle,
                                    info,
                                    startColumn,
                                    ref currentRow,
                                    criteria.Winlostdate,
                                    RowType._2hReportDeposit);
                            }
                            else
                            {
                                this.ExportMainDataRow(
                                    ref workSheet,
                                    textStyle,
                                    twoDecimalNumberStyle,
                                    twoDecimalPositiveNumberStyle,
                                    twoDecimalPositivePercentNumberStyle,
                                    info,
                                    startColumn,
                                    ref currentRow,
                                    RowType._2hReportDeposit,
                                    report.EnableVirtual,
                                    isShowVirtualTurnover);
                            }

                            if (this._8rmConfig.IsActive && sportWinLoss.EightRMTotal.LineTotal != 0)
                            {
                                info = sportWinLoss.EightRMTotal;
                                if (sportWinLoss.Id != SoccerBetTypeConstants.SoccerMajorBetType &&
                                    sportWinLoss.Id != SoccerBetTypeConstants.SoccerMinorBetType &&
                                    sportWinLoss.Id != Fanex.AccountingReports.Main.Constants.SportContants._2hReportOtherSport)
                                {
                                    this.ExportRMRow(
                                        ref workSheet,
                                        textStyle,
                                        twoDecimalNumberStyle,
                                        twoDecimalPositiveNumberStyle,
                                        twoDecimalPositivePercentNumberStyle,
                                        info,
                                        startColumn,
                                        maxColumnIndex,
                                        ref currentRow);
                                }
                                else
                                {
                                    infoRMList.Add(info);
                                }
                            }

                            if (this._barmConfig.IsActive && sportWinLoss.BARMTotal.LineTotal != 0)
                            {
                                info = sportWinLoss.BARMTotal;
                                if (sportWinLoss.Id != SoccerBetTypeConstants.SoccerMajorBetType &&
                                    sportWinLoss.Id != SoccerBetTypeConstants.SoccerMinorBetType &&
                                    sportWinLoss.Id != Fanex.AccountingReports.Main.Constants.SportContants._2hReportOtherSport)
                                {
                                    this.ExportRMRow(
                                        ref workSheet,
                                        textStyle,
                                        twoDecimalNumberStyle,
                                        twoDecimalPositiveNumberStyle,
                                        twoDecimalPositivePercentNumberStyle,
                                        info,
                                        startColumn,
                                        maxColumnIndex,
                                        ref currentRow);
                                }
                                else
                                {
                                    infoRMList.Add(info);
                                }
                            }
                        }

                        if (ibcWinLoss.ChildsInfo.Count > 0)
                        {
                            info = ibcWinLoss.NormalTotal;
                            this.ExportMainDataRow(
                                ref workSheet,
                                subTotalTextStyle,
                                subTotalTwoDecimalNumberStyle,
                                subTotalTwoDecimalPositiveNumberStyle,
                                subTotalTwoDecimalPositivePercentNumberStyle,
                                info,
                                startColumn,
                                ref currentRow,
                                RowType._2hReportVirtualAllTotal,
                                report.EnableVirtual,
                                true,
                                i != 0);

                            info = ibcWinLoss.CreditTotal;
                            this.ExportMainDataRow(
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
                                true,
                                i != 0);

                            info = ibcWinLoss.DepositTotal;
                            this.ExportMainDataRow(
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
                                true,
                                i != 0);
                        }

                        if (ibcWinLoss.Name == Labels.Sportsbook &&
                        ibcWinLoss.ChildsInfo.Count > 0)
                        {
                            var infoRMLength = infoRMList.Count;
                            for (var k = 0; k < infoRMLength; k++)
                            {
                                info = infoRMList[k];
                                this.ExportRMRow(
                                    ref workSheet,
                                    subTotalTextStyle,
                                    subTotalTwoDecimalNumberStyle,
                                    subTotalTwoDecimalPositiveNumberStyle,
                                    subTotalTwoDecimalPositivePercentNumberStyle,
                                    info,
                                    startColumn,
                                    maxColumnIndex,
                                    ref currentRow);
                            }
                        }

                        workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                    }

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        false);

                    //// GrandTotal

                    // GrandTotalValue
                    var grandTotalSeparatorColor = ExcelExtensions.GrandTotalSeparatorColor;
                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.WinLoss;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.Position;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.Commission;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Variance;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualLineTotal;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.ExtraPositionTaking;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.AfterExtraPositionTaking;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.VirtualTurnover;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.Turnover;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;
                    currentRow++;

                    info = report.CreditTotal;
                    this.ExportMainDataRow(
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
                        true,
                        true);

                    info = report.DepositTotal;
                    this.ExportMainDataRow(
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
                        true,
                        true);

                    // ReportNote
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                    currentCellRange.Merged = true;
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.FillPattern.SetSolid(backgroundColor);
                    currentCellRange.Value = MainResources.DailyWinLoss2h_MainReportFooterNote;
                    currentColumn += 7;
                    currentRow += 2;

                    //// GrandTotal
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
        /// Assigns the name of the win loss information sport.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="sportName">Name of the sport.</param>
        private void AssignWinLossInfoSportName(WinLossInfo info, string sportName)
        {
            if (info.Id == SoccerBetTypeConstants.SoccerMajorBetType)
            {
                sportName = string.IsNullOrEmpty(sportName) ? Labels.SoccerMajorBettypes : sportName;
                info.Name = sportName;
            }

            if (info.Id == SoccerBetTypeConstants.SoccerMinorBetType)
            {
                sportName = string.IsNullOrEmpty(sportName) ? Labels.SoccerMinorBettypes : sportName;
                info.Name = sportName;
            }

            if (info.Id == SportContants._2hparlaySport)
            {
                sportName = string.IsNullOrEmpty(sportName) ? Labels.Parlay : sportName;
                info.Name = sportName;
            }
        }

        /// <summary>
        /// Exports the RM row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="info">The information.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="currentRow">The current row.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportRMRow(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            int maxColumnIndex,
            ref int currentRow)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
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
            currentCell.Value = info.Variance;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualLineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.NotAvailable;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.AfterExtraPositionTaking;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.VirtualTurnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Turnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.VirtualMargin, info.VirtualTurnover, ref currentCell);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            currentColumn++;
            currentRow++;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMainReportCasinoRow(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            DateTime winLossDate,
            int rowType)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;

            currentCell.Style.HorizontalAlignment = rowType == RowType.Normal ? HorizontalAlignmentStyle.Left : HorizontalAlignmentStyle.Right;

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
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.Variance;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.VirtualLineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.LineTotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.ExtraPositionTaking;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.AfterExtraPositionTaking;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.VirtualTurnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            bool isBeforeCasinoSingleWalletLaunchDate = DateTime.Compare(winLossDate, AppSettings.CasinoSingleWalletLaunchDate) < 0;

            if (isBeforeCasinoSingleWalletLaunchDate)
            {
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCell.Value = Labels.NotAvailable;
            }
            else
            {
                currentCell.Style = twoDecimalPositivePercentNumberStyle;
                _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.VirtualMargin, info.VirtualTurnover, ref currentCell);
            }

            currentColumn++;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;

            if (isBeforeCasinoSingleWalletLaunchDate)
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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMainDataRow(
            ref ExcelWorksheet workSheet,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            int startColumn,
            ref int currentRow,
            int rowType,
            bool isEnableVirtual,
            bool isShowVirtualTurnover = false,
            bool isHideMargin = false)
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

            if (isEnableVirtual)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentCell.Value = info.Variance;
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
            }

            if (isEnableVirtual)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalPositiveNumberStyle;
                currentCell.Value = info.VirtualLineTotal;

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
            }

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
            currentCell.Value = info.IsShowAvailableExtraPositionTaking ? info.ExtraPositionTaking : (object)Labels.NotAvailable;
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
            currentCell.Value = info.AfterExtraPositionTaking;
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

            if (isEnableVirtual)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNumberStyle;
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
                    case RowType._2hReportVirtualAllTotal:
                    case RowType._2hReportGrandTotal:
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        break;
                }

                currentColumn++;
            }

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

            if (isEnableVirtual)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalPositivePercentNumberStyle;

                if (!isHideMargin)
                {
                    _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.VirtualMargin, info.VirtualTurnover, ref currentCell);
                }

                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;

            if (!isHideMargin)
            {
                _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
            }

            currentColumn++;
            currentRow++;
        }
    }
}