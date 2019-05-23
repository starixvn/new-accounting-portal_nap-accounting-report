namespace Fanex.AccountingReports.Main.Services.Implement.CreditTerm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Models.CreditTerm;
    using Fanex.AccountingReports.Main.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// The export excel of Credit Term report.
    /// </summary>
    public class CreditTermExportExcelService : ServiceBase
    {
        #region declare variable

        private const string HeaderRowClass = "header";

        private const string OddRowClass = "odd-row";

        private const string OddRowRedClass = "odd-row-red-font";

        private const string EvenRowClass = "even-row";

        private const string EvenRowRedClass = "even-row-red-font";

        private const string LimitParentRowClass = "limit-parent-row";

        private const string LimitParentRowRedClass = "limit-parent-row-red-font";

        private const string LimitChildRowClass = "limit-child-row";

        private const string LimitChildRowRedClass = "limit-child-row-red-font";

        private CustomStyleCell _headerStyle;

        private CustomStyleCell _oddNormalStyle;

        private CustomStyleCell _oddRedStyle;

        private CustomStyleCell _evenNormalStyle;

        private CustomStyleCell _evenRedStyle;

        private CustomStyleCell _limitParentNormalStyle;

        private CustomStyleCell _limitParentRedStyle;

        private CustomStyleCell _limitChildNormalStyle;

        private CustomStyleCell _limitChildRedStyle;

        private CreditTermService _reportService;

        public CreditTermExportExcelService()
        {
            this._reportService = new CreditTermService();
        }

        #endregion declare variable

        #region export report.

        /// <summary>
        /// Export Pre-Agreed report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportCreditTermReport(GetCreditTermCriteria criteria)
        {
            var report = this._reportService.GetCreditTermReport(criteria);

            HSSFWorkbook excelFile = new HSSFWorkbook();
            var sheetName = MainResources.CreditTermPreAgreed_SheetName;
            ISheet worksheet = excelFile.CreateSheet(sheetName);

            var currentRow = 0;

            if (report != null && report.Count > 0)
            {
                this.InitialStyle(excelFile);
                this.ExportReportHeader(excelFile, ref worksheet, ref currentRow, this._headerStyle.TextStyle);
                this.ExportReportContent(excelFile, ref worksheet, ref currentRow, report, criteria);
            }

            excelFile.ResponseStreamExcel(MainResources.CreditTermPreAgreed_ReportName);
        }

        /// <summary>
        /// Initial the common style.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void InitialStyle(HSSFWorkbook excelFile)
        {
            this._headerStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._headerStyle, HeaderRowClass);

            this._oddNormalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._oddNormalStyle, OddRowClass);

            this._oddRedStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._oddRedStyle, OddRowRedClass);

            this._evenNormalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._evenNormalStyle, EvenRowClass);

            this._evenRedStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._evenRedStyle, EvenRowRedClass);

            this._limitParentNormalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._limitParentNormalStyle, LimitParentRowClass);

            this._limitParentRedStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._limitParentRedStyle, LimitParentRowRedClass);

            this._limitChildNormalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._limitChildNormalStyle, LimitChildRowClass);

            this._limitChildRedStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._limitChildRedStyle, LimitChildRowRedClass);
        }

        /// <summary>
        /// Export header of report.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="headerStyle">The header style.</param>
        private void ExportReportHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            ICellStyle headerStyle)
        {
            var currentColumn = 0;

            for (var i = 0; i < 11; i++)
            {
                worksheet.SetColumnWidth(i, 145 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 80 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(1, 80 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(2, 120 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(3, 100 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(7, 370 * NpoiExcelExtensions.FontWidth);

            var reportTitle = MainResources.CreditTermPreAgreed_ReportTitle;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 10, reportTitle, headerStyle);

            currentRow += 2;

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 10));
            currentRow++;
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            currentColumn = 0;

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Role, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 1, Labels.UserName, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Curr, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Pre_Agreed_Amount, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Balance, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.BaseRM, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Account_Remark, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Phone, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Checker_Name, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Remark_Time_Stamp, headerStyle);
            currentRow += 2;
        }

        /// <summary>
        /// Export report content.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            List<CreditTerm> report,
            GetCreditTermCriteria criteria)
        {
            var numberParentMember = report.Count;
            CustomStyleCell parentStyle = new CustomStyleCell(excelFile);
            CustomStyleCell childStyle = new CustomStyleCell(excelFile);

            for (var i = 0; i < numberParentMember; i++)
            {
                var parentMember = report[i];
                var cssParentClass = string.Empty;
                var cssChildClass = string.Empty;

                this.GetStyleAndClassForParentAndChildRow(ref parentStyle, ref childStyle, parentMember, ref cssParentClass, ref cssChildClass);

                this.RenderParentRow(excelFile, ref worksheet, ref currentRow, parentMember, parentStyle, cssParentClass);

                if (parentMember.GroupID > 0)
                {
                    var childRows = parentMember.Rows;

                    if (childRows != null && childRows.Count > 0)
                    {
                        var currentColumn = 0;
                        IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                        NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow + (childRows.Count - 1) + 2,
                            currentColumn,
                            string.Empty,
                            this._oddNormalStyle.TextStyle);
                        this.RenderChildHeader(excelFile, ref worksheet, ref currentRow, parentMember.CurrencyName, this._headerStyle.TextStyle);
                        this.RenderAllChildRow(
                            excelFile,
                            ref worksheet,
                            ref currentRow,
                            childRows,
                            parentMember.CurrencyName,
                            childStyle,
                            cssChildClass);
                    }
                }
            }
        }

        /// <summary>
        /// Get style and class for parent and child row.
        /// </summary>
        /// <param name="parentStyle">The style of parent row.</param>
        /// <param name="childStyle">The style of child row.</param>
        /// <param name="parentMember">The parent row.</param>
        /// <param name="cssParentClass">The class of parent row.</param>
        /// <param name="cssChildClass">The class of child row.</param>
        private void GetStyleAndClassForParentAndChildRow(
            ref CustomStyleCell parentStyle,
            ref CustomStyleCell childStyle,
            CreditTerm parentMember,
            ref string cssParentClass,
            ref string cssChildClass)
        {
            if (parentMember.OverLimit)
            {
                cssParentClass = parentMember.GroupID == 0 ? OddRowClass : EvenRowClass;
                cssChildClass = OddRowClass;
                parentStyle = parentMember.GroupID == 0 ? this._oddNormalStyle : this._evenNormalStyle;
                childStyle = this._oddNormalStyle;
            }
            else
            {
                cssParentClass = LimitParentRowClass;
                cssChildClass = LimitChildRowClass;
                parentStyle = this._limitParentNormalStyle;
                childStyle = this._limitChildNormalStyle;
            }
        }

        /// <summary>
        /// Render parent row.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="parentMember">The parent member.</param>
        /// <param name="styleCell">The style of cell.</param>
        /// <param name="cssClass">The class of row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void RenderParentRow(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            CreditTerm parentMember,
            CustomStyleCell styleCell,
            string cssClass)
        {
            var currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, parentMember.RoleName, styleCell.TextStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn + 1,
                parentMember.Username,
                styleCell.UsernameStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                parentMember.CurrencyName,
                styleCell.TextStyle);

            ICellStyle colorDecimalStyle = this.SetColorToDecimal(parentMember.PreAgreeAmount, styleCell.DecimalStyle, cssClass);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                parentMember.PreAgreeAmount,
                colorDecimalStyle);

            colorDecimalStyle = this.SetColorToDecimal(parentMember.Balance, styleCell.DecimalStyle, cssClass);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, parentMember.Balance, colorDecimalStyle);

            colorDecimalStyle = this.SetColorToDecimal(parentMember.BalanceBase, styleCell.DecimalStyle, cssClass);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, parentMember.BalanceBase, colorDecimalStyle);

            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                parentMember.AccountRemark,
                styleCell.TextStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, parentMember.PhoneNumber, styleCell.TextStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, parentMember.CheckerName, styleCell.TextStyle);

            var formatDateTime = CreditTermExcelUtil.GetDateTime(parentMember.AccountRemarkDate);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, formatDateTime, styleCell.TextStyle);

            currentRow++;
        }

        /// <summary>
        /// Render header of child table.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currencyParentName">The currency name of parent member.</param>
        /// <param name="childHeader">The child header.</param>
        private void RenderChildHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            string currencyParentName,
            ICellStyle childHeader)
        {
            var currentColumn = 1;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Role, childHeader);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.UserName, childHeader);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Curr, childHeader);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Balance, childHeader);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, currencyParentName, childHeader);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.RM, childHeader);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 2, Labels.Account_Remark, childHeader);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Remark_Time_Stamp, childHeader);

            currentRow += 2;
        }

        /// <summary>
        /// Render all child row.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="childRows">The child rows.</param>
        /// <param name="currencyParentName">The currency name of parent member.</param>
        /// <param name="styleCell">The style cell.</param>
        /// <param name="cssClass">The class of cell.</param>
        private void RenderAllChildRow(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            List<CreditTerm> childRows,
            string currencyParentName,
            CustomStyleCell styleCell,
            string cssClass)
        {
            var numberChild = childRows.Count;

            for (var i = 0; i < numberChild; i++)
            {
                var childMember = childRows[i];
                this.RenderEachChildRow(
                    excelFile,
                    ref worksheet,
                    ref currentRow,
                    childMember,
                    currencyParentName,
                    styleCell,
                    cssClass);
            }
        }

        /// <summary>
        /// Render each child row.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="childMember">The child member.</param>
        /// <param name="currencyParentName">The currency name of parent.</param>
        /// <param name="styleCell">The style of cell.</param>
        /// <param name="cssClass">The class of cell.</param>
        private void RenderEachChildRow(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            CreditTerm childMember,
            string currencyParentName,
            CustomStyleCell styleCell,
            string cssClass)
        {
            var currentColumn = 1;
            ICellStyle colorDecimalStyle = null;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, childMember.RoleName, styleCell.TextStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, childMember.Username, styleCell.UsernameStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, childMember.CurrencyName, styleCell.TextStyle);

            if (childMember.CurrencyName.Equals(currencyParentName))
            {
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, styleCell.TextStyle);
            }
            else
            {
                colorDecimalStyle = this.SetColorToDecimal(childMember.Balance, styleCell.DecimalStyle, cssClass);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, childMember.Balance, colorDecimalStyle);
            }

            colorDecimalStyle = this.SetColorToDecimal(childMember.BalanceGroup, styleCell.DecimalStyle, cssClass);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, childMember.BalanceGroup, colorDecimalStyle);

            colorDecimalStyle = this.SetColorToDecimal(childMember.BalanceBase, styleCell.DecimalStyle, cssClass);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, childMember.BalanceBase, colorDecimalStyle);

            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn + 2,
                childMember.AccountRemark,
                styleCell.ChildRemarkStyle);

            var formatRemarkDate = CreditTermExcelUtil.GetDateTime(childMember.AccountRemarkDate);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, formatRemarkDate, styleCell.TextStyle);

            currentRow++;
        }

        /// <summary>
        /// Initial style of cell.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="styleCell">The style of cell.</param>
        /// <param name="rowClass">The class of row.</param>
        private void InitCellStyle(
            HSSFWorkbook excelFile,
            ref CustomStyleCell styleCell,
            string rowClass)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            short fontBlackColor = HSSFColor.Black.Index;
            short fontRedColor = HSSFColor.Red.Index;
            short cellColor = 0;

            styleCell = InitCellStyleDetail(
                excelFile,
                palette,
                styleCell,
                rowClass,
                fontBlackColor,
                fontRedColor,
                cellColor);
        }

        /// <summary>
        /// Set cell style by class of row.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="palette">The palette.</param>
        /// <param name="styleCell">The style cell.</param>
        /// <param name="rowClass">The class of row.</param>
        /// <param name="fontBlackColor">The black color.</param>
        /// <param name="fontRedColor">The red color.</param>
        /// <param name="cellColor">The color of cell.</param>
        /// <returns>The CustomStyleCell.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private CustomStyleCell InitCellStyleDetail(
            HSSFWorkbook excelFile,
            HSSFPalette palette,
            CustomStyleCell styleCell,
            string rowClass,
            short fontBlackColor,
            short fontRedColor,
            short cellColor)
        {
            switch (rowClass)
            {
                case HeaderRowClass:
                    styleCell.TextStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
                    break;

                case OddRowClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlackColor);
                    styleCell.UsernameStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.ChildRemarkStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Left, fontBlackColor);
                    break;

                case OddRowRedClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontRedColor);
                    styleCell.UsernameStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.ChildRemarkStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Left, fontRedColor);
                    break;

                case EvenRowClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)223, (byte)234, (byte)255);
                    cellColor = HSSFColor.Tan.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlackColor);
                    styleCell.UsernameStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.ChildRemarkStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Left, fontBlackColor);
                    break;

                case EvenRowRedClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)223, (byte)234, (byte)255);
                    cellColor = HSSFColor.Tan.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontRedColor);
                    styleCell.UsernameStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.ChildRemarkStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Left, fontRedColor);
                    break;

                case LimitParentRowClass:
                    palette.SetColorAtIndex(HSSFColor.Teal.Index, (byte)214, (byte)239, (byte)186);
                    cellColor = HSSFColor.Teal.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlackColor);
                    styleCell.UsernameStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.ChildRemarkStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Left, fontBlackColor);
                    break;

                case LimitParentRowRedClass:
                    palette.SetColorAtIndex(HSSFColor.Teal.Index, (byte)214, (byte)239, (byte)186);
                    cellColor = HSSFColor.Teal.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontRedColor);
                    styleCell.UsernameStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.ChildRemarkStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Left, fontRedColor);
                    break;

                case LimitChildRowClass:
                    palette.SetColorAtIndex(HSSFColor.Turquoise.Index, (byte)232, (byte)235, (byte)211);
                    cellColor = HSSFColor.Turquoise.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlackColor);
                    styleCell.UsernameStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.ChildRemarkStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Left, fontBlackColor);
                    break;

                case LimitChildRowRedClass:
                    palette.SetColorAtIndex(HSSFColor.Turquoise.Index, (byte)232, (byte)235, (byte)211);
                    cellColor = HSSFColor.Turquoise.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontRedColor);
                    styleCell.UsernameStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.ChildRemarkStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Left, fontRedColor);
                    break;
            }

            return styleCell;
        }

        /// <summary>
        /// Set color to decimal.
        /// </summary>
        /// <param name="cellValue">The value of cell.</param>
        /// <param name="decimalStyle">The style of decimal.</param>
        /// <param name="cssClass">The class of cell.</param>
        /// <returns>The new style with color.</returns>
        private ICellStyle SetColorToDecimal(
            decimal cellValue,
            ICellStyle decimalStyle,
            string cssClass)
        {
            ICellStyle colorStyle = null;
            if (cellValue > 0)
            {
                switch (cssClass)
                {
                    case OddRowClass:
                        colorStyle = this._oddRedStyle.DecimalStyle;
                        break;

                    case EvenRowClass:
                        colorStyle = this._evenRedStyle.DecimalStyle;
                        break;

                    case LimitParentRowClass:
                        colorStyle = this._limitParentRedStyle.DecimalStyle;
                        break;

                    case LimitChildRowClass:
                        colorStyle = this._limitChildRedStyle.DecimalStyle;
                        break;
                }
            }
            else
            {
                colorStyle = decimalStyle;
            }

            return colorStyle;
        }

        #endregion export report.

        public struct CustomStyleCell
        {
            public CustomStyleCell(HSSFWorkbook excelFile)
            {
                TextStyle = excelFile.CreateCellStyle();
                DecimalStyle = excelFile.CreateCellStyle();
                UsernameStyle = excelFile.CreateCellStyle();
                ChildRemarkStyle = excelFile.CreateCellStyle();
            }

            public ICellStyle TextStyle { get; set; }

            public ICellStyle DecimalStyle { get; set; }

            public ICellStyle UsernameStyle { get; set; }

            public ICellStyle ChildRemarkStyle { get; set; }
        }
    }
}