namespace Fanex.AccountingReports.Gaming.Services.Implement.NumberGameWinLossByBetTypeMatch
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Repositories.Implement.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;

    public class WinLossByMatchService : ServiceBase
    {
        private readonly WinLossByMatchRepository _winLossByMatchRepository;

        private readonly string _winLossByMatchMainReportSessionKey =
            "Fanex.AccountingReports.Gaming.NumberGameWinLoss.WinLossByMatch.MainReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        private readonly string _winLossByMatchDetailReportSessionKey =
            "Fanex.AccountingReports.Gaming.NumberGameWinLoss.WinLossByMatch.DetailReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

        public WinLossByMatchService()
        {
            _winLossByMatchRepository = new WinLossByMatchRepository();
        }

        
        public WinLossInfo GetMainReport(GetWinLossByMatchMainReportCriteria criteria)
        {
            var rawData = _winLossByMatchRepository.GetMainReport(criteria);
            WinLossInfo report = new WinLossInfo
            {
                DetailInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.Total
                }
            };

            if (rawData == null || rawData.Count == 0)
            {
                return report;
            }

            var matchGroups = rawData.GroupBy(data => new
            {
                MatchId = data.MatchId,
                MatchCode = data.MatchCode
            });

            foreach (var matchGroup in matchGroups)
            {
                WinLossInfo matchWinLoss = new WinLossInfo
                {
                    Id = matchGroup.Key.MatchId,
                    Name = matchGroup.Key.MatchCode,
                    DetailInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo
                    {
                        Name = matchGroup.Key.MatchCode,
                    }
                };

                var sportGroups = matchGroup.GroupBy(data => data.SportType);
                foreach (var sportGroup in sportGroups)
                {
                    WinLossInfo sportWinLoss = new WinLossInfo
                    {
                        Id = sportGroup.Key,
                        DetailInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo()
                    };

                    sportWinLoss.Name = CacheRef.GetSportName(sportWinLoss.Id);
                    sportWinLoss.GrandTotal.Name = sportWinLoss.Name;
                    foreach (var data in sportGroup)
                    {
                        WinLossInfo info = new WinLossInfo
                        {
                            Id = data.BetType,
                            CustomerWinLoss = data.CustomerWinLoss,
                            AgencyWinLoss = data.AgencyWinLoss,
                            Commission = data.Commission,
                            LineTotal = data.LineTotal,
                            VirtualLineTotal = data.VirtualLineTotal,
                            Turnover = data.Turnover
                        };

                        info.Name = CacheRef.GetBettype(info.Id);
                        sportWinLoss.GrandTotal.Sum(info);
                        info = WinLossInfo.Format(info);
                        sportWinLoss.DetailInfo.Add(info);
                    }

                    matchWinLoss.GrandTotal.Sum(sportWinLoss.GrandTotal);
                    sportWinLoss.GrandTotal = WinLossInfo.Format(sportWinLoss.GrandTotal);
                    matchWinLoss.DetailInfo.Add(sportWinLoss);
                }

                matchWinLoss.DetailInfo = matchWinLoss.DetailInfo.OrderByDescending(detailInfo => detailInfo.Id).ToList();
                report.GrandTotal.Sum(matchWinLoss.GrandTotal);
                matchWinLoss.GrandTotal = WinLossInfo.Format(matchWinLoss.GrandTotal);
                report.DetailInfo.Add(matchWinLoss);
            }

            SessionManager.Set<WinLossInfo>(_winLossByMatchMainReportSessionKey, report);
            return report;
        }

        
        public WinLossInfo GetDetailReport(GetWinLossByMatchDetailReportCriteria criteria)
        {
            var rawData = GetReportData<WinLossByMatchDetailEntity>(criteria);
            WinLossInfo report = new WinLossInfo
            {
                Name = string.Format(
                    GamingResources.NumberGameWinLossByBetTypeMatch_ByMatchDetailReportTitle,
                    criteria.MatchCode,
                    CacheRef.GetBettype(criteria.BetTypeId),
                    criteria.ReportDate.ToShortFormatedDate()),
                DetailInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.Total,
                    DetailInfo = new List<WinLossInfo>()
                }
            };

            if (rawData == null || !rawData.Any())
            {
                return report;
            }

            Dictionary<int, int> roleIndex = new Dictionary<int, int>();
            roleIndex.Add(0, RoleConstants.Company);
            report.GrandTotal.DetailInfo.Add(new WinLossInfo());
            roleIndex.Add(1, RoleConstants.Member);
            report.GrandTotal.DetailInfo.Add(new WinLossInfo());
            roleIndex.Add(2, RoleConstants.Agent);
            report.GrandTotal.DetailInfo.Add(new WinLossInfo());
            roleIndex.Add(3, RoleConstants.Master);
            report.GrandTotal.DetailInfo.Add(new WinLossInfo());
            roleIndex.Add(4, RoleConstants.Super);
            report.GrandTotal.DetailInfo.Add(new WinLossInfo());

            for (var i = 0; i < report.GrandTotal.DetailInfo.Count; i++)
            {
                report.GrandTotal.DetailInfo[i].RoleLevelId = roleIndex[i];
            }

            foreach (var data in rawData)
            {
                WinLossInfo reportDetail = new WinLossInfo
                {
                    Id = data.CustomerId,
                    Name = data.UserName,
                    DetailInfo = Enumerable.Repeat(new WinLossInfo(), 5).ToList()
                };

                var winLossList = new List<WinLossInfo>
                    {
                        new WinLossInfo
                        {
                            RoleLevelId = RoleConstants.Company,
                            Commission = Converter.ToDecimal(data.GrossCommission),
                            Turnover = Converter.ToDecimal(data.CompanyTurnover),
                            LineTotal = Converter.ToDecimal(data.CompanyTotal)
                        },
                        new WinLossInfo
                        {
                            RoleLevelId = RoleConstants.Member,
                            WinLoss = Converter.ToDecimal(data.MemberWinLoss),
                            Commission = Converter.ToDecimal(data.MemberCommission),
                            LineTotal = Converter.ToDecimal(data.MemberTotal)
                        },
                        new WinLossInfo
                        {
                            RoleLevelId = RoleConstants.Agent,
                            WinLoss = Converter.ToDecimal(data.AgentWinLoss),
                            Commission = Converter.ToDecimal(data.AgentCommission),
                            LineTotal = Converter.ToDecimal(data.AgentTotal)
                        },
                        new WinLossInfo
                        {
                            RoleLevelId = RoleConstants.Master,
                            WinLoss = Converter.ToDecimal(data.MasterWinLoss),
                            Commission = Converter.ToDecimal(data.MasterCommission),
                            LineTotal = Converter.ToDecimal(data.MasterTotal)
                        },
                        new WinLossInfo
                        {
                            RoleLevelId = RoleConstants.Super,
                            WinLoss = Converter.ToDecimal(data.SuperWinLoss),
                            Commission = Converter.ToDecimal(data.SuperCommission),
                            LineTotal = Converter.ToDecimal(data.SuperTotal)
                        }
                    };

                for (var i = 0;
                    i < winLossList.Count && i < reportDetail.DetailInfo.Count;
                    i++)
                {
                    var winLossInfo = winLossList[i];
                    report.GrandTotal.DetailInfo[i].Sum(winLossInfo);
                    winLossInfo = WinLossInfo.Format(winLossInfo);
                    reportDetail.DetailInfo[i] = winLossInfo;
                }

                report.DetailInfo.Add(reportDetail);
            }

            for (var i = 0; i < report.GrandTotal.DetailInfo.Count; i++)
            {
                report.GrandTotal.DetailInfo[i] = WinLossInfo.Format(report.GrandTotal.DetailInfo[i]);
            }

            SessionManager.Set<WinLossInfo>(_winLossByMatchDetailReportSessionKey, report);
            return report;
        }

        public void ExportMainReport(GetWinLossByMatchMainReportCriteria criteria)
        {
            WinLossInfo report = SessionManager.Get<WinLossInfo>(_winLossByMatchMainReportSessionKey);
            if (report == null)
            {
                report = GetMainReport(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.NumberGameWinLossByBetTypeMatch_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 8;
            var currentRow = startRow;

            this.ExportReport_WinLossByMatchMainReportHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                criteria);
            this.ExportReport_WinLossByMatchMainReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                criteria,
                report);

            var reportName = string.Format(
                GamingResources.NumberGameWinLossByBetTypeMatch_ReportName,
                Converter.ToDateTime(criteria.ReportDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        
        public void ExportDetailReport(GetWinLossByMatchDetailReportCriteria criteria)
        {
            WinLossInfo report = SessionManager.Get<WinLossInfo>(_winLossByMatchDetailReportSessionKey);
            if (report == null)
            {
                report = GetDetailReport(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.NumberGameWinLossByBetTypeMatch_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 15;
            var currentRow = startRow;
            this.ExportReport_WinLossByMatchDetailReportHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                report != null ? report.Name : string.Empty);

            this.ExportReport_WinLossByMatchDetailReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                criteria,
                report);

            var reportName = string.Format(
                GamingResources.NumberGameWinLossByBetTypeMatch_ReportName,
                Converter.ToDateTime(criteria.ReportDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        
        private void ExportReport_WinLossByMatchMainReportHeader(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            GetWinLossByMatchMainReportCriteria criteria)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);

            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 3].Width = 120 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 7].Width = 120 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 8].Width = 120 * ExcelExtensions.FontWidth;

            var reportTitle = string.Format(
                GamingResources.NumberGameWinLossByBetTypeMatch_ByBetTypeMainReportTitle,
                criteria.ReportDate.ToShortFormatedDate()).ToUpper();
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = reportTitle;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Match;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "Member Win Loss";
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "SMA Win Loss";
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Commission;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
            currentCellRange.Merged = true;
            currentCellRange.Value = "House";
            currentCellRange.Style = headerTextStyle;
            currentColumn += 5;
            currentRow++;

            currentColumn = startColumn + 4;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = "Real Income";
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = "Virtual Win Loss";
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Variance;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Margin;
            currentColumn++;
            currentRow++;
        }

        
        private void ExportReport_WinLossByMatchMainReportContent(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            GetWinLossByMatchMainReportCriteria criteria,
            WinLossInfo report)
        {
            if (report == null ||
                report.DetailInfo == null ||
                report.DetailInfo.Count == 0)
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                return;
            }

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                true,
                false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var matchTotalRowColor = ExcelExtensions.SubTotalRowColor;
            var matchTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(matchTotalRowColor, true);
            var matchTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                matchTotalRowColor,
                true,
                false,
                false);
            var matchTotalRowTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                matchTotalRowColor,
                true,
                true,
                false);
            var matchTotalRowTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                matchTotalRowColor,
                true,
                true,
                false);

            var liveIndicatorTotalRowColor = ExcelExtensions.EvenRowColor;
            var liveIndicatorTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(liveIndicatorTotalRowColor, true);
            var liveIndicatorTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                liveIndicatorTotalRowColor,
                true,
                false,
                false);
            var liveIndicatorTotalRowTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                liveIndicatorTotalRowColor,
                true,
                true,
                false);
            var liveIndicatorTotalRowTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                liveIndicatorTotalRowColor,
                true,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotal1stRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            grandTotal1stRowTextStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            grandTotal1stRowTwoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotal1stRowTwoDecimalPositiveNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotal1stRowTwoDecimalPositivePercentNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            WinLossInfo info;
            var matchGroupsLength = report.DetailInfo.Count;
            for (var i = 0; i < matchGroupsLength; i++)
            {
                var matchGroup = report.DetailInfo[i];
                info = matchGroup.GrandTotal;
                ExportReport_WinLossByMatchMainReport_WinLossRow(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    maxColumnIndex,
                    matchTotalRowTextStyle,
                    matchTotalRowTwoDecimalNumberStyle,
                    matchTotalRowTwoDecimalPositiveNumberStyle,
                    matchTotalRowTwoDecimalPositivePercentNumberStyle,
                    info,
                    GroupTypeConstants.Match);

                var liveIndicatorGroupsLength = matchGroup.DetailInfo.Count;
                for (var j = 0; j < liveIndicatorGroupsLength; j++)
                {
                    var liveIndicatorGroup = matchGroup.DetailInfo[j];
                    info = liveIndicatorGroup.GrandTotal;
                    ExportReport_WinLossByMatchMainReport_WinLossRow(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        maxColumnIndex,
                        liveIndicatorTotalRowTextStyle,
                        liveIndicatorTotalRowTwoDecimalNumberStyle,
                        liveIndicatorTotalRowTwoDecimalPositiveNumberStyle,
                        liveIndicatorTotalRowTwoDecimalPositivePercentNumberStyle,
                        info,
                        GroupTypeConstants.LiveIndicator);

                    var betTypesLength = liveIndicatorGroup.DetailInfo.Count;
                    for (var k = 0; k < betTypesLength; k++)
                    {
                        info = liveIndicatorGroup.DetailInfo[k];
                        ExportReport_WinLossByMatchMainReport_WinLossRow(
                            ref workSheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            maxColumnIndex,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            GroupTypeConstants.Normal);
                    }
                }
            }

            info = report.GrandTotal;
            ExportReport_WinLossByMatchMainReport_GrandTotalRow(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                grandTotal1stRowTextStyle,
                grandTotal1stRowTwoDecimalNumberStyle,
                grandTotal1stRowTwoDecimalPositiveNumberStyle,
                grandTotal1stRowTwoDecimalPositivePercentNumberStyle,
                info);
        }

        
        private void ExportReport_WinLossByMatchMainReport_WinLossRow(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            GroupTypeConstants groupType)
        {
            var currentColumn = startColumn;
            ExcelCell currentCell;

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = textStyle;
            switch (groupType)
            {
                case GroupTypeConstants.Match:
                    currentCell.Value = string.Join(
                        string.Empty,
                          Enumerable.Repeat(" ", 2)) + info.Name;
                    break;

                case GroupTypeConstants.LiveIndicator:
                    currentCell.Value = string.Join(
                        string.Empty,
                          Enumerable.Repeat(" ", 4)) + info.Name;
                    break;

                case GroupTypeConstants.Normal:
                    currentCell.Value = string.Join(
                        string.Empty,
                          Enumerable.Repeat(" ", 6)) + info.Name;
                    break;

                default:
                    currentCell.Value = info.Name;
                    break;
            }

            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.CustomerWinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.AgencyWinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Commission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.LineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.VirtualLineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.Variance;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            NumberGameWinLossByBetTypeMatchService.ExportColumnMargin(
                info.Turnover,
                info.Margin,
                ref currentCell);
            currentColumn++;
            currentRow++;
        }

        
        private void ExportReport_WinLossByMatchMainReport_GrandTotalRow(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info)
        {
            var currentColumn = startColumn;
            ExcelCell currentCell;

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCell.Value = info.Name;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.CustomerWinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.AgencyWinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Commission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.LineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.VirtualLineTotal;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = info.Variance;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositivePercentNumberStyle;
            NumberGameWinLossByBetTypeMatchService.ExportColumnMargin(
                info.Turnover,
                info.Margin,
                ref currentCell);
            currentColumn++;
            currentRow++;
        }

        
        private void ExportReport_WinLossByMatchDetailReportHeader(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            string reportName)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);

            workSheet.Columns[startColumn].Width = 100 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = reportName;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Value = Labels.Symbol_No;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.User_Name;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "House Turnover";
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Gross_Comm;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = "Member";
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Agent;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Master;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Super;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;
            currentRow++;

            currentColumn = startColumn + 4;

            for (var i = 0; i < 4; i++)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Win_Loss;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Commission;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Total;
                currentColumn++;
            }

            currentRow++;
        }

        
        private void ExportReport_WinLossByMatchDetailReportContent(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            GetWinLossByMatchDetailReportCriteria criteria,
            WinLossInfo report)
        {
            if (report == null ||
                report.DetailInfo == null ||
                report.DetailInfo.Count == 0)
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                return;
            }

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                true,
                false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotal1stRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            grandTotal1stRowTextStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            grandTotal1stRowTwoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotal1stRowTwoDecimalPositiveNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var grandTotal1stRowTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            grandTotal1stRowTwoDecimalPositivePercentNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var currentColumn = startColumn;
            ExcelCell currentCell;

            WinLossInfo info;
            var reportDetailLength = report.DetailInfo.Count;
            Dictionary<int, int> roleIndex = new Dictionary<int, int>();
            roleIndex.Add(RoleConstants.Company, 0);
            roleIndex.Add(RoleConstants.Member, 1);
            roleIndex.Add(RoleConstants.Agent, 2);
            roleIndex.Add(RoleConstants.Master, 3);
            roleIndex.Add(RoleConstants.Super, 4);

            for (var i = 0; i < reportDetailLength; i++)
            {
                info = report.DetailInfo[i];
                NumberGameWinLossByBetTypeMatchService.SetOddEvenColor_ExportExcel(
                    ref textStyle,
                    ref twoDecimalNumberStyle,
                    ref twoDecimalPositiveNumberStyle,
                    ref twoDecimalPositivePercentNumberStyle,
                    (i + 1) % 2 == 0);

                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCell.Value = i + 1;
                currentColumn++;

                ExportReport_WinLossByMatchDetailReport_WinLossRow(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    maxColumnIndex,
                    textStyle,
                    twoDecimalNumberStyle,
                    twoDecimalPositiveNumberStyle,
                    twoDecimalPositivePercentNumberStyle,
                    info,
                    RowTypeConstants.Normal,
                    roleIndex);
            }

            info = report.GrandTotal;

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = grandTotal1stRowTextStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;
            ExportReport_WinLossByMatchDetailReport_WinLossRow(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                maxColumnIndex,
                grandTotal1stRowTextStyle,
                grandTotal1stRowTwoDecimalNumberStyle,
                grandTotal1stRowTwoDecimalPositiveNumberStyle,
                grandTotal1stRowTwoDecimalPositivePercentNumberStyle,
                info,
                RowTypeConstants.GrandTotal,
                roleIndex);
        }

        
        private void ExportReport_WinLossByMatchDetailReport_WinLossRow(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            CellStyle textStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle twoDecimalPositiveNumberStyle,
            CellStyle twoDecimalPositivePercentNumberStyle,
            WinLossInfo info,
            RowTypeConstants rowType,
            Dictionary<int, int> roleIndex)
        {
            var currentColumn = startColumn;
            ExcelCell currentCell;
            currentColumn = startColumn + 1;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCell.Value = info.Name;
            currentColumn++;

            var winLoss = info.DetailInfo[roleIndex[RoleConstants.Company]];
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = winLoss.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.Commission;
            currentColumn++;

            winLoss = info.DetailInfo[roleIndex[RoleConstants.Member]];
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.WinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.Commission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.LineTotal;
            currentColumn++;

            winLoss = info.DetailInfo[roleIndex[RoleConstants.Agent]];
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.WinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.Commission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.LineTotal;
            currentColumn++;

            winLoss = info.DetailInfo[roleIndex[RoleConstants.Master]];
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.WinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.Commission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.LineTotal;
            currentColumn++;

            winLoss = info.DetailInfo[roleIndex[RoleConstants.Super]];
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.WinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.Commission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentCell.Value = winLoss.LineTotal;
            currentColumn++;
            currentRow++;
        }
    }
}
