// --------------------------------------------------------------------------
// <copyright file="TraderPerformanceOtherSportsService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.01.06">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.TraderPerformanceOtherSports
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceOtherSports;
    using Fanex.AccountingReports.Main.Repositories.Implement;
    using Fanex.AccountingReports.Main.Repositories.Interface;
    using Fanex.AccountingReports.Main.Services.Interface;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// Trader performance service.
    /// </summary>
    public partial class TraderPerformanceOtherSportsService : ServiceBase, ITraderPerformanceOtherSportsService
    {
        private const short SubTotalColorIndex = 29;

        private const short GrandTotalColorIndex = 32;

        private const short GrandTotalBorderColorIndex = 55;

        private const short WinLossGroupColorIndex = 41;

        private const short TurnOverGroupColorIndex = 47;

        private const short LiveBetCountGroupColorIndex = 51;

        private const short MarginGroupColorIndex = 58;

        private const short CommissionGroupColorIndex = 34;

        private const short OddColorIndex = 18;

        private const short EvenColorIndex = 37;

        private const string NegativeNumberFormat = "#,##0.00;[Red](-#,##0.00)";

        private const string PositiveNumberFormat = "[Red]#,##0.00;-#,##0.00";

        private const string NormalNumberFormat = "#,##0.00;-#,##0.00";

        private const string NegativePercentFormat = "#,##0.00%;[Red](-#,##0.00%)";

        private const string PositivePercentFormat = "[Red]#,##0.00%;-#,##0.00%";

        private const string NormalPercentFormat = "#,##0.00%;-#,##0.00%";

        private const string IntNumberFormat = "#,###";

        private const string SoccerTitle = "ForSoccers";

        private const string OtherSportTitle = "ForOtherSports";

        private readonly ITraderPerformanceOtherSportsRepository _traderPerformanceOtherSportsRepository;

        private string _traderPerformanceSessionKey = "Fanex.AccountingReports.Main.TraderPerformance." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _traderPerformanceDetailSessionKey = "Fanex.AccountingReports.Main.TraderPerformanceDetail." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public TraderPerformanceOtherSportsService()
            : this(null)
        {
        }

        public TraderPerformanceOtherSportsService(ITraderPerformanceOtherSportsRepository traderPerformanceRepository)
        {
            _traderPerformanceOtherSportsRepository = _traderPerformanceOtherSportsRepository ?? new TraderPerformanceOtherSportsRepository();
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Trader performance data.</returns>
        public TraderPerformanceOtherSports GetReport(GetReportCriteria criteria)
        {
            var result = new TraderPerformanceOtherSports();
            var traderSportType = criteria.IsSoccer ? SoccerTitle : OtherSportTitle;

            try
            {
                result = this._traderPerformanceOtherSportsRepository.GetTraderPerformanceOtherSportsReport(criteria);
                SessionManager.Set<TraderPerformanceOtherSports>(_traderPerformanceSessionKey + traderSportType, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<TraderPerformanceOtherSports>(_traderPerformanceSessionKey + traderSportType, null);
                throw;
            }
        }

        /// <summary>
        /// Sorts the report data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Report data after sort.</returns>
        public TraderPerformanceOtherSports SortReportData(GetReportCriteria criteria)
        {
            var traderSportType = criteria.IsSoccer ? SoccerTitle : OtherSportTitle;

            var report = SessionManager.Get<TraderPerformanceOtherSports>(_traderPerformanceSessionKey + traderSportType);

            if (report == null)
            {
                report = this.GetReport(criteria);
            }

            this.SortReportData(report, criteria);

            return report;
        }

        /// <summary>
        /// Gets the detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Trader performance detail data.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public TraderPerformanceOtherSportsDetail GetDetailReport(GetDetailReportCriteria criteria)
        {
            var result = new TraderPerformanceOtherSportsDetail
            {
                GrandTotal = new TraderPerformanceOtherSportsDetail()
            };

            var traderSportType = criteria.IsSoccer ? SoccerTitle : OtherSportTitle;
            var startIndex = criteria.StartIndex;

            try
            {
                if (criteria.IsPagingCallBack)
                {
                    result = SessionManager.Get<TraderPerformanceOtherSportsDetail>(_traderPerformanceDetailSessionKey + traderSportType);
                }
                else
                {
                    var reportDetailData = this._traderPerformanceOtherSportsRepository.GetTraderPerformanceOtherSportsDetailReport(criteria);

                    var traderGroups = reportDetailData.ReportRows.GroupBy(data => new
                        {
                            data.Id,
                            data.Name
                        });

                    foreach (var traderGroup in traderGroups)
                    {
                        var winLossTotal = new TraderPerformanceOtherSportsDetail();
                        var turnOverTotal = new TraderPerformanceOtherSportsDetail();
                        var liveBetCountTotal = new TraderPerformanceOtherSportsDetail();
                        var marginTotal = new TraderPerformanceOtherSportsDetail();
                        var commissionTotal = new TraderPerformanceOtherSportsDetail();

                        var traderGroupInfo = new TraderPerformanceOtherSportsDetail
                        {
                            Id = traderGroup.Key.Id,
                            Name = traderGroup.Key.Name,
                            GrandTotal = new TraderPerformanceOtherSportsDetail()
                        };

                        foreach (var data in traderGroup)
                        {
                            var traderInfo = new TraderPerformanceOtherSportsDetail
                            {
                                EventDate = data.EventDate,
                                LeagueName = data.LeagueName,
                                HomeName = data.HomeName,
                                AwayName = data.AwayName,
                                HTHomeScore = data.HTHomeScore,
                                HTAwayScore = data.HTAwayScore,
                                FTHomeScore = data.FTHomeScore,
                                FTAwayScore = data.FTAwayScore,
                                Maxbet = data.Maxbet,
                                ChildInfo = data.ChildInfo
                            };

                            winLossTotal.CalculateTotal(traderInfo.ChildInfo[0]);
                            turnOverTotal.CalculateTotal(traderInfo.ChildInfo[1]);
                            liveBetCountTotal.CalculateTotal(traderInfo.ChildInfo[2]);
                            commissionTotal.CalculateTotal(traderInfo.ChildInfo[4]);

                            traderInfo.ChildInfo[0].FormatTwoDecimalNumberData(traderInfo.ChildInfo[0]);
                            traderInfo.ChildInfo[1].FormatTwoDecimalNumberData(traderInfo.ChildInfo[1]);
                            traderInfo.ChildInfo[3].FormatTwoDecimalNumberData(traderInfo.ChildInfo[3]);
                            traderInfo.ChildInfo[4].FormatTwoDecimalNumberData(traderInfo.ChildInfo[4]);

                            traderGroupInfo.ReportRows.Add(traderInfo);
                        }

                        marginTotal.CalculateMarginTotal(winLossTotal, turnOverTotal);

                        winLossTotal.FormatTwoDecimalNumberData(winLossTotal);
                        turnOverTotal.FormatTwoDecimalNumberData(turnOverTotal);
                        marginTotal.FormatTwoDecimalNumberData(marginTotal);
                        commissionTotal.FormatTwoDecimalNumberData(commissionTotal);

                        traderGroupInfo.GrandTotal.ChildInfo.Add(winLossTotal);
                        traderGroupInfo.GrandTotal.ChildInfo.Add(turnOverTotal);
                        traderGroupInfo.GrandTotal.ChildInfo.Add(liveBetCountTotal);
                        traderGroupInfo.GrandTotal.ChildInfo.Add(marginTotal);
                        traderGroupInfo.GrandTotal.ChildInfo.Add(commissionTotal);

                        result.ReportRows.Add(traderGroupInfo);
                    }

                    result.GrandTotal = reportDetailData.GrandTotal;
                    result.ReportLength = result.ReportRows.Count;

                    SessionManager.Set<TraderPerformanceOtherSportsDetail>(_traderPerformanceDetailSessionKey + traderSportType, result);
                }

                this.SortSecondSubLevelReport(result, criteria);

                var reportPagingData = new TraderPerformanceOtherSportsDetail
                {
                    ReportRows = result.ReportRows
                        .Skip(startIndex)
                        .Take(criteria.PageSize)
                        .ToList(),
                    GrandTotal = result.GrandTotal,
                    ReportLength = result.ReportLength
                };

                return reportPagingData;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<TraderPerformanceOtherSportsDetail>(_traderPerformanceDetailSessionKey + traderSportType, null);
                throw;
            }
        }

        /// <summary>
        /// Sorts the detail report data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Detail report data after sort.</returns>
        public TraderPerformanceOtherSportsDetail SortDetailReportData(GetDetailReportCriteria criteria)
        {
            var traderSportType = criteria.IsSoccer ? SoccerTitle : OtherSportTitle;
            var report = SessionManager.Get<TraderPerformanceOtherSportsDetail>(_traderPerformanceDetailSessionKey + traderSportType);

            if (report == null)
            {
                report = this.GetDetailReport(criteria);
            }

            var pagingReport = new TraderPerformanceOtherSportsDetail
            {
                ReportRows = report.ReportRows
                .Skip(criteria.StartIndex)
                .Take(criteria.PageSize)
                .ToList(),
                GrandTotal = report.GrandTotal,
                ReportLength = report.ReportLength
            };

            this.SortDetailReportData(pagingReport, criteria);

            return pagingReport;
        }

        /// <summary>
        /// Renders the merge cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="endColumn">The end column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void RenderMergeCell(
            ref ISheet worksheet,
            ref IRow row,
            int currentRow,
            ref int currentColumn,
            int endRow,
            int endColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(currentColumn);

            if (cellValue is string)
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else if (cellValue is int)
            {
                cell.SetCellValue(Convert.ToInt32(cellValue));
            }

            cell.CellStyle = cellStyle;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, endRow, currentColumn, endColumn));
            currentColumn = endColumn + 1;
        }

        /// <summary>
        /// Renders the cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of report.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void RenderCell(
            ref ISheet worksheet,
            ref IRow row,
            int currentRow,
            ref int currentColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(currentColumn);

            if (cellValue is string)
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else if (cellValue is int)
            {
                cell.SetCellValue(Convert.ToInt32(cellValue));
            }

            cell.CellStyle = cellStyle;
            currentColumn += 1;
        }

        /// <summary>
        /// Exports the cell value.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void ExportCellValue(
           ref ISheet worksheet,
           ref IRow row,
           ref int currentRow,
           ref int currentColumn,
           decimal cellValue,
           ICellStyle cellStyle)
        {
            if (cellValue == 0)
            {
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, null, cellStyle);
            }
            else
            {
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, cellValue, cellStyle);
            }
        }

        /// <summary>
        /// Creates the decimal number style.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="backgroundColorIndex">Index of the background color.</param>
        /// <param name="textAlign">The text align.</param>
        /// <param name="isBold">If set to <c>true</c> [is bold].</param>
        /// <param name="isNegative">If set to <c>true</c> [is negative].</param>
        /// <param name="isPositive">If set to <c>true</c> [is positive].</param>
        /// <returns>Decimal number style.</returns>
        private ICellStyle CreateDecimalNumberStyle(
            HSSFWorkbook excelFile,
            short backgroundColorIndex,
            HorizontalAlignment textAlign,
            bool isBold,
            bool isNegative,
            bool isPositive)
        {
            var decimalNumberStyle = excelFile.CreateCellStyle();
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            IFont font = excelFile.CreateFont();
            font.FontHeightInPoints = MainConstants.FontSize;
            font.FontName = MainConstants.FontName;

            if (isBold)
            {
                font.Boldweight = (short)FontBoldWeight.Bold;
            }

            if (isNegative)
            {
                decimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            }
            else if (isPositive)
            {
                decimalNumberStyle.DataFormat = dataFormat.GetFormat(PositiveNumberFormat);
            }
            else
            {
                decimalNumberStyle.DataFormat = dataFormat.GetFormat(NormalNumberFormat);
            }

            if (backgroundColorIndex != HSSFColor.White.Index)
            {
                decimalNumberStyle.FillForegroundColor = backgroundColorIndex;
            }
            else
            {
                decimalNumberStyle.FillForegroundColor = HSSFColor.White.Index;
            }

            decimalNumberStyle.FillPattern = FillPattern.SolidForeground;
            decimalNumberStyle.Alignment = textAlign;
            decimalNumberStyle.VerticalAlignment = VerticalAlignment.Center;
            decimalNumberStyle.SetFont(font);

            SetBorderToCell(decimalNumberStyle);

            return decimalNumberStyle;
        }

        /// <summary>
        /// Creates the decimal percent style.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="backgroundColorIndex">Index of the background color.</param>
        /// <param name="textAlign">The text align.</param>
        /// <param name="isBold">If set to <c>true</c> [is bold].</param>
        /// <param name="isNegative">If set to <c>true</c> [is negative].</param>
        /// <param name="isPositive">If set to <c>true</c> [is positive].</param>
        /// <returns>Decimal percent style.</returns>
        private ICellStyle CreateDecimalPercentStyle(
           HSSFWorkbook excelFile,
           short backgroundColorIndex,
           HorizontalAlignment textAlign,
           bool isBold,
           bool isNegative,
           bool isPositive)
        {
            var decimalPercentStyle = excelFile.CreateCellStyle();
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            IFont font = excelFile.CreateFont();
            font.FontHeightInPoints = MainConstants.FontSize;
            font.FontName = MainConstants.FontName;

            if (isBold)
            {
                font.Boldweight = (short)FontBoldWeight.Bold;
            }

            if (isNegative)
            {
                decimalPercentStyle.DataFormat = dataFormat.GetFormat(NegativePercentFormat);
            }
            else if (isPositive)
            {
                decimalPercentStyle.DataFormat = dataFormat.GetFormat(PositivePercentFormat);
            }
            else
            {
                decimalPercentStyle.DataFormat = dataFormat.GetFormat(NormalPercentFormat);
            }

            if (backgroundColorIndex != HSSFColor.White.Index)
            {
                decimalPercentStyle.FillForegroundColor = backgroundColorIndex;
            }
            else
            {
                decimalPercentStyle.FillForegroundColor = HSSFColor.White.Index;
            }

            decimalPercentStyle.FillPattern = FillPattern.SolidForeground;
            decimalPercentStyle.Alignment = textAlign;
            decimalPercentStyle.VerticalAlignment = VerticalAlignment.Center;
            decimalPercentStyle.SetFont(font);

            SetBorderToCell(decimalPercentStyle);

            return decimalPercentStyle;
        }

        /// <summary>
        /// Sets the index of the color.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void SetColorIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(WinLossGroupColorIndex, (byte)240, (byte)245, (byte)255);
            palette.SetColorAtIndex(TurnOverGroupColorIndex, (byte)233, (byte)250, (byte)239);
            palette.SetColorAtIndex(LiveBetCountGroupColorIndex, (byte)248, (byte)248, (byte)210);
            palette.SetColorAtIndex(MarginGroupColorIndex, (byte)254, (byte)241, (byte)236);
            palette.SetColorAtIndex(CommissionGroupColorIndex, (byte)246, (byte)246, (byte)239);
            palette.SetColorAtIndex(EvenColorIndex, (byte)228, (byte)242, (byte)253);
            palette.SetColorAtIndex(SubTotalColorIndex, (byte)244, (byte)244, (byte)190);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        /// <summary>
        /// Sets the border to cell.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <returns>The cell style to set border.</returns>
        private ICellStyle SetBorderToCell(ICellStyle cellStyle)
        {
            cellStyle.TopBorderColor = HSSFColor.Grey80Percent.Index;
            cellStyle.BottomBorderColor = HSSFColor.Grey80Percent.Index;
            cellStyle.LeftBorderColor = HSSFColor.Grey80Percent.Index;
            cellStyle.RightBorderColor = HSSFColor.Grey80Percent.Index;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;

            return cellStyle;
        }

        /// <summary>
        /// Sorts the report data.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Report data after sort.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private TraderPerformanceOtherSports SortReportData(TraderPerformanceOtherSports report, GetReportCriteria criteria)
        {
            bool isAsc = string.Equals(criteria.SortDirection, CoreConstants.SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);

            switch (criteria.SortColumnName)
            {
                case "Trader":
                    report.ReportRows = isAsc ?
                                        report.ReportRows
                                        .OrderBy(traderInfo => traderInfo.Name)
                                        .ToList() :
                                        report.ReportRows
                                        .OrderByDescending(traderInfo => traderInfo.Name)
                                        .ToList();
                    break;

                case "WinLossOverall":
                    report.ReportRows = isAsc ?
                                        report.ReportRows
                                        .OrderBy(traderInfo => traderInfo.ChildInfo[0].Overall)
                                        .ToList() :
                                        report.ReportRows
                                        .OrderByDescending(traderInfo => traderInfo.ChildInfo[0].Overall)
                                        .ToList();
                    break;

                case "TurnOverOverall":
                    report.ReportRows = isAsc ?
                                        report.ReportRows
                                        .OrderBy(traderInfo => traderInfo.ChildInfo[1].Overall)
                                        .ToList() :
                                        report.ReportRows
                                        .OrderByDescending(traderInfo => traderInfo.ChildInfo[1].Overall)
                                        .ToList();
                    break;

                case "LiveBetCountOverall":
                    report.ReportRows = isAsc ?
                                        report.ReportRows
                                        .OrderBy(traderInfo => traderInfo.ChildInfo[2].Overall)
                                        .ToList() :
                                        report.ReportRows
                                        .OrderByDescending(traderInfo => traderInfo.ChildInfo[2].Overall)
                                        .ToList();
                    break;

                case "MarginOverall":
                    report.ReportRows = isAsc ?
                                        report.ReportRows
                                        .OrderBy(traderInfo => traderInfo.ChildInfo[3].Overall)
                                        .ToList() :
                                        report.ReportRows
                                        .OrderByDescending(traderInfo => traderInfo.ChildInfo[3].Overall)
                                        .ToList();
                    break;

                case "CommOverall":
                    report.ReportRows = isAsc ?
                                        report.ReportRows
                                        .OrderBy(traderInfo => traderInfo.ChildInfo[4].Overall)
                                        .ToList() :
                                        report.ReportRows
                                        .OrderByDescending(traderInfo => traderInfo.ChildInfo[4].Overall)
                                        .ToList();
                    break;

                default:
                    break;
            }

            return report;
        }

        private TraderPerformanceOtherSportsDetail SortDetailReportData(TraderPerformanceOtherSportsDetail report, GetDetailReportCriteria criteria)
        {
            SortSecondSubLevelReport(report, criteria);
            return report;
        }

        /// <summary>
        /// Sorts the second sub level report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Second sub level data of report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private TraderPerformanceOtherSportsDetail SortSecondSubLevelReport(TraderPerformanceOtherSportsDetail report, GetDetailReportCriteria criteria)
        {
            bool isAsc = string.Equals(criteria.SortDirection, CoreConstants.SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);

            foreach (var traderInfo in report.ReportRows)
            {
                switch (criteria.SortColumnName)
                {
                    case "WinLossOverall":
                        traderInfo.ReportRows = isAsc ?
                                            traderInfo.ReportRows
                                            .OrderBy(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[0].Overall)
                                            .ToList() :
                                            traderInfo.ReportRows
                                            .OrderByDescending(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[0].Overall)
                                            .ToList();
                        break;

                    case "TurnOverOverall":
                        traderInfo.ReportRows = isAsc ?
                                            traderInfo.ReportRows
                                            .OrderBy(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[1].Overall)
                                            .ToList() :
                                            traderInfo.ReportRows
                                            .OrderByDescending(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[1].Overall)
                                            .ToList();
                        break;

                    case "LiveBetCountOverall":
                        traderInfo.ReportRows = isAsc ?
                                            traderInfo.ReportRows
                                            .OrderBy(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[2].Overall)
                                            .ToList() :
                                            traderInfo.ReportRows
                                            .OrderByDescending(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[2].Overall)
                                            .ToList();
                        break;

                    case "MarginOverall":
                        traderInfo.ReportRows = isAsc ?
                                            traderInfo.ReportRows
                                            .OrderBy(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[3].Overall)
                                            .ToList() :
                                            traderInfo.ReportRows
                                            .OrderByDescending(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[3].Overall)
                                            .ToList();
                        break;

                    case "CommOverall":
                        traderInfo.ReportRows = isAsc ?
                                            traderInfo.ReportRows
                                            .OrderBy(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[4].Overall)
                                            .ToList() :
                                            traderInfo.ReportRows
                                            .OrderByDescending(traderChildGroupInfo => traderChildGroupInfo.ChildInfo[4].Overall)
                                            .ToList();
                        break;

                    default:
                        break;
                }
            }

            return report;
        }
    }
}
