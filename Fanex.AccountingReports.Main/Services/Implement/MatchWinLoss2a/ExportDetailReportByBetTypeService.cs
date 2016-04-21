// --------------------------------------------------------------------------
// <copyright file="ExportDetailReportByBetTypeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Export detail report by bet type.
    /// </summary>
    public partial class MatchWinLoss2aService : ServiceBase, IMatchWinLoss2aService
    {
        /// <summary>
        /// Exports the type of the detail report by bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportDetailReportByBetType(GetDetailReportByBetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_matchWinLoss2aDetailReportByBetTypeSessionKey);
                if (report == null)
                {
                    bool isBeforeBalanceUpDay = criteria.Winlostdate < ReportService.BalanceUp.BalanceUpDay ?
                    true :
                    false;

                    if (isBeforeBalanceUpDay)
                    {
                        report = this.GetDetailReportBeforeBalanceUpDayByBetType(criteria);
                    }
                    else
                    {
                        report = this.GetDetailReportByBetType(criteria);
                    }
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = MainResources.MatchWinLoss2a_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                var currentRow = 0;
                var reportLength = report.ChildsInfo.Count;

                this.ExportDetailReportByBetTypeHeader(
                    excelFile,
                    ref worksheet,
                    ref currentRow,
                    criteria);

                currentRow++;

                if (report != null && reportLength > 0)
                {
                    this.SetColorIndex(excelFile);
                    IDataFormat dataFormat = excelFile.CreateDataFormat();
                    ReportCellStyle reportCellStyle = new ReportCellStyle(ref excelFile);
                    ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
                    grandTotalTextStyle.BorderTop = BorderStyle.Medium;
                    grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
                    ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
                    grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
                    grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
                    grandTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);

                    for (var rowIndex = 0; rowIndex < reportLength; rowIndex++)
                    {
                        var winLossInfo = report.ChildsInfo[rowIndex];
                        this.ExportDetailReportRow_ByBetType(
                            ref worksheet,
                            ref currentRow,
                            winLossInfo,
                            reportCellStyle);
                        currentRow++;
                    }

                    this.ExportDetailReportTotal_ByBetType(
                        ref worksheet,
                        ref currentRow,
                        report.GrandTotal.Name,
                        report.GrandTotal,
                        grandTotalTextStyle,
                        grandTotalDecimalNumberStyle);
                }

                var reportName = string.Format(
                    MainResources.MatchWinLoss2aDetailByBetType_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                NpoiExcelExtensions.ResponseStreamExcel(excelFile, reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void ExportMetricGamingDetailReportByBetType(GetMetricGamingDetailByBetTypeCriteria criteria)
        {
            try
            {
                var getDetailReportByBetTypeCriteria = new GetDetailReportByBetTypeCriteria
                {
                    Bettype = criteria.Bettype,
                    BetTypeName = criteria.BetTypeName,
                    LiveIndicator = criteria.LiveIndicator,
                    Sporttype = criteria.Sporttype,
                    Winlostdate = criteria.Winlostdate
                };

                var report = SessionManager.Get<WinLossInfo>(_matchWinLoss2aMetricGamingDetailReportByBetTypeSessionKey);
                if (report == null)
                {
                    report = this.GetMetricGamingDetailByBetType(criteria);
                }

                getDetailReportByBetTypeCriteria.Sporttype = Labels.MetricGaming_Superlive.ToUpper();
                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = MainResources.MatchWinLoss2a_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                var currentRow = 0;
                var reportLength = report.ChildsInfo.Count;

                this.ExportDetailReportByBetTypeHeader(
                    excelFile,
                    ref worksheet,
                    ref currentRow,
                    getDetailReportByBetTypeCriteria);

                currentRow++;

                if (report != null && reportLength > 0)
                {
                    this.SetColorIndex(excelFile);
                    IDataFormat dataFormat = excelFile.CreateDataFormat();
                    ReportCellStyle reportCellStyle = new ReportCellStyle(ref excelFile);
                    ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
                    grandTotalTextStyle.BorderTop = BorderStyle.Medium;
                    grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
                    ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
                    grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
                    grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
                    grandTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);

                    for (var rowIndex = 0; rowIndex < reportLength; rowIndex++)
                    {
                        var winLossInfo = report.ChildsInfo[rowIndex];
                        this.ExportDetailReportRow_ByBetType(
                            ref worksheet,
                            ref currentRow,
                            winLossInfo,
                            reportCellStyle);
                        currentRow++;
                    }

                    this.ExportDetailReportTotal_ByBetType(
                        ref worksheet,
                        ref currentRow,
                        report.GrandTotal.Name,
                        report.GrandTotal,
                        grandTotalTextStyle,
                        grandTotalDecimalNumberStyle);
                }

                var reportName = string.Format(
                    MainResources.MatchWinLoss2aDetailByBetType_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                NpoiExcelExtensions.ResponseStreamExcel(excelFile, reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the detail report by bet type header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportDetailReportByBetTypeHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            GetDetailReportByBetTypeCriteria criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            string reportTitle = string.Format(
                MainResources.MatchWinLoss2a_DetailReportByBetType_Title,
                criteria.Sporttype,
                criteria.LiveIndicator ? Labels.Live + "-" : string.Empty,
                criteria.BetTypeName,
                criteria.Winlostdate.ToShortFormatedDate()).ToUpper();

            var currentColumn = 0;

            IRow row = worksheet.CreateRow(currentRow);
            for (var i = 0; i < 11; i++)
            {
                worksheet.SetColumnWidth(i, 140 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 200 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(1, 350 * ExcelExtensions.FontWidth);

            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 10, reportTitle, headerStyle);
            currentRow += 2;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 10));

            currentRow++;
            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;

            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Bet_Type, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Win_Loss, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Position, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Comm, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Virtual_Comm, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Line_Total, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Virtual_Line_Total, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Variance, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Turnover, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Margin_Percent, headerStyle);
        }

        /// <summary>
        /// Exports the type of the detail report row_ by bet.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="winLossInfo">The win loss information.</param>
        /// <param name="reportCellStyle">The report cell style.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportDetailReportRow_ByBetType(
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo winLossInfo,
            ReportCellStyle reportCellStyle)
        {
            var currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);

            string betType = winLossInfo.LiveIndicatorId ?
                string.Format("!{0} - {1} {2}", Labels.Live,  winLossInfo.Name, winLossInfo.MatchCode) :
                string.Format("{0} {1}", winLossInfo.Name, winLossInfo.MatchCode);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, betType, reportCellStyle.CenterTextStyle);
            string match = string.Format("{0}-vs-{1}", winLossInfo.HomeTeamName + (winLossInfo.IsNeutral ? " (N)" : string.Empty), winLossInfo.AwayTeamName);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, match, reportCellStyle.CenterTextStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.WinLoss_Str,
                winLossInfo.WinLoss,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Position_Str,
                winLossInfo.Position,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Commission_Str,
                winLossInfo.Commission,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualComm_Str,
                winLossInfo.VirtualComm,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.LineTotal_Str,
                winLossInfo.LineTotal,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualLineTotal_Str,
                winLossInfo.VirtualLineTotal,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Variance_Str,
                winLossInfo.Variance,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Turnover_Str,
                winLossInfo.Turnover,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Margin_Str,
                winLossInfo.Margin,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
        }

        /// <summary>
        /// Exports the type of the detail report total_ by bet.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="title">The title.</param>
        /// <param name="grandTotal">The grand total.</param>
        /// <param name="totalTextStyle">The total text style.</param>
        /// <param name="totalDecimalNumberStyle">The total decimal number style.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportDetailReportTotal_ByBetType(
            ref ISheet worksheet,
            ref int currentRow,
            string title,
            WinLossInfo grandTotal,
            ICellStyle totalTextStyle,
            ICellStyle totalDecimalNumberStyle)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, totalTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, totalTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, totalTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, totalTextStyle);
            var startMergeColumn = currentColumn - 4;
            this.RenderMergeCell(
                ref worksheet,
                ref row,
                currentRow,
                ref startMergeColumn,
                currentRow,
                currentColumn - 1,
                title,
                totalTextStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.Commission_Str,
                grandTotal.Commission,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.VirtualComm_Str,
                grandTotal.VirtualComm,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.LineTotal_Str,
                grandTotal.LineTotal,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.VirtualLineTotal_Str,
                grandTotal.VirtualLineTotal,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.Variance_Str,
                grandTotal.Variance,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.Turnover_Str,
                grandTotal.Turnover,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.Margin_Str,
                grandTotal.Margin,
                totalTextStyle,
                totalDecimalNumberStyle);
        }
    }
}