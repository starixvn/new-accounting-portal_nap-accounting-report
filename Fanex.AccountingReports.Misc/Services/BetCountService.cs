namespace Fanex.AccountingReports.Misc.Services.Implement
{
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.BetCount;
    using Fanex.AccountingReports.Misc.Models;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.Common;
    using Models.BetCount;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetCountService : ServiceBase
    {
        private static Dictionary<object, string> _sportTypeNameCache;

        private enum ReportRowType
        {
            /// <summary>
            /// The normal row.
            /// </summary>
            NormalRow = 1,

            /// <summary>
            /// The total.
            /// </summary>
            Total = 2,

            /// <summary>
            /// The grand total.
            /// </summary>
            GrandTotal = 3
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public Transaction GetReport(GetBetCountReportCriteria criteria)
        {
            var sessionKey = criteria.IsHistoryReport ?
                MiscConstants.BetCountHistoryReportSessionKey :
                MiscConstants.BetCountSessionKey;

            var rawData = GetReportData<dynamic>(criteria);
            Transaction report = new Transaction
            {
                TransactionList = new List<Transaction>(),
                Total = new Transaction
                {
                    SiteName = Labels.Total
                }
            };

            rawData.GroupBy(data => data.IsDeposit)
                   .ToList()
                   .ForEach(group =>
                     {
                         Transaction transByIsDepositionGroup = new Transaction
                         {
                             TransactionList = new List<Transaction>(),
                             Total = new Transaction()
                         };

                         transByIsDepositionGroup.SiteName = Converter.ToBool(group.Key) ?
                             Labels.Deposit :
                             Labels.Credit;
                         transByIsDepositionGroup.Total.SiteName = transByIsDepositionGroup.SiteName;

                         group.ToList()
                              .ForEach(transaction =>
                                   {
                                       var transactionData = new Transaction
                                       {
                                           SiteName = transaction.Site,
                                           BetCount = Converter.ToInt(transaction.BetCount),
                                           Turnover = Converter.ToDecimal(transaction.TurnOver),
                                           MemberWinLoss = Converter.ToDecimal(transaction.MemberWL),
                                           AgentWinLoss = Converter.ToDecimal(transaction.AgentWL),
                                           MasterWinLoss = Converter.ToDecimal(transaction.MasterWL),
                                           SuperWinLoss = Converter.ToDecimal(transaction.SuperWL),
                                           CompanyWinLoss = Converter.ToDecimal(transaction.RealIncome),
                                           TotalWinLoss = Converter.ToDecimal(transaction.SMAWL),
                                           Commission = Converter.ToDecimal(transaction.Commission),
                                           ExtraPT = Converter.ToDecimal(transaction.ExtraPT),
                                       };

                                       transactionData.MarginPercent = CalculateMargin(transactionData.MemberWinLoss, transactionData.Turnover);
                                       transByIsDepositionGroup.SumDataToTotal(transactionData);
                                       transByIsDepositionGroup.TransactionList.Add(transactionData);
                                   });

                         transByIsDepositionGroup.Total.MarginPercent = CalculateMargin(
                             transByIsDepositionGroup.Total.MemberWinLoss,
                             transByIsDepositionGroup.Total.Turnover);

                         report.TransactionList.Add(transByIsDepositionGroup);
                     });

            for (int i = 0; i < report.TransactionList.Count; i++)
            {
                var transactionGroup = report.TransactionList[i];
                report.SumDataToTotal(transactionGroup.Total);
            }

            report.Total.MarginPercent = this.CalculateMargin(report.Total.MemberWinLoss, report.Total.Turnover);
            SessionManager.Set<Transaction>(sessionKey, report);
            return report;

            SessionManager.Set<Transaction>(sessionKey, null);
            return null;
        }

        /// <summary>
        /// Sorts the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public Transaction SortReport(GetBetCountReportCriteria criteria)
        {
            var sessionKey = criteria.IsHistoryReport ?
                MiscConstants.BetCountHistoryReportSessionKey :
                MiscConstants.BetCountSessionKey;

            var report = SessionManager.Get<Transaction>(sessionKey);
            if (report == null)
            {
                report = this.GetReport(criteria);
            }

            if (report != null &&
                report.TransactionList != null &&
                report.TransactionList.Count > 0)
            {
                var siteGroupLength = report.TransactionList.Count;
                for (int i = 0; i < siteGroupLength; i++)
                {
                    var propertyName = this.GetSortedColumnName(criteria.SortBy);
                    report.TransactionList[i].TransactionList.SortByProperty<Transaction>(
                        criteria.SortDirection,
                        propertyName);
                }
            }

            SessionManager.Set<Transaction>(sessionKey, report);
            return report;

            SessionManager.Set<Transaction>(sessionKey, null);
            return null;
        }

        #region ExportExcel

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetBetCountReportCriteria criteria)
        {
            var sessionKey = criteria.IsHistoryReport ?
                MiscConstants.BetCountHistoryReportSessionKey :
                MiscConstants.BetCountSessionKey;

            var report = SessionManager.Get<Transaction>(sessionKey);
            if (report == null)
            {
                report = this.GetReport(criteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = MiscResources.BetCountReport_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportBetCountReportHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                criteria);

            this.ExportBetCountReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                criteria,
                report);

            excelFile.ReponseStream(string.Format(
                MiscResources.BetCountReport_ReportName,
                criteria.Datestart.ToShortFormatedDateWithSlash().Replace('/', '_'),
                criteria.Dateend.ToShortFormatedDateWithSlash().Replace('/', '_')));
        }

        /// <summary>
        /// Exports the bet count report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportBetCountReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetBetCountReportCriteria criteria)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentColumn = startColumn;
            for (int colIndex = 0; colIndex < 12; colIndex++)
            {
                worksheet.Columns[startColumn + colIndex].Width = 150 * ExcelExtensions.FontWidth;
            }

            var title = string.Format(
                MiscResources.BetCount_ReportTitle,
                criteria.Datestart.ToShortFormatedDate(),
                criteria.Dateend.ToShortFormatedDate());

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 11);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow += 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 11);

            //// First Row
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Site;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Bet_Count;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 3;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.SMA_Winloss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 4;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "Commission";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "ExtraPT";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "House Real Income";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;

            //// Second Row
            currentColumn = startColumn + 2;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "Turnover";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "Win Loss";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Margin_Percent;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Agent;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Master;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Super;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Total;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the content of the bet count report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        private void ExportBetCountReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetBetCountReportCriteria criteria,
            Transaction report)
        {
            if (report != null &&
                report.TransactionList != null &&
                report.TransactionList.Count > 0)
            {
                var groups = report.TransactionList;
                var siteLength = groups.Count;
                for (int i = 0; i < siteLength; i++)
                {
                    var group = groups[i];
                    var groupLength = group.TransactionList.Count;
                    this.ExportTransactionRow(
                        ref worksheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        group.Total,
                        ReportRowType.Total);

                    foreach (var transaction in group.TransactionList)
                    {
                        this.ExportTransactionRow(
                            ref worksheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            transaction,
                            ReportRowType.NormalRow);
                    }
                }

                this.ExportTransactionRow(
                    ref worksheet,
                      startRow,
                      startColumn,
                      ref currentRow,
                      report.Total,
                      ReportRowType.GrandTotal);
            }
            else
            {
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 9);
            }
        }

        /// <summary>
        /// Exports the transaction row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="rowType">Type of the row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTransactionRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            Transaction transaction,
            ReportRowType rowType)
        {
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                true,
                false);

            switch (rowType)
            {
                case ReportRowType.NormalRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
                    twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                        NumberFormatConstants.TwoDecimalDigitFormat,
                        ExcelExtensions.BackGroundColor,
                        false,
                        true,
                        false);
                    twoDecimalPositiveNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.Total:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.TotalRowColor, true);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.TotalRowColor, true, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.TotalRowColor, true, false, false);
                    twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                        NumberFormatConstants.TwoDecimalDigitFormat,
                        ExcelExtensions.TotalRowColor,
                        true,
                        true,
                        false);
                    twoDecimalPositiveNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.GrandTotal:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                    textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.GrandTotalRowColor, true, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    intStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);

                    twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.GrandTotalRowColor, true, false, false);
                    twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                        NumberFormatConstants.TwoDecimalDigitFormat,
                        ExcelExtensions.GrandTotalRowColor,
                        true,
                        true,
                        false);
                    twoDecimalPositiveNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    twoDecimalPositiveNumberStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    break;
            }

            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.SiteName;
            currentCellRange.Style = textStyle;
            switch (rowType)
            {
                case ReportRowType.NormalRow:
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.Total:
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    break;

                case ReportRowType.GrandTotal:
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    break;
            }

            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.BetCount;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.Turnover;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.MemberWinLoss;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.MarginPercent;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.AgentWinLoss;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.MasterWinLoss;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.SuperWinLoss;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.TotalWinLoss;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.Commission;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.ExtraPT;
            currentCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.CompanyWinLoss;
            currentCellRange.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;
            currentRow++;
        }

        #endregion ExportExcel

        /// <summary>
        /// Calculates the margin.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="turnover">The turnover.</param>
        /// <returns>The margin.</returns>
        private decimal CalculateMargin(decimal winLoss, decimal turnover)
        {
            return turnover != 0 ? winLoss / turnover * 100 : 0;
        }

        /// <summary>
        /// Gets the name of the sorted column.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>The name of the sorted column.</returns>
        private string GetSortedColumnName(string sortBy)
        {
            var columnName = string.Empty;
            switch (sortBy)
            {
                case "TotalTurnover":
                    columnName = "Turnover";
                    break;

                case "MemberWinLoss":
                    columnName = "MemberWinLoss";
                    break;

                case "MarginPercent":
                    columnName = "MarginPercent";
                    break;

                case "AgentWinLoss":
                    columnName = "AgentWinLoss";
                    break;

                case "MasterWinLoss":
                    columnName = "MasterWinLoss";
                    break;

                case "SuperWinLoss":
                    columnName = "SuperWinLoss";
                    break;

                case "TotalWinLoss":
                    columnName = "TotalWinLoss";
                    break;

                case "CompanyWinLoss":
                    columnName = "CompanyWinLoss";
                    break;

                case "Commission":
                    columnName = "Commission";
                    break;

                case "ExtraPT":
                    columnName = "ExtraPT";
                    break;
            }

            return columnName;
        }

        public List<ProductInfo> GetProducts(GetProductsCriteria criteria)
        {
            var rawData = this.GetReportData<dynamic>(criteria);
            List<ProductInfo> products = new List<ProductInfo>();
            var productGroups = rawData.GroupBy(rawInfo => rawInfo.ProductName);

            foreach (var productGroup in productGroups)
            {
                var product = new ProductInfo
                {
                    Name = Converter.ToString2(productGroup.Key),
                    ChildsInfo = new List<ProductInfo>()
                };

                if (MiscConstants.ProductsHaveSports.Contains(product.Name.ToLower()))
                {
                    products.Add(product);
                    continue;
                }

                var sportGroups = productGroup.GroupBy(rawInfo => new { rawInfo.SportID, rawInfo.SportName });

                foreach (var sportGroup in sportGroups)
                {
                    var sport = CreateSportInfo(sportGroup.Key);
                    foreach (var data in sportGroup)
                    {
                        sport.ChildsInfo.Add(CreateProductInfo(data));
                    }

                    product.ChildsInfo.Add(sport);
                }

                products.Add(product);
            }

            return products;
        }

        public ProductInfo GenerateSports(IEnumerable<dynamic> rawData)
        {
            if (rawData.Count() == 0)
            {
                return new ProductInfo
                {
                    Id = 0,
                    ChildsInfo = new List<ProductInfo>()
                };
            }

            var indGroup = new ProductInfo
            {
                Id = Converter.ToInt(rawData.FirstOrDefault().SportID),
                ChildsInfo = new List<ProductInfo>()
            };
            indGroup.Name = this.GetSportName(indGroup.Id);
            indGroup.Id = GetGroupId(indGroup.Id);
            foreach (var data in rawData)
            {
                indGroup.ChildsInfo.Add(CreateProductInfo(data));
            }

            return indGroup;
        }

        public int GetGroupId(int id)
        {
            switch (id)
            {
                case MiscConstants.MiscSoccerGroupId:
                    return MiscConstants.MiscSoccerGroupId;

                case MiscConstants.MiscParLayGroupId:
                    return MiscConstants.MiscParLayGroupId;

                case MiscConstants.MiscBasketballGroupId:
                    return MiscConstants.MiscBasketballGroupId;

                case MiscConstants.MiscEsportsGroupId:
                    return MiscConstants.MiscEsportsGroupId;

                case MiscConstants.SBAdjustmentId:
                    return MiscConstants.SBAdjustmentId;
            }

            return -111;
        }

        public ProductInfo GenerateOtherSports(IEnumerable<dynamic> rawData)
        {
            var indGroup = CreateGroupInfo(MiscConstants.MiscOtherSportId);
            indGroup.Name = Labels.Others;
            var sportGroups = rawData.GroupBy(rawinfo => new { rawinfo.SportID, rawinfo.SportName });
            foreach (var sportGroup in sportGroups)
            {
                var sport = CreateSportInfo(sportGroup.Key);
                foreach (var data in sportGroup)
                {
                    sport.ChildsInfo.Add(CreateProductInfo(data));
                }

                indGroup.ChildsInfo.Add(sport);
            }

            return indGroup;
        }

        public ProductInfo CreateSportInfo(dynamic sportGroup)
        {
            return new ProductInfo
            {
                Id = Converter.ToInt(sportGroup.SportID),
                Name = Converter.ToString2(sportGroup.SportName),
                ChildsInfo = new List<ProductInfo>()
            };
        }

        public ProductInfo CreateProductInfo(dynamic data)
        {
            return new ProductInfo
            {
                Id = Converter.ToInt(data.Bettype),
                Name = Converter.ToString2(data.BettypeName)
            };
        }

        public ProductInfo CreateGroupInfo(dynamic key)
        {
            return new ProductInfo
            {
                Id = Converter.ToInt(key),
                ChildsInfo = new List<ProductInfo>()
            };
        }

        public ProductInfo AddSportGroup(IGrouping<dynamic, dynamic> rawData)
        {
            var indGroup = CreateGroupInfo(rawData.Key.SportID);
            indGroup.Name = Converter.ToString2(rawData.Key.SportName);
            foreach (var data in rawData)
            {
                indGroup.ChildsInfo.Add(CreateProductInfo(data));
            }

            return indGroup;
        }

        public List<ProductInfo> CalculateSportData(IEnumerable<dynamic> rawData, List<ProductInfo> sports)
        {
            if (rawData.Where(x => x.ProductID == MiscConstants.MiscSportbookId).Any())
            {
                CreateSportBookGroups(rawData, sports);
            }

            if (rawData.Where(x => x.ProductID == MiscConstants.MiscVirtualSportsId).Any())
            {
                CreateVirtualSportGroup(rawData, sports);
            }

            if (rawData.Where(x => x.ProductID == MiscConstants.MiscNG2ProductID).Any())
            {
                CreateNG2SportGroup(rawData, sports);
            }

            if (rawData.Where(x => x.ProductID == 3).Any())
            {
                CreateNGSportGroup(rawData, sports);
            }

            if (rawData.Where(x => x.ProductID == MiscConstants.MiscBetradarVirtualSportsId).Any())
            {
                CreateBetradarVirtualSportGroup(rawData, sports);
            }

            if (rawData.Where(x => x.ProductID == MiscConstants.BetBuilderProductID).Any())
            {
                CreateBetBuilderSportGroup(rawData, sports);
            }

            if (rawData.Where(x => x.ProductID == MiscConstants.SabaClubProductId).Any())
            {
                CreateSabaClubSportGroup(rawData, sports);
            }

            return sports;
        }

        public List<ProductInfo> CreateSportBookGroups(IEnumerable<dynamic> rawData, List<ProductInfo> sports)
        {
            var sportbookRaw = rawData.Where(r => r.ProductID == MiscConstants.MiscSportbookId);
            var soccerGroupData = sportbookRaw.Where(r => r.SportID == MiscConstants.MiscSoccerGroupId);
            var otherSportGroupData = sportbookRaw.Where(r =>
                r.SportID != MiscConstants.MiscSoccerGroupId
                && r.SportID != MiscConstants.MiscParLaySportId
                && r.SportID != MiscConstants.MiscBasketballGroupId
                && r.SportID != MiscConstants.MiscEsportsGroupId
                && r.SportID != MiscConstants.SBAdjustmentId);
            var parlayGroupData = sportbookRaw.Where(r => r.SportID == MiscConstants.MiscParLaySportId);
            var basketballGroupData = sportbookRaw.Where(r => r.SportID == MiscConstants.MiscBasketballGroupId);
            var esportsGroupData = sportbookRaw.Where(r => r.SportID == MiscConstants.MiscEsportsGroupId);
            var sbAdjustmentGroupData = sportbookRaw.Where(r => r.SportID == MiscConstants.SBAdjustmentId);

            sports.Add(GenerateSports(soccerGroupData));
            sports.Add(GenerateSports(basketballGroupData));
            sports.Add(GenerateSports(esportsGroupData));
            sports.Add(GenerateOtherSports(otherSportGroupData));
            sports.Add(GenerateSports(parlayGroupData));
            sports.Add(GenerateSports(sbAdjustmentGroupData));

            return sports;
        }

        public List<ProductInfo> CreateVirtualSportGroup(IEnumerable<dynamic> rawData, List<ProductInfo> sports)
        {
            var virtualsportRaw = rawData.Where(r => r.ProductID == MiscConstants.MiscVirtualSportsId);
            var virtualsportGroups = virtualsportRaw.GroupBy(rawinfo => new { rawinfo.SportID, rawinfo.SportName });
            foreach (var virtualsportGroup in virtualsportGroups)
            {
                sports.Add(AddSportGroup(virtualsportGroup));
            }

            return sports;
        }

        public List<ProductInfo> CreateBetradarVirtualSportGroup(IEnumerable<dynamic> rawData, List<ProductInfo> sports)
        {
            var virtualsportRaw = rawData.Where(r => r.ProductID == MiscConstants.MiscBetradarVirtualSportsId);
            var virtualsportGroups = virtualsportRaw.GroupBy(rawinfo => new { rawinfo.SportID, rawinfo.SportName });
            foreach (var virtualsportGroup in virtualsportGroups)
            {
                sports.Add(AddSportGroup(virtualsportGroup));
            }

            return sports;
        }

        public List<ProductInfo> CreateNG2SportGroup(IEnumerable<dynamic> rawData, List<ProductInfo> sports)
        {
            var numberGame2sportRaw = rawData.Where(r => r.ProductID == MiscConstants.MiscNG2ProductID);
            var numberGame2sportGroups = numberGame2sportRaw.GroupBy(rawinfo => new { rawinfo.SportID, rawinfo.SportName });
            foreach (var numberGame2sportGroup in numberGame2sportGroups)
            {
                sports.Add(AddSportGroup(numberGame2sportGroup));
            }

            return sports;
        }

        public List<ProductInfo> CreateNGSportGroup(IEnumerable<dynamic> rawData, List<ProductInfo> sports)
        {
            var numberGamesportRaw = rawData.Where(r => r.ProductID == 3);
            var numberGamesportGroups = numberGamesportRaw.GroupBy(rawinfo => new { rawinfo.SportID, rawinfo.SportName });
            foreach (var numberGamesportGroup in numberGamesportGroups)
            {
                sports.Add(AddSportGroup(numberGamesportGroup));
            }

            return sports;
        }

        public List<ProductInfo> CreateBetBuilderSportGroup(IEnumerable<dynamic> rawData, List<ProductInfo> sports)
        {
            var betBuilderRaw = rawData.Where(r => r.ProductID == MiscConstants.BetBuilderProductID);
            var betBuilderSportGroups = betBuilderRaw.GroupBy(rawinfo => new { rawinfo.SportID, rawinfo.SportName });
            foreach (var group in betBuilderSportGroups)
            {
                if (sports.FirstOrDefault(x => x.Name == group.Key.SportName) != null)
                {
                    foreach (var item in group)
                    {
                        sports.FirstOrDefault(x => x.Name == group.Key.SportName).ChildsInfo.Add(new ProductInfo
                        {
                            Id = item.Bettype,
                            Name = item.BettypeName
                        });
                    }
                }
                else
                {
                    sports.Add(AddSportGroup(group));
                }
            }

            return sports;
        }

        public List<ProductInfo> CreateSabaClubSportGroup(IEnumerable<dynamic> rawData, List<ProductInfo> sports)
        {
            var sportRaw = rawData.Where(r => r.ProductID == 42);
            var sportGroups = sportRaw.GroupBy(rawinfo => new { rawinfo.SportID, rawinfo.SportName });
            foreach (var sportGroup in sportGroups)
            {
                sports.Add(AddSportGroup(sportGroup));
            }

            return sports;
        }

        public List<ProductInfo> GetSports(GetProductsCriteria criteria)
        {
            var rawData = this.GetReportData<dynamic>(criteria);
            List<ProductInfo> sports = new List<ProductInfo>();
            var sportTypeList = rawData.Select(info => info.SportID as dynamic).Distinct().ToList();
            BetCountService._sportTypeNameCache = CacheRef.SaveCacheRefResponse(
                sportTypeList, CacheRef.GetSportName, ref BetCountService._sportTypeNameCache);
            CalculateSportData(rawData, sports);
            SessionManager.Set<List<ProductInfo>>(MiscConstants.MiscSportsSessionKey, sports);
            return sports;
        }

        public string GetSportName(int sportId)
        {
            switch (sportId)
            {
                case MiscConstants.MiscSoccerGroupId:
                    return CacheRef.GetSportName(SportContants.Soccer);

                case MiscConstants.MiscParLayGroupId:
                    return Labels.Parlay;

                case MiscConstants.MiscBasketballGroupId:
                    return CacheRef.GetSportName(SportContants.Basketball);

                case MiscConstants.MiscEsportsGroupId:
                    return "E-Sports";

                case MiscConstants.SBAdjustmentId:
                    return "SB Adjustment";
            }

            return string.Empty;
        }
    }
}