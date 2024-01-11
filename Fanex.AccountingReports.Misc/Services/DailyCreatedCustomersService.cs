namespace Fanex.AccountingReports.Misc.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Criteria.DailyCreatedCustomers;
    using Fanex.AccountingReports.Misc.Models.DailyCreatedCustomers;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyCreatedCustomersService : ServiceBase
    {
        /// <summary>
        /// The maximum number of rows that one sheet supports.
        /// </summary>
        private static readonly int MaxRowsSheet = 65000;

        /// <summary>
        /// The name of total cell.
        /// </summary>
        private static readonly string TotalName = "Total";

        /// <summary>
        /// The session key to get report.
        /// </summary>
        private string _dailyCreatedCustomersSessionKey;

        /// <summary>
        /// The session key to get detail report.
        /// </summary>
        private string _dailyCreatedCustomersDetailSessionKey;

        public DailyCreatedCustomersService()
        {
            this._dailyCreatedCustomersSessionKey = "Fanex.AccountingReports.Misc.DailyCreatedCustomers.DailyCreatedCustomersReport."
                                                        + AppSession.CurrentUser.Username;
            this._dailyCreatedCustomersDetailSessionKey = "Fanex.AccountingReports.Misc.DailyCreatedCustomers.DailyCreatedCustomersDetailsReport."
                                                        + AppSession.CurrentUser.Username;
        }

        /// <summary>
        /// Type of Report.
        /// </summary>
        private enum ReportRowType
        {
            /// <summary>
            /// The total.
            /// </summary>
            Total = 1,

            /// <summary>
            /// The grand total.
            /// </summary>
            GrandTotal = 2,

            /// <summary>
            /// The normal even row.
            /// </summary>
            NormalEvenRow = 3,

            /// <summary>
            /// The normal odd row.
            /// </summary>
            NormalOddRow = 4
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public SummaryInformation GetReport(GetDailyCreatedCustomersCriteria criteria)
        {
            var report = new SummaryInformation
            {
                CreatingDetails = new List<SummaryInformation>(),
            };

            if (!criteria.IsValid())
            {
                return report;
            }

            var rawData = GetReportData<dynamic>(criteria);
            foreach (var row in rawData)
            {
                DateTime createdDate = Converter.ToDateTime(row.CreateDate);
                SummaryInformation info = new SummaryInformation
                {
                    CreatedDate = createdDate.ToShortFormatedDate(),
                    SuperAmount = Converter.ToInt(row.AmountSAg),
                    DirectMasterAmount = Converter.ToInt(row.AmountMAg),
                    DirectAgentAmount = Converter.ToInt(row.AmountAg),
                    MasterAmount = Converter.ToInt(row.AmountMaster),
                    AgentAmount = Converter.ToInt(row.AmountAgent),
                    MemberAmount = Converter.ToInt(row.AmountMember)
                };

                report.SumDataToTotal(info);
                report.CreatingDetails.Add(info);
            }

            return report;
        }

        /// <summary>
        /// Gets the report details.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report details.</returns>
        public List<DetailInformation> GetReportDetails(GetDailyCreatedCustomersDetailsCriteria criteria)
        {
            var report = new List<DetailInformation>();
            if (!criteria.IsValid())
            {
                return report;
            }

            var rawData = GetReportData<dynamic>(criteria);
            foreach (var row in rawData)
            {
                DateTime createdDate = Converter.ToDateTime(row.creatdate);
                DetailInformation info = new DetailInformation
                {
                    CreatedDate = createdDate.ToLongFormatedDate(),
                    UserId = Converter.ToInt(row.custid),
                    UserName = Converter.ToString2(row.username),
                    Creator = Converter.ToString2(row.creator)
                };

                report.Add(info);
            }

            return report;
        }

        #region ExportExcel

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetDailyCreatedCustomersCriteria criteria)
        {
            var report = this.GetReport(criteria);

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            var currentRow = 0;
            if (report != null
                    && report.CreatingDetails != null
                    && report.CreatingDetails.Count > 0)
            {
                var infoLength = report.CreatingDetails.Count;
                var numberSheets = Math.Ceiling((decimal)infoLength / MaxRowsSheet);
                for (int i = 0; i < numberSheets; i++)
                {
                    string sheetName = "Sheet " + (i + 1);
                    ExcelWorksheet workSheet = worksheets.Add(sheetName);

                    int startRow = 0;
                    int startColumn = 0;
                    currentRow = startRow;
                    var startIndex = i * MaxRowsSheet;
                    var endIndex = (i == (numberSheets - 1)) ? infoLength : (i + 1) * MaxRowsSheet;

                    this.ExportReportHeader(ref workSheet, startRow, startColumn, ref currentRow, criteria);
                    this.ExportReportContent(ref workSheet, startRow, startColumn, ref currentRow, criteria, startIndex, endIndex, report);
                }
            }
            else
            {
                string sheetName = MiscResources.DailyCreatedCustomersReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);
                workSheet.ExportEmptyReport(0, 0, ref currentRow, 6);
            }

            excelFile.ReponseStream(string.Format(
                MiscResources.DailyCreatedCustomersReport_ReportName,
                criteria.Fromdate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                criteria.Todate.ToShortFormatedDateWithSlash().Replace('/', '_')));
        }

        /// <summary>
        /// Exports the report details.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReportDetails(GetDailyCreatedCustomersDetailsCriteria criteria)
        {
            var report = this.GetReportDetails(criteria);

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            var currentRow = 0;
            if (report != null
                    && report.Count > 0)
            {
                var infoLength = report.Count;
                var numberSheets = Math.Ceiling((decimal)infoLength / MaxRowsSheet);

                for (int i = 0; i < numberSheets; i++)
                {
                    string sheetName = "Sheet " + (i + 1);
                    ExcelWorksheet workSheet = worksheets.Add(sheetName);

                    int startRow = 0;
                    int startColumn = 0;
                    currentRow = startRow;
                    var startIndex = i * MaxRowsSheet;
                    var endIndex = (i == (numberSheets - 1)) ? infoLength : (i + 1) * MaxRowsSheet;

                    this.ExportReportDetailsHeader(ref workSheet, startRow, startColumn, ref currentRow, criteria);
                    this.ExportReportDetailsContent(ref workSheet, startRow, startColumn, ref currentRow, criteria, startIndex, endIndex, report);
                }
            }
            else
            {
                string sheetName = string.Format(MiscResources.DailyCreatedCustomersDetailsReport_SheetName, criteria.RoleName);
                ExcelWorksheet workSheet = worksheets.Add(sheetName);
                workSheet.ExportEmptyReport(0, 0, ref currentRow, 3);
            }

            excelFile.ReponseStream(string.Format(
                MiscResources.DailyCreatedCustomersDetailsReport_ReportName,
                criteria.RoleName,
                criteria.Date.ToShortFormatedDateWithSlash().Replace('/', '_')));
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetDailyCreatedCustomersCriteria criteria)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            CellRange currentCellRange;
            var currentColumn = startColumn;

            for (int colIndex = 0; colIndex < 7; colIndex++)
            {
                worksheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            var title = string.Format(
                MiscResources.DailyCreatedCustomersReport_ReportTitle,
                criteria.Fromdate.ToShortFormatedDate(),
                criteria.Todate.ToShortFormatedDate());

            currentColumn = startColumn;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 6);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow += 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 6);

            currentColumn = startColumn;

            this.RenderCellHeader(ref worksheet, currentRow, ref currentColumn, true, Labels.Date, headerColor);

            this.RenderCellHeader(ref worksheet, currentRow, ref currentColumn, true, Labels.Super, headerColor);

            this.RenderCellHeader(ref worksheet, currentRow, ref currentColumn, true, Labels.Direct_Master, headerColor);

            this.RenderCellHeader(ref worksheet, currentRow, ref currentColumn, true, Labels.Direct_Agent, headerColor);

            this.RenderCellHeader(ref worksheet, currentRow, ref currentColumn, true, Labels.Master, headerColor);

            this.RenderCellHeader(ref worksheet, currentRow, ref currentColumn, true, Labels.Agent, headerColor);

            this.RenderCellHeader(ref worksheet, currentRow, ref currentColumn, true, Labels.Member, headerColor);

            currentRow++;
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetDailyCreatedCustomersCriteria criteria,
            int startIndex,
            int endIndex,
            SummaryInformation report)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                var info = report.CreatingDetails[i];
                this.ExportRow(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    info,
                    i % 2 == 0 ? ReportRowType.NormalEvenRow : ReportRowType.NormalOddRow);
            }

            this.ExportRow(
                ref worksheet,
                startRow,
                startColumn,
                ref currentRow,
                report,
                ReportRowType.GrandTotal);
        }

        /// <summary>
        /// Exports the row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="info">The information.</param>
        /// <param name="rowType">Type of the row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            SummaryInformation info,
            ReportRowType rowType)
        {
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);

            switch (rowType)
            {
                case ReportRowType.NormalOddRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.NormalEvenRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.EvenRowColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.GrandTotal:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                    textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.GrandTotalRowColor, true, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    intStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    break;
            }

            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = textStyle;

            if (rowType == ReportRowType.GrandTotal)
            {
                currentCellRange.Value = TotalName;
            }
            else
            {
                currentCellRange.Value = info.CreatedDate;
            }

            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.SuperAmount;
            currentCellRange.Style = intStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.DirectMasterAmount;
            currentCellRange.Style = intStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.DirectAgentAmount;
            currentCellRange.Style = intStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.MasterAmount;
            currentCellRange.Style = intStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.AgentAmount;
            currentCellRange.Style = intStyle;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.MemberAmount;
            currentCellRange.Style = intStyle;
            currentColumn++;

            currentRow++;
        }

        /// <summary>
        /// Exports the report details header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportDetailsHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetDailyCreatedCustomersDetailsCriteria criteria)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentColumn = startColumn;
            worksheet.Columns[startColumn].Width = 100 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex < 4; colIndex++)
            {
                worksheet.Columns[startColumn + colIndex].Width = 160 * ExcelExtensions.FontWidth;
            }

            var title = string.Format(
                MiscResources.DailyCreatedCustomersDetailsReport_ReportTitle,
                criteria.RoleName,
                criteria.Date.ToShortFormatedDate());

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 3);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow += 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 3);

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Symbol_Index;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.UserName;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Created_Date;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Created_By;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the content of the report details.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="report">The report.</param>
        private void ExportReportDetailsContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetDailyCreatedCustomersDetailsCriteria criteria,
            int startIndex,
            int endIndex,
            List<DetailInformation> report)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                var info = report[i];
                this.ExportDetailsRow(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    info,
                    i % 2 == 0 ? ReportRowType.NormalEvenRow : ReportRowType.NormalOddRow,
                    i);
            }
        }

        /// <summary>
        /// Exports the details row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="info">The information.</param>
        /// <param name="rowType">Type of the row.</param>
        /// <param name="index">The index.</param>
        private void ExportDetailsRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            DetailInformation info,
            ReportRowType rowType,
            int index)
        {
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);

            switch (rowType)
            {
                case ReportRowType.NormalOddRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.NormalEvenRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.EvenRowColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;
            }

            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Style = intStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCellRange.Value = index + 1;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.UserName;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.CreatedDate;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = info.Creator;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Render cell header of Daily Created Customer.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="mergedCell">The boolean variable to tell that cell is merged or not.</param>
        /// <param name="label">The label of cell.</param>
        /// <param name="headerColor">The color of cell.</param>
        private void RenderCellHeader(
            ref ExcelWorksheet worksheet,
            int currentRow,
            ref int currentColumn,
            bool mergedCell,
            string label,
            Color headerColor)
        {
            CellRange currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = mergedCell;
            currentCellRange.Value = label;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;
        }

        #endregion ExportExcel
    }
}