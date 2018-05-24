namespace Fanex.AccountingReports.Main.Services.Implement.CreditTerm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Models.CreditTerm;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface.CreditTerm;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// The export excel service of Installment Amount report.
    /// </summary>
    public class InstallmentAmountExportExcelService : ServiceBase, IInstallmentAmountExportExcelService
    {
        #region declare variable

        private const string HeaderRowClass = "header";

        private const string OddRowNormalClass = "odd-row";

        private const string OddRowBlueClass = "odd-row-blue-font";

        private const string OddRowRedClass = "odd-row-red-font";

        private const string EvenRowNormalClass = "even-row";

        private const string EvenRowBlueClass = "even-row-blue-font";

        private const string EvenRowRedClass = "even-row-red-font";

        private CustomStyleCell _headerStyle;

        private CustomStyleCell _oddNormalStyle;

        private CustomStyleCell _oddBlueStyle;

        private CustomStyleCell _oddRedStyle;

        private CustomStyleCell _evenNormalStyle;

        private CustomStyleCell _evenBlueStyle;

        private CustomStyleCell _evenRedStyle;

        private string _installmentAmountSessionKey = string.Empty;

        private InstallmentAmountService _reportService;

        #endregion declare variable

        public InstallmentAmountExportExcelService()
        {
            this._reportService = new InstallmentAmountService();

            if (AppSession.CurrentUser != null)
            {
                this._installmentAmountSessionKey = CreditTermExcelUtil.InstallmentAmountPrefixSessionKey
                                                    + AppSession.CurrentUser.Username;
            }
            else
            {
                this._installmentAmountSessionKey = CreditTermExcelUtil.InstallmentAmountPrefixSessionKey;
            }
        }

        #region export report
        /// <summary>
        /// Export Installment Amount report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportInstallmentAmountReport(GetInstallAmountCriteria criteria)
        {
            try
            {
                var sessionKeyWithRoleID = this._installmentAmountSessionKey + "." + criteria.RoleID;

                var report = SessionManager.Get<List<InstallAmount>>(sessionKeyWithRoleID);

                if (report == null)
                {
                    report = this._reportService.GetInstallmentAmountReport(criteria);
                }

                var currentRow = 0;
                var roleName = CreditTermExcelUtil.GetRoleNameByID(criteria.RoleID);

                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = string.Format(
                        MainResources.CreditTermInstallAmount_SheetName,
                        roleName);
                ISheet worksheet = excelFile.CreateSheet(sheetName);

                if (report != null)
                {
                    this.InitialStyle(excelFile);
                    this.ExportReportHeader(excelFile, ref worksheet, ref currentRow, roleName);
                    this.ExportReportContent(excelFile, ref worksheet, ref currentRow, report);
                }

                excelFile.ToReponseStream(string.Format(
                    MainResources.CreditTermInstallAmount_ReportName,
                    roleName));
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
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
            this.InitCellStyle(excelFile, ref this._oddNormalStyle, OddRowNormalClass);

            this._oddBlueStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._oddBlueStyle, OddRowBlueClass);

            this._oddRedStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._oddRedStyle, OddRowRedClass);

            this._evenNormalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._evenNormalStyle, EvenRowNormalClass);

            this._evenBlueStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._evenBlueStyle, EvenRowBlueClass);

            this._evenRedStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._evenRedStyle, EvenRowRedClass);
        }

        /// <summary>
        /// Export header of report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="roleName">The role name.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            string roleName)
        {
            var currentColumn = 0;

            for (var i = 0; i < 6; i++)
            {
                worksheet.SetColumnWidth(i, 150 * ExcelExtensions.FontWidth);
            }

            var reportTitle = string.Format(
                    MainResources.CreditTermInstallAmount_ReportTitle,
                    roleName);
            IRow row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow + 1,
                currentColumn + 5,
                reportTitle,
                this._headerStyle.TextStyle);

            currentRow += 2;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 5));

            currentRow++;
            currentColumn = 0;
            row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow + 1,
                currentColumn,
                Labels.Member,
                this._headerStyle.TextStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Curr, this._headerStyle.TextStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow + 1,
                currentColumn,
                Labels.Balance,
                this._headerStyle.TextStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow + 1,
                currentColumn,
                Labels.BaseRM,
                this._headerStyle.TextStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow + 1,
                currentColumn,
                Labels.InstallmentAmount,
                this._headerStyle.TextStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow + 1,
                currentColumn,
                Labels.DayOfInstallment,
                this._headerStyle.TextStyle);

            currentRow += 2;
        }

        /// <summary>
        /// Export report content.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            List<InstallAmount> report)
        {
            var numberRows = report.Count;
            var cssClass = string.Empty;
            CustomStyleCell cellStyle = new CustomStyleCell(excelFile);

            for (var i = 0; i < numberRows; i++)
            {
                var memberRow = report[i];
                cssClass = i % 2 == 0 ? OddRowBlueClass : EvenRowBlueClass;
                cellStyle = i % 2 == 0 ? this._oddNormalStyle : this._evenNormalStyle;

                this.RenderRow(excelFile, ref worksheet, ref currentRow, memberRow, cssClass, cellStyle);
            }
        }

        /// <summary>
        /// Render row.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="memberRow">The member row.</param>
        /// <param name="cssClass">The class of row.</param>
        /// <param name="styleCell">The style of cell.</param>
        private void RenderRow(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            InstallAmount memberRow,
            string cssClass,
            CustomStyleCell styleCell)
        {
            var currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, memberRow.Username, styleCell.TextStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, memberRow.CurrencyName, styleCell.TextStyle);

            ICellStyle colorDecimalStyle = this.SetColorToDecimal(memberRow.Balance, styleCell.DecimalStyle, cssClass);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, memberRow.Balance, colorDecimalStyle);

            colorDecimalStyle = this.SetColorToDecimal(memberRow.BalanceBase, styleCell.DecimalStyle, cssClass);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, memberRow.BalanceBase, colorDecimalStyle);

            colorDecimalStyle = this.SetColorToDecimal(memberRow.InstallmentAmount, styleCell.DecimalStyle, cssClass);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                memberRow.InstallmentAmount,
                colorDecimalStyle);

            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                memberRow.NumberDayInstallment,
                styleCell.IntStyle);

            currentRow++;
        }

        /// <summary>
        /// Initial the style of cell.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="styleCell">The style of cell.</param>
        /// <param name="rowClass">The class of row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void InitCellStyle(
           HSSFWorkbook excelFile,
           ref CustomStyleCell styleCell,
           string rowClass)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            short fontBlueColor = HSSFColor.Blue.Index;
            short fontRedColor = HSSFColor.Red.Index;
            short fontBlackColor = HSSFColor.Black.Index;
            short cellColor;

            switch (rowClass)
            {
                case HeaderRowClass:
                    styleCell.TextStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
                    break;

                case OddRowNormalClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlackColor);
                    styleCell.IntStyle = NpoiExcelExtensions.IntStyle(excelFile, cellColor, false, fontBlackColor);
                    break;

                case OddRowBlueClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlueColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlueColor);
                    styleCell.IntStyle = NpoiExcelExtensions.IntStyle(excelFile, cellColor, false, fontBlueColor);
                    break;

                case OddRowRedClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontRedColor);
                    styleCell.IntStyle = NpoiExcelExtensions.IntStyle(excelFile, cellColor, false, fontRedColor);
                    break;

                case EvenRowNormalClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)228, (byte)242, (byte)253);
                    cellColor = HSSFColor.Tan.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlackColor);
                    styleCell.IntStyle = NpoiExcelExtensions.IntStyle(excelFile, cellColor, false, fontBlackColor);
                    break;

                case EvenRowBlueClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)228, (byte)242, (byte)253);
                    cellColor = HSSFColor.Tan.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlueColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlueColor);
                    styleCell.IntStyle = NpoiExcelExtensions.IntStyle(excelFile, cellColor, false, fontBlueColor);
                    break;

                case EvenRowRedClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)228, (byte)242, (byte)253);
                    cellColor = HSSFColor.Tan.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontRedColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontRedColor);
                    styleCell.IntStyle = NpoiExcelExtensions.IntStyle(excelFile, cellColor, false, fontRedColor);
                    break;
            }
        }

        /// <summary>
        /// Set color to decimal.
        /// </summary>
        /// <param name="cellValue">The value of cell.</param>
        /// <param name="decimalStyle">The style of decimal.</param>
        /// <param name="cssClass">The class of row.</param>
        /// <returns>The new style with color.</returns>
        private ICellStyle SetColorToDecimal(
            double cellValue,
            ICellStyle decimalStyle,
            string cssClass)
        {
            ICellStyle cellStyle = null;

            if (cellValue >= 0)
            {
                switch (cssClass)
                {
                    case OddRowBlueClass:
                        cellStyle = this._oddBlueStyle.DecimalStyle;
                        break;
                    case EvenRowBlueClass:
                        cellStyle = this._evenBlueStyle.DecimalStyle;
                        break;
                }
            }
            else
            {
                switch (cssClass)
                {
                    case OddRowBlueClass:
                        cellStyle = this._oddRedStyle.DecimalStyle;
                        break;
                    case EvenRowBlueClass:
                        cellStyle = this._evenRedStyle.DecimalStyle;
                        break;
                }
            }

            return cellStyle;
        }

        #endregion export report

        public struct CustomStyleCell
        {
            public CustomStyleCell(HSSFWorkbook excelFile)
            {
                TextStyle = excelFile.CreateCellStyle();
                DecimalStyle = excelFile.CreateCellStyle();
                IntStyle = excelFile.CreateCellStyle();
            }

            public ICellStyle TextStyle { get; set; }

            public ICellStyle DecimalStyle { get; set; }

            public ICellStyle IntStyle { get; set; }
        }
    }
}
