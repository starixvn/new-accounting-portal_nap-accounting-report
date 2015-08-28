// -----------------------------------------------------------------------
// <copyright file="ExcelUtility.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Utilities.AGCasinoWinloss
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Utilities.CellStyles;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Criteria.AGCasinoWinloss;
    using Fanex.AccountingReports.Licensee.Models.AGCasinoWinloss;
    using Fanex.AccountingReports.Licensee.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExcelUtility
    {
        private AGCasinoWinlossData _data;

        private HSSFWorkbook _workbook;

        private ISheet _sheet;

        private CustomColor _customColor;

        private BaseCellStyles _cellStyles;

        private GetMainReportCriteria _criteria;

        private bool _isMainReport;

        private int _columnsCount;

        private int _rowsCount;

        private int _startCol = 0;

        private int _startRow = 0;

        public ExcelUtility(AGCasinoWinlossData data, GetMainReportCriteria criteria)
        {
            this._data = data;
            this._workbook = new HSSFWorkbook();
            this._sheet = this._workbook.CreateSheet(Labels.Report);
            this._criteria = criteria;
            this._isMainReport = this.IsMainReport();
            this._columnsCount = this._isMainReport ? AGCasinoWinlossConstants.MainReportColumnCount : AGCasinoWinlossConstants.SMAMReportColumnCount;
            this._rowsCount = this._data.Data.Count;
            this._customColor = new CustomColor(this._workbook);
            this._cellStyles = new BaseCellStyles(this._workbook, this._customColor);
        }

        public HSSFWorkbook ExportExcel()
        {
            this.RenderFile();
            return this._workbook;
        }

        private void RenderFile()
        {
            this.RenderTitle();
            this.RenderHeader();
            this.RenderBody();
            this.RenderSubTotal();
            this.RenderFooter();
        }

        /// <summary>
        /// Renders the title.
        /// </summary>
        private void RenderTitle()
        {
            var titleRow = this._sheet.CreateRow(this._startRow++);
            var titleCell = titleRow.CreateCell(0);
            titleCell.CellStyle = this._cellStyles.ReportTitle;
            titleCell.SetCellValue(this.GetReportTitle(this._criteria));
            for (int i = 1; i < AGCasinoWinlossConstants.ExcelColumnCount; i++)
            {
                titleCell = titleRow.CreateCell(i);
                titleCell.CellStyle = this._cellStyles.ReportTitle;
            }

            this._sheet.AddMergedRegion(new CellRangeAddress(titleRow.RowNum, titleRow.RowNum, 0, AGCasinoWinlossConstants.ExcelColumnCount - 1));
            this._startRow++;
        }

        /// <summary>
        /// Gets the report title.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Report title.</returns>
        private string GetReportTitle(GetMainReportCriteria criteria)
        {
            string reportTitle = string.Empty;
            var criteriaType = criteria.GetType();
            if (criteriaType.Equals(typeof(GetMainReportCriteria)) || criteriaType.Equals(typeof(GetSuperReportCriteria)))
            {
                reportTitle = string.Format(
                    Labels.AGCasinoWinlossTitle,
                    criteria.LicenseeName,
                    criteria.FromDate.ToString(FormatConstants.MMMDDYYYYString),
                    criteria.ToDate.ToString(FormatConstants.MMMDDYYYYString),
                    criteria.CurrencyName);
            }
            else
            {
                var masterCriteria = (GetMasterReportCriteria)criteria;
                reportTitle = string.Format(
                       Labels.AGCasinoWinlossTitle,
                       masterCriteria.CustomerName,
                       masterCriteria.FromDate.ToString(FormatConstants.MMMDDYYYYString),
                       masterCriteria.ToDate.ToString(FormatConstants.MMMDDYYYYString),
                       masterCriteria.CurrencyName);
            }

            return reportTitle;
        }

        /// <summary>
        /// Renders the header.
        /// </summary>
        private void RenderHeader()
        {
            var headerRow = this._sheet.CreateRow(this._startRow++);
            int colIndex = this._startCol;
            this.RenderHeaderCell(headerRow, colIndex++, this._isMainReport ? Labels.Date : Labels.UserName);
            this.RenderHeaderCell(headerRow, colIndex++, Labels.Currency);
            this.RenderHeaderCell(headerRow, colIndex++, Labels.Turnover);
            this.RenderHeaderCell(headerRow, colIndex++, Labels.TotalTrans);
            this.RenderHeaderCell(headerRow, colIndex++, Labels.CustomerWinLoss);
            this.RenderHeaderCell(headerRow, colIndex++, Labels.LicenseeWinLoss);
            this.RenderHeaderCell(headerRow, colIndex++, Labels.AmountDueToAGC);
        }

        private void RenderHeaderCell(IRow headerRow, int colIndex, string textValue)
        {
            var headerCell = headerRow.CreateCell(colIndex);
            headerCell.CellStyle = this._cellStyles.ReportBodyHeader;
            headerCell.SetCellValue(textValue);
        }

        /// <summary>
        /// Renders the body.
        /// </summary>
        private void RenderBody()
        {
            bool oddEven = true;
            AGCasinoWinloss previousDataRow = null;
            int startMergeRow = this._startRow;
            for (int i = 0; i < this._rowsCount; i++)
            {
                var currentDataRow = this._data.Data[i];
                if (currentDataRow.Date == Labels.Total || currentDataRow.UserName == Labels.Total)
                {
                    return;
                }

                // Block Change
                if (previousDataRow != null
                    && ((this._isMainReport && (previousDataRow.Date != currentDataRow.Date)) || (previousDataRow.UserName != currentDataRow.UserName)))
                {
                    oddEven = !oddEven;
                    if (i > startMergeRow)
                    {
                        this._sheet.AddMergedRegion(new CellRangeAddress(startMergeRow, this._startRow - 1, 0, 0));
                    }

                    startMergeRow = this._startRow;
                }

                this.RenderBodyRow(currentDataRow, oddEven);
                previousDataRow = currentDataRow;
            }

            if (this._startRow - 1 > startMergeRow)
            {
                this._sheet.AddMergedRegion(new CellRangeAddress(startMergeRow, this._startRow - 1, 0, 0));
            }
        }

        /// <summary>
        /// Renders the body row.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <param name="oddOrEven">If set to <c>true</c> [odd or even].</param>
        private void RenderBodyRow(AGCasinoWinloss dataRow, bool oddOrEven)
        {
            var bodyRow = this._sheet.CreateRow(this._startRow++);
            int startColumn = this._startCol;
            this.RenderBodyCell(bodyRow.CreateCell(startColumn++), oddOrEven, this._isMainReport ? dataRow.Date : dataRow.UserName);
            this.RenderBodyCell(bodyRow.CreateCell(startColumn++), oddOrEven, dataRow.CurrencyName);
            this.RenderBodyCell(bodyRow.CreateCell(startColumn++), oddOrEven, dataRow.Turnover);
            this.RenderBodyCell(bodyRow.CreateCell(startColumn++), oddOrEven, dataRow.TotalTrans);
            this.RenderBodyCell(bodyRow.CreateCell(startColumn++), oddOrEven, dataRow.CustomerWinLoss);
            this.RenderBodyCell(bodyRow.CreateCell(startColumn++), oddOrEven, dataRow.LicenseeWinLoss);
            this.RenderBodyCell(bodyRow.CreateCell(startColumn++), oddOrEven, dataRow.AmountDueToAsiaGamings);
        }

        /// <summary>
        /// Renders the body cell.
        /// </summary>
        /// <param name="cell">The current cell.</param>
        /// <param name="oddOrEven">If set to <c>true</c> [odd or even].</param>
        /// <param name="cellValue">The cell value.</param>
        private void RenderBodyCell(ICell cell, bool oddOrEven, object cellValue)
        {
            if (cellValue is string)
            {
                cell.CellStyle = oddOrEven ? this._cellStyles.ReportBodyData.OddText : this._cellStyles.ReportBodyData.EvenText;
            }
            else if (cellValue is int)
            {
                cell.CellStyle = oddOrEven ? this._cellStyles.ReportBodyData.OddInteger : this._cellStyles.ReportBodyData.EvenInteger;
            }
            else
            {
                cell.CellStyle = oddOrEven ? this._cellStyles.ReportBodyData.OddDecimal : this._cellStyles.ReportBodyData.EvenDecimal;
            }

            NpoiExcelExtensions.SetCellValue(cell, cellValue);
        }

        /// <summary>
        /// Renders the sub total.
        /// </summary>
        private void RenderSubTotal()
        {
            for (int i = 0; i < this._data.Total.Count; i++)
            {
                var currentRow = this._sheet.CreateRow(this._startRow++);
                var currentDataRow = this._data.Total[i];
                int startColumn = this._startCol;
                this.RenderSubTotalCell(currentRow.CreateCell(startColumn++), this._isMainReport ? currentDataRow.Date : currentDataRow.UserName);
                this.RenderSubTotalCell(currentRow.CreateCell(startColumn++), currentDataRow.CurrencyName);
                this.RenderSubTotalCell(currentRow.CreateCell(startColumn++), currentDataRow.Turnover);
                this.RenderSubTotalCell(currentRow.CreateCell(startColumn++), currentDataRow.TotalTrans);
                this.RenderSubTotalCell(currentRow.CreateCell(startColumn++), currentDataRow.CustomerWinLoss);
                this.RenderSubTotalCell(currentRow.CreateCell(startColumn++), currentDataRow.LicenseeWinLoss);
                this.RenderSubTotalCell(currentRow.CreateCell(startColumn++), currentDataRow.AmountDueToAsiaGamings);
            }

            this._sheet.AddMergedRegion(new CellRangeAddress(this._startRow - this._data.Total.Count, this._startRow - 1, 0, 0));
        }

        /// <summary>
        /// Renders the sub total cell.
        /// </summary>
        /// <param name="cell">The current cell.</param>
        /// <param name="cellValue">The cell value.</param>
        private void RenderSubTotalCell(ICell cell, object cellValue)
        {
            if (cellValue is string)
            {
                cell.CellStyle = this._cellStyles.ReportBodySubTotal.Text;
            }
            else if (cellValue is int)
            {
                cell.CellStyle = this._cellStyles.ReportBodySubTotal.Integer;
            }
            else
            {
                cell.CellStyle = this._cellStyles.ReportBodySubTotal.Decimal;
            }

            NpoiExcelExtensions.SetCellValue(cell, cellValue);
        }

        /// <summary>
        /// Renders the footer.
        /// </summary>
        private void RenderFooter()
        {
            bool isFirstFooterRow = true;
            for (int i = 0; i < this._data.GrandTotal.Count; i++)
            {
                int startCol = this._startCol;
                var footerDataRow = this._data.GrandTotal[i];
                var footerRow = this._sheet.CreateRow(this._startRow++);
                this.RenderFooterCell(footerRow.CreateCell(startCol++), isFirstFooterRow, this._isMainReport ? footerDataRow.Date : footerDataRow.UserName);
                this.RenderFooterCell(footerRow.CreateCell(startCol++), isFirstFooterRow, footerDataRow.CurrencyName);
                this.RenderFooterCell(footerRow.CreateCell(startCol++), isFirstFooterRow, footerDataRow.Turnover);
                this.RenderFooterCell(footerRow.CreateCell(startCol++), isFirstFooterRow, footerDataRow.TotalTrans);
                this.RenderFooterCell(footerRow.CreateCell(startCol++), isFirstFooterRow, footerDataRow.CustomerWinLoss);
                this.RenderFooterCell(footerRow.CreateCell(startCol++), isFirstFooterRow, footerDataRow.LicenseeWinLoss);
                this.RenderFooterCell(footerRow.CreateCell(startCol++), isFirstFooterRow, footerDataRow.AmountDueToAsiaGamings);
                isFirstFooterRow = false;
            }

            this._sheet.AddMergedRegion(new CellRangeAddress(this._startRow - this._data.GrandTotal.Count, this._startRow - 1, 3, 3));
            this._sheet.AddMergedRegion(new CellRangeAddress(this._startRow - this._data.GrandTotal.Count, this._startRow - 1, 0, 0));
            this.AutoSizeColumns();
        }

        /// <summary>
        /// Renders the footer cell.
        /// </summary>
        /// <param name="cell">The current cell.</param>
        /// <param name="isFirstFooterRow">If set to <c>true</c> [is first footer row].</param>
        /// <param name="cellValue">The cell value.</param>
        private void RenderFooterCell(ICell cell, bool isFirstFooterRow, object cellValue)
        {
            if (cellValue is string)
            {
                cell.CellStyle = isFirstFooterRow ? this._cellStyles.ReportBodyFooter.TextUpper : this._cellStyles.ReportBodyFooter.Text;
            }
            else if (cellValue is int)
            {
                cell.CellStyle = isFirstFooterRow ? this._cellStyles.ReportBodyFooter.IntegerUpper : this._cellStyles.ReportBodyFooter.Integer;
            }
            else
            {
                cell.CellStyle = isFirstFooterRow ? this._cellStyles.ReportBodyFooter.DecimalUpper : this._cellStyles.ReportBodyFooter.Decimal;
            }

            NpoiExcelExtensions.SetCellValue(cell, cellValue);
        }

        private bool IsMainReport()
        {
            return this._criteria.GetType().Equals(typeof(GetMainReportCriteria));
        }

        private void AutoSizeColumns()
        {
            for (int i = 0; i < AGCasinoWinlossConstants.ExcelColumnCount; i++)
            {
                this._sheet.SetColumnWidth(i, 140 * ExcelExtensions.FontWidth);
            }
        }
    }
}
