namespace Fanex.AccountingReports.Special.Services.Implement.DailyTurnover
{
    using System;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// The implement of Daily Turnover Excel Utilities.
    /// </summary>
    public static class ExcelUtil
    {
        /// <summary>
        /// Set border to cell.
        /// </summary>
        /// <param name="cellStyle">The style of cell.</param>
        /// <returns>The cell with border.</returns>
        public static ICellStyle SetBorderToCell(ICellStyle cellStyle)
        {
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;

            cellStyle.BottomBorderColor = HSSFColor.Grey80Percent.Index;
            cellStyle.LeftBorderColor = HSSFColor.Grey80Percent.Index;
            cellStyle.RightBorderColor = HSSFColor.Grey80Percent.Index;
            cellStyle.TopBorderColor = HSSFColor.Grey80Percent.Index;

            return cellStyle;
        }

        /// <summary>
        /// Render cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row in excel file.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="endColumn">The end column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        public static void RenderCell(
            ref ISheet worksheet,
            ref IRow row,
            int currentRow,
            ref int currentColumn,
            int endRow,
            int endColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            for (var i = currentRow; i <= endRow; i++)
            {
                row = worksheet.GetRow(i) ?? worksheet.CreateRow(i);

                for (var j = currentColumn; j <= endColumn; j++)
                {
                    ICell cell = row.CreateCell(j);

                    if (cellValue is string)
                    {
                        cell.SetCellValue((string)cellValue);
                    }
                    else if (cellValue is decimal)
                    {
                        cell.SetCellValue(Convert.ToDouble(cellValue));
                    }
                    else if (cellValue is double)
                    {
                        cell.SetCellValue((double)cellValue);
                    }

                    cell.CellStyle = cellStyle;
                }
            }

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, endRow, currentColumn, endColumn));
            currentColumn = endColumn + 1;
        }

        public struct CustomStyleCell
        {
            public CustomStyleCell(HSSFWorkbook excelFile)
            {
                TextStyle = excelFile.CreateCellStyle();
                DecimalStyle = excelFile.CreateCellStyle();
            }

            public ICellStyle TextStyle { get; set; }

            public ICellStyle DecimalStyle { get; set; }
        }
    }
}
