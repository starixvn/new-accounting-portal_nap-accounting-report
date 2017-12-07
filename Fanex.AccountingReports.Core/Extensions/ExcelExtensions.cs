namespace Fanex.AccountingReports.Core.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Web;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Resources;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ExcelExtensions
    {
        #region Constanst values

        public const int FontWidth = 30;
        public const int FontSize = 160;
        public const int MaxRowPerSheet = 20000;

        public static readonly Color BackGroundColor = Color.White;
        public static readonly Color HeaderTitleColor = Color.FromArgb(60, 94, 165);
        public static readonly Color EvenRowColor = Color.FromArgb(228, 242, 253);
        public static readonly Color OddRowColor = Color.White;
        public static readonly Color SubTotalRowColor = Color.FromArgb(194, 217, 236);
        public static readonly Color TotalRowColor = Color.FromArgb(244, 244, 190);
        public static readonly Color GrandTotalRowColor = Color.FromArgb(219, 225, 187);
        public static readonly Color GrandTotalSeparatorColor = Color.FromArgb(255, 102, 0);
        public static readonly Color RevenueLicenseeColor = Color.FromArgb(213, 132, 52);
        public static readonly Color ParentColor = Color.FromArgb(194, 222, 236);
        public static readonly Color ChildColor = Color.FromArgb(194, 226, 236);
        public static readonly Color NoteInformationCellColor = Color.FromArgb(181, 196, 208);
        public static readonly Color BorderColor = Color.Gray;

        private const string FontName = "Tahoma";
        private const string NegativeNumberFormat = "[>=0]#,##0{0}_);[RED][<0]-#,##0{1}";
        private const string NormalNumberFormat = "#,##0{0}_);-#,##0{1}";
        private const string PositiveNumberFormat = "[RED][>0]#,##0{0}_);[<0]-#,##0{1};#,##0{2}";

        #endregion Constanst values

        #region Response Stream

        /// <summary>
        /// Reponses the stream.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="reportName">Name of the report.</param>
        public static void ReponseStream(this ExcelFile excelFile, string reportName)
        {
            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", reportName));
            excelFile.SaveXls(response.OutputStream);
        }

        /// <summary>
        /// Exports Excel file to response stream.
        /// </summary>
        /// <param name="workbook">The workbook contains the data.</param>
        /// <param name="fileName">The file name.</param>
        public static void ToReponseStream(this IWorkbook workbook, string fileName)
        {
            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", fileName));
            workbook.Write(response.OutputStream);
        }

        #endregion Response Stream

        #region Title Cell Format

        /// <summary>
        /// Formats the title cell.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public static void FormatTitleCell(
            this CellStyle cellStyle,
            Color backgroundColor)
        {
            cellStyle.FormatCellText(backgroundColor, true);
            cellStyle.Font.Color = Color.White;
            cellStyle.VerticalAlignment = VerticalAlignmentStyle.Center;
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            cellStyle.WrapText = true;
        }

        /// <summary>
        /// Gets the cell title style.
        /// </summary>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <returns>The cell title style.</returns>
        public static CellStyle GetCellTitleStyle(Color backgroundColor)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatTitleCell(backgroundColor);
            return cellStyle;
        }

        #endregion

        #region Text Cell Format

        public static CellStyle GetCellTextStyle(
            Color backgroundColor,
            bool isBold)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellText(backgroundColor, isBold);
            return cellStyle;
        }

        public static CellStyle GetCellTextStyle(
            Color backgroundColor,
            bool isBold,
            HorizontalAlignmentStyle textAlign,
            bool isItalic,
            Color fontColor)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellText(backgroundColor, isBold, textAlign, isItalic, fontColor);
            return cellStyle;
        }

        public static CellStyle GetCellTextStyle(
            Color backgroundColor,
            bool isBold,
            HorizontalAlignmentStyle textAlign)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellText(backgroundColor, isBold, textAlign);
            return cellStyle;
        }

        public static void FormatCellText(
            this CellStyle cellStyle,
            Color backgroundColor,
            bool isBold)
        {
            cellStyle.FormatCellText(backgroundColor, isBold, HorizontalAlignmentStyle.General);
        }

        public static void FormatCellText(
            this CellStyle cellStyle,
            Color backgroundColor,
            bool isBold,
            HorizontalAlignmentStyle textAlign)
        {
            cellStyle.FormatCellText(backgroundColor, isBold, textAlign, false, Color.Black);
        }

        /// <summary>
        /// Formats the cell text.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="isBold">If set to <c>true</c> [is bold].</param>
        /// <param name="textAlign">The text align.</param>
        /// <param name="isItalic">If set to <c>true</c> [is italic].</param>z
        /// <param name="fontColor">Color of the font.</param>
        public static void FormatCellText(
            this CellStyle cellStyle,
            Color backgroundColor,
            bool isBold,
            HorizontalAlignmentStyle textAlign,
            bool isItalic,
            Color fontColor)
        {
            cellStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, ExcelUtil.Spreadsheet.LineStyle.Thin);
            cellStyle.Font.Name = FontName;
            cellStyle.Font.Size = FontSize;
            if (backgroundColor != null)
            {
                cellStyle.FillPattern.SetSolid(backgroundColor);
            }

            cellStyle.VerticalAlignment = VerticalAlignmentStyle.Center;
            cellStyle.HorizontalAlignment = textAlign;
            cellStyle.Font.Italic = isItalic;
            if (isBold)
            {
                cellStyle.Font.Weight = ExcelFont.BoldWeight;
            }

            if (fontColor != null)
            {
                cellStyle.Font.Color = fontColor;
            }
        }

        #endregion Text Cell Format

        #region Number Cell Format

        public static CellStyle GetCellNumberStyle(
            string symbol,
            Color backgroundColor,
            bool isBold,
            bool isPossitive,
            bool isNagative)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellNumberStyle(symbol, backgroundColor, isBold, isPossitive, isNagative);
            return cellStyle;
        }

        public static CellStyle GetCellNumberStyle(
            string symbol,
            Color backgroundColor,
            Color fontColor,
            bool isBold,
            bool isPossitive,
            bool isNagative)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellNumberStyle(symbol, backgroundColor, fontColor, isBold, isPossitive, isNagative);
            return cellStyle;
        }

        /// <summary>
        /// Formats the cell number style.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="isBold">If set to <c>true</c> [is bold].</param>
        /// <param name="isPositive">If set to <c>true</c> [is positive].</param>
        /// <param name="isNegative">If set to <c>true</c> [is negative].</param>
        public static void FormatCellNumberStyle(
            this CellStyle cellStyle,
            string symbol,
            Color backgroundColor,
            bool isBold,
            bool isPositive,
            bool isNegative)
        {
            cellStyle.FormatCellNumberStyle(
                symbol,
                backgroundColor,
                Color.Black,
                isBold,
                isPositive,
                isNegative);
        }

        /// <summary>
        /// Formats the cell number style.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="fontColor">Color of the font.</param>
        /// <param name="isBold">If set to <c>true</c> [is bold].</param>
        /// <param name="isPositive">If set to <c>true</c> [is positive].</param>
        /// <param name="isNegative">If set to <c>true</c> [is negative].</param>
        public static void FormatCellNumberStyle(
            this CellStyle cellStyle,
            string symbol,
            Color backgroundColor,
            Color fontColor,
            bool isBold,
            bool isPositive,
            bool isNegative)
        {
            cellStyle.VerticalAlignment = VerticalAlignmentStyle.Center;
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            cellStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, ExcelUtil.Spreadsheet.LineStyle.Thin);

            if (isNegative)
            {
                cellStyle.NumberFormat = string.Format(NegativeNumberFormat, symbol, symbol);
            }
            else if (isPositive)
            {
                cellStyle.NumberFormat = string.Format(PositiveNumberFormat, symbol, symbol, symbol);
            }
            else
            {
                cellStyle.NumberFormat = string.Format(NormalNumberFormat, symbol, symbol);
            }

            cellStyle.Font.Name = FontName;
            cellStyle.Font.Size = FontSize;

            if (backgroundColor != null)
            {
                cellStyle.FillPattern.SetSolid(backgroundColor);
            }

            if (fontColor != null)
            {
                cellStyle.Font.Color = fontColor;
            }

            if (isBold)
            {
                cellStyle.Font.Weight = ExcelFont.BoldWeight;
            }
        }

        #endregion

        #region Row Format

        public static void ExportEmptyRow(
            this ExcelWorksheet ws,
            int startRow,
            int startColumn,
            ref int currentRow,
            int endColumn)
        {
            var currentColumn = startColumn;
            var currentCellRange = ws.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, endColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = string.Empty;
            currentCellRange.Style.FormatCellText(Color.White, false);
            currentCellRange.Style.Font.Color = Color.White;
            currentRow++;
        }

        #endregion Row Format
        #region Report Format

        /// <summary>
        /// Exports the empty report.
        /// </summary>
        /// <param name="ws">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="endColumn">The end column.</param>
        public static void ExportEmptyReport(
            this ExcelWorksheet ws,
            int startRow,
            int startColumn,
            ref int currentRow,
            int endColumn)
        {
            var currentColumn = startColumn;
            var currentCellRange = ws.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, endColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.EmptyReportMessage;
            currentCellRange.Style.FormatCellText(Color.White, true);
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentRow++;
        }

        /// <summary>
        /// Analyses the one line text.
        /// </summary>
        /// <param name="oneLineText">The one line text.</param>
        /// <param name="maxCharPerRow">The maximum character per row.</param>
        /// <returns>Expand row number.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public static int AnalyseOneLineText(string oneLineText, int maxCharPerRow)
        {
            switch (maxCharPerRow)
            {
                case 0:
                    return 0;
                case 1:
                    return oneLineText.Length;
                default:
                    break;
            }

            var actualRow = 0;
            var wordList = oneLineText.Split(new char[] { ' ' });
            var wordsLength = wordList.Length;
            var currentRowLength = 0;
            for (var i = 0; i < wordsLength; i++)
            {
                var word = wordList[i];
                var wordLength = word.Length;
                if (currentRowLength >= maxCharPerRow)
                {
                    actualRow += currentRowLength / maxCharPerRow;
                    currentRowLength = currentRowLength % maxCharPerRow;
                }

                if (wordLength == 0)
                {
                    currentRowLength++;
                    continue;
                }

                if (wordLength > 0 && currentRowLength + wordLength <= maxCharPerRow)
                {
                    currentRowLength += wordLength + 1;
                    continue;
                }

                if (wordLength > 0 && maxCharPerRow < currentRowLength + wordLength)
                {
                    actualRow++;
                    currentRowLength = wordLength + 1;
                    continue;
                }
            }

            if (currentRowLength != 0)
            {
                actualRow++;
            }

            return actualRow;
        }

        /// <summary>
        /// Formats the long text.
        /// </summary>
        /// <param name="longText">The long text.</param>
        /// <param name="excludeRowNumber">The exclude row number.</param>
        /// <param name="textRowSpan">The text row span.</param>
        /// <param name="width">The width.</param>
        /// <param name="addPaddingBottom">If set to <c>true</c> [add padding bottom].</param>
        /// <returns>Expand row number.</returns>
        public static int FormatLongText(
            string longText,
            int excludeRowNumber,
            int textRowSpan,
            int width,
            bool addPaddingBottom)
        {
            var maxCharPerRow = width * 102 / 21000;
            if (string.IsNullOrEmpty(longText) || maxCharPerRow == 0)
            {
                return 0;
            }

            var extraHeightPerRow = 200;
            var actualRow = 0;
            var oneLineTexts = longText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var oneLineText in oneLineTexts)
            {
                actualRow += AnalyseOneLineText(oneLineText, maxCharPerRow);
            }

            if (addPaddingBottom && actualRow > 0)
            {
                actualRow++;
            }

            var maxRow = textRowSpan < excludeRowNumber ? excludeRowNumber : textRowSpan;
            if (actualRow < maxRow)
            {
                actualRow = maxRow;
            }

            return extraHeightPerRow * (actualRow - excludeRowNumber);
        }

        #endregion
    }
}
