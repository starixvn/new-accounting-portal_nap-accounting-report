// --------------------------------------------------------------------------
// <copyright file="ExportAllDetailReportService.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Match win loss 2a service.
    /// </summary>
    public partial class MatchWinLoss2aService : ServiceBase, IMatchWinLoss2aService
    {
        /// <summary>
        /// Exports all detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportAllDetailReport(GetMainReportCriteria criteria)
        {
            try
            {
                bool isBeforeBalanceUpDay = criteria.Winlostdate < ReportService.BalanceUp.BalanceUpDay ?
                    true :
                    false;
                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = MainResources.MatchWinLoss2a_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                string reportTitle = string.Format(
                    MainResources.MatchWinLoss2aAll_ReportTitle,
                    criteria.Winlostdate.ToShortFormatedDate());
                var currentRow = 0;
                var currentColum = 0;
                ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
                ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);

                if (criteria.IsGetMainReport)
                {
                    var summaryReportByBetType = this.GetAllDetailReportByBetType(criteria, isBeforeBalanceUpDay);
                    this.SummaryReportByBetType_InitColumnSize(ref worksheet);
                    this.ExportSummaryReportByBetTypeTitle(
                        ref worksheet,
                        ref currentRow,
                        reportTitle,
                        headerStyle);
                    currentRow++;
                    this.ExportSummaryReportByBetTypeHeaderInfo(
                        ref worksheet,
                        ref currentRow,
                        headerStyle);
                    currentRow++;
                    this.ExportSummaryReportByBetTypeContent(
                        excelFile,
                        ref worksheet,
                        ref currentRow,
                        summaryReportByBetType);

                    currentRow++;
                    IRow row = worksheet.CreateRow(currentRow);
                    this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColum, currentRow, currentColum + 11, null, textStyle);
                }

                if (criteria.IsGetDetailReport)
                {
                    currentRow++;

                    var getAllDetailReportBySportType = new GetAllDetailReportBySportsCriteria
                    {
                        Winlostdate = criteria.Winlostdate
                    };

                    WinLossInfo allDetailReport = isBeforeBalanceUpDay ?
                    this.GetAllDetailReportBeforeBalanceUpDay(getAllDetailReportBySportType) :
                    this.GetAllDetailReport(criteria);

                    this.ExportAllDetailReportBySport(
                        criteria,
                        excelFile,
                        ref worksheet,
                        ref currentRow,
                        allDetailReport);
                }

                var reportName = string.Format(
                    MainResources.MatchWinLoss2aAll_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                NpoiExcelExtensions.ResponseStreamExcel(excelFile, reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Gets the type of all detail report by bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isBeforeBalanceUpDay">If set to <c>true</c> [is before balance up day].</param>
        /// <returns>Summary report by bet type.</returns>
        private SummaryReportByBetType GetAllDetailReportByBetType(GetMainReportCriteria criteria, bool isBeforeBalanceUpDay)
        {
            var summaryReportByBetType = SessionManager.Get<SummaryReportByBetType>(_matchWinLoss2aReportByBetTypeSessionKey);
            var getSummaryReportByBetTypeCriteria = new GetWinLossByBetTypeCriteria
            {
                Winlostdate = criteria.Winlostdate
            };

            if (summaryReportByBetType == null)
            {
                if (!isBeforeBalanceUpDay)
                {
                    this.GetMainReport(criteria);
                }
                else
                {
                    this.GetMainReportByBetType(getSummaryReportByBetTypeCriteria);
                }

                summaryReportByBetType = SessionManager.Get<SummaryReportByBetType>(_matchWinLoss2aReportByBetTypeSessionKey);
            }

            return summaryReportByBetType;
        }

        /// <summary>
        /// Exports the row_ all detail report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="decimalNumberStyle">The decimal number style.</param>
        /// <param name="reportRowType">Type of the report row.</param>
        /// <param name="title">The title.</param>
        /// <param name="winLossInfo">The win loss information.</param>
        private void ExportRow_AllDetailReport(
            ref ISheet worksheet,
            ref int currentRow,
            ICellStyle textStyle,
            ICellStyle decimalNumberStyle,
            ReportRowType reportRowType,
            string title,
            WinLossInfo winLossInfo)
        {
            switch (reportRowType)
            {
                case ReportRowType.BetTypeTotal:
                case ReportRowType.SubTotal:
                case ReportRowType.Total:
                case ReportRowType.GrandTotal:
                    this.ExportAllDetailReportTotalRow(
                        ref worksheet,
                        ref currentRow,
                        textStyle,
                        decimalNumberStyle,
                        title,
                        winLossInfo);
                    break;

                case ReportRowType.EightRM:
                case ReportRowType.BARM:
                    this.ExportAllDetailReportRMRow(
                        ref worksheet,
                        ref currentRow,
                        textStyle,
                        decimalNumberStyle,
                        title,
                        winLossInfo);
                    break;

                default:
                    this.ExportAllDetailReportRow(
                        ref worksheet,
                        ref currentRow,
                        textStyle,
                        decimalNumberStyle,
                        title,
                        winLossInfo);
                    break;
            }
        }

        /// <summary>
        /// Exports all detail report total row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="decimalNumberStyle">The decimal number style.</param>
        /// <param name="title">The title.</param>
        /// <param name="winLossInfo">The win loss information.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportAllDetailReportTotalRow(
            ref ISheet worksheet,
            ref int currentRow,
            ICellStyle textStyle,
            ICellStyle decimalNumberStyle,
            string title,
             WinLossInfo winLossInfo)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            var startMergeColumn = currentColumn - 7;
            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref startMergeColumn, currentRow, currentColumn - 1, title, textStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Commission_Str,
                winLossInfo.Commission,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualComm_Str,
                winLossInfo.VirtualComm,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.LineTotal_Str,
                winLossInfo.LineTotal,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualLineTotal_Str,
                winLossInfo.VirtualLineTotal,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Variance_Str,
                winLossInfo.Variance,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Turnover_Str,
                winLossInfo.Turnover,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Margin_Str,
                winLossInfo.Margin,
                textStyle,
                decimalNumberStyle);
        }

        /// <summary>
        /// Exports all detail report ringgit malaysia row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="decimalNumberStyle">The decimal number style.</param>
        /// <param name="title">The title.</param>
        /// <param name="winLossInfo">The win loss information.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportAllDetailReportRMRow(
            ref ISheet worksheet,
            ref int currentRow,
            ICellStyle textStyle,
            ICellStyle decimalNumberStyle,
            string title,
             WinLossInfo winLossInfo)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            var startMergeColumn = currentColumn - 9;
            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref startMergeColumn, currentRow, currentColumn - 1, title, textStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.LineTotal_Str,
                winLossInfo.LineTotal,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualLineTotal_Str,
                winLossInfo.VirtualLineTotal,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Variance_Str,
                winLossInfo.Variance,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Turnover_Str,
                winLossInfo.Turnover,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Margin_Str,
                winLossInfo.Margin,
                textStyle,
                decimalNumberStyle);
        }

        /// <summary>
        /// Exports all detail report row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="decimalNumberStyle">The decimal number style.</param>
        /// <param name="title">The title.</param>
        /// <param name="winLossInfo">The win loss information.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportAllDetailReportRow(
            ref ISheet worksheet,
            ref int currentRow,
            ICellStyle textStyle,
            ICellStyle decimalNumberStyle,
            string title,
             WinLossInfo winLossInfo)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, winLossInfo.EventDate, textStyle);
            string matchInfo = string.Format("{0}-vs-{1}", winLossInfo.HomeTeamName + (winLossInfo.IsNeutral ? " (N)" : string.Empty), winLossInfo.AwayTeamName);
            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 2, matchInfo, textStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.WinLoss_Str,
                winLossInfo.WinLoss,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Position_Str,
                winLossInfo.Position,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Commission_Str,
                winLossInfo.Commission,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualComm_Str,
                winLossInfo.VirtualComm,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.LineTotal_Str,
                winLossInfo.LineTotal,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualLineTotal_Str,
                winLossInfo.VirtualLineTotal,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Variance_Str,
                winLossInfo.Variance,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Turnover_Str,
                winLossInfo.Turnover,
                textStyle,
                decimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Margin_Str,
                winLossInfo.Margin,
                textStyle,
                decimalNumberStyle);
        }
    }
}