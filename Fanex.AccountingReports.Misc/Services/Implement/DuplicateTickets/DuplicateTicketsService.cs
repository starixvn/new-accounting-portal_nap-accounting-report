// -----------------------------------------------------------------------
// <copyright file="DuplicateTicketsService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Misc.Services.Implement.DuplicateTickets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.DuplicateTickets;
    using Fanex.AccountingReports.Misc.Models.DuplicateTickets;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.AccountingReports.Misc.Services.Interface.DuplicateTickets;
    using Newtonsoft.Json;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DuplicateTicketsService : ServiceBase, IDuplicateTicketsService
    {
        private const short TitleColorIndex = 51;

        private const short EvenRowColorIndex = 52;        

        private string _duplicateTicketsSessionKey = MiscConstants.DuplicateTicketsPrefixSessionKey + 
                                             (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the name of the duplicate ticket by user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List duplicate ticket by user.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public DuplicateTicketsInfo GetDuplicateTicketByUserName(GetDuplicateTicketsCriteria criteria)
        {
            DuplicateTicketsInfo duplicateTicketsInfo = new DuplicateTicketsInfo();

            List<DuplicateTicketsByUserName> result = new List<DuplicateTicketsByUserName>();
            var reportData = this.GetDuplicateTicketData(criteria);
            var duplicateTicketGroup = reportData.GroupBy(x => new
            {
                x.username,
                x.custid,
                x.typename,
                x.winlostdate,
                x.balance,
                x.stake,
                x.odds
            }).ToList();

            DuplicateTicketsUtility utility = new DuplicateTicketsUtility();

            foreach (var duplicateTicketByUserName in duplicateTicketGroup)
            {                
                var subTickets = utility.AssignSubTicketInfo(duplicateTicketByUserName);
                var groups = subTickets.GroupBy(x => new { x.GroupNo }).ToList();
                foreach (var group in groups)
                {
                    if (group.Count() == 1)
                    {
                        continue;
                    }
                    else
                    {
                        var mainTicket = utility.AssignMainTicketInfo(duplicateTicketByUserName);
                        foreach (var ticket in group)
                        {
                            mainTicket.TicketInfo.Add(ticket);
                        }

                        result.Add(mainTicket);
                    }
                }             
            }

            duplicateTicketsInfo.Rows = result;
            duplicateTicketsInfo.MD5Hash = utility.GetMD5Hash(JsonConvert.SerializeObject(result));
            SessionManager.Set<DuplicateTicketsInfo>(this._duplicateTicketsSessionKey, duplicateTicketsInfo);

            return duplicateTicketsInfo;
        }

        /// <summary>
        /// Gets the duplicate ticket data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List user ticket data.</returns>
        public List<dynamic> GetDuplicateTicketData(GetDuplicateTicketsCriteria criteria)
        {
            try
            {
                List<dynamic> report = new List<dynamic>();

                report = GetReportData<dynamic>(criteria).ToList();
                return report;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        #region Export Report.

        /// <summary>
        /// Exports the duplicate ticket report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportDuplicateTicketReport(GetDuplicateTicketsCriteria criteria)
        {
            var report = SessionManager.Get<DuplicateTicketsInfo>(this._duplicateTicketsSessionKey);

            if (report == null)
            {
                report = this.GetDuplicateTicketByUserName(criteria);
            }

            HSSFWorkbook excelFile = new HSSFWorkbook();
            string sheetName = MiscResources.DuplicateTickets_SheetName;
            ISheet worksheet = excelFile.CreateSheet(sheetName);
            this.SetColorAtIndex(excelFile);
            int currentRow = 0;
            this.InitColumn(ref worksheet);
            this.ExportReportTitle(excelFile, ref worksheet, ref currentRow, criteria);
            currentRow += 2;
            this.ExportReportHeader(excelFile, ref worksheet, ref currentRow);
            currentRow++;
            this.ExportReportContent(report.Rows, excelFile, worksheet, ref currentRow);

            string fileName = string.Format(
                    MiscResources.DuplicateTickets_ReportName,
                    criteria.Date.ToShortFormatedDateWithSlash().Replace("/", "_"));
            NpoiExcelExtensions.ResponseStreamExcel(excelFile, fileName);
        }

        private void InitColumn(ref ISheet worksheet)
        {
            for (int i = 0; i < 12; i++)
            {
                worksheet.SetColumnWidth(i, 170 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 80 * NpoiExcelExtensions.FontWidth);
        }

        /// <summary>
        /// Exports the report title.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportTitle(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, GetDuplicateTicketsCriteria criteria)
        {
            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            string reportTitle = string.Format(
                MiscResources.DuplicateTickets_ReportTitle,
                criteria.Date.ToShortFormatedDate());

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 11, reportTitle, headerStyle);
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        private void ExportReportHeader(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, "#", headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.UserName, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CustomerID, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Bet_Type, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.WinLostDate, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.BalanceBeforeBet, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Stake, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Odds, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Status, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Reference_Number, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.BetTime, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.TimeDifference, headerStyle);
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportContent(
            List<DuplicateTicketsByUserName> report,
            HSSFWorkbook excelFile,
            ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 0;
            int rowIndex = 0;
            IRow row = null;
            ICellStyle userTitle = NpoiExcelExtensions.TextStyle(excelFile, TitleColorIndex, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle rightTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle decimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle numberStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle rightNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle evenRowTextStyle = NpoiExcelExtensions.TextStyle(excelFile, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle evenRowRightTextStyle = NpoiExcelExtensions.TextStyle(excelFile, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle evenRowDecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, EvenRowColorIndex, false);
            ICellStyle evenRowNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle evenRowRightNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Right);

            for (int i = 0; i < report.Count; i++)
            {
                var ticketByUser = report[i];
                currentColumn = 0;
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                int numberDuplicateTickets = ticketByUser.TicketInfo.Count;
                string refNo = ticketByUser.TicketInfo.FirstOrDefault().ReferenceNumber.ToString();
                string duplicateTicketOfUser = string.Format(MiscResources.DuplicateTicket_OfUser, refNo);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 11, duplicateTicketOfUser, userTitle);
                currentRow++;
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                currentColumn = 1;
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow + numberDuplicateTickets - 1,
                    currentColumn,
                    ticketByUser.UserName,
                    i % 2 == 0 ? evenRowTextStyle : textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow + numberDuplicateTickets - 1,
                    currentColumn,
                    ticketByUser.CustId,
                    i % 2 == 0 ? evenRowTextStyle : textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow + numberDuplicateTickets - 1,
                    currentColumn,
                    ticketByUser.BetType,
                    i % 2 == 0 ? evenRowTextStyle : textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow + numberDuplicateTickets - 1,
                    currentColumn,
                    ticketByUser.WinLostDate,
                    i % 2 == 0 ? evenRowTextStyle : textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow + numberDuplicateTickets - 1,
                    currentColumn,
                    ticketByUser.BalanceBeforeBet,
                    i % 2 == 0 ? evenRowDecimalStyle : decimalStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow + numberDuplicateTickets - 1,
                    currentColumn,
                    ticketByUser.Stake,
                    i % 2 == 0 ? evenRowDecimalStyle : decimalStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow + numberDuplicateTickets - 1,
                    currentColumn,
                    ticketByUser.Odds,
                    i % 2 == 0 ? evenRowDecimalStyle : decimalStyle);

                int subCount = 0;
                foreach (var ticketInfo in ticketByUser.TicketInfo)
                {                    
                    currentColumn = 0;
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        rowIndex + 1,
                        i % 2 == 0 ? evenRowNumberStyle : numberStyle);
                    currentColumn = 8;
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        ticketInfo.Status,
                        i % 2 == 0 ? evenRowTextStyle : textStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        ticketInfo.ReferenceNumber,
                        i % 2 == 0 ? evenRowRightTextStyle : rightTextStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        ticketInfo.BetTime,
                        i % 2 == 0 ? evenRowTextStyle : textStyle);
                    if (ticketInfo.TimeDifference == 0 && subCount == 0)
                    {
                        NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow,
                            currentColumn,
                            "-",
                            i % 2 == 0 ? evenRowRightTextStyle : rightTextStyle);
                    }
                    else
                    {
                        NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow,
                            currentColumn,
                            ticketInfo.TimeDifference,
                            i % 2 == 0 ? evenRowRightNumberStyle : rightNumberStyle);
                    }

                    currentRow++;
                    rowIndex++;
                    subCount++;
                }
            }
        }

        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(TitleColorIndex, (byte)153, (byte)204, (byte)255);
            palette.SetColorAtIndex(EvenRowColorIndex, (byte)228, (byte)242, (byte)253);
        }

        #endregion Export Report.
    }
}