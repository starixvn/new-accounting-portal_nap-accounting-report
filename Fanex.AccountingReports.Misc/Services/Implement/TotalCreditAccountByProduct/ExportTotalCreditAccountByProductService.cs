// -----------------------------------------------------------------------
// <copyright file="ExportTotalCreditAccountByProductService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.06.05">Created</change>
// </history>
namespace Fanex.AccountingReports.Misc.Services.Implement.TotalCreditAccountByProduct
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.TotalCreditAccountByProduct;
    using Fanex.AccountingReports.Misc.Models.TotalCreditAccountByProduct;
    using Fanex.AccountingReports.Misc.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class TotalCreditAccountByProductService
    {
        private const short GrandTotalColorIndex = 49;

        private const short GrandTotalBorderColorIndex = 51;

        private string _intNumberFormat = "#,##0";

        /// <summary>
        /// Exports the total credit account by product data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportTotalCreditAccountByProductData(GetTotalCreditAccountByProductCriteria criteria)
        {
            var report = SessionManager.Get<TotalCreditAccountByProduct>(_totalCreditAccountByProductSessionKey);

            if (report == null)
            {
                report = this.GetTotalCreditAccountByProductData(criteria);
            }

            HSSFWorkbook excelFile = new HSSFWorkbook();
            this.SetColorIndex(excelFile);
            string productName = this.GetProductBySportType(criteria.SportType);
            string sheetName = string.Format(MiscResources.TotalCreditAccountByProduct_SheetName, productName);
            ISheet worksheet = excelFile.CreateSheet(sheetName);
            int currentRow = 0;

            this.InitTotalCreditAccountByProductColumn(worksheet);
            this.ExportTotalCreditAccountByProductTitle(excelFile, ref worksheet, ref currentRow, criteria);
            currentRow += 3;

            this.ExportTotalCreditAccountByProductHeader(excelFile, ref worksheet, ref currentRow);
            currentRow = 5;
            this.ExportTotalCreditAccountByProductContent(excelFile, ref worksheet, ref currentRow, report);
            this.ExportTotalCreditAccountByProductFooter(excelFile, ref worksheet, ref currentRow, report.Total);
            string reportTitleFormat = this.GetTotalCreditAccountByProductReportTitleFormat(criteria);
            string reportNameFormat = reportTitleFormat.Replace(" ", "_");
            string reportName = string.Format(reportNameFormat, productName.Replace(" ", "_"));
            excelFile.ToReponseStream(reportName);
        }

        /// <summary>
        /// Exports the bet count detail data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportBetCountDetailData(GetBetCountDetailCriteria criteria)
        {
            var report = SessionManager.Get<BetCountDetail>(_betCountDetailSessionKey);

            if (report == null)
            {
                report = this.GetBetCountDetailByCustId(criteria);
            }

            HSSFWorkbook excelFile = new HSSFWorkbook();
            this.SetColorIndex(excelFile);
            var productName = this.GetProductBySportType(criteria.SportType);
            string sheetName = string.Format(MiscResources.BetCountDetail_SheetName, productName);
            ISheet worksheet = excelFile.CreateSheet(sheetName);
            int currentRow = 0;

            this.InitBetCountDetailColumn(worksheet);
            this.ExportBetCountDetailTitle(excelFile, ref worksheet, ref currentRow, criteria);
            currentRow += 3;
            this.ExportBetCountDetailHeader(excelFile, ref worksheet, ref currentRow);
            currentRow++;
            this.ExportBetCountDetailContent(excelFile, ref worksheet, ref currentRow, report);
            this.ExportBetCountDetailFooter(excelFile, ref worksheet, ref currentRow, report.Total);
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            string reportName = string.Format(
                MiscResources.BetCountDetail_ReportName,
                productName.Replace(" ", "_"),
                firstDayOfMonth.ToShortFormatedDateWithSlash().Replace("/", "_"),
                DateTime.Now.ToShortFormatedDateWithSlash().Replace("/", "_"),
                criteria.SuperUserName);
            excelFile.ToReponseStream(reportName);
        }

        private void InitTotalCreditAccountByProductColumn(ISheet worksheet)
        {
            for (int i = 0; i < 10; i++)
            {
                worksheet.SetColumnWidth(i, 170 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 60 * NpoiExcelExtensions.FontWidth);
        }

        private void ExportTotalCreditAccountByProductTitle(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            GetTotalCreditAccountByProductCriteria criteria)
        {
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            int currentColumn = 0;
            string productName = this.GetProductBySportType(criteria.SportType);
            string reportTitleFormat = this.GetTotalCreditAccountByProductReportTitleFormat(criteria);
            string reportTitle = string.Format(reportTitleFormat, productName);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 9, reportTitle, headerStyle);
        }

        /// <summary>
        /// Exports the total credit account by product header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        private void ExportTotalCreditAccountByProductHeader(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow)
        {
            IRow row = null;
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            int currentColumn = 0;
            row = worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.Master, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.Agent, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.Member, headerStyle);

            currentRow++;
            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Enabled, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Disabled, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Enabled, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Disabled, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Enabled, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Disabled, headerStyle);

            currentColumn = 0;
            currentRow = 3;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, "#", headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Super, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Status, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Currency, headerStyle);
        }

        /// <summary>
        /// Exports the total content of the credit account by product.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportTotalCreditAccountByProductContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            TotalCreditAccountByProduct report)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle centerTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle intStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false);
            intStyle.DataFormat = dataFormat.GetFormat(_intNumberFormat);
            IRow row = null;
            int currentColumn = 0;
            for (int i = 0; i < report.ReportRows.Count; i++)
            {
                row = worksheet.CreateRow(currentRow);
                var rowData = report.ReportRows[i];
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, i + 1, intStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.UserName, textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    rowData.IsDisable ? Labels.Disabled : Labels.Enabled,
                    centerTextStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    rowData.Currency,
                    centerTextStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.MasterEnable, intStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.MasterDisable, intStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.AgentEnable, intStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.AgentDisable, intStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.MemberEnable, intStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.MemberDisable, intStyle);
                currentRow++;
                currentColumn = 0;
            }
        }

        /// <summary>
        /// Exports the total credit account by product footer.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="total">The total.</param>
        private void ExportTotalCreditAccountByProductFooter(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            TotalCreditAccountByProduct total)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            textStyle.BorderTop = BorderStyle.Medium;
            textStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle intStyle = NpoiExcelExtensions.IntStyle(excelFile, GrandTotalColorIndex, true);
            intStyle.BorderTop = BorderStyle.Medium;
            intStyle.TopBorderColor = GrandTotalBorderColorIndex;
            intStyle.DataFormat = dataFormat.GetFormat(_intNumberFormat);
            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 3, Labels.Total, textStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.MasterEnable, intStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.MasterDisable, intStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.AgentEnable, intStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.AgentDisable, intStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.MemberEnable, intStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.MemberDisable, intStyle);
        }

        private void InitBetCountDetailColumn(ISheet worksheet)
        {
            for (int i = 0; i < 5; i++)
            {
                worksheet.SetColumnWidth(i, 170 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 60 * NpoiExcelExtensions.FontWidth);
        }

        /// <summary>
        /// Exports the bet count detail title.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportBetCountDetailTitle(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            GetBetCountDetailCriteria criteria)
        {
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            int currentColumn = 0;
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            string productName = this.GetProductBySportType(criteria.SportType);
            string reportTitle = string.Format(
                MiscResources.BetCountDetail_ReportTitle,
                productName,
                firstDayOfMonth.ToShortFormatedDate(),
                DateTime.Now.ToShortFormatedDate(),
                criteria.SuperUserName);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 4, reportTitle, headerStyle);
        }

        /// <summary>
        /// Exports the bet count detail header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        private void ExportBetCountDetailHeader(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow)
        {
            IRow row = null;
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            int currentColumn = 0;
            row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, "#", headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Member, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Status, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.SettledBetCount, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.RunningBetCount, headerStyle);
        }

        /// <summary>
        /// Exports the content of the bet count detail.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportBetCountDetailContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            BetCountDetail report)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle centerTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle intStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false);
            intStyle.DataFormat = dataFormat.GetFormat(_intNumberFormat);
            IRow row = null;
            int currentColumn = 0;
            for (int i = 0; i < report.ReportRows.Count; i++)
            {
                row = worksheet.CreateRow(currentRow);
                var rowData = report.ReportRows[i];
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, i + 1, intStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.UserName, textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    rowData.IsDisable ? Labels.Disabled : Labels.Enabled,
                    centerTextStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.SettledBetCount, intStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowData.RunningBetCount, intStyle);
                currentRow++;
                currentColumn = 0;
            }
        }

        /// <summary>
        /// Exports the bet count detail footer.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="total">The total.</param>
        private void ExportBetCountDetailFooter(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            BetCountDetail total)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            textStyle.BorderTop = BorderStyle.Medium;
            textStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle intStyle = NpoiExcelExtensions.IntStyle(excelFile, GrandTotalColorIndex, true);
            intStyle.BorderTop = BorderStyle.Medium;
            intStyle.TopBorderColor = GrandTotalBorderColorIndex;
            intStyle.DataFormat = dataFormat.GetFormat(_intNumberFormat);
            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 2, Labels.Total, textStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.SettledBetCount, intStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.RunningBetCount, intStyle);
        }

        /// <summary>
        /// Sets the index of the color.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void SetColorIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private string GetProductBySportType(int id)
        {
            string product = string.Empty;
            switch (id)
            {
                case ProductContants.Casino:
                    product = Labels.Casino;
                    break;

                case ProductContants.Racing:
                    product = Labels.Racing;
                    break;

                case ProductContants.Bingo:
                    product = Labels.Bingo;
                    break;

                case ProductContants.NumberGame:
                    product = Labels.NumberGame;
                    break;

                case ProductContants.LiveCasino:
                    product = Labels.LiveCasino;
                    break;

                case ProductContants.VirtualSports:
                    product = Labels.VirtualSports;
                    break;

                case ProductContants.Keno:
                    product = Labels.Keno;
                    break;

                case ProductContants.ColossusBets:
                    product = Labels.ColossusBets;
                    break;

                case ProductContants.Allbet:
                    product = Labels.Allbet;
                    break;

                case ProductContants.MetricGamingSuperlive:
                    product = Labels.MetricGamingSuperLive;
                    break;

                case ProductContants.MacauGames:
                    product = Labels.MacauGames;
                    break;

                default:
                    break;
            }

            return product;
        }

        /// <summary>
        /// Gets the total credit account by product report title format.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Report title format.</returns>
        private string GetTotalCreditAccountByProductReportTitleFormat(GetTotalCreditAccountByProductCriteria criteria)
        {
            string reportTitleFormat = string.Empty;

            if (criteria.ShowEnabled == true && criteria.ShowDisabled == true)
            {
                reportTitleFormat = "Total Credit Accounts Enabled and Disabled on {0}";
            }
            else if (criteria.ShowEnabled == true && criteria.ShowDisabled == false)
            {
                reportTitleFormat = "Total Credit Accounts Enabled on {0}";
            }
            else
            {
                reportTitleFormat = "Total Credit Accounts Disabled on {0}";
            }

            return reportTitleFormat;
        }
    }
}