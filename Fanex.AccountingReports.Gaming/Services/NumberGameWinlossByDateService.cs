namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByDate;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByDate;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;

    public class NumberGameWinlossByDateService : ServiceBase
    {
        public static void Round(ref NumberGameWinLossDetail winLoss)
        {
            if (winLoss != null)
            {
                winLoss.CustomerWinloss = Math.Round(winLoss.CustomerWinloss, 2);
                winLoss.AgencyWinloss = Math.Round(winLoss.AgencyWinloss, 2);
                winLoss.Comm = Math.Round(winLoss.Comm, 2);
                winLoss.LineTotal = Math.Round(winLoss.LineTotal, 2);
                winLoss.VirtualTotal = Math.Round(winLoss.VirtualTotal, 2);
                winLoss.Turnover = Math.Round(winLoss.Turnover, 2);
                winLoss.Variance = Math.Round(winLoss.Variance, 2);
            }
        }

        public List<EntityBase<int, string>> GetLicensee(GetNumberGameLicenseeCriteria criteria)
        {
            var result = new List<EntityBase<int, string>>();
            var rawData = GetReportData<dynamic>(criteria);
            if (rawData != null)
            {
                foreach (var row in rawData)
                {
                    result.Add(new EntityBase<int, string>()
                    {
                        Id = row.LicenseeID,
                        Name = row.Licenseename
                    });
                }
            }

            return result;
        }



        
        public NumberGameWinLossDetail GetNumberGameWinLossByDate(GetNumberGameWinLossByDateCriteria criteria)
        {
            var result = new NumberGameWinLossDetail
            {
                ChildInfo = new List<NumberGameWinLossDetail>(),
                GrandTotal = new NumberGameWinLossDetail()
            };

            var rawData = GetReportData<dynamic>(criteria);
            if (rawData != null)
            {
                rawData
                    .GroupBy(r => r.DATE)
                    .ToList()
                    .ForEach(group =>
                    {
                        var winLossByDate = new NumberGameWinLossDetail
                        {
                            WinLossDate = DateTimeExtensions.ToShortFormatedDate(Converter.ToDateTime(group.Key)),
                            ChildInfo = new List<NumberGameWinLossDetail>(),
                            GrandTotal = new NumberGameWinLossDetail()
                        };

                        group.GroupBy(l => l.SportType)
                             .ToList()
                             .ForEach(sport =>
                             {
                                 var winLossInfo = new NumberGameWinLossDetail
                                 {
                                     Status = CacheRef.GetSportName(sport.Key),
                                     ChildInfo = new List<NumberGameWinLossDetail>(),
                                     GrandTotal = new NumberGameWinLossDetail()
                                 };

                                 sport.ToList().ForEach(bettype =>
                                 {
                                     var detail = new NumberGameWinLossDetail
                                     {
                                         Bettype = CacheRef.GetBettype(bettype.bettype),
                                         CustomerWinloss = (decimal)bettype.winlost,
                                         AgencyWinloss = (decimal)bettype.position,
                                         Comm = (decimal)bettype.commission,
                                         LineTotal = (decimal)bettype.linetotal,
                                         VirtualTotal = (decimal)bettype.virtuallinetotal,
                                         Variance = (decimal)bettype.linetotal - (decimal)bettype.virtuallinetotal,
                                         Turnover = (decimal)bettype.turnover
                                     };

                                     detail.Margin = this.GetWinLossMargin(detail.Turnover, detail.LineTotal);
                                     winLossInfo.AddToGrandTotal(detail);
                                     Round(ref detail);
                                     winLossInfo.ChildInfo.Add(detail);
                                 });

                                 winLossInfo.GrandTotal.Margin = this.GetWinLossMargin(winLossInfo.GrandTotal.Turnover, winLossInfo.GrandTotal.LineTotal);
                                 winLossByDate.ChildInfo.Add(winLossInfo);
                                 winLossByDate.AddToGrandTotal(winLossInfo.GrandTotal);
                             });

                        winLossByDate.GrandTotal.Margin = this.GetWinLossMargin(winLossByDate.GrandTotal.Turnover, winLossByDate.GrandTotal.LineTotal);
                        result.ChildInfo.Add(winLossByDate);
                        result.AddToGrandTotal(winLossByDate.GrandTotal);
                    });

                result.GrandTotal.Margin = this.GetWinLossMargin(result.GrandTotal.Turnover, result.GrandTotal.LineTotal);
            }

            SessionManager.Set<NumberGameWinLossDetail>(GamingConstants.NumberGameWinLossByDateSessionKey, result);
            return result;
        }

        public List<EntityBase<int, string>> GetCurrencies()
        {
            var result = new List<EntityBase<int, string>>();
            Dictionary<int, string> currList = ReportService.ExchangeCurrencies;

            foreach (var row in currList)
            {
                if (ReportService.IsExcludeCurrency(row.Value))
                {
                    continue;
                }

                result.Add(new EntityBase<int, string>()
                {
                    Id = row.Key,
                    Name = row.Value
                });
            }

            return result;
        }

        public void ExportNumberGameByDate(GetNumberGameWinLossByDateCriteria criteria)
        {
            var fullReport = SessionManager.Get<NumberGameWinLossDetail>(GamingConstants.NumberGameWinLossByDateSessionKey);
            if (fullReport == null)
            {
                fullReport = this.GetNumberGameWinLossByDate(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.DirectBalance_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                criteria);

            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            string reportName = "NumberGameWinlossByDate";
            excelFile.ReponseStream(reportName);
        }

        private string GetWinLossMargin(decimal turnOver, decimal lineTotal)
        {
            if (turnOver == 0)
            {
                return Labels.NotAvailable;
            }

            return (lineTotal / turnOver).ToString();
        }

        
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetNumberGameWinLossByDateCriteria criteria)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            worksheet.Columns[startColumn].Width = 140 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 1].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 2].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 3].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 4].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 5].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 6].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 7].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 8].Width = 120 * ExcelExtensions.FontWidth;
            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentColumn = startColumn;
            var title = "Number Game Win Loss By Date - From " + criteria.FromDate + " To " + criteria.ToDate;
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 8);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;

            // First Row
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Date;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "Member Win Loss";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "SMA Win Loss";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Comm;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
            currentCellRange.Merged = true;
            currentCellRange.Value = "House";
            currentCellRange.Style.FormatTitleCell(headerColor);

            worksheet.Cells[currentRow + 1, currentColumn].Value = Labels.Turnover;
            worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);
            currentColumn++;

            worksheet.Cells[currentRow + 1, currentColumn].Value = "Real Income";
            worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);
            currentColumn++;

            worksheet.Cells[currentRow + 1, currentColumn].Value = "Virtual Win Loss";
            worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);
            currentColumn++;

            worksheet.Cells[currentRow + 1, currentColumn].Value = Labels.Variance;
            worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);
            currentColumn++;

            worksheet.Cells[currentRow + 1, currentColumn].Value = Labels.Margin;
            worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);

            currentRow += 2;
        }

        private void ExportReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberGameWinLossDetail report)
        {
            if (report != null &&
           report.ChildInfo != null &&
           report.ChildInfo.Count > 0)
            {
                var reportLength = report.ChildInfo.Count;

                this.ExportDirectBalanceDetail(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    report.ChildInfo);

                this.ExportDirectBalanceTotal(ref worksheet, startRow, startColumn, ref currentRow, report.GrandTotal);
            }
            else
            {
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 8);
            }
        }

        
        private void ExportDirectBalanceDetail(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            List<NumberGameWinLossDetail> winLossDateList)
        {
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var textParentStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.ParentColor, true);
            var textChildStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.ChildColor, true);

            var twoDecimalPositiveNumberParentStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.ParentColor,
                true,
                true,
                false);
            var twoDecimalNumberParentStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.ParentColor,
                true,
                false,
                false);
            var twoDecimalNumberMarginParentStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.ParentColor,
                true,
                true,
                false);

            var twoDecimalPositiveNumberChildStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.ChildColor,
                true,
                true,
                false);
            var twoDecimalNumberChildStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.ChildColor,
                true,
                false,
                false);
            var twoDecimalNumberMarginChildStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.ChildColor,
                true,
                true,
                false);

            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            var twoDecimalNumberPositiveStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                true,
                false);
            var twoDecimalNumberMarginStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.BackGroundColor,
                false,
                true,
                false);

            for (var k = 0; k < winLossDateList.Count; k++)
            {
                var data = winLossDateList[k];
                ExcelCell currCellRange;
                var currentColumn = startColumn;
                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = data.WinLossDate;
                currCellRange.Style = textParentStyle;
                currentColumn++;

                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = data.GrandTotal.CustomerWinloss;
                currCellRange.Style = twoDecimalPositiveNumberParentStyle;
                currentColumn++;

                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = data.GrandTotal.AgencyWinloss;
                currCellRange.Style = twoDecimalNumberParentStyle;
                currentColumn++;

                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = data.GrandTotal.Comm;
                currCellRange.Style = twoDecimalNumberParentStyle;
                currentColumn++;

                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = data.GrandTotal.Turnover;
                currCellRange.Style = twoDecimalNumberParentStyle;
                currentColumn++;

                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = data.GrandTotal.LineTotal;
                currCellRange.Style = twoDecimalPositiveNumberParentStyle;
                currentColumn++;

                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = data.GrandTotal.VirtualTotal;
                currCellRange.Style = twoDecimalPositiveNumberParentStyle;
                currentColumn++;

                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = data.GrandTotal.Variance;
                currCellRange.Style = twoDecimalPositiveNumberParentStyle;
                currentColumn++;

                currCellRange = worksheet.Cells[currentRow, currentColumn];
                currCellRange.Value = Converter.ToDecimal(data.GrandTotal.Margin);
                currCellRange.Style = twoDecimalNumberMarginParentStyle;
                currentColumn++;

                currentRow++;
                var statusLength = data.ChildInfo.Count;
                for (int i = statusLength - 1; i >= 0; i--)
                {
                    currentColumn = startColumn;
                    var statusData = data.ChildInfo[i];
                    ExcelCell statusCellRange;
                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = statusData.Status;
                    statusCellRange.Style = textChildStyle;
                    currentColumn++;

                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = statusData.GrandTotal.CustomerWinloss;
                    statusCellRange.Style = twoDecimalPositiveNumberChildStyle;
                    currentColumn++;

                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = statusData.GrandTotal.AgencyWinloss;
                    statusCellRange.Style = twoDecimalNumberChildStyle;
                    currentColumn++;

                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = statusData.GrandTotal.Comm;
                    statusCellRange.Style = twoDecimalNumberChildStyle;
                    currentColumn++;

                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = statusData.GrandTotal.Turnover;
                    statusCellRange.Style = twoDecimalNumberChildStyle;
                    currentColumn++;

                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = statusData.GrandTotal.LineTotal;
                    statusCellRange.Style = twoDecimalPositiveNumberChildStyle;
                    currentColumn++;

                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = statusData.GrandTotal.VirtualTotal;
                    statusCellRange.Style = twoDecimalPositiveNumberChildStyle;
                    currentColumn++;

                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = statusData.GrandTotal.Variance;
                    statusCellRange.Style = twoDecimalPositiveNumberChildStyle;
                    currentColumn++;

                    statusCellRange = worksheet.Cells[currentRow, currentColumn];
                    statusCellRange.Value = Converter.ToDecimal(statusData.GrandTotal.Margin);
                    statusCellRange.Style = twoDecimalNumberMarginChildStyle;
                    currentColumn++;

                    currentRow++;
                    var detailLength = statusData.ChildInfo.Count;
                    for (int j = 0; j < detailLength; j++)
                    {
                        currentColumn = startColumn;
                        var detailData = statusData.ChildInfo[j];
                        ExcelCell detailCellRange;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = detailData.Bettype;
                        detailCellRange.Style = textStyle;
                        currentColumn++;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = detailData.CustomerWinloss;
                        detailCellRange.Style = twoDecimalNumberPositiveStyle;
                        currentColumn++;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = detailData.AgencyWinloss;
                        detailCellRange.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = detailData.Comm;
                        detailCellRange.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = detailData.Turnover;
                        detailCellRange.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = detailData.LineTotal;
                        detailCellRange.Style = twoDecimalNumberPositiveStyle;
                        currentColumn++;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = detailData.VirtualTotal;
                        detailCellRange.Style = twoDecimalNumberPositiveStyle;
                        currentColumn++;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = detailData.Variance;
                        detailCellRange.Style = twoDecimalNumberPositiveStyle;
                        currentColumn++;

                        detailCellRange = worksheet.Cells[currentRow, currentColumn];
                        detailCellRange.Value = Converter.ToDecimal(detailData.Margin);
                        detailCellRange.Style = twoDecimalNumberMarginStyle;
                        currentColumn++;

                        currentRow++;
                    }
                }
            }
        }

        
        private void ExportDirectBalanceTotal(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberGameWinLossDetail grandTotal)
        {
            var textGrandTotalStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            var twoDecimalNumberMarginStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);

            var currentColumn = startColumn;
            ExcelCell grandCellRange;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = "Grand Total";
            grandCellRange.Style = textGrandTotalStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.CustomerWinloss;
            grandCellRange.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.AgencyWinloss;
            grandCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.Comm;
            grandCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.Turnover;
            grandCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.LineTotal;
            grandCellRange.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.VirtualTotal;
            grandCellRange.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.Variance;
            grandCellRange.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = Converter.ToDecimal(grandTotal.Margin);
            grandCellRange.Style = twoDecimalNumberMarginStyle;
            currentColumn++;
        }
    }
}
