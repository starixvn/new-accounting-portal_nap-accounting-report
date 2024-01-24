namespace Fanex.AccountingReports.Core.Extensions
{
    using System;
    using System.Drawing;
    using System.Web;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public static class NpoiExcelExtensions
    {
        public const string FontName = "Tahoma";
        public const string IntNumberFormat = "#,##0";
        public const string TwoDecimalDigitNumber = "##,##0.00";
        public const string NegativeDecimalNumberFormat = "[>0]#,##0.00;[<0][Red]-#,##0.00;#,##0.00";
        public const string PositiveDecimalNumberFormat = "[>0][Red]#,##0.00;[<0]-#,##0.00;#,##0.00";
        public const string PercentFormat = "#,##0.00%;-#,##0.00%";
        public const string NegativePercentFormat = "[>0]#,##0.00%;[<0][Red]-#,##0.00%;#,##0.00%";
        public const string PositivePercentFormat = "[>0][Red]#,##0.00%;[<0]-#,##0.00%;#,##0.00%";
        public const string NormalPercentFormat = "#,##0.00%;-#,##0.00%";

        private const int VerticalAlignCenter = 1;

        public const int FontWidth = 30;
        public const int FontHeight = 20;
        public const int FontSize = 8;
        public const int MaxRowPerSheet = 20000;

        public static readonly Color BackGroundColor = Color.White;
        public static readonly Color ReportTitleColor = Color.FromArgb(255, 217, 97);
        public static readonly Color HeaderTitleColor = Color.FromArgb(60, 94, 165);
        public static readonly Color EvenRowColor = Color.FromArgb(228, 242, 253);
        public static readonly Color OddRowColor = Color.White;
        public static readonly Color SubTotalRowColor = Color.FromArgb(244, 244, 190);
        public static readonly Color TotalRowColor = Color.FromArgb(244, 244, 190);
        public static readonly Color GrandTotalRowColor = Color.FromArgb(219, 225, 187);
        public static readonly Color GrandTotalSeparatorColor = Color.FromArgb(255, 102, 0);
        public static readonly Color RevenueLicenseeColor = Color.FromArgb(213, 132, 52);
        public static readonly Color ParentColor = Color.FromArgb(194, 222, 236);
        public static readonly Color ChildColor = Color.FromArgb(194, 226, 236);
        public static readonly Color NoteInformationCellColor = Color.FromArgb(181, 196, 208);
        public static readonly Color BorderColor = Color.Gray;

        public enum Alignment
        {
            Left,

            Center,

            Right,
        }

        public static ICellStyle HeaderStyle(HSSFWorkbook excelFile)
        {
            ICellStyle cellStyle = excelFile.CreateCellStyle();
            HSSFPalette palette = excelFile.GetCustomPalette();
            IFont font = excelFile.CreateFont();
            palette.SetColorAtIndex(HSSFColor.Pink.Index, (byte)60, (byte)94, (byte)165);
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.Color = HSSFColor.White.Index;
            font.FontHeightInPoints = FontSize;
            font.FontName = FontName;

            cellStyle.FillForegroundColor = HSSFColor.Pink.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = (VerticalAlignment)VerticalAlignCenter;
            cellStyle.SetFont(font);
            cellStyle = SetBorderToCell(cellStyle);
            return cellStyle;
        }

        public static ICellStyle TextStyle(
            HSSFWorkbook excelFile,
            short cellColor,
            bool bold,
            Alignment align,
            short fontColor = 8)
        {
            ICellStyle cellStyle = excelFile.CreateCellStyle();
            IFont font = excelFile.CreateFont();

            font.FontHeightInPoints = FontSize;
            font.FontName = FontName;
            font.Color = fontColor;

            if (bold)
            {
                font.Boldweight = (short)FontBoldWeight.Bold;
            }

            cellStyle.FillForegroundColor = cellColor;
            cellStyle.FillPattern = FillPattern.SolidForeground;

            switch (align)
            {
                case Alignment.Right:
                    cellStyle.Alignment = HorizontalAlignment.Right;
                    break;

                case Alignment.Left:
                    cellStyle.Alignment = HorizontalAlignment.Left;
                    break;

                case Alignment.Center:
                    cellStyle.Alignment = HorizontalAlignment.Center;
                    break;
            }

            cellStyle.VerticalAlignment = (VerticalAlignment)VerticalAlignCenter;
            cellStyle.SetFont(font);
            cellStyle = SetBorderToCell(cellStyle);
            return cellStyle;
        }

        public static ICellStyle DecimalStyle(
            HSSFWorkbook excelFile,
            short cellColor,
            bool bold,
            short fontColor = 8)
        {
            ICellStyle cellStyle = excelFile.CreateCellStyle();
            IFont font = excelFile.CreateFont();
            IDataFormat dataFormat = excelFile.CreateDataFormat();

            font.FontHeightInPoints = FontSize;
            font.FontName = FontName;
            font.Color = fontColor;

            if (bold)
            {
                font.Boldweight = (short)FontBoldWeight.Bold;
            }

            cellStyle.FillForegroundColor = cellColor;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.Alignment = HorizontalAlignment.Right;
            cellStyle.VerticalAlignment = (VerticalAlignment)VerticalAlignCenter;
            cellStyle.DataFormat = dataFormat.GetFormat(TwoDecimalDigitNumber);
            cellStyle.SetFont(font);
            cellStyle = SetBorderToCell(cellStyle);
            return cellStyle;
        }

        public static ICellStyle IntStyle(
            HSSFWorkbook excelFile,
            short cellColor,
            bool bold,
            short fontColor = 8)
        {
            ICellStyle cellStyle = excelFile.CreateCellStyle();
            IFont font = excelFile.CreateFont();
            IDataFormat dataFormat = excelFile.CreateDataFormat();

            font.FontHeightInPoints = FontSize;
            font.FontName = FontName;
            font.Color = fontColor;

            if (bold)
            {
                font.Boldweight = (short)FontBoldWeight.Bold;
            }

            cellStyle.FillForegroundColor = cellColor;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.Alignment = HorizontalAlignment.Right;
            cellStyle.VerticalAlignment = (VerticalAlignment)VerticalAlignCenter;
            cellStyle.DataFormat = dataFormat.GetFormat(IntNumberFormat);
            cellStyle.SetFont(font);
            cellStyle = SetBorderToCell(cellStyle);
            return cellStyle;
        }

        public static ICellStyle IntStyle(
            HSSFWorkbook excelFile,
            short cellColor,
            bool bold,
            Alignment align,
            short fontColor = 8)
        {
            ICellStyle cellStyle = excelFile.CreateCellStyle();
            IFont font = excelFile.CreateFont();
            IDataFormat dataFormat = excelFile.CreateDataFormat();

            font.FontHeightInPoints = FontSize;
            font.FontName = FontName;
            font.Color = fontColor;

            if (bold)
            {
                font.Boldweight = (short)FontBoldWeight.Bold;
            }

            switch (align)
            {
                case Alignment.Right:
                    cellStyle.Alignment = HorizontalAlignment.Right;
                    break;

                case Alignment.Left:
                    cellStyle.Alignment = HorizontalAlignment.Left;
                    break;

                case Alignment.Center:
                    cellStyle.Alignment = HorizontalAlignment.Center;
                    break;
            }

            cellStyle.FillForegroundColor = cellColor;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.VerticalAlignment = (VerticalAlignment)VerticalAlignCenter;
            cellStyle.DataFormat = dataFormat.GetFormat(IntNumberFormat);
            cellStyle.SetFont(font);
            cellStyle = SetBorderToCell(cellStyle);
            return cellStyle;
        }

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
                    ICell cell = row.GetCell(j) ?? row.CreateCell(j);

                    if (i == currentRow && j == currentColumn)
                    {
                        SetCellValue(cell, cellValue);
                    }

                    if (i == currentRow ||
                        i == endRow ||
                        j == currentColumn ||
                        j == endColumn)
                    {
                        cell.CellStyle = cellStyle;
                    }
                }
            }

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, endRow, currentColumn, endColumn));
            currentColumn = endColumn + 1;
        }

        public static void ResponseStreamExcel(this IWorkbook workbook, string fileName)
        {
            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", fileName));
            workbook.Write(response.OutputStream);
        }

        public static void SetCellValue(ICell cell, object cellValue)
        {
            if ((cellValue != null) && (cellValue is string))
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is int)
            {
                cell.SetCellValue((int)cellValue);
            }
            else if (cellValue is long)
            {
                cell.SetCellValue((long)cellValue);
            }
            else if (cellValue is double)
            {
                cell.SetCellValue((double)cellValue);
            }
            else if (cellValue is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else if (cellValue is bool)
            {
                cell.SetCellValue((bool)cellValue);
            }
            else if (cellValue is System.DateTime)
            {
                cell.SetCellValue((System.DateTime)cellValue);
            }
        }

        private static ICellStyle SetBorderToCell(ICellStyle cellStyle)
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
    }
}
