namespace Fanex.AccountingReports.Misc.Services.Implement.DuplicateTickets
{
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.DuplicateTickets;
    using Fanex.AccountingReports.Misc.Models.DuplicateTickets;
    using Fanex.AccountingReports.Misc.Resources;
    using Logging;
    using Newtonsoft.Json;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class DuplicateTicketsService : ServiceBase
    {
        private const short TitleColorIndex = 51;

        private const short EvenRowColorIndex = 52;

        private readonly string _duplicateTicketsSessionKey = MiscConstants.DuplicateTicketsPrefixSessionKey +
                                             (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public DuplicateTicketsInfo GetDuplicateTicketByUserName(GetDuplicateTicketsCriteria criteria)
        {
            DuplicateTicketsInfo duplicateTicketsInfo = new DuplicateTicketsInfo();

            List<DuplicateTicketsByUserName> result = new List<DuplicateTicketsByUserName>();
            var reportData = this.GetDuplicateTicketData(criteria);
            var cashOutService = new DuplicateTicketsCashOutServiceImp();
            var duplicateTicketGroup = reportData.GroupBy(x => new
            {
                x.username,
                x.custid,
                x.typename,
                x.winlostdate,
                x.balance,
                x.stake,
                x.odds,
                x.bettype,
                x.refnoGroup,
                x.matchid,
                x.ip,
                x.WalletType,
                x.betteam,
                x.oddsid
            }).ToList();

            DuplicateTicketsUtility utility = new DuplicateTicketsUtility();

            foreach (var duplicateTicketByUserName in duplicateTicketGroup)
            {
                var mainTicket = utility.AssignMainTicketInfo(duplicateTicketByUserName);
                var subTickets = utility.AssignSubTicketInfo(duplicateTicketByUserName);
                var groupHavingMultipleTickets = subTickets.GroupBy(x => new { x.GroupNo }).Where(g => g.Count() > 1).ToList();
                foreach (var group in groupHavingMultipleTickets)
                {
                    var transIds = string.Join(",", group.Select(s => s.ReferenceNumber));
                    var isDuplicateCashOut = cashOutService.IsDuplicateTicketsCashOut(mainTicket.BetTypeId, transIds);

                    if (mainTicket.BetTypeId != 911 || isDuplicateCashOut)
                    {
                        mainTicket.TicketInfo.AddRange(group);
                    }
                }

                result.Add(mainTicket);
            }

            duplicateTicketsInfo.Rows = result;
            var numOfDuplicateTickets = result.Sum(x => x.TicketInfo.Count);
            duplicateTicketsInfo.HaveDuplicateTickets = numOfDuplicateTickets > 0;
            duplicateTicketsInfo.MD5Hash = utility.GetMD5Hash(JsonConvert.SerializeObject(result));


            if (result.Count > 0)
            {
                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                LogService.Info($"Misc / Duplicate Tickets by Username: {AppSession.CurrentUser.Username}" + Environment.NewLine + json);
            }

            return duplicateTicketsInfo;
        }

        public List<dynamic> GetDuplicateTicketData(GetDuplicateTicketsCriteria criteria)
        {
            return GetReportData<dynamic>(criteria).ToList();
        }

        #region Export Report.

        public void ExportDuplicateTicketReport(GetDuplicateTicketsCriteria criteria)
        {
            var report = this.GetDuplicateTicketByUserName(criteria);

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
            for (int i = 0; i < 14; i++)
            {
                worksheet.SetColumnWidth(i, 170 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(12, 170 * 5 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(0, 80 * NpoiExcelExtensions.FontWidth);
        }

        private void ExportReportTitle(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, GetDuplicateTicketsCriteria criteria)
        {
            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            string reportTitle = string.Format(
                MiscResources.DuplicateTickets_ReportTitle,
                criteria.Date.ToShortFormatedDate());

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 14, reportTitle, headerStyle);
        }

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
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Site, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, "Betfrom", headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, "TransDesc", headerStyle);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportContent(
            List<DuplicateTicketsByUserName> report,
            HSSFWorkbook excelFile,
            ISheet worksheet,
            ref int currentRow)
        {
            var index = 0;
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

            ICellStyle textRedStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center, IndexedColors.Red.Index);
            ICellStyle evenRowRedTextStyle = NpoiExcelExtensions.TextStyle(excelFile, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Center, IndexedColors.Red.Index);

            for (int i = 0; i < report.Count; i++)
            {
              
                var ticketByUser = report[i];
                if (ticketByUser.TicketInfo != null && ticketByUser.TicketInfo.Any())
                {
                    currentColumn = 0;
                    row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                    int numberDuplicateTickets = ticketByUser.TicketInfo.Count;
                    string refNo = ticketByUser.TicketInfo.Any() ? ticketByUser.TicketInfo.FirstOrDefault().ReferenceNumber.ToString() : string.Empty;
                    string duplicateTicketOfUser = string.Format(MiscResources.DuplicateTicket_OfUser, refNo);
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 14, duplicateTicketOfUser, userTitle);
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
                        index % 2 == 0 ? evenRowTextStyle : textStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow + numberDuplicateTickets - 1,
                        currentColumn,
                        ticketByUser.CustId,
                        index % 2 == 0 ? evenRowTextStyle : textStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow + numberDuplicateTickets - 1,
                        currentColumn,
                        ticketByUser.BetType,
                        index % 2 == 0 ? evenRowTextStyle : textStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow + numberDuplicateTickets - 1,
                        currentColumn,
                        ticketByUser.WinLostDate,
                        index % 2 == 0 ? evenRowTextStyle : textStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow + numberDuplicateTickets - 1,
                        currentColumn,
                        ticketByUser.BalanceBeforeBet,
                        index % 2 == 0 ? evenRowDecimalStyle : decimalStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow + numberDuplicateTickets - 1,
                        currentColumn,
                        ticketByUser.Stake,
                        index % 2 == 0 ? evenRowDecimalStyle : decimalStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow + numberDuplicateTickets - 1,
                        currentColumn,
                        ticketByUser.Odds,
                        index % 2 == 0 ? evenRowDecimalStyle : decimalStyle);

                    int subCount = 0;

                    foreach (var ticketInfo in ticketByUser.TicketInfo)
                    {
                        var isFistTicket = Math.Abs(ticketInfo.TimeDifference) <= 0 && subCount == 0;
                        currentColumn = 0;
                        NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow,
                            currentColumn,
                            rowIndex + 1,
                            index % 2 == 0 ? evenRowNumberStyle : numberStyle);
                        currentColumn = 8;
                        NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow,
                            currentColumn,
                            ticketInfo.Status,
                            index % 2 == 0 ? evenRowTextStyle : textStyle);

                        if(index % 2 == 0 && isFistTicket)
                        {
                            NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow,
                            currentColumn,
                            ticketInfo.ReferenceNumber,
                            evenRowTextStyle);
                        }
                        else if(index % 2 != 0 && isFistTicket)
                        {
                            NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow,
                            currentColumn,
                            ticketInfo.ReferenceNumber,
                            textStyle);
                        }
                        else if(index % 2 == 0 && !isFistTicket)
                        {
                            NpoiExcelExtensions.RenderCell(
                               ref worksheet,
                               ref row,
                               currentRow,
                               ref currentColumn,
                               currentRow,
                               currentColumn,
                               ticketInfo.ReferenceNumber,
                               evenRowRedTextStyle);
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
                              ticketInfo.ReferenceNumber,
                              textRedStyle);
                        }

                        NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow,
                            currentColumn,
                            ticketInfo.BetTime,
                            index % 2 == 0 ? evenRowTextStyle : textStyle);
                        if (isFistTicket)
                        {
                            NpoiExcelExtensions.RenderCell(
                                ref worksheet,
                                ref row,
                                currentRow,
                                ref currentColumn,
                                currentRow,
                                currentColumn,
                                "-",
                                index % 2 == 0 ? evenRowRightTextStyle : rightTextStyle);
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
                                index % 2 == 0 ? evenRowRightNumberStyle : rightNumberStyle);
                        }

                        NpoiExcelExtensions.RenderCell(
                           ref worksheet,
                           ref row,
                           currentRow,
                           ref currentColumn,
                           currentRow,
                           currentColumn,
                           ticketInfo.Site,
                           index % 2 == 0 ? evenRowTextStyle : textStyle);

                        NpoiExcelExtensions.RenderCell(
                           ref worksheet,
                           ref row,
                           currentRow,
                           ref currentColumn,
                           currentRow,
                           currentColumn,
                           ticketInfo.BetFrom,
                           index % 2 == 0 ? evenRowTextStyle : textStyle);

                        NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow,
                            currentColumn,
                            ticketInfo.TransDesc,
                            index % 2 == 0 ? evenRowTextStyle : textStyle);

                        currentRow++;
                        rowIndex++;
                        subCount++;
                    }

                    index++;
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
