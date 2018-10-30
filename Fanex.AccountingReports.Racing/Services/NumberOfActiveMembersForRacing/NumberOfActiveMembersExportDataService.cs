namespace Fanex.AccountingReports.Racing.Services.Implement.NumberOActiveMembersForRacing
{
    using System;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Models.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Resources;

    /// <summary>
    /// Export Data Service.
    /// </summary>
    public class NumberOfActiveMembersExportDataService : ServiceBase
    {
        private string _numberOfActiveMemberReportSessionKey = "Fanex.AccountingReports.Racing.NumberOfActiveMember." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private NumberOfActiveMembersService _numberOfActiveMemberService;

        public NumberOfActiveMembersExportDataService()
        {
            this._numberOfActiveMemberService = new NumberOfActiveMembersService();
        }

        /// <summary>
        /// Exports the number of active member.
        /// </summary>
        /// <param name="getNumberOfActiveMembersCriteria">The get number of active members criteria.</param>
        public void ExportNumberOfActiveMembers(NumberOfActiveMembersCriteria getNumberOfActiveMembersCriteria)
        {
            var result = new NumberOfActiveMembers();

            try
            {
                var fullReport = SessionManager.Get<NumberOfActiveMembers>(_numberOfActiveMemberReportSessionKey);

                if (fullReport == null)
                {
                    fullReport = _numberOfActiveMemberService.GetNumberOfActiveMembers(getNumberOfActiveMembersCriteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = RacingResources.NumberOfActiveMember_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportTitle(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    getNumberOfActiveMembersCriteria);

                this.ExportNumberOfActiveMembersContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport);

                string reportName = string.Format(
                   RacingResources.NumberOfActiveMember_ReportName,
                   getNumberOfActiveMembersCriteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                   getNumberOfActiveMembersCriteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the report title.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="getNumberOfActiveMembersCriteria">The get number of active members criteria.</param>
        private void ExportReportTitle(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberOfActiveMembersCriteria getNumberOfActiveMembersCriteria)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;

            worksheet.Columns[startColumn].Width = 400 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 1].Width = 200 * ExcelExtensions.FontWidth;

            var currentColumn = startColumn;
            CellRange currentCellRange;

            var title = string.Format(
                                    RacingResources.NumberOfActiveMember_Title,
                                    getNumberOfActiveMembersCriteria.FromDate.ToShortFormatedDate(),
                                    getNumberOfActiveMembersCriteria.ToDate.ToShortFormatedDate());
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;
        }

        /// <summary>
        /// Exports the content of the number of active members.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="reportData">The report data.</param>
        private void ExportNumberOfActiveMembersContent(
           ref ExcelWorksheet worksheet,
           int startRow,
           int startColumn,
           ref int currentRow,
           NumberOfActiveMembers reportData)
        {
            ExcelCell currentCell;
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            var intNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.IntNumberFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            intNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var currentColumn = startColumn;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.TotalActiveMember;
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = reportData.TotalActiveMembers;
            currentCell.Style = intNumberStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            currentColumn++;
            currentRow++;
        }
    }
}