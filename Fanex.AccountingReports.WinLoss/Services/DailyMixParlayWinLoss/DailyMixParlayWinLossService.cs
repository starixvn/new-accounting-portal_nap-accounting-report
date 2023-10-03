namespace Fanex.AccountingReports.WinLoss.Services.Implement.DailyMixParlayWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Constants;
    using Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.DailyMixParlayWinLoss;
    using Fanex.AccountingReports.WinLoss.Repositories.Implement;
    using Fanex.AccountingReports.WinLoss.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyMixParlayWinLossService : ServiceBase
    {
        private Dictionary<object, string> _sportTypeNameCache;
        private Dictionary<object, string> _betTypeNameCache;
        private DailyMixParlayWinLossRepository _reportRepository;

        public DailyMixParlayWinLossService()
            : this(null)
        {
        }

        public DailyMixParlayWinLossService(DailyMixParlayWinLossRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new DailyMixParlayWinLossRepository();
        }

        /// <summary>
        /// Gets the combination win loss report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The combination win loss report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria)
        {
            WinLossInfo report = new WinLossInfo
            {
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.Grand_Total
                },
                ChildsInfo = new List<WinLossInfo>()
            };

            var rawData = this._reportRepository.GetCombinationWinLossReport(criteria);
            if (rawData != null)
            {
                var combinationGroups = rawData.GroupBy(data => data.Combinations);
                foreach (var combinationGroup in combinationGroups)
                {
                    var combinationWinLoss = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Total
                        }
                    };

                    combinationWinLoss.Name = this.ConvertCombinationType(Converter.ToString2(combinationGroup.Key));
                    var liveTypeGroups = combinationGroup.GroupBy(data => data.LiveType);
                    foreach (var liveTypeGroup in liveTypeGroups)
                    {
                        var liveTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(liveTypeGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo
                            {
                                Name = Labels.Sub_Total
                            }
                        };

                        liveTypeWinLoss.Name = this.ConvertLiveType(liveTypeWinLoss.Id);
                        foreach (var data in liveTypeGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Status = Converter.ToString2(data.Status),
                                MemberWinLoss = Converter.ToDecimal(data.MemberWinLoss),
                                MemberTurnover = Converter.ToDecimal(data.MemberTurnover),
                                BetCount = Converter.ToInt(data.BetCount),
                                SMAWinLoss = Converter.ToDecimal(data.SMAWinLoss),
                                Commission = Converter.ToDecimal(data.Commission),
                                RealIncome = Converter.ToDecimal(data.RealIncome),
                                RealTurnover = Converter.ToDecimal(data.RealTurnover),
                                ExtraPT = Converter.ToDecimal(data.ExtraPT),
                                MemberBonus = Converter.ToDecimal(data.MemberBonus),
                                HouseBonus = Converter.ToDecimal(data.HouseBonus)
                            };

                            info.Name = string.Format(Labels.Mix_Parlay + " ({0})", info.Status);
                            info.Margin = this.CalculateMargin(info.RealIncome, info.RealTurnover);
                            liveTypeWinLoss.GrandTotal.Add(info);
                            info = this.FormatWinLossInfo(info);
                            liveTypeWinLoss.ChildsInfo.Add(info);

                            liveTypeWinLoss.TotalRowDetail++;
                            combinationWinLoss.TotalRowDetail++;
                        }

                        liveTypeWinLoss.GrandTotal.Margin = this.CalculateMargin(
                            liveTypeWinLoss.GrandTotal.RealIncome,
                            liveTypeWinLoss.GrandTotal.RealTurnover);
                        combinationWinLoss.GrandTotal.Add(liveTypeWinLoss.GrandTotal);
                        liveTypeWinLoss.GrandTotal = this.FormatWinLossInfo(liveTypeWinLoss.GrandTotal);
                        combinationWinLoss.ChildsInfo.Add(liveTypeWinLoss);
                    }

                    combinationWinLoss.GrandTotal.Margin = this.CalculateMargin(
                        combinationWinLoss.GrandTotal.RealIncome,
                        combinationWinLoss.GrandTotal.RealTurnover);
                    report.GrandTotal.Add(combinationWinLoss.GrandTotal);
                    combinationWinLoss.GrandTotal = this.FormatWinLossInfo(combinationWinLoss.GrandTotal);
                    report.ChildsInfo.Add(combinationWinLoss);
                }

                report.GrandTotal.Margin = this.CalculateMargin(
                    report.GrandTotal.RealIncome,
                    report.GrandTotal.RealTurnover);
                report.GrandTotal = this.FormatWinLossInfo(report.GrandTotal);
            }

            SessionManager.Set<WinLossInfo>(WinLossContants.DailyMixParlayCombinationReport, report);
            return report;
        }

        /// <summary>
        /// Gets the advance mix parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The advance mix parlay report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetAdvanceMixParlayReport(GetAdvanceMixParlayReportCriteria criteria)
        {
            WinLossInfo report = new WinLossInfo
            {
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.Grand_Total
                },
                ChildsInfo = new List<WinLossInfo>()
            };

            var rawData = this.GetReportData<dynamic>(criteria);
            if (rawData != null)
            {
                var sportTypeList = rawData.Select(data => Converter.ToInt(data.SportType))
                                           .ToList();
                this.GetSportName(sportTypeList, ref this._sportTypeNameCache);

                var liveTypeGroups = rawData.GroupBy(data => data.LiveType);
                foreach (var liveTypeGroup in liveTypeGroups)
                {
                    var liveTypeWinLoss = new WinLossInfo
                    {
                        Id = Converter.ToInt(liveTypeGroup.Key),
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Total
                        }
                    };

                    liveTypeWinLoss.Name = this.ConvertLiveType(liveTypeWinLoss.Id);
                    var sportGroups = liveTypeGroup.GroupBy(data => data.SportType);
                    foreach (var sportGroup in sportGroups)
                    {
                        var sportWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(sportGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo
                            {
                                Name = Labels.Sub_Total
                            }
                        };

                        sportWinLoss.Name = this._sportTypeNameCache[sportWinLoss.Id];
                        foreach (var data in sportGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = criteria.Bettype,
                                Status = Converter.ToString2(data.status),
                                MemberWinLoss = Converter.ToDecimal(data.MemberWinLoss),
                                SMAWinLoss = Converter.ToDecimal(data.SMAWinLoss),
                                Commission = Converter.ToDecimal(data.Commission),
                                RealIncome = Converter.ToDecimal(data.RealIncome),
                                RealTurnover = Converter.ToDecimal(data.RealTurnover),
                                ExtraPT = Converter.ToDecimal(data.ExtraPT),
                                MemberBonus = Converter.ToDecimal(data.MemberBonus),
                                HouseBonus = Converter.ToDecimal(data.HouseBonus)
                            };

                            info.Name = string.Format(Labels.Mix_Parlay + " ({0})", info.Status);
                            info.Margin = this.CalculateMargin(info.RealIncome, info.RealTurnover);
                            sportWinLoss.GrandTotal.Add(info);
                            info = this.FormatWinLossInfo(info);
                            sportWinLoss.ChildsInfo.Add(info);

                            sportWinLoss.TotalRowDetail++;
                            liveTypeWinLoss.TotalRowDetail++;
                        }

                        sportWinLoss.GrandTotal.Margin = this.CalculateMargin(
                           sportWinLoss.GrandTotal.RealIncome,
                           sportWinLoss.GrandTotal.RealTurnover);
                        liveTypeWinLoss.GrandTotal.Add(sportWinLoss.GrandTotal);
                        sportWinLoss.GrandTotal = this.FormatWinLossInfo(sportWinLoss.GrandTotal);
                        liveTypeWinLoss.ChildsInfo.Add(sportWinLoss);
                    }

                    liveTypeWinLoss.GrandTotal.Margin = this.CalculateMargin(
                      liveTypeWinLoss.GrandTotal.RealIncome,
                      liveTypeWinLoss.GrandTotal.RealTurnover);
                    report.GrandTotal.Add(liveTypeWinLoss.GrandTotal);
                    liveTypeWinLoss.GrandTotal = this.FormatWinLossInfo(liveTypeWinLoss.GrandTotal);
                    report.ChildsInfo.Add(liveTypeWinLoss);
                }

                report.GrandTotal.Margin = this.CalculateMargin(
                    report.GrandTotal.RealIncome,
                    report.GrandTotal.RealTurnover);
                report.GrandTotal = this.FormatWinLossInfo(report.GrandTotal);
            }

            SessionManager.Set<WinLossInfo>(WinLossContants.DailyMixParlayAdvanceMixParlayReport, report);
            return report;
        }

        /// <summary>
        /// Gets the name of the sport.
        /// </summary>
        /// <param name="sportIdList">The identifier list.</param>
        /// <param name="currentStorage">The current storage.</param>
        /// <returns>The name of the sport.</returns>
        public Dictionary<object, string> GetSportName(
            List<dynamic> sportIdList,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var sportIdInfo in sportIdList)
            {
                if (!currentStorage.ContainsKey(sportIdInfo))
                {
                    var name = string.Empty;
                    int id = Converter.ToInt(sportIdInfo);
                    switch (id)
                    {
                        case -1:
                            name = "Mixed Sports";
                            break;

                        case -2:
                            name = "Mixed Other Sports";
                            break;

                        default:
                            name = CacheRef.GetSportName(id);
                            break;
                    }

                    currentStorage.Add(sportIdInfo, name);
                }
            }

            return currentStorage;
        }

        /// <summary>
        /// Exports the combination win loss report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria)
        {
            var report = SessionManager.Get<WinLossInfo>(WinLossContants.DailyMixParlayCombinationReport);
            if (report == null)
            {
                report = this.GetCombinationWinLossReport(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = WinLossResources.DailyMixParlayWinLoss_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 13;
            var currentRow = startRow;

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, backgroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, true, false);
            var positiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var subRowColor = ExcelExtensions.TotalRowColor;
            var subRowTextStyle = ExcelExtensions.GetCellTextStyle(subRowColor, true);
            var subRowIntStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, subRowColor, true, false, false);
            var subRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                subRowColor,
                true,
                false,
                false);
            var subRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                subRowColor,
                true,
                true,
                false);
            var subRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                subRowColor,
                true,
                true,
                false);

            var liveTypeTotalColor = Color.FromArgb(222, 210, 190);
            var totalRowTextStyle = ExcelExtensions.GetCellTextStyle(liveTypeTotalColor, true);
            var totalRowIntStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.IntNumberFormat,
                liveTypeTotalColor,
                true,
                false,
                false);
            var totalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                liveTypeTotalColor,
                true,
                false,
                false);
            var totalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                liveTypeTotalColor,
                true,
                true,
                false);
            var totalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                liveTypeTotalColor,
                true,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            var grandTotalRowIntStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, grandTotalRowColor, true, false, false);
            var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            var grandTotalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            var grandTotalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);

            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 150 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 1].Width = 160 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 10].Width = 120 * ExcelExtensions.FontWidth;

            var title = string.Format(
                WinLossResources.DailyMixParlayWinLoss_CombinationWinLoss_ReportTitle,
                ReportService.GetSiteTypeName(criteria.Site),
                criteria.FromDate.ToShortFormatedDate(),
                criteria.ToDate.ToShortFormatedDate());
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            // 1stRow
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Combinations;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Type;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Bet_Type;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.SMA_Win_Loss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.ExtraPT;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Commission;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.House;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;
            currentRow++;

            // 2ndRow
            currentColumn = startColumn + 3;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Win_Loss;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Turnover;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Bet_Count;
            currentCell.Style = headerTextStyle;
            currentColumn += 4;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.RealIncome;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.RealTurnover;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Margin;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = "Member Bonus";
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = "House Bonus";
            currentCell.Style = headerTextStyle;
            currentColumn++;
            currentRow++;

            if (report != null && report.ChildsInfo.Count > 0)
            {
                var len = report.ChildsInfo.Count;
                WinLossInfo info;
                for (var i = 0; i < len; i++)
                {
                    var combinationGroup = report.ChildsInfo[i];
                    var combinationGroupLength = combinationGroup.ChildsInfo.Count;
                    for (var j = 0; j < combinationGroupLength; j++)
                    {
                        var liveTypeGroup = combinationGroup.ChildsInfo[j];
                        var liveTypeGroupLength = liveTypeGroup.ChildsInfo.Count;
                        for (var k = 0; k < liveTypeGroupLength; k++)
                        {
                            info = liveTypeGroup.ChildsInfo[k];
                            currentColumn = startColumn;

                            if (j == 0 && k == 0)
                            {
                                var combinationColor = this.GetExcelCombinationColor(combinationGroup.Name);
                                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(
                                    currentRow,
                                    currentColumn,
                                    currentRow + combinationGroup.TotalRowDetail + combinationGroupLength,
                                    currentColumn);
                                currentCellRange.Merged = true;
                                currentCellRange.Value = combinationGroup.Name;
                                currentCellRange.Style = textStyle;
                                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                                currentCellRange.Style.FillPattern.SetSolid(combinationColor);
                            }

                            currentColumn++;
                            if (k == 0)
                            {
                                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + liveTypeGroupLength, currentColumn);
                                currentCellRange.Merged = true;
                                currentCellRange.Value = liveTypeGroup.Name;
                                currentCellRange.Style = textStyle;
                                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                            }

                            currentColumn++;
                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Name;
                            currentCell.Style = textStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.MemberWinLoss;
                            currentCell.Style = twoDecimalPositiveNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.MemberTurnover;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.BetCount;
                            currentCell.Style = intStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.SMAWinLoss;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.ExtraPT;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Commission;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.RealIncome;
                            currentCell.Style = twoDecimalPositiveNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.RealTurnover;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Margin;
                            currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.MemberBonus;
                            currentCell.Style = twoDecimalPositiveNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.HouseBonus;
                            currentCell.Style = twoDecimalPositiveNumberStyle;
                            currentColumn++;
                            currentRow++;
                        }

                        info = liveTypeGroup.GrandTotal;
                        currentColumn = startColumn + 1;

                        currentColumn++;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Name;
                        currentCell.Style = totalRowTextStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MemberWinLoss;
                        currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MemberTurnover;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.BetCount;
                        currentCell.Style = totalRowIntStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.SMAWinLoss;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.ExtraPT;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.RealIncome;
                        currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.RealTurnover;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Margin;
                        currentCell.Style = totalRowPositiveTwoDecimalPercentNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MemberBonus;
                        currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.HouseBonus;
                        currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;
                        currentRow++;
                    }

                    info = combinationGroup.GrandTotal;
                    currentColumn = startColumn;

                    currentColumn++;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = subRowTextStyle;

                    currentColumn++;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = subRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.MemberWinLoss;
                    currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.MemberTurnover;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.BetCount;
                    currentCell.Style = subRowIntStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.SMAWinLoss;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.ExtraPT;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Commission;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Margin;
                    currentCell.Style = subRowPositiveTwoDecimalPercentNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.MemberBonus;
                    currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.HouseBonus;
                    currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;
                    currentRow++;
                }

                info = report.GrandTotal;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
                currentCellRange.Merged = true;
                currentCellRange.Value = info.Name;
                currentCellRange.Style = grandTotalRowTextStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                currentCellRange.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn += 3;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.MemberWinLoss;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.MemberTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BetCount;
                currentCell.Style = grandTotalRowIntStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.SMAWinLoss;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.ExtraPT;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Commission;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealIncome;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Margin;
                currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.MemberBonus;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.HouseBonus;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                currentColumn++;
                currentRow++;
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            var reportName = string.Format(
                WinLossResources.DailyMixParlayWinLoss_CombinationWinLoss_ReportName,
                criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);

        }

        /// <summary>
        /// Exports the advance mix parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportAdvanceMixParlayReport(GetAdvanceMixParlayReportCriteria criteria)
        {
            var report = SessionManager.Get<WinLossInfo>(WinLossContants.DailyMixParlayAdvanceMixParlayReport);
            if (report == null)
            {
                report = this.GetAdvanceMixParlayReport(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = WinLossResources.DailyMixParlayWinLoss_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 9;
            var currentRow = startRow;

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, true, false);
            var positiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var subRowColor = ExcelExtensions.TotalRowColor;
            var subRowTextStyle = ExcelExtensions.GetCellTextStyle(subRowColor, true);
            var subRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, subRowColor, true, false, false);
            var subRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, subRowColor, true, true, false);
            var subRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                subRowColor,
                true,
                true,
                false);

            var sportTypeTotalColor = Color.FromArgb(222, 210, 190);
            var totalRowTextStyle = ExcelExtensions.GetCellTextStyle(sportTypeTotalColor, true);
            var totalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, sportTypeTotalColor, true, false, false);
            var totalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                sportTypeTotalColor,
                true,
                true,
                false);
            var totalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                sportTypeTotalColor,
                true,
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
            var grandTotalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            var grandTotalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);

            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 150 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 8].Width = 120 * ExcelExtensions.FontWidth;

            var title = string.Format(
                "Parlay Win Loss by Product by {0}  - From {1} To {2}",
                ReportService.GetSiteTypeName(criteria.Site),
                criteria.FromDate.ToShortFormatedDate(),
                criteria.ToDate.ToShortFormatedDate());
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Sport_Type;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Bet_Type;
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
            currentCell.Value = Labels.ExtraPT;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Commission;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.RealIncome;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.RealTurnover;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Margin;
            currentCell.Style = headerTextStyle;
            currentColumn++;
            currentRow++;

            if (report != null && report.ChildsInfo.Count > 0)
            {
                var len = report.ChildsInfo.Count;
                WinLossInfo info;
                for (var i = 0; i < len; i++)
                {
                    var liveTypeGroup = report.ChildsInfo[i];
                    var liveTypeGroupLength = liveTypeGroup.ChildsInfo.Count;
                    for (var j = 0; j < liveTypeGroupLength; j++)
                    {
                        var sportGroup = liveTypeGroup.ChildsInfo[j];
                        var sportGroupLength = sportGroup.ChildsInfo.Count;
                        for (var k = 0; k < sportGroupLength; k++)
                        {
                            info = sportGroup.ChildsInfo[k];
                            currentColumn = startColumn;
                            if (j == 0 && k == 0)
                            {
                                var liveTypeColor = this.GetExcelLiveTypeColor(liveTypeGroup.Id);
                                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(
                                    currentRow,
                                    currentColumn,
                                    currentRow + liveTypeGroup.TotalRowDetail + liveTypeGroupLength,
                                    currentColumn);
                                currentCellRange.Merged = true;
                                currentCellRange.Value = liveTypeGroup.Name;
                                currentCellRange.Style = textStyle;
                                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                                currentCellRange.Style.FillPattern.SetSolid(liveTypeColor);
                            }

                            currentColumn++;
                            if (k == 0)
                            {
                                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + sportGroupLength, currentColumn);
                                currentCellRange.Merged = true;
                                currentCellRange.Value = sportGroup.Name;
                                currentCellRange.Style = textStyle;
                                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            }

                            currentColumn++;
                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Name;
                            currentCell.Style = textStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.MemberWinLoss;
                            currentCell.Style = twoDecimalPositiveNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.SMAWinLoss;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.ExtraPT;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Commission;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.RealIncome;
                            currentCell.Style = twoDecimalPositiveNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.RealTurnover;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Margin;
                            currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.MemberBonus;
                            currentCell.Style = twoDecimalPositiveNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.HouseBonus;
                            currentCell.Style = twoDecimalPositiveNumberStyle;
                            currentColumn++;
                            currentRow++;
                        }

                        info = sportGroup.GrandTotal;
                        currentColumn = startColumn + 1;

                        currentColumn++;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Name;
                        currentCell.Style = totalRowTextStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MemberWinLoss;
                        currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.SMAWinLoss;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.ExtraPT;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.RealIncome;
                        currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.RealTurnover;
                        currentCell.Style = totalRowTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Margin;
                        currentCell.Style = totalRowPositiveTwoDecimalPercentNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MemberBonus;
                        currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.HouseBonus;
                        currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                        currentColumn++;
                        currentRow++;
                    }

                    info = liveTypeGroup.GrandTotal;
                    currentColumn = startColumn;

                    currentColumn++;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = subRowTextStyle;

                    currentColumn++;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = subRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.MemberWinLoss;
                    currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.SMAWinLoss;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.ExtraPT;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Commission;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = subRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Margin;
                    currentCell.Style = subRowPositiveTwoDecimalPercentNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.MemberBonus;
                    currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.HouseBonus;
                    currentCell.Style = subRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;
                    currentRow++;
                }

                info = report.GrandTotal;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
                currentCellRange.Merged = true;
                currentCellRange.Value = info.Name;
                currentCellRange.Style = grandTotalRowTextStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                currentCellRange.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn += 3;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.MemberWinLoss;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.SMAWinLoss;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.ExtraPT;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Commission;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealIncome;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Margin;
                currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.MemberBonus;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.HouseBonus;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                currentColumn++;
                currentRow++;
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            var reportName = string.Format(
                WinLossResources.DailyMixParlayWinLoss_AdvanceMixParlay_ReportName,
                criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Gets the mix parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The mix parlay report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            var rawData = this.GetReportData<dynamic>(criteria);
            WinLossInfo report = new WinLossInfo
            {
                GrandTotal = new WinLossInfo
                {
                    Name = Labels.All_Total
                },
                ChildsInfo = new List<WinLossInfo>()
            };

            if (rawData != null)
            {
                var sportGroups = rawData.GroupBy(data => data.sporttype);
                var betTypeList = rawData.Select(data => Converter.ToInt(data.bettype))
                                         .ToList();
                CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                foreach (var sportGroup in sportGroups)
                {
                    var sportWinLoss = new WinLossInfo
                    {
                        Id = Converter.ToInt(sportGroup.Key),
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo()
                    };

                    sportWinLoss.Name = this.GetSportName(sportWinLoss.Id);
                    sportWinLoss.GrandTotal.Name = sportWinLoss.Name + " " + Labels.Total;
                    foreach (var data in sportGroup)
                    {
                        WinLossInfo info = new WinLossInfo
                        {
                            Id = Converter.ToInt(data.bettype),
                            Status = Converter.ToString2(data.status),
                            MemberWinLoss = Converter.ToDecimal(data.WinLost),
                            SMAWinLoss = Converter.ToDecimal(data.Position),
                            Commission = Converter.ToDecimal(data.Commission),
                            RealIncome = Converter.ToDecimal(data.LineTotal),
                            RealTurnover = Converter.ToDecimal(data.Turnover)
                        };

                        info.Name = this._betTypeNameCache[info.Id];
                        info.Margin = this.CalculateMargin(info.RealIncome, info.RealTurnover);
                        sportWinLoss.GrandTotal.Add(info);
                        info = this.FormatWinLossInfo(info);
                        sportWinLoss.ChildsInfo.Add(info);
                    }

                    sportWinLoss.ChildsInfo = sportWinLoss.ChildsInfo.OrderBy(info => info.Id).ToList();
                    sportWinLoss.GrandTotal.Margin = this.CalculateMargin(
                        sportWinLoss.GrandTotal.RealIncome,
                        sportWinLoss.GrandTotal.RealTurnover);
                    report.GrandTotal.Add(sportWinLoss.GrandTotal);
                    sportWinLoss.GrandTotal = this.FormatWinLossInfo(sportWinLoss.GrandTotal);
                    report.ChildsInfo.Add(sportWinLoss);
                }

                report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
                report.GrandTotal.Margin = this.CalculateMargin(
                    report.GrandTotal.RealIncome,
                    report.GrandTotal.RealTurnover);
                report.GrandTotal = this.FormatWinLossInfo(report.GrandTotal);
                SessionManager.Set<WinLossInfo>(WinLossContants.DailyMixParlayNormalMixParlayReport, report);
                return report;
            }

            SessionManager.Set<WinLossInfo>(WinLossContants.DailyMixParlayNormalMixParlayReport, null);
            return null;
        }

        /// <summary>
        /// Exports the mix parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            var report = SessionManager.Get<WinLossInfo>(WinLossContants.DailyMixParlayNormalMixParlayReport);
            if (report == null)
            {
                report = this.GetMixParlayReport(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = WinLossResources.DailyMixParlayWinLoss_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 6;
            var currentRow = startRow;

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, true, false);
            var positiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var sportTypeTotalColor = ExcelExtensions.TotalRowColor;
            var totalRowTextStyle = ExcelExtensions.GetCellTextStyle(sportTypeTotalColor, true);
            var totalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, sportTypeTotalColor, true, false, false);
            var totalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                sportTypeTotalColor,
                true,
                true,
                false);
            var totalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                sportTypeTotalColor,
                true,
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
            var grandTotalRowPositiveTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            var grandTotalRowPositiveTwoDecimalPercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);

            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 150 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 6].Width = 120 * ExcelExtensions.FontWidth;

            var title = WinLossResources.DailyMixParlayWinLoss_NormalMixParlay_ReportTitle;
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Bet_Type;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Win_Loss;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Position;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Comm;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Line_Total;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Turnover;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Margin;
            currentCell.Style = headerTextStyle;
            currentColumn++;
            currentRow++;

            if (report != null && report.ChildsInfo.Count > 0)
            {
                var len = report.ChildsInfo.Count;
                WinLossInfo info;
                for (var i = 0; i < len; i++)
                {
                    var sportGroup = report.ChildsInfo[i];
                    var sportGroupLength = sportGroup.ChildsInfo.Count;
                    for (var j = 0; j < sportGroupLength; j++)
                    {
                        info = sportGroup.ChildsInfo[j];
                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = string.Format("{0} ({1})", info.Name, info.Status);
                        currentCell.Style = textStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MemberWinLoss;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.SMAWinLoss;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.RealIncome;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.RealTurnover;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Margin;
                        currentCell.Style = positiveTwoDecimalPercentNumberStyle;
                        currentColumn++;
                        currentRow++;
                    }

                    info = sportGroup.GrandTotal;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = totalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.MemberWinLoss;
                    currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.SMAWinLoss;
                    currentCell.Style = totalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Commission;
                    currentCell.Style = totalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealIncome;
                    currentCell.Style = totalRowPositiveTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.RealTurnover;
                    currentCell.Style = totalRowTwoDecimalNumberStyle;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Margin;
                    currentCell.Style = totalRowPositiveTwoDecimalPercentNumberStyle;
                    currentColumn++;
                    currentRow++;

                    workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                info = report.GrandTotal;
                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Name;
                currentCell.Style = grandTotalRowTextStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.MemberWinLoss;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.SMAWinLoss;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Commission;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealIncome;
                currentCell.Style = grandTotalRowPositiveTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.RealTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Margin;
                currentCell.Style = grandTotalRowPositiveTwoDecimalPercentNumberStyle;
                currentCell.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                currentColumn++;
                currentRow++;
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            var reportName = WinLossResources.DailyMixParlayWinLoss_NormalMixParlay_ReportName;
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Formats the win loss information.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The win loss information.</returns>
        private WinLossInfo FormatWinLossInfo(WinLossInfo winLoss)
        {
            winLoss.BetCount_Str = winLoss.BetCount.Format(0);
            winLoss.MemberWinLoss_Str = winLoss.MemberWinLoss.Format(2);
            winLoss.SMAWinLoss_Str = winLoss.SMAWinLoss.Format(2);

            winLoss.MemberTurnover_Str = winLoss.MemberTurnover.Format(2);
            winLoss.RealTurnover_Str = winLoss.RealTurnover.Format(2);
            winLoss.Commission_Str = winLoss.Commission.Format(2);
            winLoss.RealIncome_Str = winLoss.RealIncome.Format(2);
            winLoss.ExtraPT_Str = winLoss.ExtraPT.Format(2);

            if (winLoss.RealTurnover != 0)
            {
                winLoss.Margin_Str = (winLoss.Margin * 100).Format(2);
            }
            else
            {
                winLoss.Margin_Str = Labels.NotAvailable;
            }

            winLoss.MemberBonus_Str = winLoss.MemberBonus.Format(2);
            winLoss.HouseBonus_Str = winLoss.HouseBonus.Format(2);

            return winLoss;
        }

        /// <summary>
        /// Converts the type of the combination.
        /// </summary>
        /// <param name="combination">The combination.</param>
        /// <returns>The type of the combination.</returns>
        private string ConvertCombinationType(string combination)
        {
            string name = string.Empty;
            if (!string.IsNullOrEmpty(combination) &&
                combination.Contains(WinLossContants.DailyMixParlayReportCombinationPrefix))
            {
                name = string.Format(
                    WinLossResources.DailyMixParlayWinLoss_CombinationNameFormat,
                    combination.Substring(WinLossContants.DailyMixParlayReportCombinationPrefix.Length),
                    string.Empty); // Exp: 2XCombinations

                if (string.Equals(
                    combination,
                    WinLossContants.DailyMixParlayReportMaxCombination,
                    StringComparison.OrdinalIgnoreCase))
                {
                    name += WinLossResources.DailyMixParlayWinLoss_MaxCombinationSuffix;  // Exp: 2XCombinations & up
                }
            }
            else
            {
                name = Labels.Unknow;
            }

            return name;
        }

        /// <summary>
        /// Converts the type of the live.
        /// </summary>
        /// <param name="liveType">Type of the live.</param>
        /// <returns>The type of the live.</returns>
        private string ConvertLiveType(int liveType)
        {
            string name = string.Empty;
            switch (liveType)
            {
                case LiveTypeConstants.NonLive:
                    name = Labels.Non_Live;
                    break;

                case LiveTypeConstants.Live:
                    name = Labels.Live;
                    break;

                case LiveTypeConstants.MixLiveTypes:
                    name = Labels.Mix_Live_and_Non_Live;
                    break;
            }

            return name;
        }

        private decimal CalculateMargin(decimal lineTotal, decimal turnover)
        {
            decimal result = 0;
            if (turnover != 0)
            {
                result = lineTotal / turnover;
            }

            return result;
        }

        /// <summary>
        /// Gets the color of the excel combination.
        /// </summary>
        /// <param name="combinationName">Name of the combination.</param>
        /// <returns>The color of the excel combination.</returns>
        private Color GetExcelCombinationColor(string combinationName)
        {
            var color = ExcelExtensions.BackGroundColor;
            switch (combinationName)
            {
                case CombinationTypeConstants._2XCombinations:
                    color = Color.FromArgb(221, 217, 196);
                    break;

                case CombinationTypeConstants._3XCombinations:
                    color = Color.FromArgb(202, 240, 145);
                    break;

                case CombinationTypeConstants._4XCombinations:
                    color = Color.FromArgb(183, 222, 232);
                    break;

                case CombinationTypeConstants._5XCombinations:
                    color = Color.FromArgb(253, 233, 217);
                    break;

                case CombinationTypeConstants._6XCombinationsAndUp:
                    color = Color.FromArgb(242, 220, 229);
                    break;
            }

            return color;
        }

        /// <summary>
        /// Gets the color of the excel live type.
        /// </summary>
        /// <param name="liveTypeId">The live type identifier.</param>
        /// <returns>The color of the excel live type.</returns>
        private Color GetExcelLiveTypeColor(int liveTypeId)
        {
            var color = ExcelExtensions.BackGroundColor;
            switch (liveTypeId)
            {
                case LiveTypeConstants.NonLive:
                    color = Color.FromArgb(202, 240, 145);
                    break;

                case LiveTypeConstants.Live:
                    color = Color.FromArgb(221, 217, 196);
                    break;

                case LiveTypeConstants.MixLiveTypes:
                    color = Color.FromArgb(197, 217, 241);
                    break;
            }

            return color;
        }

        /// <summary>
        /// Gets the name of the sport.
        /// </summary>
        /// <param name="sportType">Type of the sport.</param>
        /// <returns>The name of the sport.</returns>
        private string GetSportName(int sportType)
        {
            var sportTotalName = string.Empty;
            switch (sportType)
            {
                case SportContants.Soccer:
                    sportTotalName = Labels.Soccer + " " + Labels.Sport;
                    break;

                case SportContants.CombinationMixParlay:
                    sportTotalName = Labels.Multi + " " + Labels.Sport;
                    break;

                default:
                    sportTotalName = Labels.Others + " " + Labels.Sport;
                    break;
            }

            return sportTotalName;
        }
    }
}