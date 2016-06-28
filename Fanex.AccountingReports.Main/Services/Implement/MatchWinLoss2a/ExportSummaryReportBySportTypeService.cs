// --------------------------------------------------------------------------
// <copyright file="ExportSummaryReportBySportTypeService.cs" company="Nexcel Solutions Vietnam">
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
    /// Export summary report by sport type.
    /// </summary>
    public partial class MatchWinLoss2aService : ServiceBase, IMatchWinLoss2aService
    {
        /// <summary>
        /// Exports the type of the summary report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportSummaryReportBySportType(GetMainReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_matchWinLoss2aReportBySportTypeSessionKey);
                if (report == null)
                {
                    bool isBeforeBalanceUpDay = criteria.Winlostdate < ReportService.BalanceUp.BalanceUpDay ?
                    true :
                    false;
                    if (isBeforeBalanceUpDay)
                    {
                        this.GetMainReportBySportType(criteria);
                    }
                    else
                    {
                        this.GetMainReport(criteria);
                    }

                    report = SessionManager.Get<WinLossInfo>(_matchWinLoss2aReportBySportTypeSessionKey);
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = MainResources.MatchWinLoss2a_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                int currentRow = 0;

                this.ExportSummaryReportBySportTypeHeader(excelFile, ref worksheet, ref currentRow, criteria);
                currentRow += 1;
                this.ExportSummaryReportBySportTypeContent(excelFile, ref worksheet, ref currentRow, report);

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.MatchWinLoss2aBySportType_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                NpoiExcelExtensions.ResponseStreamExcel(excelFile, reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the summary report by sport type header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportSummaryReportBySportTypeHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            GetMainReportCriteria criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            string reportTitle = string.Format(
                MainResources.MatchWinLoss2aBySportType_ReportTitle,
                criteria.Winlostdate.ToShortFormatedDate());
            var currentColumn = 0;

            IRow row = worksheet.CreateRow(currentRow);

            for (var i = 0; i < 8; i++)
            {
                worksheet.SetColumnWidth(i, 140 * ExcelExtensions.FontWidth);
            }

            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 7, reportTitle, headerStyle);
            currentRow += 2;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 7));

            currentRow++;
            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;

            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Bet_Type, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Comm, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Virtual_Comm, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Line_Total, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Virtual_Line_Total, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Variance, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Turnover, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Margin_Percent, headerStyle);
        }

        /// <summary>
        /// Exports the content of the summary report by sport type.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportSummaryReportBySportTypeContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo report)
        {
            this.SetColorIndex(excelFile);

            if (report != null &&
              report.NonThirdPartyInfo != null)
            {
                this.ExportNonThirdPartyBySportType(
                    excelFile,
                    ref worksheet,
                    ref currentRow,
                    report);

                currentRow++;
            }

            if (report != null &&
                report.ThirdPartyInfo != null)
            {
                this.ExportThirdPartyBySportType(
                    excelFile,
                    ref worksheet,
                    ref currentRow,
                    report);
            }

            this.ExportGrandTotalReportBySportType(
                excelFile,
                ref worksheet,
                ref currentRow,
                report);
        }

        /// <summary>
        /// Exports the type of the third party by sport.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportThirdPartyBySportType(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo report)
        {
            ReportCellStyle reportCellStyle = new ReportCellStyle(ref excelFile);

            var thirdPartyInfo = report.ThirdPartyInfo;
            var thirdPartyInfoLength = report.ThirdPartyInfo.ChildsInfo.Count;

            for (var thirdPartyInfoRowIndex = 0; thirdPartyInfoRowIndex < thirdPartyInfoLength; thirdPartyInfoRowIndex++)
            {
                var thirdPartySportInfo = thirdPartyInfo.ChildsInfo[thirdPartyInfoRowIndex];
                this.ExportRow_SummaryReportBySportType(
                    ref worksheet,
                    ref currentRow,
                    thirdPartySportInfo.Name,
                    thirdPartySportInfo,
                    reportCellStyle,
                    ReportRowType.ThirdParty,
                    ReportGroupType.ThirdPartyGroup);

                currentRow++;
            }
        }

        /// <summary>
        /// Exports the type of the non third party by sport.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNonThirdPartyBySportType(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo report)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ReportCellStyle reportCellStyle = new ReportCellStyle(ref excelFile);
            ICellStyle subTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            ICellStyle subTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, SubTotalColorIndex, true);
            subTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);
              
            var nonThirdPartyInfo = report.NonThirdPartyInfo;
            var nonThirdPartyInfoLength = report.NonThirdPartyInfo.ChildsInfo.Count;

            for (var nonThirdPartyInfoRowIndex = 0; nonThirdPartyInfoRowIndex < nonThirdPartyInfoLength; nonThirdPartyInfoRowIndex++)
            {
                var sportInfo = nonThirdPartyInfo.ChildsInfo[nonThirdPartyInfoRowIndex];
                var sportInfoLength = sportInfo.ChildsInfo.Count;

                for (var sportInfoRowIndex = 0; sportInfoRowIndex < sportInfoLength; sportInfoRowIndex++)
                {
                    var sportInfoDetail = sportInfo.ChildsInfo[sportInfoRowIndex];
                    string betTypeTitle = (sportInfoDetail.LiveIndicatorId ? "!" + Labels.Live + " - " : string.Empty) + sportInfoDetail.Name;
                    this.ExportRow_SummaryReportBySportType(
                        ref worksheet,
                        ref currentRow,
                        betTypeTitle,
                        sportInfoDetail,
                        reportCellStyle,
                        ReportRowType.Normal,
                        ReportGroupType.NoneThirdPartyGroup);

                    currentRow++;
                }

                this.ExportRowTotal_SummaryReportBySportType(
                    ref worksheet,
                    ref currentRow,
                    sportInfo.Name,
                    sportInfo.GrandTotal,
                    subTotalTextStyle,
                    subTotalDecimalNumberStyle,
                    ReportRowType.SubTotal);

                currentRow++;
            }

            this.ExportRowTotal_SummaryReportBySportType(
                ref worksheet,
                ref currentRow,
                nonThirdPartyInfo.GrandTotal.Name,
                nonThirdPartyInfo.GrandTotal,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle,
                ReportRowType.GrandTotal);

            currentRow++;

            this.ExportRowTotal_SummaryReportBySportType(
                ref worksheet,
                ref currentRow,
                report.EightRMTotal.Name,
                report.EightRMTotal,
                subTotalTextStyle,
                subTotalDecimalNumberStyle,
                ReportRowType.EightRM);

            currentRow++;

            this.ExportRowTotal_SummaryReportBySportType(
                ref worksheet,
                ref currentRow,
                report.BARMTotal.Name,
                report.BARMTotal,
                subTotalTextStyle,
                subTotalDecimalNumberStyle,
                ReportRowType.BARM);
        }

        /// <summary>
        /// Exports the type of the grand total report by sport.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportGrandTotalReportBySportType(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo report)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
            grandTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);

            this.ExportRowTotal_SummaryReportBySportType(
                ref worksheet,
                ref currentRow,
                report.GrandTotal.Name,
                report.GrandTotal,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle,
                ReportRowType.GrandTotal);
        }

        /// <summary>
        /// Exports the type of the row_ summary report by sport.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="betTypeTitle">The bet type title.</param>
        /// <param name="winLossInfo">The win loss information.</param>
        /// <param name="reportCellStyle">The report cell style.</param>
        /// <param name="reportRowType">The report row type.</param>
        /// <param name="reportGroupType">Type of the report group.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportRow_SummaryReportBySportType(
            ref ISheet worksheet,
            ref int currentRow,
            string betTypeTitle,
            WinLossInfo winLossInfo,
            ReportCellStyle reportCellStyle,
            ReportRowType reportRowType,
            ReportGroupType reportGroupType)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            this.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                betTypeTitle,
                reportGroupType == ReportGroupType.NoneThirdPartyGroup ? reportCellStyle.TextStyle : reportCellStyle.CenterTextStyle);
            if (reportRowType != ReportRowType.EightRM && reportRowType != ReportRowType.BARM)
            {
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
            }
            else
            {
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, winLossInfo.Name, reportCellStyle.TextStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, winLossInfo.Name, reportCellStyle.TextStyle);
                var startMergeColumn = currentColumn - 3;
                this.RenderMergeCell(ref worksheet, ref row, currentRow, ref startMergeColumn, currentRow, currentColumn, winLossInfo.Name, reportCellStyle.TextStyle);
            }

            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.LineTotal_Str,
                winLossInfo.LineTotal,
                reportCellStyle.BoldTextStyle,
                reportCellStyle.BoldDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualLineTotal_Str,
                winLossInfo.VirtualLineTotal,
                reportCellStyle.BoldTextStyle,
                reportCellStyle.BoldDecimalNumberStyle);
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
        /// Exports the type of the row total_ summary report by sport.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="betTypeTitle">The bet type title.</param>
        /// <param name="winLossInfo">The win loss information.</param>
        /// <param name="totalTextStyle">The total text style.</param>
        /// <param name="totalDecimalNumberStyle">The total decimal number style.</param>
        /// <param name="reportRowType">Type of the report row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportRowTotal_SummaryReportBySportType(
           ref ISheet worksheet,
           ref int currentRow,
           string betTypeTitle,
           WinLossInfo winLossInfo,
           ICellStyle totalTextStyle,
           ICellStyle totalDecimalNumberStyle,
           ReportRowType reportRowType)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, betTypeTitle, totalTextStyle);
            if (reportRowType != ReportRowType.EightRM && reportRowType != ReportRowType.BARM)
            {
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    winLossInfo.Commission_Str,
                    winLossInfo.Commission,
                    totalTextStyle,
                    totalDecimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    winLossInfo.VirtualComm_Str,
                    winLossInfo.VirtualComm,
                    totalTextStyle,
                    totalDecimalNumberStyle);
            }
            else
            {
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, winLossInfo.Name, totalTextStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, winLossInfo.Name, totalTextStyle);
                var startMergeColumn = currentColumn - 3;
                this.RenderMergeCell(ref worksheet, ref row, currentRow, ref startMergeColumn, currentRow, currentColumn - 1, winLossInfo.Name, totalTextStyle);
            }

            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.LineTotal_Str,
                winLossInfo.LineTotal,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualLineTotal_Str,
                winLossInfo.VirtualLineTotal,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Variance_Str,
                winLossInfo.Variance,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Turnover_Str,
                winLossInfo.Turnover,
                totalTextStyle,
                totalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Margin_Str,
                winLossInfo.Margin,
                totalTextStyle,
                totalDecimalNumberStyle);
        }
    }
}